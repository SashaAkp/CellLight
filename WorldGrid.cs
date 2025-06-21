using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CellLight;

public class WorldGrid : WorldObj
{
    public Vector2 bounds;
    public Rectangle rect;
    public Rectangle Srect;
    public WorldGrid(Vector2 position, Vector2 bounds, Vector2 Screen)
    {
        this.position = position;this.bounds = bounds;this.Screen=Screen;
        rect = new Rectangle((int)position.X, (int)position.Y, (int)bounds.X, (int)bounds.Y);
        Srect = new Rectangle((int)position.X, (int)position.Y, (int)(bounds.X*size.X), (int)(bounds.Y * size.Y));
    }
    public void SUpdateGrid()
    {
        Srect = new Rectangle((int)(Sposition.X), (int)(Sposition.Y), (int)(bounds.X*Ssize.X), (int)(bounds.Y*Ssize.Y));
    }
}
