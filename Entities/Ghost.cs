using System;
using System.Collections.Generic;
using System.Linq; // Використовуємо LINQ на повну
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade2D.Entities;

public class Ghost : Entity
{
    private readonly Random _random = new();
    private Vector2 _direction = new Vector2(1, 0);

    private readonly Vector2[] _directions =
    {
        new Vector2(0, -1), // Вгору
        new Vector2(0, 1),  // Вниз
        new Vector2(-1, 0), // Ліворуч
        new Vector2(1, 0)   // Праворуч
    };

    private Point _lastTile = new Point(-1, -1);
    
    // Зсув карти з вашого Game1.cs (відступи від країв екрану)
    private readonly Vector2 _mapOffset = new Vector2(16, 16); 

    public Ghost(Vector2 position, Texture2D texture) : base(position)
    {
        Texture = texture;
        Speed = 120f;
    }

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, 24, 24);

    public void Update(GameTime gameTime, List<Wall> walls)
    {
        Vector2 nextPosition = Position + _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Rectangle nextBounds = new((int)nextPosition.X, (int)nextPosition.Y, Bounds.Width, Bounds.Height);

        // LINQ: Перевіряємо, чи є стіна безпосередньо перед носом привида
        bool collision = walls.Any(wall => nextBounds.Intersects(wall.Bounds));

        // Визначаємо точний індекс плитки в масиві карти (0, 1, 2...) з урахуванням зсуву карти
        Point currentTile = new Point(
            (int)(Position.X - _mapOffset.X) / 32, 
            (int)(Position.Y - _mapOffset.Y) / 32
        );

        // Привид приймає рішення про поворот ТОЛЬКИ коли заходить на нову клітинку АБО якщо вперся в стіну
        if (currentTile != _lastTile || collision)
        {
            _lastTile = currentTile;
            ChooseNewDirection(walls, currentTile, collision);
        }

        // Рухаємось вперед, якщо шлях вільний
        if (!collision)
        {
            Position = nextPosition;
        }
    }

    private void ChooseNewDirection(List<Wall> walls, Point currentTile, bool allowReverse)
    {
        Vector2 reverseDirection = -_direction;

        // Математичний ідеальний центр плитки лабіринту, де ЗАРАЗ перебуває привид
        Vector2 tileCenterPosition = new Vector2(currentTile.X * 32, currentTile.Y * 32) + _mapOffset;
        
        // Корекція центру (розмір плитки 32х32, розмір привида 24х24. Різниця 8 пікселів, тобто зсув на 4 для центру)
        tileCenterPosition += new Vector2(4, 4);

        // СУПЕР-LINQ: Скануємо всі 4 напрямки від ІДЕАЛЬНОГО центру плитки.
        // Це повністю захищає від випадкового зачіпання стін сусідніх коридорів!
        var availableDirections = _directions
            .Where(dir => 
            {
                // Прораховуємо прямокутник колізії на наступній плитці
                Vector2 targetPosition = tileCenterPosition + dir * 32;
                Rectangle testBounds = new((int)targetPosition.X, (int)targetPosition.Y, Bounds.Width, Bounds.Height);
                
                // Напрямок вільний, якщо LINQ не знайшов жодної стіни на шляху
                return !walls.Any(wall => testBounds.Intersects(wall.Bounds));
            })
            .Where(dir => allowReverse || dir != reverseDirection) // Заборона розвороту на перехрестях
            .ToList();

        // Якщо привид у глухому куті — дозволяємо повернутися назад
        if (!availableDirections.Any() && !allowReverse)
        {
            availableDirections.Add(reverseDirection);
        }

        // Обираємо випадковий шлях із дійсно доступних варіантів
        if (availableDirections.Any())
        {
            _direction = availableDirections.ElementAt(_random.Next(availableDirections.Count));
            
            // ЗОЛОТЕ ПРАВИЛО РОЗУМНОГО РУХУ: Примусово вирівнюємо привида по осі повороту.
            // Якщо він повертає вбік (X), вирівнюємо його по висоті коридору (Y).
            // Якщо повертає вгору/вниз (Y), вирівнюємо його по ширині коридору (X).
            if (_direction.X != 0) Position = new Vector2(Position.X, tileCenterPosition.Y);
            if (_direction.Y != 0) Position = new Vector2(tileCenterPosition.X, Position.Y);
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Порожній через вимоги інтерфейсу
    }
}