using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CyberWebQuestGame
{
    public class PaginaServidor
    {
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public string ExtraCss { get; set; }

        public PaginaServidor()
        {
            Links = new Dictionary<string, string>();
            ExtraCss = "";
        }

        public string GerarHtml(bool temaHackerAtivo)
        {
            string bgBody = temaHackerAtivo ? "#000000" : "#121212";
            string corTexto = temaHackerAtivo ? "#00ff00" : "#00ffcc";
            string bgContainer = temaHackerAtivo ? "#051105" : "#1e1e1e";
            string corBorda = temaHackerAtivo ? "#00ff00" : "#00ffcc";

            string html = @"
            <html>
            <head>
                <style>
                    body { font-family: 'Consolas', monospace; background-color: " + bgBody + @"; color: " + corTexto + @"; padding: 20px; margin: 0; " + ExtraCss + @" }
                    .container { background: " + bgContainer + @"; padding: 25px; border-radius: 6px; border: 1px solid " + corBorda + @"; box-shadow: 0 0 15px rgba(0,255,204,0.2); }
                    h1 { color: " + (temaHackerAtivo ? "#00ff66" : "#ff0055") + @"; border-bottom: 1px dashed " + (temaHackerAtivo ? "#00ff66" : "#ff0055") + @"; padding-bottom: 8px; margin-top: 0; }
                    p { color: " + (temaHackerAtivo ? "#aaffaa" : "#cccccc") + @"; line-height: 1.5; font-size: 14px; }
                    .links-box { margin-top: 25px; padding: 12px; background: " + (temaHackerAtivo ? "#020802" : "#161616") + @"; border-left: 3px solid " + (temaHackerAtivo ? "#00ff00" : "#ff0055") + @"; border-radius: 3px; }
                    .links-box h3 { margin: 0 0 8px 0; color: " + (temaHackerAtivo ? "#ffff00" : "#ffcc00") + @"; font-size: 13px; text-transform: uppercase; }
                    ul { margin: 0; padding-left: 18px; }
                    li { margin-bottom: 6px; }
                    a { color: " + corTexto + @"; text-decoration: none; font-weight: bold; cursor: pointer; }
                    a:hover { text-decoration: underline; color: #ffffff; }
                    .login-box { margin-top: 20px; padding: 15px; background: #161616; border: 1px dashed #ffcc00; }
                    input { background: #222; color: #fff; border: 1px solid #555; padding: 5px; margin: 3px; font-family: monospace; }
                    button { background: #ff0055; color: #fff; border: none; padding: 6px 12px; cursor: pointer; font-weight: bold; }
                </style>
                <script>
                    function navegar(url) {
                        window.external.NavegarViaWebBrowser(url);
                    }
                    function fazerLogin() {
                        var u = document.getElementById('txtUser').value;
                        var p = document.getElementById('txtPass').value;
                        window.external.ProcessarLogin(u, p);
                    }
                </script>
            </head>
            <body>
                <div class='container'>
                    <h1>" + Titulo + "</h1>" +
                    "<p>" + Conteudo.Replace("\n", "<br/>") + "</p>";

            if (Conteudo.Contains("[LOGIN_FORM]"))
            {
                html = html.Replace("[LOGIN_FORM]", @"
                    <div class='login-box'>
                        <h3>Terminal de Autenticação Corporativa</h3>
                        Utilizador: <input type='text' id='txtUser'/><br/>
                        Password: <input type='password' id='txtPass'/><br/>
                        <button onclick='fazerLogin()'>Autenticar</button>
                    </div>
                ");
            }

            if (Links.Count > 0)
            {
                html += "<div class='links-box'><h3>Diretórios Disponíveis:</h3><ul>";
                foreach (var link in Links)
                {
                    html += "<li><a href='javascript:navegar(\"" + link.Value + "\")'>" + link.Key + "</a></li>";
                }
                html += "</ul></div>";
            }

            html += "</div></body></html>";
            return html;
        }
    }

    [System.Runtime.InteropServices.ComVisible(true)]
    public class CyberForm : Form
    {
        private TextBox txtUrl;
        private Button btnIr;
        private WebBrowser webBrowser;
        private ProgressBar progressBarRastreio;
        private Label lblStatusRastreio;

        private Dictionary<string, PaginaServidor> servidorLocal;
        private const string DOMINIO_BASE = "http://net-secure.local";
        private const string URL_INICIAL = DOMINIO_BASE + "/index.html";

        private int nivelRastreio = 15;
        private Timer timerAlerta;

        private bool temporizadorCongeladoPorProxy = false;
        private bool temporizadorDesativadoPorAdmin = false;
        private bool temaHackerAtivo = false;
        private bool caracteresBizarrosAtivos = false;

        public CyberForm()
        {
            AtualizarTextosVisuais();
            this.Width = 950;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 18, 18);

            lblStatusRastreio = new Label();
            lblStatusRastreio.Text = "RASTREIO DA FIREWALL: 15%";
            lblStatusRastreio.Location = new Point(15, 12);
            lblStatusRastreio.Width = 300;
            lblStatusRastreio.ForeColor = Color.FromArgb(0, 255, 100);
            lblStatusRastreio.Font = new Font("Consolas", 10, FontStyle.Bold);

            progressBarRastreio = new ProgressBar();
            progressBarRastreio.Location = new Point(320, 12);
            progressBarRastreio.Width = 600;
            progressBarRastreio.Height = 22;
            progressBarRastreio.Maximum = 100;
            progressBarRastreio.Value = nivelRastreio;
            progressBarRastreio.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            txtUrl = new TextBox();
            txtUrl.Location = new Point(15, 45);
            txtUrl.Width = 820;
            txtUrl.Font = new Font("Consolas", 11);
            txtUrl.BackColor = Color.FromArgb(30, 30, 30);
            txtUrl.ForeColor = Color.White;
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.KeyDown += TxtUrl_KeyDown;

            btnIr = new Button();
            btnIr.Text = "Aceder";
            btnIr.Location = new Point(845, 43);
            btnIr.Width = 75;
            btnIr.Height = 27;
            btnIr.BackColor = Color.FromArgb(255, 0, 85);
            btnIr.ForeColor = Color.White;
            btnIr.FlatStyle = FlatStyle.Flat;
            btnIr.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnIr.Click += BtnIr_Click;

            webBrowser = new WebBrowser();
            webBrowser.Location = new Point(15, 85);
            webBrowser.Width = 905;
            webBrowser.Height = 560;
            webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = true;

            this.Controls.Add(lblStatusRastreio);
            this.Controls.Add(progressBarRastreio);
            this.Controls.Add(txtUrl);
            this.Controls.Add(btnIr);
            this.Controls.Add(webBrowser);

            timerAlerta = new Timer();
            timerAlerta.Interval = 2000;
            timerAlerta.Tick += TimerAlerta_Tick;
            timerAlerta.Start();

            ConstruirServidor();
            CarregarPagina(URL_INICIAL);
        }

        private void AtualizarTextosVisuais()
        {
            if (caracteresBizarrosAtivos)
            {
                this.Text = "C&#!# W£b Q#3st: !nfa#ãõ Xtr3m4 [M0D0 C#@$]";
            }
            else
            {
                this.Text = "Cyber Web Quest: Invasão Extrema [MODO CAOS]";
            }
        }

        public void ProcessarLogin(string user, string pass)
        {
            user = user.Trim();
            pass = pass.Trim();

            if (user == "tyty404" && pass == "404iserror")
            {
                temporizadorDesativadoPorAdmin = true;
                MessageBox.Show("Credenciais de SysAdmin aceites! O temporizador de rastreio foi PERMANENTEMENTE DESATIVADO.", "ACESSO TOTAL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (user == "devofdevs" && pass == "123")
            {
                caracteresBizarrosAtivos = true;
                AtualizarTextosVisuais();
                MessageBox.Show("Modo de corrupção ativado! Todas as interfaces gráficas começam a apresentar caracteres estranhos.", "ERRO DE SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CarregarPagina(txtUrl.Text);
            }
            else if (user == "anonymus" && pass == "ytdpl")
            {
                temaHackerAtivo = true;
                MessageBox.Show("Tema Hacker ativado com sucesso!", "ESTILO ALTERADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarPagina(txtUrl.Text);
            }
            else
            {
                MessageBox.Show("Credenciais inválidas! Nenhum efeito aplicado.", "FALHA DE LOGIN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void NavegarViaWebBrowser(string url)
        {
            if (url.Contains("armadilha"))
            {
                AdicionarRastreio(25);
                MessageBox.Show("Alerta! Acionaste uma armadilha de contrainteligência da IA!", "ARMADILHA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (url.Contains("proxy_tempo"))
            {
                AdicionarRastreio(-20);
                MessageBox.Show("Proxy ativado! O rastreio desceu (-20%) e o tempo está CONGELADO enquanto estiveres nesta página.", "TEMPO CONGELADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (url.Contains("bizarro"))
            {
                AdicionarRastreio(5);
                MessageBox.Show("O servidor respondeu com dados bizarros... Sentiste uma tontura digital.", "ERRO BIZARRO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                AdicionarRastreio(8);
            }

            CarregarPagina(url);
        }

        private void BtnIr_Click(object sender, EventArgs e)
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

        private void TimerAlerta_Tick(object sender, EventArgs e)
        {
            if (temporizadorDesativadoPorAdmin || temporizadorCongeladoPorProxy) return;

            AdicionarRastreio(3);
        }

        private void AdicionarRastreio(int qtd)
        {
            if (temporizadorDesativadoPorAdmin && qtd > 0) return;

            nivelRastreio += qtd;
            if (nivelRastreio > 100) nivelRastreio = 100;
            if (nivelRastreio < 0) nivelRastreio = 0;

            progressBarRastreio.Value = nivelRastreio;

            string txtStatus = caracteresBizarrosAtivos ? "R@5TR310: " : "RASTREIO: ";

            if (nivelRastreio < 50)
            {
                lblStatusRastreio.Text = txtStatus + nivelRastreio + "% [ESTÁVEL]";
                lblStatusRastreio.ForeColor = Color.FromArgb(0, 255, 100);
            }
            else if (nivelRastreio < 80)
            {
                lblStatusRastreio.Text = txtStatus + nivelRastreio + "% [ALERTA]";
                lblStatusRastreio.ForeColor = Color.FromArgb(255, 165, 0);
            }
            else
            {
                lblStatusRastreio.Text = txtStatus + nivelRastreio + "% [PERIGO IMINENTE]";
                lblStatusRastreio.ForeColor = Color.FromArgb(255, 0, 85);
            }

            if (nivelRastreio >= 100)
            {
                timerAlerta.Stop();
                MessageBox.Show("Foste completamente isolado pela Firewall! Fim de jogo.", "DERROTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void CarregarPagina(string url)
        {
            url = url.Trim().ToLower();

            if (!url.StartsWith("http") && !url.StartsWith("javascript"))
            {
                url = DOMINIO_BASE + (url.StartsWith("/") ? "" : "/") + url;
                if (!url.EndsWith(".html") && !url.EndsWith("/")) url += "/";
            }

            txtUrl.Text = url;

            if (url.Contains("proxy_tempo"))
            {
                temporizadorCongeladoPorProxy = true;
            }
            else
            {
                temporizadorCongeladoPorProxy = false;
            }

            if (servidorLocal.ContainsKey(url))
            {
                PaginaServidor pag = servidorLocal[url];

                string tituloFinal = caracteresBizarrosAtivos ? CorromperTexto(pag.Titulo) : pag.Titulo;
                string conteudoFinal = caracteresBizarrosAtivos ? CorromperTexto(pag.Conteudo) : pag.Conteudo;

                PaginaServidor pagModificada = new PaginaServidor
                {
                    Titulo = tituloFinal,
                    Conteudo = conteudoFinal,
                    Links = pag.Links,
                    ExtraCss = pag.ExtraCss
                };

                webBrowser.DocumentText = pagModificada.GerarHtml(temaHackerAtivo);

                if (url.Contains("nucleo_vitoria_2026"))
                {
                    timerAlerta.Stop();
                    MessageBox.Show("LENDÁRIO! Sobreviviste ao caos e às armadilhas!", "VITÓRIA DE ELITE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
            }
            else
            {
                AdicionarRastreio(20);
                webBrowser.DocumentText = "<html><body style='background:#121212;color:#ff0055;padding:30px;font-family:monospace;'><h1>Erro 404 - Acesso Bloqueado</h1><p>O diretório acionou uma armadilha (+20%).</p></body></html>";
            }
        }

        private string CorromperTexto(string texto)
        {
            char[] arr = texto.ToCharArray();
            for (int i = 0; i < arr.Length; i += 3)
            {
                if (arr[i] == 'a') arr[i] = '@';
                else if (arr[i] == 'e') arr[i] = '£';
                else if (arr[i] == 'i') arr[i] = '!';
                else if (arr[i] == 'o') arr[i] = '0';
                else if (arr[i] == 's') arr[i] = '$';
            }
            return new string(arr);
        }

        private void ConstruirServidor()
        {
            servidorLocal = new Dictionary<string, PaginaServidor>();

            servidorLocal.Add(URL_INICIAL, new PaginaServidor
            {
                Titulo = "Portal Principal - NetSecure Corp",
                Conteudo = "Bem-vindo ao portal. Navega com cuidado pelas armadilhas da rede.",
                Links = new Dictionary<string, string> {
                    { "Logs de Auditoria", DOMINIO_BASE + "/logs.html" },
                    { "Painel de Manutenção", DOMINIO_BASE + "/manutencao/" },
                    { "Repositório Cloud (Armadilha)", DOMINIO_BASE + "/armadilha_cloud.html" },
                    { "Sub-rede Desconhecida (Bizarro)", DOMINIO_BASE + "/bizarro/gato_psiq.html" }
                }
            });

            servidorLocal.Add(DOMINIO_BASE + "/armadilha_cloud.html", new PaginaServidor
            {
                Titulo = "Honeypot de Cloud",
                Conteudo = "Isto é uma armadilha colocada pela equipa de segurança!",
                Links = new Dictionary<string, string> { { "Fugir para o Início", URL_INICIAL } }
            });

            servidorLocal.Add(DOMINIO_BASE + "/bizarro/gato_psiq.html", new PaginaServidor
            {
                Titulo = "Terminal do Gato Quântico",
                Conteudo = "Encontraste uma entidade digital bizarra a flutuar no sistema.",
                ExtraCss = "background-color: #2b0033; color: #ff00ff;",
                Links = new Dictionary<string, string> {
                    { "Tentar falar com o gato (Armadilha)", DOMINIO_BASE + "/armadilha_gato.html" },
                    { "Voltar à Realidade", URL_INICIAL }
                }
            });

            servidorLocal.Add(DOMINIO_BASE + "/armadilha_gato.html", new PaginaServidor
            {
                Titulo = "Arranhão Cibernético",
                Conteudo = "O gato era uma armadilha da IA!",
                Links = new Dictionary<string, string> { { "Voltar ao Início", URL_INICIAL } }
            });

            servidorLocal.Add(DOMINIO_BASE + "/logs.html", new PaginaServidor
            {
                Titulo = "Registos de Sistema",
                Conteudo = "Dica: Procura pelo Proxy de Resfriamento na área de manutenção para congelar o tempo enquanto lá estiveres.",
                Links = new Dictionary<string, string> { { "Voltar ao Início", URL_INICIAL } }
            });

            servidorLocal.Add(DOMINIO_BASE + "/manutencao/", new PaginaServidor
            {
                Titulo = "Setor de Manutenção",
                Conteudo = "Ferramentas administrativas disponíveis.",
                Links = new Dictionary<string, string> {
                    { "Terminal de Autenticação de Utilizador", DOMINIO_BASE + "/manutencao/user/" },
                    { "Ver Chave de Acesso (Cifrada)", DOMINIO_BASE + "/manutencao/chave_rot13.txt" },
                    { "Proxy de Resfriamento (CONGELA O TEMPO)", DOMINIO_BASE + "/manutencao/proxy_tempo.html" },
                    { "Voltar ao Início", URL_INICIAL }
                }
            });

            servidorLocal.Add(DOMINIO_BASE + "/manutencao/user/", new PaginaServidor
            {
                Titulo = "Terminal de Utilizador - Manutenção",
                Conteudo = "Insere as tuas credenciais de acesso para invocar os protocolos de sistema.\n\n[LOGIN_FORM]",
                Links = new Dictionary<string, string> {
                    { "Voltar à Manutenção", DOMINIO_BASE + "/manutencao/" },
                    { "Voltar ao Início", URL_INICIAL }
                }
            });

            servidorLocal.Add(DOMINIO_BASE + "/manutencao/proxy_tempo.html", new PaginaServidor
            {
                Titulo = "Proxy de Resfriamento Ativo",
                Conteudo = "Estás a utilizar um túnel de proxy anónimo. O temporizador de rastreio da IA está AGORA CONGELADO! Podes tomar o teu tempo aqui.",
                ExtraCss = "background-color: #003322; color: #00ff88;",
                Links = new Dictionary<string, string> { { "Voltar à Manutenção", DOMINIO_BASE + "/manutencao/" } }
            });

            servidorLocal.Add(DOMINIO_BASE + "/manutencao/chave_rot13.txt", new PaginaServidor
            {
                Titulo = "chave_rot13.txt",
                Conteudo = "Chave detetada: aphybh_gh_sol\n\n(Aplica decodificação ROT13 de 13 casas para encontrar o diretório correto do núcleo).",
                Links = new Dictionary<string, string> { { "Voltar à Manutenção", DOMINIO_BASE + "/manutencao/" } }
            });

            // ROTA SECRETA DESCODIFICADA COM O LINK PARA A VITÓRIA
            servidorLocal.Add(DOMINIO_BASE + "/aphybh_gh_sol/", new PaginaServidor
            {
                Titulo = "Setor Descodificado por ROT13",
                Conteudo = "Conseguiste decifrar a chave com sucesso! O acesso ao núcleo final está desbloqueado.",
                Links = new Dictionary<string, string> {
                    { "Aceder ao Núcleo Central de Vitória", DOMINIO_BASE + "/nucleo_vitoria_2026/" },
                    { "Voltar ao Início", URL_INICIAL }
                }
            });

            servidorLocal.Add(DOMINIO_BASE + "/nucleo_vitoria_2026/", new PaginaServidor
            {
                Titulo = "NÚCLEO CENTRAL - ACESSO TOTAL",
                Conteudo = "Brilhante! Desvendaste o labirinto e derubaste a firewall.\n\nFlag obtida: FLAG{ULTIMATE_WEB_QUEST_2026}",
                Links = new Dictionary<string, string> { { "Reiniciar", URL_INICIAL } }
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
            Application.Run(new CyberForm());
        }
    }
}