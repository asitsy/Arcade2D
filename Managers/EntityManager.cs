using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Interfaces;
using Arcade2D.Entities; // додано для доступу до базового класу Entity

namespace Arcade2D.Managers;

public class EntityManager
{
    private readonly List<Entity> _entities = new();     // тут головний список, який зберігає абсолютно всі сутності в грі
    
    private readonly List<IUpdatable> _updatableEntities = new();
    private readonly List<IRenderable> _renderableEntities = new();

    public List<T> GetEntities<T>() where T : Entity     // тут узагальнений (Generic) метод для отримання списку сутностей конкретного типу
    {
        List<T> result = new List<T>();
        foreach (var entity in _entities)
        {
            if (entity is T typedEntity)
            {
                result.Add(typedEntity);
            }
        }
        return result;
    }

    public void Add(Entity entity)
    {
        if (!_entities.Contains(entity))
        {
            _entities.Add(entity);
        }
        
        if (entity is IUpdatable updatable && !_updatableEntities.Contains(updatable)) 
            _updatableEntities.Add(updatable);
            
        if (entity is IRenderable renderable && !_renderableEntities.Contains(renderable)) 
            _renderableEntities.Add(renderable);
    }

    public void DestroyEntity(Entity entity)
    {
        _entities.Remove(entity);
        
        if (entity is IUpdatable updatable) _updatableEntities.Remove(updatable);
        if (entity is IRenderable renderable) _renderableEntities.Remove(renderable);
    }

    public void Clear()
    {
        _entities.Clear();
        _updatableEntities.Clear();
        _renderableEntities.Clear();
    }

    public void Update(GameTime gameTime)
    {
        for (int i = _updatableEntities.Count - 1; i >= 0; i--)
        {
            if (i < _updatableEntities.Count)
            {
                _updatableEntities[i].Update(gameTime);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _renderableEntities)
        {
            entity.Draw(spriteBatch);
        }
    }
}