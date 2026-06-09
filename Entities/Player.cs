using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Managers;
using Arcade2D.Utils;

namespace Arcade2D.Entities;

public class Player : Entity
{
    private Vector2 _currentDirection = new Vector2(-1, 0); 
    private float _animationTimer = 0f;
    private int _animationFrame = 0;
    private bool _isMoving = false; // Змінна для відстеження стану руху

    public Player(Vector2 position, Texture2D texture) : base(position)
    {
        Texture = texture;
    }

    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 24, 24);

    public void Update(GameTime gameTime, CollisionManager collisionManager) 
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
        if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

        if (direction != Vector2.Zero)
        {
            _isMoving = true;
            direction.Normalize();
            _currentDirection = direction; 
            
            _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_animationTimer > 0.07f) 
            {
                // Почергово перемикаємо кадри: 0 (повністю відкритий) -> 1 (напіввідкритий)
                _animationFrame = (_animationFrame + 1) % 2; 
                _animationTimer = 0f;
            }
        }
        else
        {
            _isMoving = false; // Гравець зупинився
        }

        Vector2 nextPosition = Position + direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);

        bool collides = collisionManager.CheckWallCollision(nextBounds);         

        if (!collides)
        {
            Position = nextPosition;
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
            int startX = 456;    
            int startY = 0;

            // Якщо Пакман СТОЇТЬ на місці, малюємо його як ідеальне закрите коло.
            // В оригіналі цей загальний кадр знаходиться в першому рядку на X = 488, Y = 0
            if (!_isMoving)
            {
                Rectangle closedMouthRect = new Rectangle(488, 0, size, size);
                spriteBatch.Draw(Texture, Bounds, closedMouthRect, Color.White);
                return;
            }

            // Якщо він рухається, вираховуємо рядок (Y) залежно від напрямку руху
            if (Math.Abs(_currentDirection.X) > Math.Abs(_currentDirection.Y))
            {
                if (_currentDirection.X > 0) startY = 0;          // Рядок 1 (Y=0): Вправо
                else startY = size;                               // Рядок 2 (Y=16): Вліво
            }
            else
            {
                if (_currentDirection.Y < 0) startY = size * 2;   // Рядок 3 (Y=32): Вгору
                else startY = size * 3;                           // Рядок 4 (Y=48): Вниз
            }

            // Зміщення по X для кадрів руху (0 або 1)
            int currentFrameX = startX + (_animationFrame * size);
            Rectangle sourceRect = new Rectangle(currentFrameX, startY, size, size);

            spriteBatch.Draw(Texture, Bounds, sourceRect, Color.White);
        }
        else if (Texture != null)
        {
            spriteBatch.Draw(Texture, Bounds, ColorPalette.SoftYellow);
        }
    }
}