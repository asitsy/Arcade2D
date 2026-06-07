using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus; 
using Arcade2D.Entities;
using Arcade2D.Utils;
using Arcade2D.Managers;

namespace Arcade2D;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Player _player;
    private EntityManager _entityManager;
    private CollisionManager _collisionManager; 
    private GameState _currentState;

    private Texture2D _pixel;
    private SpriteFont _gameFont;

    private int _score;
    private Texture2D _dimTexture;

    private float _freezeTimer = 0f;
    private bool IsGhostsFrozen => _freezeTimer > 0f;

    private readonly Vector2 _mapOffset = new Vector2(16, 16);

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
        _entityManager = new EntityManager();
        _collisionManager = new CollisionManager(_entityManager); // Ініціалізація

        string fontPath = @"/System/Library/Fonts/Supplemental/Courier New.ttf"; 
        if (!File.Exists(fontPath)) fontPath = @"/System/Library/Fonts/Supplemental/Arial.ttf";

        if (File.Exists(fontPath))
        {
            byte[] fontData = File.ReadAllBytes(fontPath);
            _gameFont = TtfFontBaker.Bake(fontData, 22, 1024, 1024, new[] { CharacterRange.BasicLatin })
                                    .CreateSpriteFont(GraphicsDevice);
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
        _freezeTimer = 0f;
        _currentState = GameState.Playing;
        _entityManager.Clear();

        Texture2D wallTexture = new Texture2D(GraphicsDevice, 1, 1);
        wallTexture.SetData(new[] { ColorPalette.Lavender });

        Texture2D pelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        pelletTexture.SetData(new[] { ColorPalette.SoftYellow });

        Texture2D powerPelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        powerPelletTexture.SetData(new[] { ColorPalette.NeonPink });

        Texture2D ghostTexture = new Texture2D(GraphicsDevice, 1, 1);
        ghostTexture.SetData(new[] { ColorPalette.NeonPink });

        _player = new Player(new Vector2(32 * 10 + 4, 32 * 10 + 4) + _mapOffset, _pixel);
        _entityManager.Add(_player);

        _entityManager.Add(new Ghost(new Vector2(32 * 1, 32 * 1) + _mapOffset, ghostTexture));
        _entityManager.Add(new Ghost(new Vector2(32 * 19, 32 * 1) + _mapOffset, ghostTexture));
        _entityManager.Add(new Ghost(new Vector2(32 * 1, 32 * 19) + _mapOffset, ghostTexture));
        _entityManager.Add(new Ghost(new Vector2(32 * 19, 32 * 19) + _mapOffset, ghostTexture));

        int[,] map =
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1},
            {1,0,1,1,0,1,0,1,1,1,0,1,1,0,1,0,1,1,1,0,1},
            {1,0,1,0,0,0,0,0,0,1,0,1,0,0,0,0,1,0,0,0,1},
            {1,0,1,0,1,1,1,1,0,1,0,1,0,1,1,1,1,0,1,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
            {1,1,1,0,1,0,1,1,1,1,0,1,1,1,0,1,0,1,1,0,1},
            {1,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,1,0,1},
            {1,0,1,1,1,0,1,0,1,1,0,1,0,1,0,1,1,0,1,0,1},
            {1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1},
            {1,1,0,0,1,1,1,0,0,0,0,0,0,0,1,1,1,0,1,1,1},
            {1,0,0,0,1,0,0,0,1,1,0,1,1,0,0,0,1,0,0,0,1},
            {1,0,1,0,1,0,1,0,0,0,0,0,0,0,1,0,1,1,1,0,1},
            {1,0,1,0,0,0,1,1,1,1,0,1,1,1,1,0,0,0,1,0,1},
            {1,0,1,1,1,0,1,0,0,0,0,0,0,1,0,0,1,0,1,0,1},
            {1,0,0,0,0,0,1,0,1,1,1,1,0,1,0,1,1,0,0,0,1},
            {1,0,1,1,1,0,0,0,1,0,0,1,0,0,0,0,1,0,1,0,1},
            {1,0,1,0,0,0,1,0,1,0,0,1,1,1,1,0,1,0,1,0,1},
            {1,0,1,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,1},
            {1,0,0,0,0,0,0,0,1,1,1,1,1,1,0,1,1,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        int tileSize = 32;
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                Vector2 pos = new Vector2(col * tileSize, row * tileSize) + _mapOffset;
                if (map[row, col] == 1)
                {
                    _entityManager.Add(new Wall(pos, wallTexture));
                }
                else if (map[row, col] == 0)
                {
                    bool isPowerPellet = (row == 1 && col == 1) || (row == 3 && col == 12) || (row == 19 && col == 1) || (row == 16 && col == 19);
                    if (isPowerPellet)
                    {
                        _entityManager.Add(new PowerPellet(pos + new Vector2(8, 8), powerPelletTexture));
                    }
                    else if ((row + col) % 2 == 0)
                    {
                        if (!((row >= 9 && row <= 11) && (col >= 9 && col <= 11)))
                        {
                            _entityManager.Add(new Pellet(pos + new Vector2(10, 10), pelletTexture));
                        }
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
            _player.Update(gameTime, _collisionManager); // оновлення гравець

            if (IsGhostsFrozen)
            {
                _freezeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            var currentGhosts = _entityManager.GetEntities<Ghost>(); // оновлення для привідів
            foreach (Ghost ghost in currentGhosts)
            {
                if (!IsGhostsFrozen)
                {
                    ghost.Update(gameTime, _collisionManager);
                }
            }

            // Виклик менеджера колізій (тут була купаа старих циклів for)
            bool hitByGhost = _collisionManager.UpdateGameplayCollisions(_player, ref _score, ref _freezeTimer);
            if (hitByGhost)
            {
                _currentState = GameState.GameOver;
            }

            // Перевірка перемоги LINQ Any()
            if (!_entityManager.GetEntities<Pellet>().Any() && !_entityManager.GetEntities<PowerPellet>().Any())
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

        if (IsGhostsFrozen)
        {
            Texture2D freezeOverlay = new Texture2D(GraphicsDevice, 1, 1);
            freezeOverlay.SetData(new[] { new Color(0, 150, 255, 100) });
            foreach (Ghost ghost in _entityManager.GetEntities<Ghost>())
            {
                _spriteBatch.Draw(freezeOverlay, ghost.Bounds, Color.White);
            }
        }

        if (_gameFont != null)
        {
            _spriteBatch.DrawString(_gameFont, "ARCADE 2D", new Vector2(710, 30), Color.White);
            _spriteBatch.DrawString(_gameFont, "----------", new Vector2(710, 55), ColorPalette.Lavender);
            _spriteBatch.DrawString(_gameFont, $"SCORE: {_score}", new Vector2(710, 90), ColorPalette.SoftYellow);
            
            int totalPelletsLeft = _entityManager.GetEntities<Pellet>().Count + _entityManager.GetEntities<PowerPellet>().Count;
            _spriteBatch.DrawString(_gameFont, $"LEFT: {totalPelletsLeft}", new Vector2(710, 130), ColorPalette.Lavender);

            if (IsGhostsFrozen)
            {
                _spriteBatch.DrawString(_gameFont, "FREEZE:", new Vector2(710, 180), Color.Cyan);
                _spriteBatch.DrawString(_gameFont, $"{_freezeTimer:F1}s", new Vector2(710, 210), Color.Cyan);
            }
        }

        if (_currentState == GameState.GameOver)
        {
            _spriteBatch.Draw(_dimTexture, new Rectangle(0, 0, 904, 704), Color.White);
            if (_gameFont != null)
            {
                _spriteBatch.DrawString(_gameFont, "GAME OVER", new Vector2(260, 300), ColorPalette.NeonPink);
                _spriteBatch.DrawString(_gameFont, "Press ENTER to Restart", new Vector2(200, 350), Color.White);
            }
        }
        else if (_currentState == GameState.Victory)
        {
            _spriteBatch.Draw(_dimTexture, new Rectangle(0, 0, 904, 704), Color.White);
            if (_gameFont != null)
            {
                _spriteBatch.DrawString(_gameFont, "YOU WIN!", new Vector2(280, 300), ColorPalette.SoftYellow);
                _spriteBatch.DrawString(_gameFont, "Press ENTER to Play Again", new Vector2(190, 350), Color.White);
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}