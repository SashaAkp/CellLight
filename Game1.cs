using CellLight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using System;

namespace CellLight;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    int W = 301, H = 301, n, s, ticks = 0;
    float[,] valPastPast, valPast, valPres, valFut, valN;
    int[,] valStat;
    float f = 0, timec = 0, timecp = 0, timed = 0, timedp = 0, tps = 60;

    Rectangle sourceRect;
    WorldGrid destRect;
    Texture2D pixelmap;
    Color[] pixelcolors;
    GridEditor gridEditor;

    Camera camera;
    Desktop _desktop;
    EditorGUI _editor;
    RadioButton[] _brushs;

    KeyboardState nowk, previousk, k;
    MouseState m;

    bool pause = false, vismdpr=false;
    int visualMode = 0;
    float[] contrast;
    float slidermultip = 10;
    /*      0-sin like
            1-mono
            2-fequency based
            3-coeficient*/
    /*      0-field
            1-wall
            2_-absorbtion:
                1-down
                2-right
                3-up
                4-left
            >=30-source*/
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        //FPS STAFF
        base.IsFixedTimeStep = true;
        _graphics.SynchronizeWithVerticalRetrace = true;
        TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);
    }

    protected override void Initialize()
    {
        // GRAPHICS INITIALIZE
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        camera = new Camera();

        destRect = new WorldGrid(new Vector2(0,0), new Vector2(_graphics.PreferredBackBufferHeight, _graphics.PreferredBackBufferHeight), new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        destRect.position = new Vector2((_graphics.PreferredBackBufferWidth - destRect.Srect.Width) / 2, (_graphics.PreferredBackBufferHeight - destRect.Srect.Height) / 2);destRect.SUpdate(camera.position, camera.zoom);destRect.SUpdateGrid();
        sourceRect = new Rectangle(0, 0, W, H);

        pixelcolors = new Color[W * H];
        pixelmap = new Texture2D(_graphics.GraphicsDevice, W, H);

        for (int i = 0; i < pixelcolors.Length; i++)
        {
            pixelcolors[i] = new Color(0, 0, 0, 0);
        }
        pixelmap.SetData(0, new Rectangle(0, 0, W, H), pixelcolors, 0, W * H);

        contrast =new float[4]{3,3,3,1};

        MyraEnvironment.Game = this;
        _editor=new EditorGUI();
        _desktop = new Desktop();
        _desktop.Root = _editor;
        _editor.ContrastSlider.Value = contrast[visualMode]/slidermultip;
        _brushs=new RadioButton[7] {_editor.Brush1, _editor.Brush2 , _editor.Brush3 , _editor.Brush4 , _editor.Brush5, _editor.Brush6, _editor.Brush7 };
        _brushs[0].IsPressed = true;
        _editor.Nvaluetxtbox.Text = "1";
        // CELLS INITIALIZE
        gridEditor = new GridEditor(sourceRect, destRect, camera);
        valPastPast = new float[W, H]; valPast = new float[W, H]; valPres = new float[W, H]; valFut = new float[W, H]; valN = new float[W, H]; valStat = new int[W, H];

        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                valPast[x, y] = 0; valPres[x, y] = 0; valFut[x, y] = 0; valN[x, y] =1f;

                if (x == 0) valStat[x, y] = 22;
                else if (x == W - 1) valStat[x, y] = 24;
                else if (y == 0) valStat[x, y] = 21;
                else if (y == H - 1) valStat[x, y] = 23;
                else valStat[x, y] = 0;

            }
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        //Keyboard and Mouse Controlling
        k = Keyboard.GetState();
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            camera.Move(new Vector2(1, 0));camera.WorldObjUpdate(destRect); destRect.SUpdateGrid();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            camera.Move(new Vector2(-1, 0)); camera.WorldObjUpdate(destRect); destRect.SUpdateGrid();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            camera.Move(new Vector2(0, -1)); camera.WorldObjUpdate(destRect); destRect.SUpdateGrid();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            camera.Move(new Vector2(0, 1)); camera.WorldObjUpdate(destRect); destRect.SUpdateGrid();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.X))
        {
            camera.Zoom(1.05f); camera.WorldObjUpdate(destRect); destRect.SUpdateGrid();
        }
        else if (Keyboard.GetState().IsKeyDown(Keys.Z))
        {
            camera.Zoom(1/1.05f); camera.WorldObjUpdate(destRect); destRect.SUpdateGrid();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.C) && vismdpr == false)
        {
            if (visualMode < contrast.Length-1)
                visualMode += 1;
            else
                visualMode = 0;
            vismdpr = true;
            _editor.ContrastSlider.Value = contrast[visualMode]/slidermultip;
        }
        else if(!Keyboard.GetState().IsKeyDown(Keys.C) && vismdpr == true)
            vismdpr = false;

        m = Mouse.GetState();
        if (m.LeftButton == ButtonState.Pressed)
        {
            valStat=gridEditor.ChangeCell(valStat, valN, new Vector2(m.X, m.Y));
        }

        nowk = Keyboard.GetState();
        if (nowk.IsKeyDown(Keys.Space) && !previousk.IsKeyDown(Keys.Space))
        {
            pause = !pause;
        }
        previousk = nowk;
        //GUIContac
        contrast[visualMode] = slidermultip*_editor.ContrastSlider.Value;
        _editor.ContrastLabel.Text = "Contrast: "+MathF.Round(contrast[visualMode], 3).ToString();
        if(!(_editor.Nvaluetxtbox.IsMouseInside == true || _editor.Nvaluebtn.IsMouseInside == true || _editor.Nvaluelabel.IsMouseInside == true))
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                gridEditor.SetBrush(0); _brushs[0].IsPressed = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                gridEditor.SetBrush(1); _brushs[1].IsPressed = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                gridEditor.SetBrush(21); _brushs[2].IsPressed = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                gridEditor.SetBrush(22); _brushs[3].IsPressed = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                gridEditor.SetBrush(23); _brushs[4].IsPressed = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D6))
            {
                gridEditor.SetBrush(24); _brushs[5].IsPressed = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D7))
            {
                gridEditor.SetBrush(30); _brushs[6].IsPressed = true;
            }
        }
        for (int i = 0; i < _brushs.Length; i++)
        {
            if (_brushs[0].IsPressed == true)
                gridEditor.SetBrush(0);
            else if (_brushs[1].IsPressed == true)
                gridEditor.SetBrush(1);
            else if (_brushs[2].IsPressed == true)
                gridEditor.SetBrush(21);
            else if (_brushs[3].IsPressed == true)
                gridEditor.SetBrush(22);
            else if (_brushs[4].IsPressed == true)
                gridEditor.SetBrush(23);
            else if (_brushs[5].IsPressed == true)
                gridEditor.SetBrush(24);
            else if (_brushs[6].IsPressed == true)
                gridEditor.SetBrush(30);
        }
        if(_editor.Nvaluebtn.IsPressed == true)
        {
            gridEditor.SetN(float.Parse(_editor.Nvaluetxtbox.Text));
        }
        if (_editor.Nvaluetxtbox.IsMouseInside == true || _editor.Nvaluebtn.IsMouseInside == true || _editor.Nvaluelabel.IsMouseInside == true)
            _editor.Nvaluetxtbox.Enabled = true;
        else
            _editor.Nvaluetxtbox.Enabled = false;
        //CellsCalculating
        if (!pause && IsActive && (timec - timecp) >= 1 / tps)
        {
            Count();
            PostCount();
            ticks += 1;
            timecp = timec;
        }
        timec = (float)gameTime.TotalGameTime.TotalSeconds;
        base.Draw(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //FPS
        timedp = timed; timed = (float)gameTime.TotalGameTime.TotalSeconds;
        //Console.Write("\rCFPS: {0} DFPS: {1}", 1 / (timec - timecp), 1 / (timed - timedp));
        _editor.FTPSlabel.Text= "TPS: " + ((int)(1 / (timec - timecp))).ToString()+ "\nFPS: " + ((int)(1 / (timed - timedp))).ToString();
        //ColorsCalculating
        ColCount();
        pixelmap.SetData(pixelcolors);
        //SpritesDrawing
        GraphicsDevice.Clear(Color.DarkTurquoise);
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp,null, null, null);
        _spriteBatch.Draw(pixelmap, destRect.Srect, sourceRect, Color.White);
        _spriteBatch.End();
        _desktop.Render();
        base.Draw(gameTime);
    }
    void Count()
    {
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                s = valStat[x, y];
                switch (s)
                {
                    case 0:
                        valFut[x, y] = 2 * valPres[x, y] - valPast[x, y] + (valPres[x + 1, y] + valPres[x - 1, y] + valPres[x, y + 1] + valPres[x, y - 1] - 4 * valPres[x, y]) / (2 * MathF.Pow(valN[x, y], 2));
                        break;
                    case 1:
                        valFut[x, y] = 0;
                        break;
                    case 21:
                        valFut[x, y] = (valPres[x, y + 1] - valPres[x, y]) / valN[x, y + 1] + valPres[x, y];
                        break;
                    case 22:
                        valFut[x, y] = (valPres[x + 1, y] - valPres[x, y]) / valN[x + 1, y] + valPres[x, y];
                        break;
                    case 23:
                        valFut[x, y] = (valPres[x, y - 1] - valPres[x, y]) / valN[x, y - 1] + valPres[x, y];
                        break;
                    case 24:
                        valFut[x, y] = (valPres[x - 1, y] - valPres[x, y]) / valN[x - 1, y] + valPres[x, y];
                        break;
                    default:
                        valPres[x,y] = MathF.Sin(0.1f * ticks);
                        break;
                }
            }
        }
    }
    void PostCount()
    {
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                if (valStat[x,y]>=30)
                    continue;
                valPastPast[x, y] = valPast[x, y];
                valPast[x, y] = valPres[x, y];
                valPres[x, y] = valFut[x, y];
            }
        }
    }
    void ColCount()
    {
        int nn = 0;
        if (visualMode == 0)
        {
            for (int yy = 0; yy < H; yy++)
            {
                for (int xx = 0; xx < W; xx++)
                {
                    s = valStat[xx, yy];
                    f = contrast[visualMode]*MathF.Abs(valPres[xx, yy]);
                    if (s == 21 || s == 22 || s == 23 || s == 24)
                        pixelcolors[nn] = Color.Coral;
                    else if (s == 1)
                        pixelcolors[nn] = Color.Beige;
                    else
                        pixelcolors[nn] = new Color(MathF.Sin(3f * f), MathF.Sin(6f * f), MathF.Sin(9f * f));
                    nn++;
                }
            }
        }
        else if (visualMode == 1)
        {
            for (int yy = 0; yy < H; yy++)
            {
                for (int xx = 0; xx < W; xx++)
                {
                    s = valStat[xx, yy];
                    f = contrast[visualMode]*MathF.Abs(valPres[xx, yy]);
                    if (s == 21 || s == 22 || s == 23 || s == 24)
                        pixelcolors[nn] = Color.Coral;
                    else if (s == 1)
                        pixelcolors[nn] = Color.Beige;
                    else
                        pixelcolors[nn] = new Color(f, f, f);
                    nn++;
                }
            }
        }
        else if (visualMode == 2)
        {
            for (int yy = 0; yy < H; yy++)
            {
                for (int xx = 0; xx < W; xx++)
                {
                    s = valStat[xx, yy];
                    f = contrast[visualMode] * MathF.Sqrt(MathF.Abs((valPres[xx, yy] + valPastPast[xx, yy]-2*valPast[xx, yy]) / valPast[xx, yy]));
                    if (s == 21 || s == 22 || s == 23 || s == 24)
                        pixelcolors[nn] = Color.Coral;
                    else if (s == 1)
                        pixelcolors[nn] = Color.Beige;
                    else
                        pixelcolors[nn] = new Color(MathF.Sin(3f * f), MathF.Sin(6f * f), MathF.Sin(9f * f));
                    nn++;
                }
            }
        }
        else if (visualMode == 3)
        {
            for (int yy = 0; yy < H; yy++)
            {
                for (int xx = 0; xx < W; xx++)
                {
                    s = valStat[xx, yy];
                    f = contrast[visualMode] * valN[xx, yy];
                    if (s == 21 || s == 22 || s == 23 || s == 24)
                        pixelcolors[nn] = Color.Coral;
                    else if (s == 1)
                        pixelcolors[nn] = Color.Beige;
                    else
                        pixelcolors[nn] = new Color(MathF.Sin(3f * f), MathF.Sin(6f * f), MathF.Sin(9f * f));
                    nn++;
                }
            }
        }


    }
}