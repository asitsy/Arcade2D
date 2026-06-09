using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Entities;
using Arcade2D.Utils;
using Arcade2D; 

namespace Arcade2D.States;

public class GameplayState : State
{
    public GameplayState(Game1 game, GraphicsDevice graphicsDevice) : base(game, graphicsDevice)
    {
    }

    public override void Update(GameTime gameTime)
    {
        if (Game.IsPlayerSpedUp)
        {
            Game.PlayerInstance.Speed = 320f;
            Game.PlayerSpeedTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            Game.PlayerInstance.Speed = 200f;
        }

        Game.PlayerInstance.Update(gameTime, Game.CollisionManagerInstance);

        if (Game.IsGhostsFrozen)
        {
            Game.FreezeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        var currentGhosts = Game.EntityManagerInstance.GetEntities<Ghost>(); 
        foreach (Ghost ghost in currentGhosts)
        {
            if (!Game.IsGhostsFrozen)
            {
                ghost.Update(gameTime, Game.CollisionManagerInstance);
            }
        }

        bool hitByGhost = Game.CollisionManagerInstance.UpdateGameplayCollisions(
            Game.PlayerInstance, ref Game.Score, ref Game.FreezeTimer, ref Game.PlayerSpeedTimer);
        
        if (hitByGhost)
        {
            Game.LastScore = Game.Score;
            _ = Game.SaveLastScoreAsync(Game.Score);
            Game.ChangeState(Game.GameOverStateInstance);
        }

        if (!Game.EntityManagerInstance.GetEntities<Pellet>().Any() && 
            !Game.EntityManagerInstance.GetEntities<PowerPellet>().Any() && 
            !Game.EntityManagerInstance.GetEntities<SpeedPellet>().Any())
        {
            Game.LastScore = Game.Score;
            _ = Game.SaveLastScoreAsync(Game.Score);
            Game.ChangeState(Game.VictoryStateInstance);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Game.EntityManagerInstance.Draw(spriteBatch);

        if (Game.IsGhostsFrozen)
        {
            foreach (Ghost ghost in Game.EntityManagerInstance.GetEntities<Ghost>())
            {
                spriteBatch.Draw(Game.PixelTexture, ghost.Bounds, new Color(0, 150, 255, 100));
            }
        }

        if (Game.GameFont != null)
        {
            spriteBatch.DrawString(Game.GameFont, "ARCADE 2D", new Vector2(710, 30), Color.White);
            spriteBatch.DrawString(Game.GameFont, "----------", new Vector2(710, 55), ColorPalette.Lavender);
            spriteBatch.DrawString(Game.GameFont, $"SCORE: {Game.Score}", new Vector2(710, 90), ColorPalette.SoftYellow);
            spriteBatch.DrawString(Game.GameFont, $"LAST:  {Game.LastScore}", new Vector2(710, 125), Color.MediumSeaGreen);
            
            int totalPelletsLeft = Game.EntityManagerInstance.GetEntities<Pellet>().Count + 
                                   Game.EntityManagerInstance.GetEntities<PowerPellet>().Count + 
                                   Game.EntityManagerInstance.GetEntities<SpeedPellet>().Count;
            spriteBatch.DrawString(Game.GameFont, $"LEFT: {totalPelletsLeft}", new Vector2(710, 165), ColorPalette.Lavender);

            if (Game.IsGhostsFrozen)
            {
                spriteBatch.DrawString(Game.GameFont, "FREEZE:", new Vector2(710, 215), Color.Cyan);
                spriteBatch.DrawString(Game.GameFont, $"{Game.FreezeTimer:F1}s", new Vector2(710, 245), Color.Cyan);
            }

            if (Game.IsPlayerSpedUp)
            {
                spriteBatch.DrawString(Game.GameFont, "SPEED UP:", new Vector2(710, 295), ColorPalette.Gold);
                spriteBatch.DrawString(Game.GameFont, $"{Game.PlayerSpeedTimer:F1}s", new Vector2(710, 325), ColorPalette.Gold);
            }
        }
    }
}