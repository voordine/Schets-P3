using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class SchetsWin : Form
{
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    bool vast;

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void afsluiten(object obj, EventArgs ea)
    {
        this.Close();
    }

    public SchetsWin()
    {
        ISchetsTool[] deTools = { new PenTool()         
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new TekstTool()
                                , new GumTool()
                                , new RingTool()
                                , new CirkelTool()
                                };
        String[] deKleuren = { "Black", "White", "Gray", "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Magenta", "Pink"};

        this.ClientSize = new Size(700, 510);
        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                    {   vast=true;
                                        this.schetscontrol.Schets.gewijzigd = true;
                                        huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisLos (schetscontrol, mea.Location);
                                        vast = false;
                                    };
        schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                    {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar);
                                    };
        this.Controls.Add(schetscontrol);

        this.Closing += VensterSluiten;

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);

    }

    public void VensterSluiten(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (this.schetscontrol.Schets.gewijzigd == true)
        {
            if (MessageBox.Show(
                                    "Er zijn wijzingen gedaan sinds de laatste keer dat er opgeslagen is.\n" +
                                    "Weet je zeker dat je je kunstwerk wilt verlaten?", "Pas op!", MessageBoxButtons.OKCancel)
                                    == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

    }

    public void SlaOp(object o, EventArgs ea)
    {
        SaveFileDialog filetypen = new SaveFileDialog();
        filetypen.Filter = "JPEG-files|*.jpg|PNG-files|*.png|BMP-files|*.bmp|Alle files|*.*";
        filetypen.Title = "Kunstwerk opslaan als...";
        if (filetypen.ShowDialog() == DialogResult.OK)
        {
            this.schetscontrol.Schets.bitmap.Save(filetypen.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            this.schetscontrol.Schets.gewijzigd = false;
        }
    }

    /*public void opslaan(object o, EventArgs ea)
    {
        StreamWriter w = new StreamWriter(filenaam);
        foreach (SchetsObject schetsobj in ObjectenLijst)
            w.WriteLine(schetsobj.ToString);
        w.Close();

    }*/

    /*public void inlezen(object o, EventArgs ea)
    {
        ObjectenLijst.Clear();
        StreamReader r = new StreamReader(filenaam);
        string regel;
        while (regel= r.ReadLine()) != null)
            SchetsObject.Add(new SchetsObject(regel));
        r.Close();
        this.Invalidate();
    }*/

    private void maakFileMenu()
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menu.DropDownItems.Add("Opslaan &als...", null, this.SlaOp);
        menuStrip.Items.Add(menu);
    }

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {   
            ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
        ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
        foreach (string k in kleuren)
            submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
        menu.DropDownItems.Add(submenu);
        menuStrip.Items.Add(menu);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(45, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {   
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);
            
        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";  
        clear.Location = new Point(  0, 0); 
        clear.Click += schetscontrol.Schoon;        
            
        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate"; 
        rotate.Location = new Point( 80, 0); 
        rotate.Click += schetscontrol.Roteer; 
           
        Label penkleur = new Label(); paneel.Controls.Add(penkleur);
        penkleur.Text = "Penkleur:"; 
        penkleur.Location = new Point(180, 3); 
        penkleur.AutoSize = true;

        /*Button undo = new Button(); paneel.Controls.Add(undo);
        undo.Text = "Undo";
        undo.Name = "UndoButton";
        undo.Enabled = false;
        undo.Location = new Point(380, 0);
        undo.Click += schetscontrol.Undo;*/

        ComboBox cbb = new ComboBox(); paneel.Controls.Add(cbb);
        cbb.Location = new Point(240, 0); 
        cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
        cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
        foreach (string k in kleuren)
            cbb.Items.Add(k);
        cbb.SelectedIndex = 0;

        //schuifregelaar voor de lijndikte
        TrackBar tb = new TrackBar(); paneel.Controls.Add(tb);
        Label tbtekst = new Label(); paneel.Controls.Add(tbtekst);
        tbtekst.Location = new Point(400, 3);
        tbtekst.AutoSize = true;
        tbtekst.Text = "Lijndikte:";
        tb.Location = new Point(450, 0);
        tb.AutoSize = true;
        tb.Minimum = 0;
        tb.Maximum = 8;
        tb.Orientation = Orientation.Horizontal;
        tb.Name = "tblijndikte";
        tb.ValueChanged += schetscontrol.VeranderSchuif;
        tb.MouseUp += schetscontrol.VeranderSchuif;

    }

}