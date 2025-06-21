using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;

namespace CellLight;

public class GridEditor
{
    WorldGrid dest;
    Camera camera;
    Rectangle source;
    Vector2 mpos, mcellpos;
    float simX, simY, Nvalue=1;
    int brushID=0;
    /*      0-field
            1-wall
            2_-absorbtion:
                1-down
                2-right
                3-up
                4-left
            >=30-source*/
    public GridEditor(Rectangle source, WorldGrid dest, Camera camera)
    {
        this.source = source; this.dest = dest;this.camera = camera;
        simX = source.Width / dest.bounds.X;
        simY = source.Height / dest.bounds.Y;
    }
    public int[,] ChangeCell(int[,] valStat, float[,] valN, Vector2 Smpos)
    {
        mpos=(Smpos+camera.position*camera.zoom+dest.Screen*(camera.zoom-1)/2)/(camera.zoom);
        mpos -= dest.position;
        mcellpos = new Vector2(0.1f*(int)(mpos.X * simX*10), 0.1f*(int)(mpos.Y * simY*10));
        if(mcellpos.X<=source.Width-1 && mcellpos.Y <= source.Height-1 && mcellpos.X >=1 && mcellpos.Y >= 1)
        {
            valStat[(int)mcellpos.X, (int)mcellpos.Y] = brushID;
            if (brushID == 0)
                valN[(int)mcellpos.X, (int)mcellpos.Y] = Nvalue;
        }
        return valStat;
    }
    public void SetBrush(int id)
    {
        brushID = id;
    }
    public void SetN(float N)
    {
        Nvalue = N;
    }

}
