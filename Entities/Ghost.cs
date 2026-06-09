using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Managers;

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

    private int _ghostType = 0; 
    private float _animationTimer = 0f;
    private int _animationFrame = 0;

    // Автоматичний лічильник: гарантує, що привиди будуть різнокольоровими
    // навіть якщо в Game1 або GameplayState забули передати ID кольору.
    private static int _globalGhostCounter = 0;

    // значення за замовчуванням встановлено в -1 для увімкнення авто-розподілу
    public Ghost(Vector2 position, Texture2D texture, int ghostType = -1) : base(position)
    {
        Texture = texture;
        
        if (ghostType == -1)
        {
            // Якщо тип не вказано, автоматично циклічно видаємо: 0, 1, 2, 3
            _ghostType = _globalGhostCounter % 4;
            _globalGhostCounter++;
        }
        else
        {
            _ghostType = ghostType;
        }

        Speed = 120f;
    }

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, 24, 24);

    public void Update(GameTime gameTime, CollisionManager collisionManager)     
    {
        _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_animationTimer > 0.15f)
        {
            _animationFrame = (_animationFrame + 1) % 2; 
            _animationTimer = 0f;
        }

        Vector2 nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Rectangle nextBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);

        bool collision = collisionManager.CheckWallCollision(nextBounds);         

        if (collision)
        {
            Point currentTile = new((int)((Position.X - _mapOffset.X) / 32), (int)((Position.Y - _mapOffset.Y) / 32));
            ChooseNewDirection(collisionManager, currentTile, true);
            
            nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            nextBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);
            
            if (collisionManager.CheckWallCollision(nextBounds)) 
            {
                return; 
            }
        }

        Point tile = new((int)((Position.X - _mapOffset.X) / 32), (int)((Position.Y - _mapOffset.Y) / 32));
        if (tile != _lastTile)
        {
            _lastTile = tile;
            if (_random.Next(100) < 50) 
            {
                ChooseNewDirection(collisionManager, tile, false);
            }
        }

        nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        nextBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);

        if (!collisionManager.CheckWallCollision(nextBounds))
        {
            Position = nextPosition;
        }
    }

    private void ChooseNewDirection(CollisionManager collisionManager, Point currentTile, bool allowReverse)     
    {
        Vector2 reverseDirection = -_direction;
        Vector2 tileCenterPosition = new Vector2(currentTile.X * 32, currentTile.Y * 32) + _mapOffset;
        tileCenterPosition += new Vector2(4, 4);

        var availableDirections = _directions
            .Where(dir => 
            {
                Vector2 targetPosition = tileCenterPosition + dir * 32;
                Rectangle testBounds = new((int)targetPosition.X, (int)targetPosition.Y, Bounds.Width, Bounds.Height);
                
                return !collisionManager.CheckWallCollision(testBounds);                 
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null && Texture.Width > 1)
        {
            int size = 16; 
            int startX = 456; // Початок блоку персонажів
            
            // Математичний розрахунок Y-координати для кожного типу:
            // 0 - Червоний (Y=64), 1 - Рожевий (Y=80), 2 - Блакитний (Y=96), 3 - Помаранчевий (Y=112)
            int startY = 64 + (_ghostType * size);
            
            int directionOffsetX = 0;
            if (_direction.X > 0) directionOffsetX = 0;          
            else if (_direction.X < 0) directionOffsetX = size * 2; 
            else if (_direction.Y < 0) directionOffsetX = size * 4; 
            else if (_direction.Y > 0) directionOffsetX = size * 6; 

            int currentX = startX + directionOffsetX + (_animationFrame * size);

            Rectangle sourceRect = new Rectangle(currentX, startY, size, size);
            spriteBatch.Draw(Texture, Bounds, sourceRect, Color.White);
        }
        else
        {
            base.Draw(spriteBatch);
        }
    }
}