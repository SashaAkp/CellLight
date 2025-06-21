using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CellLight;

public class Camera
{
    public Vector2 position;
    public float zoom, maxzoom=10, minzoom=0.1f;
    public float velocity=3;
    public Camera(Vector2 position, float zoom)
    {
        this.position = position;this.zoom = zoom;
    }
    public Camera()
    {
        position = Vector2.Zero;zoom = 1;
    }
    public void Move(Vector2 direction)
    {
        position += direction/zoom*velocity;
    }
    public void Zoom(float mzoom)
    {
        zoom*=mzoom;
        if(zoom > maxzoom)
            zoom = maxzoom;
        if (zoom < minzoom)
            zoom = minzoom;
    }
    public void WorldObjUpdate(WorldObj obj)
    {
        obj.SUpdate(position, zoom);
    }
}
