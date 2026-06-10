using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Utils;

namespace Arcade2D.States;

public class MenuState : State
{
    // Ширина вікна 904. Кнопка має ширину 260. (904 - 260) / 2 = 322 (ідеальний центр)
    private readonly Rectangle _playButtonRect = new Rectangle(322, 300, 260, 70);

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
            Game.RestartGame(); 
            Game.ChangeState(Game.GameplayStateInstance);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (Game.GameplayStateInstance != null)
            Game.GameplayStateInstance.Draw(gameTime, spriteBatch);

        spriteBatch.Draw(Game.DimTexture, new Rectangle(0, 0, 904, 704), Color.White);

        if (Game.GameFont != null)
        {
            float colorPulse = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) + 1.0) / 2.0f;
            Color neonColor = Color.Lerp(ColorPalette.Lavender, ColorPalette.NeonPink, colorPulse);

            spriteBatch.Draw(Game.PixelTexture, _playButtonRect, new Color(20, 20, 40, 245));
            DrawBorder(spriteBatch, _playButtonRect, 3, neonColor);
            
            string textPlay = "PLAY";
            Vector2 sizePlay = Game.GameFont.MeasureString(textPlay);
            float playX = _playButtonRect.X + (_playButtonRect.Width - sizePlay.X) / 2;
            float playY = _playButtonRect.Y + (_playButtonRect.Height - sizePlay.Y) / 2;
            spriteBatch.DrawString(Game.GameFont, textPlay, new Vector2(playX, playY), neonColor);

            string authorText = "Arcade2D made by Anastasiia Tsyban";
            Vector2 sizeAuthor = Game.GameFont.MeasureString(authorText);
            float authorX = (904 - sizeAuthor.X) / 2; // Центруємо відносно всього екрана (904)
            spriteBatch.DrawString(Game.GameFont, authorText, new Vector2(authorX, 480), ColorPalette.Lavender * 0.8f);
        }
    }

    private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, int thickness, Color color)
    {
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.Right - thickness, rectangle.Y, thickness, rectangle.Height), color);
        spriteBatch.Draw(Game.PixelTexture, new Rectangle(rectangle.X, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
    }
}