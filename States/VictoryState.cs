using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arcade2D.Utils;
using Arcade2D; 

namespace Arcade2D.States;

public class VictoryState : State
{
    public VictoryState(Game1 game, GraphicsDevice graphicsDevice) : base(game, graphicsDevice)
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
        Game.GameplayStateInstance.Draw(gameTime, spriteBatch);

        spriteBatch.Draw(Game.DimTexture, new Rectangle(0, 0, 904, 704), Color.White);
        if (Game.GameFont != null)
        {
            spriteBatch.DrawString(Game.GameFont, "YOU WIN!", new Vector2(280, 300), ColorPalette.SoftYellow);
            spriteBatch.DrawString(Game.GameFont, "Press ENTER to Play Again", new Vector2(190, 350), Color.White);
        }
    }
}