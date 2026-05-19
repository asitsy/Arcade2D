using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Interfaces;

namespace Arcade2D.Entities;

public abstract class Entity :
    IUpdatable,
    IRenderable,
    ICollidable
{
    public Vector2 Position { get; set; }

    public Texture2D Texture { get; set; }

    public float Speed { get; set; } = 200f;

    protected Entity(Vector2 position)
    {
        Position = position;
    }

    public virtual Rectangle Bounds =>
        new(
            (int)Position.X,
            (int)Position.Y,
            32,
            32
        );

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            Texture,
            Bounds,
            Color.White
        );
    }

    public abstract void Update(GameTime gameTime);
}