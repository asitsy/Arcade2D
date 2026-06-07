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

    private Point _lastTile = new Point(-1, -1);

    public Ghost(Vector2 position, Texture2D texture) : base(position)
    {
        Texture = texture;
        Speed = 120f;
    }

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, 24, 24);

    public void Update(GameTime gameTime, List<Wall> walls)
    {
        Vector2 nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Rectangle nextBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);

        bool collision = false;
        foreach (Wall wall in walls)
        {
            if (nextBounds.Intersects(wall.Bounds))
            {
                collision = true;
                break;
            }
        }

        Point currentTile = new Point((int)Position.X / 32, (int)Position.Y / 32);

        bool alignedWithTile = ((int)Position.X % 32 == 0 && (int)Position.Y % 32 == 0) || 
                              ((int)Position.X % 32 <= 4 && (int)Position.Y % 32 <= 4); 

        if (collision)
        {
            ChooseNewDirection(walls, allowReverse: true);
        }
        else if (currentTile != _lastTile && alignedWithTile)
        {
            _lastTile = currentTile;
            ChooseNewDirection(walls, allowReverse: false);
            
            Position = new Vector2(currentTile.X * 32, currentTile.Y * 32);
            nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        Rectangle finalBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);
        bool finalCollision = false;
        foreach (Wall wall in walls)
        {
            if (finalBounds.Intersects(wall.Bounds))
            {
                finalCollision = true;
                break;
            }
        }

        if (!finalCollision)
        {
            Position = nextPosition;
        }
    }

    private void ChooseNewDirection(List<Wall> walls, bool allowReverse)
    {
        List<Vector2> availableDirections = new();
        Vector2 reverseDirection = -_direction; 

        foreach (Vector2 dir in _directions)
        {
            Vector2 testPosition = Position + dir * 32;
            Rectangle testBounds = new((int)testPosition.X, (int)testPosition.Y, Bounds.Width, Bounds.Height);

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
                // тут я хотіла зробити стратегічне правило: якщо ми на перехресті, не розвертаємось назад, 
                // щоб привид йшов ТІЛЬКИ вперед чи повертав набік
                // назад повертаємо лише якщо це тупик (allowReverse == true).
                if (dir == reverseDirection && !allowReverse)
                {
                    continue; 
                }
                availableDirections.Add(dir);
            }
        }

        if (availableDirections.Count == 0 && !allowReverse)
        {
            availableDirections.Add(reverseDirection);
        }

        if (availableDirections.Count > 0)
        {
            _direction = availableDirections[_random.Next(availableDirections.Count)];
        }
    }

    public override void Update(GameTime gameTime)
    {
    }
}