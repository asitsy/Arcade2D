using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Utils;
using Arcade2D; 

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
        // Спочатку малюємо ігрове поле на задньому плані, щоб воно було видне під затемненням
        Game.GameplayStateInstance.Draw(gameTime, spriteBatch);

        spriteBatch.Draw(Game.DimTexture, new Rectangle(0, 0, 904, 704), Color.White);
        if (Game.GameFont != null)
        {
            spriteBatch.DrawString(Game.GameFont, "GAME OVER", new Vector2(260, 300), ColorPalette.NeonPink);
            spriteBatch.DrawString(Game.GameFont, "Press ENTER to Restart", new Vector2(200, 350), Color.White);
        }
    }
}