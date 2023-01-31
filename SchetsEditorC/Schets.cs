using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

public class Schets
{
    public Bitmap bitmap;
    public List<SchetsObject> ObjectenLijst = new List<SchetsObject>();
    public bool gewijzigd { get; set; }

    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        //foreach (SchetsObject schetsobj in ObjectenLijst)
            //schetsobj.Draw(this.BitmapGraphics);
        
        gr.DrawImage(bitmap, 0, 0);
    }

    public void toevoegen(SchetsObject schetsobj)
    {
        ObjectenLijst.Add(schetsobj);
    }
    public void weghalen(SchetsControl s, Point p)
    {
        for(int n = ObjectenLijst.Count - 1; n > -1; n--)
        {
            SchetsObject so = ObjectenLijst[n];

            /*if (so.RemoveElements(s, p, so))
                break;*/
        }

    }

    

    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }

    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }

}
