using System;
using System.Windows.Forms;
using System.Drawing;
public class MinhaJanela : Form
{
    public MinhaJanela()
    {
        this.Text = "Niveis";
        Button btn = new Button();
        btn.Text = "Nivel 1";
        btn.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom.exe");
        };
        Button abr = new Button();
        abr.Text = "Nivel 2";
        abr.Location = new Point(0, 40);
        abr.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom2.exe");
        };
        Button des = new Button();
        des.Text = "Nivel 3";
        des.Location = new Point(0, 80);
        des.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom3.exe");
        };
        Button qua = new Button();
        qua.Text = "Nivel 4";
        qua.Location = new Point(0, 120);
        qua.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom4.exe");
        };
        Button cin = new Button();
        cin.Text = "Nivel 5";
        cin.Location = new Point(0, 160);
        cin.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom5.exe");
        };
        Button sei = new Button();
        sei.Text = "Nivel 6";
        sei.Location = new Point(0, 200);
        sei.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom6.exe");
        };
        Button sev = new Button();
        sev.Text = "Nivel 7";
        sev.Location = new Point(0, 240);
        sev.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("topsecretroom7.exe");
        };
        Button san = new Button();
        san.Text = "Sandbox";
        san.Location = new Point(160, 80);
        san.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("sandbox.exe");
        };
        Button oht = new Button();
        oht.Text = "Outros jogos";
        oht.Location = new Point(160, 160);
        oht.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("webview\\view.exe");
        };
        this.Controls.Add(btn);
        this.Controls.Add(abr);
        this.Controls.Add(des);
        this.Controls.Add(qua);
        this.Controls.Add(cin);
        this.Controls.Add(sei);
        this.Controls.Add(sev);
        this.Controls.Add(san);
        this.Controls.Add(oht);
    }

    [STAThread]
    static void Main()
    {
        Application.Run(new MinhaJanela());
    }
}