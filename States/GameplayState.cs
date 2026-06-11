using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Entities;
using Arcade2D.Utils;

namespace Arcade2D.States;

public class GameplayState : State
{
    public float FreezeTimer { get; private set; } = 0f;
    public float PlayerSpeedTimer { get; private set; } = 0f;
    
    public int Lives { get; private set; } = 1; 

    public bool IsGhostsFrozen => FreezeTimer > 0f;
    public bool IsPlayerSpedUp => PlayerSpeedTimer > 0f;

    public GameplayState(Game1 game, GraphicsDevice graphicsDevice) : base(game, graphicsDevice)
    {
    }

    public void ResetTimers()
    {
        FreezeTimer = 0f;
        PlayerSpeedTimer = 0f;
        Lives = 1; 
    }

    public override void Update(GameTime gameTime)
    {
        if (IsPlayerSpedUp)
        {
            Game.PlayerInstance.SetSpedUp(true);
            PlayerSpeedTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            Game.PlayerInstance.SetSpedUp(false);
        }

        Game.PlayerInstance.Update(gameTime, Game.CollisionManagerInstance);

        if (IsGhostsFrozen)
        {
            FreezeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        var currentGhosts = Game.EntityManagerInstance.GetEntities<Ghost>(); 
        foreach (Ghost ghost in currentGhosts)
        {
            if (!IsGhostsFrozen)
            {
                ghost.Update(gameTime, Game.CollisionManagerInstance, Game.PlayerInstance);
            }
        }

        float currentFreeze = FreezeTimer;
        float currentSpeed = PlayerSpeedTimer;

        bool hitByGhost = Game.CollisionManagerInstance.UpdateGameplayCollisions(
            Game.PlayerInstance, Game.ScoreManagerInstance, ref currentFreeze, ref currentSpeed);
        
        FreezeTimer = currentFreeze;
        PlayerSpeedTimer = currentSpeed;

        if (hitByGhost)
        {
            if (Lives > 0)
            {
                Lives--; 
                Game.PlayerInstance.ResetPosition(); 
                FreezeTimer = 2f; 
                PlayerSpeedTimer = 0f; 
            }
            else
            {
                // ВИПРАВЛЕНО: Синхронне збереження перед зміною стану
                Game.ScoreManagerInstance.SaveLastScore();
                Game.ChangeState(Game.GameOverStateInstance);
                return;
            }
        }

        int remainingTargets = Game.EntityManagerInstance.GetEntities<Pellet>().Count + 
                               Game.EntityManagerInstance.GetEntities<PowerPellet>().Count + 
                               Game.EntityManagerInstance.GetEntities<SpeedPellet>().Count;

        if (remainingTargets == 0)
        {
            // ВИПРАВЛЕНО: Синхронне збереження перед зміною стану
            Game.ScoreManagerInstance.SaveLastScore();
            Game.ChangeState(Game.VictoryStateInstance);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Game.EntityManagerInstance.Draw(spriteBatch);

        if (Game.GameFont != null)
        {
            spriteBatch.DrawString(Game.GameFont, "ARCADE 2D", new Vector2(710, 30), Color.White);
            spriteBatch.DrawString(Game.GameFont, "----------", new Vector2(710, 55), ColorPalette.Lavender);
            spriteBatch.DrawString(Game.GameFont, $"SCORE: {Game.ScoreManagerInstance.Score}", new Vector2(710, 90), ColorPalette.SoftYellow);
            spriteBatch.DrawString(Game.GameFont, $"LAST:  {Game.ScoreManagerInstance.LastScore}", new Vector2(710, 125), Color.MediumSeaGreen);
            
            int totalPelletsLeft = Game.EntityManagerInstance.GetEntities<Pellet>().Count + 
                                   Game.EntityManagerInstance.GetEntities<PowerPellet>().Count + 
                                   Game.EntityManagerInstance.GetEntities<SpeedPellet>().Count;
            spriteBatch.DrawString(Game.GameFont, $"LEFT: {totalPelletsLeft}", new Vector2(710, 165), ColorPalette.Lavender);

            if (IsGhostsFrozen)
            {
                spriteBatch.DrawString(Game.GameFont, "FREEZE:", new Vector2(710, 215), Color.Cyan);
                spriteBatch.DrawString(Game.GameFont, $"{FreezeTimer:F1}s", new Vector2(710, 245), Color.Cyan);
            }

            if (IsPlayerSpedUp)
            {
                spriteBatch.DrawString(Game.GameFont, "SPEED UP:", new Vector2(710, 295), Color.Orange);
                spriteBatch.DrawString(Game.GameFont, $"{PlayerSpeedTimer:F1}s", new Vector2(710, 325), Color.Orange);
            }
            
            spriteBatch.DrawString(Game.GameFont, $"LIVES: {Lives}", new Vector2(710, 375), Color.Red);
        }
    }
}