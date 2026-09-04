using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TileEngineCore
{
    public class MotorJogo
    {
        private int[,] matrizTiles;
        private int larguraMapa;
        private int alturaMapa;
        private int tamanhoTile;

        public MotorJogo(int[,] matriz, int tamanhoTile = 48)
        {
            this.matrizTiles = matriz;
            this.alturaMapa = matriz.GetLength(0);
            this.larguraMapa = matriz.GetLength(1);
            this.tamanhoTile = tamanhoTile;
        }

        public void Iniciar(string tituloJanela = "TileMatrix Engine")
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new JanelaAutonoma(matrizTiles, larguraMapa, alturaMapa, tamanhoTile, tituloJanela));
        }

        public static void IniciarJogo(int[,] matrizMapa)
        {
            MotorJogo motor = new MotorJogo(matrizMapa, 48);
            motor.Iniciar("Jogo via DllTester");
        }
    }

    internal class JanelaAutonoma : Form
    {
        private int[,] mapa;
        private int cols, linhas, tSize;
        private float posX, posY;
        private const float velocidade = 4.0f;
        private Timer temporizador;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        public JanelaAutonoma(int[,] matriz, int c, int l, int ts, string titulo)
        {
            this.mapa = matriz;
            this.cols = c;
            this.linhas = l;
            this.tSize = ts;

            this.Text = titulo;
            this.ClientSize = new Size(cols * tSize, linhas * tSize);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            DefinirPosicaoInicial();

            temporizador = new Timer();
            temporizador.Interval = 16;
            temporizador.Tick += (s, e) => {
                AtualizarMovimento();
                this.Invalidate();
            };
            temporizador.Start();
        }

        private void DefinirPosicaoInicial()
        {
            for (int y = 0; y < linhas; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (mapa[y, x] == 0)
                    {
                        posX = x * tSize + (tSize / 8);
                        posY = y * tSize + (tSize / 8);
                        return;
                    }
                }
            }
        }

        private void AtualizarMovimento()
        {
            float dirX = 0, dirY = 0;
            if ((GetAsyncKeyState(Keys.Left) & 0x8000) != 0 || (GetAsyncKeyState(Keys.A) & 0x8000) != 0) dirX = -1;
            if ((GetAsyncKeyState(Keys.Right) & 0x8000) != 0 || (GetAsyncKeyState(Keys.D) & 0x8000) != 0) dirX = 1;
            if ((GetAsyncKeyState(Keys.Up) & 0x8000) != 0 || (GetAsyncKeyState(Keys.W) & 0x8000) != 0) dirY = -1;
            if ((GetAsyncKeyState(Keys.Down) & 0x8000) != 0 || (GetAsyncKeyState(Keys.S) & 0x8000) != 0) dirY = 1;

            if (dirX != 0 || dirY != 0)
            {
                float novoX = posX + (dirX * velocidade);
                if (!TemColisao(novoX, posY)) posX = novoX;

                float novoY = posY + (dirY * velocidade);
                if (!TemColisao(posX, novoY)) posY = novoY;
            }
        }

        private bool TemColisao(float x, float y)
        {
            int pSize = tSize - 4;
            int esq = (int)(x / tSize);
            int dir = (int)((x + pSize) / tSize);
            int topo = (int)(y / tSize);
            int fundo = (int)((y + pSize) / tSize);

            if (esq < 0 || dir >= cols || topo < 0 || fundo >= linhas) return true;

            return mapa[topo, esq] > 0 || mapa[topo, dir] > 0 ||
                   mapa[fundo, esq] > 0 || mapa[fundo, dir] > 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.Clear(Color.FromArgb(20, 20, 30));

            for (int y = 0; y < linhas; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int tile = mapa[y, x];
                    if (tile > 0)
                    {
                        Color cor = (tile == 1) ? Color.Gray : Color.DarkRed;
                        g.FillRectangle(new SolidBrush(cor), x * tSize, y * tSize, tSize, tSize);
                        g.DrawRectangle(Pens.Black, x * tSize, y * tSize, tSize, tSize);
                    }
                }
            }

            g.FillRectangle(Brushes.Yellow, posX, posY, tSize - 4, tSize - 4);
            g.DrawRectangle(Pens.Orange, posX, posY, tSize - 4, tSize - 4);
        }
    }
}