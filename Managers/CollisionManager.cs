using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Arcade2D.Entities;

namespace Arcade2D.Managers;

public class CollisionManager
{
    private readonly EntityManager _entityManager;

    public CollisionManager(EntityManager entityManager)
    {
        _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
    }

    public bool CheckWallCollision(Rectangle bounds) 
    {
        return _entityManager.GetEntities<Wall>().Any(wall => bounds.Intersects(wall.Bounds));
    }

    public bool IsCollidingWithWall(Rectangle bounds) => CheckWallCollision(bounds);

    public bool UpdateGameplayCollisions(Player player, ScoreManager scoreManager, ref float freezeTimer, ref float speedTimer)
    {
        if (freezeTimer <= 0f)
        {
            if (_entityManager.GetEntities<Ghost>().Any(ghost => player.Bounds.Intersects(ghost.Bounds)))
            {
                return true; 
            }
        }

        var collidedPowerPellet = _entityManager.GetEntities<PowerPellet>()
            .FirstOrDefault(p => player.Bounds.Intersects(p.Bounds));
            
        if (collidedPowerPellet != null)
        {
            _entityManager.DestroyEntity(collidedPowerPellet);
            scoreManager.AddScore(10);
            freezeTimer = 5f; 
            return false; 
        }

        var collidedSpeedPellet = _entityManager.GetEntities<SpeedPellet>()
            .FirstOrDefault(p => player.Bounds.Intersects(p.Bounds));

        if (collidedSpeedPellet != null)
        {
            _entityManager.DestroyEntity(collidedSpeedPellet);
            scoreManager.AddScore(15);      
            speedTimer = 5f;  
            return false;
        }

        var collidedPellet = _entityManager.GetEntities<Pellet>()
            .FirstOrDefault(p => p is not PowerPellet && p is not SpeedPellet && player.Bounds.Intersects(p.Bounds));
            
        if (collidedPellet != null)
        {
            _entityManager.DestroyEntity(collidedPellet);
            scoreManager.AddScore(5);
            return false;
        }

        return false;
    }
}