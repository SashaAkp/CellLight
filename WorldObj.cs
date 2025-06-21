using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace CellLight;

public class WorldObj
{
    public Vector2 position, size, Sposition, Ssize, Screen;
    public WorldObj(Vector2 position, Vector2 size)
    {
        this.position = position;this.size = size;
        Sposition = position; Ssize = size;
    }
    public WorldObj()
    {
        position = Vector2.Zero;size = Vector2.One;
        Sposition = position; Ssize = size;
    }
    public void SUpdate(Vector2 camPos, float zoom)
    {
        Sposition = position-camPos*zoom+(position-Screen/2)*(zoom-1);
        Ssize = size*zoom;
    }
}
