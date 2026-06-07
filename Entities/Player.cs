using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Managers;

namespace Arcade2D.Entities;

public class Player : Entity
{
    public Player(Vector2 position, Texture2D texture)
        : base(position)
    {
        Texture = texture;
    }

    public override Rectangle Bounds =>
        new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            24,
            24
        );

    public void Update(GameTime gameTime, CollisionManager collisionManager) //параметр
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
        if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

        if (direction != Vector2.Zero)
            direction.Normalize();

        Vector2 nextPosition = Position + direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        Rectangle nextBounds = new Rectangle(
            (int)nextPosition.X,
            (int)nextPosition.Y,
            Bounds.Width,
            Bounds.Height
        );

        bool collides = collisionManager.CheckWallCollision(nextBounds);         // викликаємо перевірку стін через менеджер

        if (!collides)
        {
            Position = nextPosition;
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Базовий метод інтерфейсу залишається порожнім
    }
}