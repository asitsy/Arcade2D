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
            new Vector2(300, 300),
            _pixel
        );

        _walls = new List<Wall>()
        {
            new Wall(new Vector2(100, 100), wallTexture),
            new Wall(new Vector2(132, 100), wallTexture),
            new Wall(new Vector2(164, 100), wallTexture),
            new Wall(new Vector2(196, 100), wallTexture),
            new Wall(new Vector2(228, 100), wallTexture)
        };

        _pellets = new List<Pellet>()
        {
            new Pellet(new Vector2(300, 100), pelletTexture),
            new Pellet(new Vector2(350, 100), pelletTexture),
            new Pellet(new Vector2(400, 100), pelletTexture),
            new Pellet(new Vector2(450, 100), pelletTexture)
        };
    }

    protected override void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _walls);

        for (int i = _pellets.Count - 1; i >= 0; i--)
        {
            if (_player.Bounds.Intersects(_pellets[i].Bounds))
            {
                _pellets.RemoveAt(i);

                _score += 10;

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