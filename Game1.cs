using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus; 
using Arcade2D.Entities;
using Arcade2D.Utils;
using Arcade2D.Managers;
using Arcade2D.States; 

namespace Arcade2D;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Player PlayerInstance { get; private set; }  
    public EntityManager EntityManagerInstance { get; private set; }
    public CollisionManager CollisionManagerInstance { get; private set; }

    public Texture2D PixelTexture { get; private set; }     
    public Texture2D DimTexture { get; private set; }
    public SpriteFont GameFont { get; private set; }
    
    // ДОДАНО: Сховище для нашого нового оригінального спрайтшиту
    public Texture2D SpriteSheet { get; private set; }

    public int Score;
    public int LastScore = 0;
    public float FreezeTimer = 0f;
    public float PlayerSpeedTimer = 0f;

    public bool IsGhostsFrozen => FreezeTimer > 0f;
    public bool IsPlayerSpedUp => PlayerSpeedTimer > 0f;

    private readonly Vector2 _mapOffset = new Vector2(16, 16);
    private readonly string _lastScoreFilename = "highscore.txt";

    private State _currentState;
    public MenuState MenuStateInstance { get; private set; }
    public GameplayState GameplayStateInstance { get; private set; }
    public GameOverState GameOverStateInstance { get; private set; }
    public VictoryState VictoryStateInstance { get; private set; }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 904;
        _graphics.PreferredBackBufferHeight = 704;
        Content.RootDirectory = "Content";
        IsMouseVisible = true; 
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        EntityManagerInstance = new EntityManager();
        CollisionManagerInstance = new CollisionManager(EntityManagerInstance); 

        string fontPath = @"/System/Library/Fonts/Supplemental/Courier New.ttf"; 
        if (!File.Exists(fontPath)) fontPath = @"/System/Library/Fonts/Supplemental/Arial.ttf";

        if (File.Exists(fontPath))
        {
            byte[] fontData = File.ReadAllBytes(fontPath);
            GameFont = TtfFontBaker.Bake(fontData, 22, 1024, 1024, new[] { CharacterRange.BasicLatin })
                                    .CreateSpriteFont(GraphicsDevice);
        }

        PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        PixelTexture.SetData(new[] { Color.White }); 

        DimTexture = new Texture2D(GraphicsDevice, 1, 1);
        DimTexture.SetData(new[] { new Color(15, 15, 30, 215) }); 

        // ДОДАНО: Завантаження файлу та автоматичне очищення чорного фону
        string sheetPath = Path.Combine(Content.RootDirectory, "spritesheet.png");
        if (File.Exists(sheetPath))
        {
            SpriteSheet = Texture2D.FromFile(GraphicsDevice, sheetPath);
            Color[] colorData = new Color[SpriteSheet.Width * SpriteSheet.Height];
            SpriteSheet.GetData(colorData);
            
            for (int i = 0; i < colorData.Length; i++)
            {
                // Якщо піксель чисто чорний (як фон на Spriters Resource) - робимо його прозорим
                if (colorData[i].R == 0 && colorData[i].G == 0 && colorData[i].B == 0)
                {
                    colorData[i] = Color.Transparent; 
                }
            }
            SpriteSheet.SetData(colorData);
        }

        LoadLastScore();

        MenuStateInstance = new MenuState(this, GraphicsDevice);
        GameplayStateInstance = new GameplayState(this, GraphicsDevice);
        GameOverStateInstance = new GameOverState(this, GraphicsDevice);
        VictoryStateInstance = new VictoryState(this, GraphicsDevice);

        RestartGame();

        _currentState = MenuStateInstance;
    }

    public void ChangeState(State newState)
    {
        _currentState = newState;
    }

    private void LoadLastScore()
    {
        try
        {
            if (File.Exists(_lastScoreFilename))
            {
                string content = File.ReadAllText(_lastScoreFilename);
                if (int.TryParse(content, out int savedScore))
                {
                    LastScore = savedScore;
                }
            }
        }
        catch (Exception)
        {
            LastScore = 0;
        }
    }

    public async Task SaveLastScoreAsync(int score)
    {
        await Task.Run(async () =>
        {
            try
            {
                await File.WriteAllTextAsync(_lastScoreFilename, score.ToString());
            }
            catch (Exception) {}
        });
    }

    public void RestartGame()
    {
        Score = 0;
        FreezeTimer = 0f;
        PlayerSpeedTimer = 0f; 
        EntityManagerInstance.Clear();

        Texture2D wallTexture = new Texture2D(GraphicsDevice, 1, 1);
        wallTexture.SetData(new[] { ColorPalette.Lavender });

        Texture2D pelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        pelletTexture.SetData(new[] { ColorPalette.SoftYellow });

        Texture2D powerPelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        powerPelletTexture.SetData(new[] { ColorPalette.NeonPink });

        Texture2D speedPelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        speedPelletTexture.SetData(new[] { ColorPalette.Gold });

        Texture2D ghostTexture = new Texture2D(GraphicsDevice, 1, 1);
        ghostTexture.SetData(new[] { ColorPalette.NeonPink });

        // Модифіковано: Тепер передаємо Спрайтшит гравцю замість рожевого прямокутника
        PlayerInstance = new Player(new Vector2(32 * 10 + 4, 32 * 10 + 4) + _mapOffset, SpriteSheet);

        EntityManagerInstance.Add(PlayerInstance);

        // Модифіковано: Передаємо Спрайтшит усім привидам
        EntityManagerInstance.Add(new Ghost(new Vector2(32 * 1, 32 * 1) + _mapOffset, SpriteSheet));
        EntityManagerInstance.Add(new Ghost(new Vector2(32 * 19, 32 * 1) + _mapOffset, SpriteSheet));
        EntityManagerInstance.Add(new Ghost(new Vector2(32 * 1, 32 * 19) + _mapOffset, SpriteSheet));
        EntityManagerInstance.Add(new Ghost(new Vector2(32 * 19, 32 * 19) + _mapOffset, SpriteSheet));

        string mapFilePath = Path.Combine(Content.RootDirectory, "level1.txt");
        List<string> mapLines = new List<string>();

        if (File.Exists(mapFilePath))
        {
            mapLines = File.ReadAllLines(mapFilePath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        }
        else
        {
            mapLines = new List<string>
            {
                "111111111111111111111",
                "100000000000000000001",
                "111111111111111111111"
            };
        }

        int tileSize = 32;
        for (int row = 0; row < mapLines.Count; row++)
        {
            string currentLine = mapLines[row];
            for (int col = 0; col < currentLine.Length; col++)
            {
                Vector2 pos = new Vector2(col * tileSize, row * tileSize) + _mapOffset;
                char tileChar = currentLine[col];

                if (tileChar == '1')
                {
                    EntityManagerInstance.Add(new Wall(pos, wallTexture));
                }
                else if (tileChar == '0')
                {
                    bool isPowerPellet = (row == 1 && col == 1) || (row == 1 && col == 19) || (row == 19 && col == 1) || (row == 19 && col == 19);
                    bool isSpeedPellet = (row == 9 && col == 1) || (row == 9 && col == 19);

                    if (isPowerPellet)
                    {
                        EntityManagerInstance.Add(new PowerPellet(pos + new Vector2(8, 8), powerPelletTexture));
                    }
                    else if (isSpeedPellet)
                    {
                        EntityManagerInstance.Add(new SpeedPellet(pos + new Vector2(8, 8), speedPelletTexture));
                    }
                    else if ((row + col) % 2 == 0)
                    {
                        if (!((row >= 9 && row <= 11) && (col >= 9 && col <= 11)))
                        {
                            EntityManagerInstance.Add(new Pellet(pos + new Vector2(10, 10), pelletTexture));
                        }
                    }
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        _currentState?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorPalette.Background);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _currentState?.Draw(gameTime, _spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}