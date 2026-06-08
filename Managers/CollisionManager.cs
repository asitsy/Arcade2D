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

    public bool UpdateGameplayCollisions(Player player, ref int score, ref float freezeTimer, ref float speedTimer)
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
            score += 10;
            freezeTimer = 5f; 
            return false; 
        }

        var collidedSpeedPellet = _entityManager.GetEntities<SpeedPellet>()
            .FirstOrDefault(p => player.Bounds.Intersects(p.Bounds));

        if (collidedSpeedPellet != null)
        {
            _entityManager.DestroyEntity(collidedSpeedPellet);
            score += 15;      // За унікальний бонус дамо трохи більше балів
            speedTimer = 5f;  // 5 секунд підвищеної швидкості
            return false;
        }

        // Збір звичайних пелетів (Pellet)
        // Фільтруємо, щоб випадково не з'їсти PowerPellet або SpeedPellet як звичайну точку
        var collidedPellet = _entityManager.GetEntities<Pellet>()
            .Where(p => p is not PowerPellet && p is not SpeedPellet) 
            .FirstOrDefault(p => player.Bounds.Intersects(p.Bounds));
            
        if (collidedPellet != null)
        {
            _entityManager.DestroyEntity(collidedPellet);
            score += 1;
        }

        return false;
    }
}