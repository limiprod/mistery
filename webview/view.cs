using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace WebView2CscApp
{
    public class Form1 : Form
    {
        private WebView2 webView;

        public Form1()
        {
            this.Text = "Supergaming [WebView2]";
            this.Width = 1024;
            this.Height = 768;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 15);

            // Instanciar o controlo WebView2
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            // Inicializar o motor Chromium assincronamente
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                // Garante que o motor do WebView2/Edge está pronto
                await webView.EnsureCoreWebView2Async(null);

                // IMPORTANTE: Subscrever aqui o evento para capturar o postMessage do JavaScript!
                webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;

                // Carregar a página inicial (ajustado para 'all.html' conforme a sua lógica)
                CarregarFicheiroHtml("all.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message + "\n\nCertifica-te de que tens o WebView2 Runtime instalado no Windows.", "Erro Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar para evitar repetir código de leitura de HTML
        private void CarregarFicheiroHtml(string nomeFicheiro)
        {
            string caminhoHtml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeFicheiro);

            if (File.Exists(caminhoHtml))
            {
                string htmlContent = File.ReadAllText(caminhoHtml, System.Text.Encoding.UTF8);
                webView.CoreWebView2.NavigateToString(htmlContent);
            }
            else
            {
                string htmlErro =
                    "<!DOCTYPE html><html><head>" +
                    "<style>body { background: #111; color: #ff3333; font-family: 'Consolas', monospace; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; }" +
                    ".error-box { border: 1px solid #ff3333; padding: 30px; background: rgba(255,0,0,0.05); border-radius: 5px; text-align: center; }</style>" +
                    "</head><body><div class='error-box'>" +
                    "<h2>[ERRO CRÍTICO] Ficheiro '" + nomeFicheiro + "' não encontrado!</h2>" +
                    "<p>Coloca o ficheiro na mesma pasta do executável.</p>" +
                    "</div></body></html>";

                webView.CoreWebView2.NavigateToString(htmlErro);
            }
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string mensagem = e.TryGetWebMessageAsString();

            if (mensagem == "rafa")
            {
                CarregarFicheiroHtml("rafator.html");
            }
            else if (mensagem == "sonhos")
            {
                CarregarFicheiroHtml("dreamland.html");
            }
            else if (mensagem == "index")
            {
                CarregarFicheiroHtml("index.html");
            }
            else if (mensagem == "music")
            {
                CarregarFicheiroHtml("music.html");
            }
            else if (mensagem == "mist")
            {
                CarregarFicheiroHtml("misterio.html");
            }
            else if (mensagem == "home")
            {
                CarregarFicheiroHtml("all.html");
            }
        }
    }

    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}