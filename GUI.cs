using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;

namespace CellLight;

internal class GUI : Panel
{
    float size;
    Vector2 Screen;
    RadioButton ContrastRB = new RadioButton();
    public GUI(Vector2 Screen, float size)
    {
        this.size = size;this.Screen = Screen;

        //ContrastRB = new RadioButton[4];
        /*
        for (int i = 0; i < 4; i++)
        {
            ContrastRB[i] = new RadioButton();
            ContrastRB[i].Left = 100; ContrastRB[i].Top = 100;
            //Widgets.Add(ContrastRB[i]);
        }
        */
    }
}
