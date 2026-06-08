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

    public bool CheckWallCollision(Rectangle bounds) // тут LINQ перевіряє, чи перетинається переданий прямокутник із будь-якою стіною
    {
        return _entityManager.GetEntities<Wall>().Any(wall => bounds.Intersects(wall.Bounds));
    }

    // Централізована обробка ігрових зіткнень гравця з об'єктами
    public bool UpdateGameplayCollisions(Player player, ref int score, ref float freezeTimer)
    {
        // Зіткнення з привидами (якщо вони не заморожені)
        if (freezeTimer <= 0f)
        {
            if (_entityManager.GetEntities<Ghost>().Any(ghost => player.Bounds.Intersects(ghost.Bounds)))
            {
                return true; 
            }
        }

        // Збір супер-пелетів (PowerPellet) тепер ПЕРШИМ!
        // Шукаємо тільки об'єкти, які є строго PowerPellet
        var collidedPowerPellet = _entityManager.GetEntities<PowerPellet>()
            .FirstOrDefault(p => player.Bounds.Intersects(p.Bounds));
            
        if (collidedPowerPellet != null)
        {
            _entityManager.DestroyEntity(collidedPowerPellet);
            score += 10;
            freezeTimer = 5f; 
            return false; // Повертаємось, щоб не обробляти його як звичайний пелет
        }

        // Збір звичайних пелетів (Pellet) за допомогою LINQ
        // Фільтруємо через .Where, щоб випадково не зачепити PowerPellet, які успадковані від Pellet
        var collidedPellet = _entityManager.GetEntities<Pellet>()
            .Where(p => p is not PowerPellet) 
            .FirstOrDefault(p => player.Bounds.Intersects(p.Bounds));
            
        if (collidedPellet != null)
        {
            _entityManager.DestroyEntity(collidedPellet);
            score++;
        }

        return false;
    }
}