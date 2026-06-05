using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade2D.Entities;

public class Ghost : Entity
{
    private Vector2 _direction = Vector2.UnitX;

    public Ghost(
        Vector2 position,
        Texture2D texture)
        : base(position)
    {
        Texture = texture;

        Speed = 120f;
    }

    public override Rectangle Bounds =>
        new(
            (int)Position.X,
            (int)Position.Y,
            24,
            24
        );

    public void Update(
        GameTime gameTime,
        List<Wall> walls)
    {
        Vector2 nextPosition =
            Position +
            _direction *
            Speed *
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        Rectangle nextBounds =
            new(
                (int)nextPosition.X,
                (int)nextPosition.Y,
                Bounds.Width,
                Bounds.Height
            );

        bool collision = false;

        foreach (Wall wall in walls)
        {
            if (nextBounds.Intersects(wall.Bounds))
            {
                collision = true;
                break;
            }
        }

        if (collision)
        {
            _direction *= -1;
        }
        else
        {
            Position = nextPosition;
        }
    }

    public override void Update(GameTime gameTime)
    {
    }
}