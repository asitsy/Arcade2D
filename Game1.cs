using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus; // Бібліотека для динамічних шрифтів dotnet add package SpriteFontPlus
using Arcade2D.Entities;
using Arcade2D.Utils;
using Arcade2D.Managers;

namespace Arcade2D;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Player _player;
    private List<Ghost> _ghosts;
    private List<Wall> _walls;
    private List<Pellet> _pellets;

    private EntityManager _entityManager;
    private GameState _currentState;

    private Texture2D _pixel;
    private SpriteFont _gameFont;

    private int _score;
    private Texture2D _dimTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _entityManager = new EntityManager();

        string fontPath = @"/System/Library/Fonts/Supplemental/Courier New.ttf"; 
        
        if (!File.Exists(fontPath))
        {
            fontPath = @"/System/Library/Fonts/Supplemental/Arial.ttf";
        }

        if (File.Exists(fontPath))
        {
            byte[] fontData = File.ReadAllBytes(fontPath);
            _gameFont = TtfFontBaker.Bake(fontData, 24, 1024, 1024, new[] { CharacterRange.BasicLatin })
                                    .CreateSpriteFont(GraphicsDevice);
        }
        else
        {
            if (File.Exists("Arial.ttf"))
            {
                _gameFont = TtfFontBaker.Bake(File.ReadAllBytes("Arial.ttf"), 24, 1024, 1024, new[] { CharacterRange.BasicLatin })
                                        .CreateSpriteFont(GraphicsDevice);
            }
        }

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { ColorPalette.PlayerPink });

        _dimTexture = new Texture2D(GraphicsDevice, 1, 1);
        _dimTexture.SetData(new[] { new Color(0, 0, 0, 180) });

        RestartGame();
    }

    private void RestartGame()
    {
        _score = 0;
        _currentState = GameState.Playing;
        _entityManager.Clear();

        Texture2D wallTexture = new Texture2D(GraphicsDevice, 1, 1);
        wallTexture.SetData(new[] { ColorPalette.Lavender });

        Texture2D pelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        pelletTexture.SetData(new[] { ColorPalette.SoftYellow });

        Texture2D ghostTexture = new Texture2D(GraphicsDevice, 1, 1);
        ghostTexture.SetData(new[] { ColorPalette.NeonPink });

        _player = new Player(new Vector2(36, 36), _pixel);
        _entityManager.Add(_player);

        _ghosts = new List<Ghost>
        {
            new(new Vector2(32 * 13, 32 * 1), ghostTexture),
            new(new Vector2(32 * 1, 32 * 19), ghostTexture),
            new(new Vector2(32 * 26, 32 * 11), ghostTexture)
        };
        foreach (var ghost in _ghosts) _entityManager.Add(ghost);

        _walls = new List<Wall>();
        _pellets = new List<Pellet>();

        int[,] map =
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1},
            {1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,1},
            {1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,0,1,0,1},
            {1,0,0,0,0,0,1,0,1,0,0,0,1,0,1,0,0,0,1,0,1,0,0,0,0,1,0,1},
            {1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,1,0,1,0,1},
            {1,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,1,0,0,0,1},
            {1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
            {1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,1},
            {1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1},
            {1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1},
            {1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,1,0,1},
            {1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,1,0,1},
            {1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,1,0,1,0,1},
            {1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,1,0,1},
            {1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,1,0,1,0,1},
            {1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0,1},
            {1,1,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        int tileSize = 32;
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                Vector2 pos = new Vector2(col * tileSize, row * tileSize);
                if (map[row, col] == 1)
                {
                    var wall = new Wall(pos, wallTexture);
                    _walls.Add(wall);
                    _entityManager.Add(wall);
                }
                else if (map[row, col] == 0)
                {
                    if ((row + col) % 2 == 0)
                    {
                        var pellet = new Pellet(pos + new Vector2(10, 10), pelletTexture);
                        _pellets.Add(pellet);
                        _entityManager.Add(pellet);
                    }
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (_currentState == GameState.Playing)
        {
            _player.Update(gameTime, _walls);

            foreach (Ghost ghost in _ghosts)
            {
                ghost.Update(gameTime, _walls);

                if (_player.Bounds.Intersects(ghost.Bounds))
                {
                    _currentState = GameState.GameOver;
                }
            }

            for (int i = _pellets.Count - 1; i >= 0; i--)
            {
                if (_player.Bounds.Intersects(_pellets[i].Bounds))
                {
                    _entityManager.DestroyEntity(_pellets[i]);
                    
                    _pellets.RemoveAt(i);
                    _score++;
                }
            }

            if (_pellets.Count == 0)
            {
                _currentState = GameState.Victory;
            }
        }
        else
        {
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                RestartGame();
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorPalette.Background);

        _spriteBatch.Begin();

        _entityManager.Draw(_spriteBatch);

        if (_gameFont != null)
        {
            _spriteBatch.DrawString(_gameFont, $"SCORE: {_score}", new Vector2(930, 40), ColorPalette.SoftYellow);
            _spriteBatch.DrawString(_gameFont, $"PELLETS LEFT: {_pellets.Count}", new Vector2(930, 80), ColorPalette.Lavender);
        }

        if (_currentState == GameState.GameOver)
        {
            _spriteBatch.Draw(_dimTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            if (_gameFont != null)
            {
                _spriteBatch.DrawString(_gameFont, "GAME OVER", new Vector2(540, 300), ColorPalette.NeonPink);
                _spriteBatch.DrawString(_gameFont, "Press ENTER to Restart", new Vector2(490, 350), Color.White);
            }
        }
        else if (_currentState == GameState.Victory)
        {
            _spriteBatch.Draw(_dimTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            if (_gameFont != null)
            {
                _spriteBatch.DrawString(_gameFont, "YOU WIN!", new Vector2(550, 300), ColorPalette.SoftYellow);
                _spriteBatch.DrawString(_gameFont, "Press ENTER to Play Again", new Vector2(470, 350), Color.White);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}