using System.Collections.Generic;
using System.Linq; 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Interfaces;
using Arcade2D.Entities;

namespace Arcade2D.Managers;

public class EntityManager
{
    private readonly List<Entity> _entities = new();
    private readonly List<IUpdatable> _updatableEntities = new();
    private readonly List<IRenderable> _renderableEntities = new();

    public List<T> GetEntities<T>() where T : Entity     // це будезамість циклу foreach
    {
        return _entities.OfType<T>().ToList();
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