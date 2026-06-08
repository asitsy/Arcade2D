using System;
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

// Щоб уникнути конфліктів імен типів
using XnaInput = Microsoft.Xna.Framework.Input;

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

    // Прямокутник кнопки PLAY по центру ігрової зони лабіринту
    private readonly Rectangle _playButtonRect = new Rectangle(230, 300, 260, 70);

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 904;
        _graphics.PreferredBackBufferHeight = 704;
        Content.RootDirectory = "Content";
        IsMouseVisible = true; // Курсор миші тепер видно
    }

    protected override void LoadContent()
    {
        // ПОМИЛКУ ВИПРАВЛЕНО ТУТ: тепер вираз чистий і коректний
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _entityManager = new EntityManager();
        _collisionManager = new CollisionManager(_entityManager);

        string fontPath = @"/System/Library/Fonts/Supplemental/Courier New.ttf"; 
        if (!File.Exists(fontPath)) fontPath = @"/System/Library/Fonts/Supplemental/Arial.ttf";

        if (File.Exists(fontPath))
        {
            byte[] fontData = File.ReadAllBytes(fontPath);
            _gameFont = TtfFontBaker.Bake(fontData, 22, 1024, 1024, new[] { CharacterRange.BasicLatin })
                                    .CreateSpriteFont(GraphicsDevice);
        }

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White }); 

        _dimTexture = new Texture2D(GraphicsDevice, 1, 1);
        _dimTexture.SetData(new[] { new Color(15, 15, 30, 215) }); // Глибокий стильний темно-синій колір затінення

        RestartGame();
    }

    private void RestartGame()
    {
        _score = 0;
        _freezeTimer = 0f;
        _currentState = GameState.StartMenu; 
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
        _player.Texture.SetData(new[] { ColorPalette.PlayerPink }); 

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
        MouseState mouseState = Mouse.GetState();

        if (_currentState == GameState.StartMenu)
        {
            // Перевіряємо клік лівою кнопкою миші всередині меж кнопки PLAY
            bool isMousePressed = mouseState.LeftButton == XnaInput.ButtonState.Pressed;
            bool isCursorOverButton = _playButtonRect.Contains(mouseState.X, mouseState.Y);

            if ((isMousePressed && isCursorOverButton) || keyboardState.IsKeyDown(Keys.Enter))
            {
                _currentState = GameState.Playing;
            }
        }
        else if (_currentState == GameState.Playing)
        {
            _player.Update(gameTime, _collisionManager);

            if (IsGhostsFrozen)
            {
                _freezeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            var currentGhosts = _entityManager.GetEntities<Ghost>(); 
            foreach (Ghost ghost in currentGhosts)
            {
                if (!IsGhostsFrozen)
                {
                    ghost.Update(gameTime, _collisionManager);
                }
            }

            bool hitByGhost = _collisionManager.UpdateGameplayCollisions(_player, ref _score, ref _freezeTimer);
            if (hitByGhost)
            {
                _currentState = GameState.GameOver;
            }

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
            foreach (Ghost ghost in _entityManager.GetEntities<Ghost>())
            {
                _spriteBatch.Draw(_pixel, ghost.Bounds, new Color(0, 150, 255, 100));
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

        // --- ВІДОБРАЖЕННЯ СТАРТОВОГО МЕНЮ ---
        if (_currentState == GameState.StartMenu)
        {
            _spriteBatch.Draw(_dimTexture, new Rectangle(0, 0, 904, 704), Color.White);

            if (_gameFont != null)
            {
                // Ефект плавної неонової зміни кольору (дихання кнопки)
                float colorPulse = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) + 1.0) / 2.0f;
                Color neonColor = Color.Lerp(ColorPalette.Lavender, ColorPalette.NeonPink, colorPulse);

                // Заливка тіла кнопки темнішим кольором
                _spriteBatch.Draw(_pixel, _playButtonRect, new Color(20, 20, 40, 245));

                // Малюємо рамку кнопки
                DrawBorder(_playButtonRect, 3, neonColor);

                // Напис "PLAY" чітко по центру кнопки
                _spriteBatch.DrawString(_gameFont, "PLAY", new Vector2(332, 322), neonColor);

                // Інформація про автора під кнопкою (відцентрована по горизонталі)
                string authorText = "Arcade2D made by Anastasiia Tsyban";
                _spriteBatch.DrawString(_gameFont, authorText, new Vector2(160, 480), ColorPalette.Lavender * 0.8f);
            }
        }
        else if (_currentState == GameState.GameOver)
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

    private void DrawBorder(Rectangle rectangle, int thickness, Color color)
    {
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
    }
}