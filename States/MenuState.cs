using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Utils;
using Arcade2D; 

namespace Arcade2D.States;

public class MenuState : State
{
    private readonly Rectangle _playButtonRect = new Rectangle(230, 300, 260, 70);

    public MenuState(Game1 game, GraphicsDevice graphicsDevice) : base(game, graphicsDevice)
    {
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;
        bool isCursorOverButton = _playButtonRect.Contains(mouseState.X, mouseState.Y);

        if ((isMousePressed && isCursorOverButton) || keyboardState.IsKeyDown(Keys.Enter))
        {
            Game.ChangeState(Game.GameplayStateInstance);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // ДОДАНО: Спочатку малюємо лабіринт на задньому плані, щоб його було видно під затемненням
        Game.GameplayStateInstance.Draw(gameTime, spriteBatch);

        // Малюємо наше круте напівпрозоре тло поверх лабіринту
        spriteBatch.Draw(Game.DimTexture, new Rectangle(0, 0, 904, 704), Color.White);

        if (Game.GameFont != null)
        {
            float colorPulse = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) + 1.0) / 2.0f;
            Color neonColor = Color.Lerp(ColorPalette.Lavender, ColorPalette.NeonPink, colorPulse);

            spriteBatch.Draw(Game.PixelTexture, _playButtonRect, new Color(20, 20, 40, 245));
            DrawBorder(spriteBatch, _playButtonRect, 3, neonColor);
            spriteBatch.DrawString(Game.GameFont, "PLAY", new Vector2(332, 322), neonColor);

            string authorText = "Arcade2D made by Anastasiia Tsyban";
            spriteBatch.DrawString(Game.GameFont, authorText, new Vector2(160, 480), ColorPalette.Lavender * 0.8f);
        }
    }

    private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, int thickness, Color color)
    {
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
    }
}