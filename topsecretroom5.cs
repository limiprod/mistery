using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UrlQuestApp
{
    public class PaginaWeb
    {
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public Dictionary<string, string> Links { get; set; }

        public PaginaWeb()
        {
            Links = new Dictionary<string, string>();
        }

        public string GerarHtml()
        {
            string html = @"
            <html>
            <head>
                <style>
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #1e1e1e; color: #d4d4d4; padding: 25px; margin: 0; }
                    .container { background: #252526; padding: 30px; border-radius: 8px; border: 1px solid #333; box-shadow: 0 4px 10px rgba(0,0,0,0.5); }
                    h1 { color: #4ec9b0; border-bottom: 2px solid #007acc; padding-bottom: 10px; margin-top: 0; }
                    p { color: #9cdcfe; line-height: 1.6; font-size: 15px; }
                    .links-panel { margin-top: 30px; padding: 15px; background: #1f1f1f; border-left: 4px solid #007acc; border-radius: 4px; }
                    .links-panel h3 { margin: 0 0 10px 0; color: #b5cea8; font-size: 14px; text-transform: uppercase; }
                    ul { margin: 0; padding-left: 20px; }
                    li { margin-bottom: 8px; }
                    a { color: #569cd6; text-decoration: none; font-weight: bold; cursor: pointer; }
                    a:hover { text-decoration: underline; color: #9cdcfe; }
                </style>
                <script>
                    function navegarPara(url) {
                        window.external.NavegarDoWebBrowser(url);
                    }
                </script>
            </head>
            <body>
                <div class='container'>
                    <h1>" + Titulo + "</h1>" +
                    "<p>" + Conteudo.Replace("\n", "<br/>") + "</p>";

            if (Links.Count > 0)
            {
                html += "<div class='links-panel'><h3>Links Disponíveis no Servidor:</h3><ul>";
                foreach (var link in Links)
                {
                    html += "<li><a href='javascript:navegarPara(\"" + link.Value + "\")'>" + link.Key + "</a></li>";
                }
                html += "</ul></div>";
            }

            html += "</div></body></html>";
            return html;
        }
    }

    [System.Runtime.InteropServices.ComVisible(true)]
    public class Form1 : Form
    {
        private TextBox txtUrl;
        private Button btnGo;
        private WebBrowser webBrowser;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        private Dictionary<string, PaginaWeb> servidor;
        private const string DOMINIO = "http://infinitycorps.local";
        private const string PAGINA_INICIAL = DOMINIO + "/index.html";

        public Form1()
        {
            this.Text = "URL Quest - Infinity Corps [Security Edition]";
            this.Width = 900;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            txtUrl = new TextBox();
            txtUrl.Location = new Point(12, 14);
            txtUrl.Width = 780;
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Font = new Font("Segoe UI", 10);
            txtUrl.KeyDown += TxtUrl_KeyDown;

            btnGo = new Button();
            btnGo.Text = "Ir";
            btnGo.Location = new Point(800, 12);
            btnGo.Width = 70;
            btnGo.Height = 26;
            btnGo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGo.BackColor = Color.FromArgb(0, 122, 203);
            btnGo.ForeColor = Color.White;
            btnGo.FlatStyle = FlatStyle.Flat;
            btnGo.Click += BtnGo_Click;

            webBrowser = new WebBrowser();
            webBrowser.Location = new Point(12, 50);
            webBrowser.Width = 860;
            webBrowser.Height = 520;
            webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = true;

            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel("Sistema pronto. Navega pelo servidor da Infinity Corps.");
            statusStrip.Items.Add(lblStatus);

            this.Controls.Add(txtUrl);
            this.Controls.Add(btnGo);
            this.Controls.Add(webBrowser);
            this.Controls.Add(statusStrip);

            InicializarServidor();
            CarregarPagina(PAGINA_INICIAL);
        }

        public void NavegarDoWebBrowser(string url)
        {
            CarregarPagina(url);
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            CarregarPagina(txtUrl.Text);
        }

        private void TxtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CarregarPagina(txtUrl.Text);
                e.SuppressKeyPress = true;
            }
        }

        private void CarregarPagina(string url)
        {
            url = url.Trim().ToLower();

            if (!url.StartsWith("http") && !url.StartsWith("javascript"))
            {
                url = DOMINIO + (url.StartsWith("/") ? "" : "/") + url;
                if (!url.EndsWith(".html") && !url.EndsWith("/")) url += "/";
            }

            txtUrl.Text = url;

            if (servidor.ContainsKey(url))
            {
                PaginaWeb pag = servidor[url];
                webBrowser.DocumentText = pag.GerarHtml();
                lblStatus.Text = "Conexão estabelecida com sucesso.";

                if (url.Contains("pom_igrad_sol_2026"))
                {
                    MessageBox.Show("PARABÉNS! Descobriste o endpoint real decodificado e venceste o jogo!", "VITÓRIA DE HACKER", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                webBrowser.DocumentText = "<html><body style='background:#1e1e1e;color:#f44336;padding:40px;font-family:sans-serif;'><h1>Erro 404 - Não Encontrado</h1><p>O endereço <b>" + url + "</b> não existe ou a cifra está incorreta.</p></body></html>";
                lblStatus.Text = "Erro 404: Página não encontrada no servidor.";
            }
        }

        private void InicializarServidor()
        {
            servidor = new Dictionary<string, PaginaWeb>();

            servidor.Add(PAGINA_INICIAL, new PaginaWeb
            {
                Titulo = "Infinity Corps - Portal Principal",
                Conteudo = "Bem-vindo ao portal corporativo central.\n'O futuro digital pertence à Infinity Corps.'",
                Links = new Dictionary<string, string> {
                    { "Suporte Técnico", DOMINIO + "/support.html" },
                    { "Área de Administração", DOMINIO + "/admin/" }
                }
            });

            servidor.Add(DOMINIO + "/support.html", new PaginaWeb
            {
                Titulo = "Suporte Técnico",
                Conteudo = "Para aceder a dados confidenciais, tens de inspecionar a pasta de administração e decifrar a chave de segurança.",
                Links = new Dictionary<string, string> {
                    { "Voltar ao Início", PAGINA_INICIAL }
                }
            });

            servidor.Add(DOMINIO + "/admin/", new PaginaWeb
            {
                Titulo = "Área Administrativa",
                Conteudo = "Painel de controlo restrito.",
                Links = new Dictionary<string, string> {
                    { "Ver Ficheiro de Passwords (ROT3)", DOMINIO + "/admin/passwords_rot3.txt" }
                }
            });

            servidor.Add(DOMINIO + "/admin/passwords_rot3.txt", new PaginaWeb
            {
                Titulo = "passwords_rot3.txt",
                Conteudo = "Chave cifrada detetada: srp_ljudgr_vro_2026\n\n(Aviso: Esta chave está em ROT3. Tens de a decodificar recuando 3 letras para obteres o link secreto do núcleo).",
                Links = new Dictionary<string, string> {
                    { "Voltar", DOMINIO + "/admin/" }
                }
            });

            // Se o jogador tentar ir ao link cifrado diretamente:
            servidor.Add(DOMINIO + "/srp_ljudgr_vro_2026/", new PaginaWeb
            {
                Titulo = "Acesso Rejeitado - Chave Cifrada",
                Conteudo = "Estás a tentar usar a chave em formato ROT3 bruto! O servidor rejeita formatos cifrados.\n\nDecodifica a string 'srp_ljudgr_vro_2026' para texto limpo e usa esse resultado como diretório.",
                Links = new Dictionary<string, string> {
                    { "Voltar ao Início", PAGINA_INICIAL }
                }
            });

            // O ENDPOINT DA VITÓRIA (A string decodificada)
            servidor.Add(DOMINIO + "/pom_igrad_sol_2026/", new PaginaWeb
            {
                Titulo = "DIRETÓRIO CENTRAL - NÚCLEO INVASIVEL",
                Conteudo = "Incrível! Descobriste que a password decodificada era o diretório secreto.\n\nA tua flag de vitória é: FLAG{ROT3_DECRYPT_MASTER_2026}",
                Links = new Dictionary<string, string> {
                    { "Reiniciar Jogo", PAGINA_INICIAL }
                }
            });
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