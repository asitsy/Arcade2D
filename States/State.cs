using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D; 

namespace Arcade2D.States;

public abstract class State
{
    protected Game1 Game;
    protected GraphicsDevice GraphicsDevice;

    protected State(Game1 game, GraphicsDevice graphicsDevice)
    {
        Game = game;
        GraphicsDevice = graphicsDevice;
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}