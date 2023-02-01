using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Windows.Forms;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;
    protected int lijndikte;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   
        startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   
        kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);

    public virtual void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {

    }

}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            
            startpunt.X += (int)sz.Width;
            //s.Schets.ObjectenLijst.AddLast(new SchetsObject());
            s.Invalidate();
        }
        //Spaties kunnen toevoegen aan je tekst
        if (c.ToString() == " ")
        {
            startpunt.X += 20;
        }
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }
    
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {   
        return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                            , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                            );
    }

    public static Pen MaakPen(Brush b, int dikte)
    {   
        Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {   
        base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   
        s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   
        base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p, s);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);
    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   
        g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }

}
public class RingTool : TweepuntTool
{
    public override string ToString() { return "ring"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawEllipse(MaakPen(kwast, 3), RechthoekTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }

}

public class CirkelTool : RingTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.FillEllipse(kwast, RechthoekTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }

}

public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }

}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   
        g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }

}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }

    public override void MuisDrag(SchetsControl s, Point p)
    {   
        this.MuisLos(s, p);
        this.MuisVast(s, p);
    }

    public override void Compleet(Graphics g, Point p1, Point p2, SchetsControl s)
    {
        s.Schets.toevoegen(new SchetsObject(s.PenKleur, ToString(), p1, p2));
    }

}
    
public class GumTool : ISchetsTool
{
    public override string ToString() { return "gum"; }

    public void MuisDrag(SchetsControl s, Point p)
    {
        this.MuisLos(s, p);
    }

    public void MuisLos(SchetsControl s, Point p)
    {
        foreach (SchetsObject schetsobj in s.Schets.ObjectenLijst)
        {
            if (schetsobj.raak(p))
            {
                s.Schets.ObjectenLijst.Remove(schetsobj);
                s.Invalidate();
                s.Schets.BitmapGraphics.FillRectangle(Brushes.White, 0, 0, s.Schets.bitmap.Width, s.Schets.bitmap.Height);
                break;
            }
        }
    }

    public void MuisVast(SchetsControl s, Point p) { }

    public void Letter(SchetsControl s, char c) { }

}

public class SchetsObject
{
    public Point beginpunt { get; set;}
    public Point eindpunt { get; set;}
    public string tekst { get; set;}
    public Color kleur { get; set;}
    public string tool { get; set;}

    public SchetsObject(Color kleur, string t, Point p, Point q, string text = null)
    {
        this.kleur = kleur;
        this.tool = t;
        this.tekst = text;
        this.beginpunt = p;
        this.eindpunt = q;
    }

    public void TekenObject(Graphics g)
    {
        static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }

        SolidBrush kwast = new SolidBrush(kleur);

        switch (tool)
        {
            case "tekst":
                Font font = new Font("Tahoma", 40);
                SizeF sz =
                g.MeasureString(tekst, font, this.beginpunt, StringFormat.GenericTypographic);
                g.DrawString(tekst, font, kwast, this.beginpunt, StringFormat.GenericTypographic); 
                break;

            case "kader":
                g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(beginpunt, eindpunt));
                break;

            case "vlak":
                g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(beginpunt, eindpunt));
                break;

            case "ring":
                g.DrawEllipse(MaakPen(kwast, 3), RechthoekTool.Punten2Rechthoek(beginpunt, eindpunt));
                break;

            case "cirkel":
                g.FillEllipse(kwast, RechthoekTool.Punten2Rechthoek(beginpunt, eindpunt));
                break;

            case "lijn":
                g.DrawLine(MaakPen(kwast, 3), beginpunt, eindpunt);
                break;

            case "pen":
                g.DrawLine(MaakPen(kwast, 3), beginpunt, eindpunt);
                break;

        }

    }

    public bool raak(Point p)
    {
        int bovengrens = Math.Min(beginpunt.Y, eindpunt.Y);
        int ondergrens = Math.Max(beginpunt.Y, eindpunt.Y);
        int lgrens = Math.Min(beginpunt.X, eindpunt.X);
        int rgrens = Math.Max(beginpunt.X, eindpunt.X);

        switch (this.tool)
        {
            //We hebben 4 als marge gekozen 

            case "tekst":
                if ((p.X >= beginpunt.X) && (p.X <= eindpunt.X) && (p.Y >= beginpunt.Y) && (p.Y <= eindpunt.Y))
                    return true; 
                break;

            case "kader":
                if (
                    (p.X >= lgrens -4 && p.X <= lgrens +4 && p.Y >= bovengrens -4 && p.Y <= ondergrens + 4) ||
                    (p.X >= rgrens -4 && p.X <= rgrens +4 && p.Y >= bovengrens -4 && p.Y <= ondergrens + 4) ||
                    (p.X >= lgrens -4 && p.X <= rgrens +4 && p.Y >= bovengrens -4 && p.Y <= bovengrens + 4) ||
                    (p.X >= lgrens -4 && p.X <= rgrens +4 && p.Y >= ondergrens -4 && p.Y <= ondergrens + 4)
                )
                    return true;
                break;

            case "vlak":
                if ((p.X >= beginpunt.X) && (p.X <= eindpunt.X) && (p.Y >= beginpunt.Y) && (p.Y <= eindpunt.Y))
                    return true;
                break;

            case "ring":
                if ((p.X >= beginpunt.X) && (p.X <= eindpunt.X) && (p.Y >= beginpunt.Y) && (p.Y <= eindpunt.Y))
                    return true;
                break;

            case "cirkel":
                if ((p.X >= beginpunt.X) && (p.X <= eindpunt.X) && (p.Y >= beginpunt.Y) && (p.Y <= eindpunt.Y))
                    return true;
                break;

            case "lijn":
            case "pen":
                int margeafstand = 3 + 4;
                float px = eindpunt.X - beginpunt.X;
                float py = eindpunt.Y - beginpunt.Y;
                float u = ((p.X - beginpunt.X) * px + (p.Y - beginpunt.Y) * py) / ((px * px) + (py * py));

                if (u > 1)
                    u = 1;
                else if (u < 0)
                    u = 0;

                float x = beginpunt.X + u * px;
                float y = beginpunt.Y + u * py;
                float dx = x - p.X;
                float dy = y - p.Y;

                double afstand = Math.Sqrt(dx * dx + dy * dy);

                return afstand <= margeafstand;

        }

        return false;
    }
}