using System;
using System.IO;
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
    public ScoreManager ScoreManagerInstance { get; private set; }

    public Texture2D PixelTexture { get; private set; }     
    public Texture2D DimTexture { get; private set; }
    public SpriteFont GameFont { get; private set; }
    public Texture2D SpriteSheet { get; private set; }

    private State _currentState;
    public MenuState MenuStateInstance { get; private set; }
    public GameplayState GameplayStateInstance { get; private set; }
    public GameOverState GameOverStateInstance { get; private set; }
    public VictoryState VictoryStateInstance { get; private set; } 

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 904; 
        _graphics.PreferredBackBufferHeight = 704; 
        _graphics.ApplyChanges();

        EntityManagerInstance = new EntityManager();
        CollisionManagerInstance = new CollisionManager(EntityManagerInstance);
        ScoreManagerInstance = new ScoreManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        PixelTexture.SetData(new[] { Color.White });

        DimTexture = new Texture2D(GraphicsDevice, 1, 1);
        DimTexture.SetData(new[] { new Color(0, 0, 0, 150) });

        // --- ЗАВАНТАЖЕННЯ СИСТЕМНОГО ШРИФТУ ДЛЯ MAC / LINUX ---
        string fontPath = null;
        
        string[] systemFontPaths = new[]
        {
            "/System/Library/Fonts/Constants/SFCompact.ttf",
            "/System/Library/Fonts/Supplemental/Courier New.ttf",
            "/System/Library/Fonts/User/Courier New.ttf",
            "/System/Library/Fonts/Geneva.ttf",
            "/Library/Fonts/Arial.ttf",
            "/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf",
            "/usr/share/fonts/TTF/DejaVuSansMono.ttf",
            "/usr/share/fonts/truetype/freefont/FreeMono.ttf"
        };

        foreach (var path in systemFontPaths)
        {
            if (File.Exists(path))
            {
                fontPath = path;
                break;
            }
        }

        if (fontPath != null)
        {
            try
            {
                byte[] fontData = File.ReadAllBytes(fontPath);
                var fontBakeResult = TtfFontBaker.Bake(fontData, 18, 1024, 1024, new[] { CharacterRange.BasicLatin });
                GameFont = fontBakeResult.CreateSpriteFont(GraphicsDevice);
                Console.WriteLine($"[УСПІХ] Завантажено системний шрифт: {fontPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запікання системного шрифту: {ex.Message}");
            }
        }

        if (GameFont == null)
        {
            Console.WriteLine("КРИТИЧНА ПОМИЛКА: Не вдалося знайти жодного системного шрифту.");
        }
        // --- КІНЕЦЬ ЗАВАНТАЖЕННЯ СИСТЕМНОГО ШРИФТУ ---

        MapLoader mapLoader = new MapLoader(GraphicsDevice, EntityManagerInstance);
        string spritePath = Path.Combine(Content.RootDirectory, "spritesheet.png");
        SpriteSheet = mapLoader.LoadSpriteSheetWithChromaKey(spritePath, Color.Black);

        ScoreManagerInstance.LoadHighScore();

        MenuStateInstance = new MenuState(this, GraphicsDevice);
        GameplayStateInstance = new GameplayState(this, GraphicsDevice);
        GameOverStateInstance = new GameOverState(this, GraphicsDevice);
        VictoryStateInstance = new VictoryState(this, GraphicsDevice);

        RestartGame();
        ChangeState(MenuStateInstance);
    }

    public void ChangeState(State newState)
    {
        _currentState = newState;
    }

    public void RestartGame()
    {
        ScoreManagerInstance.Reset();
        if (GameplayStateInstance != null)
        {
            GameplayStateInstance.ResetTimers();
        }
        EntityManagerInstance.Clear();

        MapLoader mapLoader = new MapLoader(GraphicsDevice, EntityManagerInstance);
        string mapFilePath = Path.Combine(Content.RootDirectory, "level1.txt");
        
        PlayerInstance = mapLoader.BuildMap(mapFilePath, SpriteSheet);
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