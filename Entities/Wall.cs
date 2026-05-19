using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade2D.Entities;

public class Wall : Entity
{
    public Wall(Vector2 position, Texture2D texture)
        : base(position)
    {
        Texture = texture;
    }

    public override void Update(GameTime gameTime)
    {
    }
}