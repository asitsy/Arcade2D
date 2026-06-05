using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arcade2D.Interfaces;

namespace Arcade2D.Managers;

public class EntityManager
{
    private readonly List<IUpdatable> _updatableEntities = new();
    private readonly List<IRenderable> _renderableEntities = new();

    public void Add(object entity)
    {
        if (entity is IUpdatable updatable) _updatableEntities.Add(updatable);
        if (entity is IRenderable renderable) _renderableEntities.Add(renderable);
    }

    // Змінено назву на DestroyEntity, щоб уникнути будь-яких системних конфліктів методів розширення .NET
    public void DestroyEntity(object entity)
    {
        if (entity is IUpdatable updatable) _updatableEntities.Remove(updatable);
        if (entity is IRenderable renderable) _renderableEntities.Remove(renderable);
    }

    public void Clear()
    {
        _updatableEntities.Clear();
        _renderableEntities.Clear();
    }

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < _updatableEntities.Count; i++)
        {
            _updatableEntities[i].Update(gameTime);
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