using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Managers; // Додано для коліжн менеджера

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
    private readonly Vector2 _mapOffset = new Vector2(16, 16); 

    public Ghost(Vector2 position, Texture2D texture) : base(position)
    {
        Texture = texture;
        Speed = 120f;
    }

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, 24, 24);

    public void Update(GameTime gameTime, CollisionManager collisionManager)     // Замінено List<Wall> на CollisionManager
    {
        Vector2 nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Rectangle nextBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);

        bool collision = collisionManager.CheckWallCollision(nextBounds);         // Перевіряємо стіну попереду через менеджер

        Point currentTile = new Point(
            (int)(Position.X - _mapOffset.X) / 32, 
            (int)(Position.Y - _mapOffset.Y) / 32
        );

        if (currentTile != _lastTile || collision)
        {
            _lastTile = currentTile;
            ChooseNewDirection(collisionManager, currentTile, collision);
        }

        if (!collision)
        {
            Position = nextPosition;
        }
    }

    private void ChooseNewDirection(CollisionManager collisionManager, Point currentTile, bool allowReverse)     // Замінено List<Wall> на CollisionManager
    {
        Vector2 reverseDirection = -_direction;
        Vector2 tileCenterPosition = new Vector2(currentTile.X * 32, currentTile.Y * 32) + _mapOffset;
        tileCenterPosition += new Vector2(4, 4);

        var availableDirections = _directions
            .Where(dir => 
            {
                Vector2 targetPosition = tileCenterPosition + dir * 32;
                Rectangle testBounds = new((int)targetPosition.X, (int)targetPosition.Y, Bounds.Width, Bounds.Height);
                
                return !collisionManager.CheckWallCollision(testBounds);                 // Перевірка стіни через коліжн-менеджер
            })
            .Where(dir => allowReverse || dir != reverseDirection)
            .ToList();

        if (!availableDirections.Any() && !allowReverse)
        {
            availableDirections.Add(reverseDirection);
        }

        if (availableDirections.Any())
        {
            _direction = availableDirections.ElementAt(_random.Next(availableDirections.Count));
            
            if (_direction.X != 0) Position = new Vector2(Position.X, tileCenterPosition.Y);
            if (_direction.Y != 0) Position = new Vector2(tileCenterPosition.X, Position.Y);
        }
    }

    public override void Update(GameTime gameTime)
    {
        
    }
}