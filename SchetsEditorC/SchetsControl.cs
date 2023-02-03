using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl
{   
    public Schets schets;
    private Color penkleur;
    public int lijndikte = 3;

    public Color PenKleur
    { get { return penkleur; }
    }
    public Schets Schets
    { get { return schets;   }
    }

    public int LijnDikte
    { get { return lijndikte;}
        set
        {
            lijndikte = value;
        }
    }

    public SchetsControl()
    {   this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {   schets.Teken(pea.Graphics);
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {   schets.VeranderAfmeting(this.ClientSize);
        this.Invalidate();
    }
    public Graphics MaakBitmapGraphics()
    {   Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
    }
    public void Schoon(object o, EventArgs ea)
    {   schets.Schoon();
        schets.gewijzigd = false;
        this.Invalidate();
    }
    public void Roteer(object o, EventArgs ea)
    {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
        //Het lukte ons niet om de roteer functie te laten werken met alle aanpassingen die we hebben gedaan, dus dan maar zo :)
        MessageBox.Show("Het lijkt erop dat je een premium functie van \"Schets\" hebt ontdekt! \n" +
                        "Je gebruikt momenteel de gratis versie."
                        , "Oeps!"
                        );
    }
    public void VeranderKleur(object obj, EventArgs ea)
    {   string kleurNaam = ((ComboBox)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }
    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {   string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }

    public void VeranderLijndikte(object o, EventArgs ea)
    {
        TrackBar tb = (TrackBar)o;
        LijnDikte = tb.Value;
        this.Invalidate();
    }

    public void Redo(object o, EventArgs ea)
    {
        schets.Redo();
        schets.BitmapGraphics.FillRectangle(Brushes.White, new Rectangle(0, 0, Width, Height));
        this.Invalidate();
    }

    public void Undo(object o, EventArgs ea)
    {
        schets.Undo();
        schets.BitmapGraphics.FillRectangle(Brushes.White, new Rectangle(0, 0, Width, Height));
        this.Invalidate();
    }
}