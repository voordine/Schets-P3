using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class Schets
{
    public Bitmap bitmap;
    public List<ToolTypes> ObjectenLijst = new List<ToolTypes>();

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
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
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
        gr.DrawImage(bitmap, 0, 0);
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

    public void SlaOp(object o, EventArgs ea)
    {
        SaveFileDialog filetypen = new SaveFileDialog();
        filetypen.Filter = "JPEG-files|*.jpg|PNG-files|*.png|BMP-files|*.bmp|Alle files|*.*";
        filetypen.Title = "Kunstwerk opslaan als...";
        if (filetypen.ShowDialog() == DialogResult.OK)
        {
            /*Bitmap opgeslagenbitmap;
            opgeslagenbitmap = Schets.bitmap;
            opgeslagen*/
            bitmap.Save(filetypen.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            gewijzigd = false;

        }
    }
}
