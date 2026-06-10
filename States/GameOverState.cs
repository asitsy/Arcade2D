using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Utils;

namespace Arcade2D.States;

public class GameOverState : State
{
    public GameOverState(Game1 game, GraphicsDevice graphicsDevice) : base(game, graphicsDevice)
    {
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
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
            // Математичне центрування кожного рядка відносно ширини екрана (904)
            string t1 = "GAME OVER";
            float x1 = (904 - Game.GameFont.MeasureString(t1).X) / 2;
            spriteBatch.DrawString(Game.GameFont, t1, new Vector2(x1, 230), ColorPalette.NeonPink);
            
            string t2 = $"FINAL SCORE: {Game.ScoreManagerInstance.Score}";
            float x2 = (904 - Game.GameFont.MeasureString(t2).X) / 2;
            spriteBatch.DrawString(Game.GameFont, t2, new Vector2(x2, 300), ColorPalette.SoftYellow);
            
            string t3 = $"LAST SCORE:  {Game.ScoreManagerInstance.LastScore}";
            float x3 = (904 - Game.GameFont.MeasureString(t3).X) / 2;
            spriteBatch.DrawString(Game.GameFont, t3, new Vector2(x3, 350), Color.MediumSeaGreen);

            string t4 = "Press ENTER to Restart";
            float x4 = (904 - Game.GameFont.MeasureString(t4).X) / 2;
            spriteBatch.DrawString(Game.GameFont, t4, new Vector2(x4, 440), Color.White);
        }
    }
}