using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Managers;

namespace Arcade2D.Entities;

public class Player : Entity
{
    private Vector2 _currentDirection = new Vector2(-1, 0); 
    private float _animationTimer = 0f;
    private int _animationFrame = 0;
    private bool _isMoving = false; 
    
    // Змінна для зберігання точки спавну
    private Vector2 _startPosition; 

    public Player(Vector2 position, Texture2D texture) : base(position)
    {
        Texture = texture;
        Speed = 200f; 
        _startPosition = position; // Запам'ятовуємо, де з'явився гравець
    }

    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 24, 24);

    public void SetSpedUp(bool isSpedUp)
    {
        Speed = isSpedUp ? 320f : 200f;
    }

    // Метод для повернення на базу при втраті життя
    public void ResetPosition()
    {
        Position = _startPosition;
        _currentDirection = new Vector2(-1, 0);
        _isMoving = false;
    }

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
            // Нормалізуємо вектор, щоб рух по діагоналі не був швидшим
            direction.Normalize(); 
            _currentDirection = direction;
        }
        else
        {
            _isMoving = false;
        }

        // Анімація та РУХ відбуваються ТІЛЬКИ тоді, коли затиснута клавіша
        if (_isMoving)
        {
            _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_animationTimer > 0.1f)
            {
                _animationFrame = (_animationFrame + 1) % 2;
                _animationTimer = 0f;
            }

            Vector2 nextPosition = Position + _currentDirection * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, 24, 24);

            if (!collisionManager.CheckWallCollision(nextBounds))
            {
                Position = nextPosition;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null)
        {
            int size = 16; 
            int startX = 456;    
            int startY = 0;

            if (!_isMoving)
            {
                Rectangle closedMouthRect = new Rectangle(488, 0, size, size);
                spriteBatch.Draw(Texture, Bounds, closedMouthRect, Color.White);
                return;
            }

            if (Math.Abs(_currentDirection.X) > Math.Abs(_currentDirection.Y))
            {
                if (_currentDirection.X > 0) startY = 0;          
                else startY = size;                               
            }
            else
            {
                if (_currentDirection.Y < 0) startY = size * 2;   
                else startY = size * 3;                           
            }

            int currentFrameX = startX + (_animationFrame * size);
            Rectangle sourceRect = new Rectangle(currentFrameX, startY, size, size);

            spriteBatch.Draw(Texture, Bounds, sourceRect, Color.White);
        }
        else
        {
            base.Draw(spriteBatch);
        }
    }
}