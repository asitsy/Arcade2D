using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Entities;
using Arcade2D.Utils;

namespace Arcade2D;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Player _player;
    private List<Wall> _walls;
    private List<Pellet> _pellets;

    private Texture2D _pixel;

    private int _score;

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

        _pixel = new Texture2D(GraphicsDevice, 1, 1);

        _pixel.SetData(new[]
        {
            ColorPalette.PlayerPink
        });

        Texture2D wallTexture =
            new Texture2D(GraphicsDevice, 1, 1);

        wallTexture.SetData(new[]
        {
            ColorPalette.Lavender
        });

        Texture2D pelletTexture =
            new Texture2D(GraphicsDevice, 1, 1);

        pelletTexture.SetData(new[]
        {
            ColorPalette.SoftYellow
        });

        _player = new Player(
            new Vector2(96, 96),
            _pixel
        );

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
                Vector2 position =
                    new Vector2(
                        col * tileSize,
                        row * tileSize
                    );

                if (map[row, col] == 1)
                {
                    _walls.Add(
                        new Wall(position, wallTexture)
                    );
                }
                else if (map[row, col] == 0)
                {
                    if ((row + col) % 2 == 0)
                    {
                        _pellets.Add(
                            new Pellet(
                                position + new Vector2(10, 10),
                                pelletTexture
                            )
                        );
                    }
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _walls);

        for (int i = _pellets.Count - 1; i >= 0; i--)
        {
            if (_player.Bounds.Intersects(_pellets[i].Bounds))
            {
                _pellets.RemoveAt(i);

                _score += 1;

                System.Console.WriteLine($"Score: {_score}");
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorPalette.Background);

        _spriteBatch.Begin();

        foreach (Wall wall in _walls)
        {
            wall.Draw(_spriteBatch);
        }

        foreach (Pellet pellet in _pellets)
        {
            pellet.Draw(_spriteBatch);
        }

        _player.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}