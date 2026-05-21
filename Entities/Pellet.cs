using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade2D.Entities;

public class Pellet : Entity
{
    public Pellet(Vector2 position, Texture2D texture)
        : base(position)
    {
        Texture = texture;
    }

    public override Rectangle Bounds =>
        new(
            (int)Position.X,
            (int)Position.Y,
            6,
            6
        );

    public override void Update(GameTime gameTime)
    {
    }
}