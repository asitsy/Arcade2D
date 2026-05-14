using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Arcade2D.Entities;

public class Player : Entity
{
    public Player(Vector2 position, Texture2D texture)
        : base(position)
    {
        Texture = texture;
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            direction.Y -= 1;

        if (keyboard.IsKeyDown(Keys.S))
            direction.Y += 1;

        if (keyboard.IsKeyDown(Keys.A))
            direction.X -= 1;

        if (keyboard.IsKeyDown(Keys.D))
            direction.X += 1;

        if (direction != Vector2.Zero)
            direction.Normalize();

        Position += direction *
                    Speed *
                    (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}