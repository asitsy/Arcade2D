using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade2D.Entities;

public class PowerPellet : Pellet
{
    public PowerPellet(
        Vector2 position,
        Texture2D texture)
        : base(position, texture)
    {
    }

    public override Rectangle Bounds =>
        new(
            (int)Position.X,
            (int)Position.Y,
            12,
            12
        );
}