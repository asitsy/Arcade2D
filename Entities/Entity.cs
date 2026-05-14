using Game.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Entities;

public abstract class Entity : IUpdatable, IRenderable
{
    public Vector2 Position { get; set; }

    protected Texture2D Texture;

    protected float Speed = 100f;

    protected Entity(Vector2 position)
    {
        Position = position;
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}