using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade2D.Entities;

public class Ghost : Entity
{
    private readonly Random _random = new();

    private Vector2 _direction = new Vector2(1, 0);

    private readonly Vector2[] _directions =
    {
        new Vector2(0, -1),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(1, 0)
    };

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
            ChangeDirection(walls);
        }
        else
        {
            Position = nextPosition;
        }
    }

    private void ChangeDirection(List<Wall> walls)
    {
        List<Vector2> availableDirections = new();

        foreach (Vector2 direction in _directions)
        {
            Vector2 testPosition =
                Position +
                direction * 32;

            Rectangle testBounds =
                new(
                    (int)testPosition.X,
                    (int)testPosition.Y,
                    Bounds.Width,
                    Bounds.Height
                );

            bool blocked = false;

            foreach (Wall wall in walls)
            {
                if (testBounds.Intersects(wall.Bounds))
                {
                    blocked = true;
                    break;
                }
            }

            if (!blocked)
            {
                availableDirections.Add(direction);
            }
        }

        if (availableDirections.Count > 0)
        {
            _direction =
                availableDirections[
                    _random.Next(availableDirections.Count)
                ];
        }
    }

    public override void Update(GameTime gameTime)
    {
    }
}