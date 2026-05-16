using Microsoft.Xna.Framework;

namespace Arcade2D.Interfaces;

public interface ICollidable
{
    Rectangle Bounds { get; }
}