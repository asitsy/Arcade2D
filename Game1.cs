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
        DimTexture.SetData(new[] { Color.Black * 0.65f });

        string systemFontPath = "";
        
        string[] macFonts = new string[] 
        {
            "/System/Library/Fonts/Supplemental/Arial.ttf", // Нові macOS (Catalina+)
            "/Library/Fonts/Arial.ttf",                     
            "/System/Library/Fonts/Supplemental/Comic Sans MS.ttf",
            "/Library/Fonts/Comic Sans MS.ttf",
            "/System/Library/Fonts/Supplemental/Times New Roman.ttf"
        };

        foreach (string font in macFonts) // тут системний пошук в маці
        {
            if (File.Exists(font))
            {
                systemFontPath = font;
                break;
            }
        }

        if (string.IsNullOrEmpty(systemFontPath))
        {
            throw new Exception("Не вдалося знайти сумісний .ttf шрифт на вашому Mac. Бібліотека не підтримує .ttc формати.");
        }

        byte[] ttfData = File.ReadAllBytes(systemFontPath);
        var bakedFont = TtfFontBaker.Bake(ttfData, 18, 1024, 1024, new[] { CharacterRange.BasicLatin });
        GameFont = bakedFont.CreateSpriteFont(GraphicsDevice);


        string spritePath = Path.Combine(AppContext.BaseDirectory, "Content", "spritesheet.png");
        SpriteSheet = Texture2D.FromFile(GraphicsDevice, spritePath);
        
        Color[] spriteData = new Color[SpriteSheet.Width * SpriteSheet.Height];
        SpriteSheet.GetData(spriteData);
        for (int i = 0; i < spriteData.Length; i++)
        {
            if (spriteData[i] == Color.Black)
                spriteData[i] = Color.Transparent;
        }
        SpriteSheet.SetData(spriteData);

        ScoreManagerInstance.LoadLastScore();

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
        // Оновлюємо рекорд у пам'яті безпосередньо ПЕРЕД початком нової гри
        ScoreManagerInstance.UpdateLastScoreInMemory();

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