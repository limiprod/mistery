using System;
using System.Drawing;
using System.Windows.Forms;

namespace AiSentinelGame
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Form1 : Form
    {
        private RichTextBox txtTerminal;
        private TextBox txtInput;
        private Button btnSend;
        private Label lblAiStatus;
        private ProgressBar progressBarTrace;

        private int traceLevel = 0;
        private int securityLayer = 3;
        private bool aiHasLockedDown = false;
        private Random rand = new Random();

        public Form1()
        {
            this.Text = "AI Sentinel: Neural Lockdown [VS AURA v4.2]";
            this.Width = 950;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 20);

            txtTerminal = new RichTextBox();
            txtTerminal.Location = new Point(15, 15);
            txtTerminal.Width = 905;
            txtTerminal.Height = 460;
            txtTerminal.BackColor = Color.FromArgb(10, 10, 10);
            txtTerminal.ForeColor = Color.FromArgb(0, 255, 100);
            txtTerminal.Font = new Font("Consolas", 11, FontStyle.Regular);
            txtTerminal.ReadOnly = true;
            txtTerminal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            lblAiStatus = new Label();
            lblAiStatus.Text = "ESTADO DA IA (AURA): ATENTA | Camadas de Firewall: 3";
            lblAiStatus.Location = new Point(15, 485);
            lblAiStatus.Width = 500;
            lblAiStatus.ForeColor = Color.FromArgb(255, 100, 100);
            lblAiStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblAiStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            progressBarTrace = new ProgressBar();
            progressBarTrace.Location = new Point(520, 483);
            progressBarTrace.Width = 400;
            progressBarTrace.Height = 22;
            progressBarTrace.Maximum = 100;
            progressBarTrace.Value = 0;
            progressBarTrace.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            txtInput = new TextBox();
            txtInput.Location = new Point(15, 525);
            txtInput.Width = 800;
            txtInput.Height = 30;
            txtInput.Font = new Font("Consolas", 12);
            txtInput.BackColor = Color.FromArgb(30, 30, 30);
            txtInput.ForeColor = Color.White;
            txtInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtInput.KeyDown += TxtInput_KeyDown;

            btnSend = new Button();
            btnSend.Text = "Executar";
            btnSend.Location = new Point(825, 523);
            btnSend.Width = 95;
            btnSend.Height = 28;
            btnSend.BackColor = Color.FromArgb(0, 120, 215);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Click += BtnSend_Click;

            this.Controls.Add(txtTerminal);
            this.Controls.Add(lblAiStatus);
            this.Controls.Add(progressBarTrace);
            this.Controls.Add(txtInput);
            this.Controls.Add(btnSend);

            ImprimirTerminal("===============================================================");
            ImprimirTerminal(" [!] CONEXÃO NEURAL ESTABELECIDA CONTRA A IA 'AURA'");
            ImprimirTerminal(" [!] Objetivo: Desativar as 3 camadas de firewall e injetar o vírus.");
            ImprimirTerminal(" [!] CUIDADO: A IA monitoriza cada comando. Se o rastreio atingir 100%, estás perdido!");
            ImprimirTerminal(" [!] Digita 'help' para ver os comandos de combate lógico.");
            ImprimirTerminal("===============================================================\n");
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            ProcessarComando();
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessarComando();
                e.SuppressKeyPress = true;
            }
        }

        private void ProcessarComando()
        {
            if (aiHasLockedDown) return;

            string cmd = txtInput.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(cmd)) return;

            ImprimirTerminal("> " + txtInput.Text);
            txtInput.Clear();

            ExecutarLogicaIA(cmd);
        }

        private void ExecutarLogicaIA(string cmd)
        {
            if (cmd == "help")
            {
                ImprimirTerminal("\n--- COMANDOS TÁTICOS ---");
                ImprimirTerminal("  scan           - Analisa as vulnerabilidades atuais da IA");
                ImprimirTerminal("  bypass         - Tenta saltar uma camada de firewall (Gera rastreio)");
                ImprimirTerminal("  spoof_ip       - Engana temporariamente a IA e reduz o rastreio");
                ImprimirTerminal("  brute_core     - Força o núcleo central (Exige firewall a 0)");
                ImprimirTerminal("  matrix         - Ativa overclock visual do terminal\n");
                AdicionarRastreio(5);
            }
            else if (cmd == "scan")
            {
                ImprimirTerminal("\n[AURA SCANNER] Camadas de firewall ativas: " + securityLayer);
                if (securityLayer > 0)
                {
                    ImprimirTerminal("[AURA] AVISO: 'Atividade anómala detetada. A aumentar vigilância.'");
                    AdicionarRastreio(8);
                }
                else
                {
                    ImprimirTerminal("[+] Firewalls desativadas! Podes usar 'brute_core' para vencer.");
                    AdicionarRastreio(5);
                }
            }
            else if (cmd == "bypass")
            {
                if (securityLayer > 0)
                {
                    securityLayer--;
                    ImprimirTerminal("\n[+] SUCESSO: Camada de firewall ultrapassada! Faltam " + securityLayer + " camadas.");
                    ImprimirTerminal("[AURA IA] 'Intrusão detetada no setor de rede. A contra-atacar...'");
                    AdicionarRastreio(25);
                }
                else
                {
                    ImprimirTerminal("\n[-] As firewalls já foram todas derrubadas! Usa 'brute_core'.");
                    AdicionarRastreio(5);
                }
            }
            else if (cmd == "spoof_ip")
            {
                int reducao = rand.Next(15, 30);
                traceLevel = Math.Max(0, traceLevel - reducao);
                progressBarTrace.Value = traceLevel;
                ImprimirTerminal("\n[+] IP falseado com sucesso! Rastreio da IA desorientado (-" + reducao + "%).");
                ImprimirTerminal("[AURA IA] 'Sinal falso detetado. A recalcular coordenadas de origem...'");
            }
            else if (cmd == "brute_core")
            {
                if (securityLayer == 0)
                {
                    ImprimirTerminal("\n[+] A injetar payload de destruição quântica no núcleo da AURA...");
                    ImprimirTerminal("[AURA IA] 'Erro... Sistema... a... desmoronar... Adeus, criador.'");
                    MessageBox.Show("VENCESTE A IA! A AURA foi desligada com sucesso.", "VITÓRIA CRÍTICA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                else
                {
                    ImprimirTerminal("\n[-] ERRO CRÍTICO: A IA bloqueou o ataque! Ainda tens " + securityLayer + " camadas de firewall ativas.");
                    AdicionarRastreio(20);
                }
            }
            else if (cmd == "matrix")
            {
                txtTerminal.ForeColor = Color.FromArgb(0, 255, 255);
                ImprimirTerminal("\n[+] Overclock ciberespacial ativado. Cores secundárias alteradas.");
                AdicionarRastreio(2);
            }
            else
            {
                ImprimirTerminal("\n[-] Comando desconhecido pela shell. A AURA registou a tentativa falhada.");
                AdicionarRastreio(12);
            }

            AtualizarPainelIA();
        }

        private void AdicionarRastreio(int quantidade)
        {
            traceLevel += quantidade;
            if (traceLevel > 100) traceLevel = 100;
            if (traceLevel < 0) traceLevel = 0;

            progressBarTrace.Value = traceLevel;

            if (traceLevel >= 100 && !aiHasLockedDown)
            {
                aiHasLockedDown = true;
                ImprimirTerminal("\n===============================================================");
                ImprimirTerminal(" [ALERTA VERMELHO] A IA AURA RASTREIOU A TUA LOCALIZAÇÃO!");
                ImprimirTerminal(" [!] O teu terminal foi isolado e os teus dados apagados.");
                ImprimirTerminal("===============================================================");
                MessageBox.Show("Foste apanhado pela IA AURA! Rastreio atingiu 100%.", "DERROTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void AtualizarPainelIA()
        {
            if (traceLevel < 40)
            {
                lblAiStatus.Text = "ESTADO DA IA: CALMA | Firewall: " + securityLayer + " | Rastreio: " + traceLevel + "%";
                lblAiStatus.ForeColor = Color.FromArgb(0, 255, 100);
            }
            else if (traceLevel < 80)
            {
                lblAiStatus.Text = "ESTADO DA IA: ALERTA | Firewall: " + securityLayer + " | Rastreio: " + traceLevel + "%";
                lblAiStatus.ForeColor = Color.FromArgb(255, 165, 0);
            }
            else
            {
                lblAiStatus.Text = "ESTADO DA IA: FÚRIA / RASTREIO IMINENTE | Firewall: " + securityLayer + " | Rastreio: " + traceLevel + "%";
                lblAiStatus.ForeColor = Color.FromArgb(255, 50, 50);
            }
        }

        private void ImprimirTerminal(string texto)
        {
            txtTerminal.AppendText(texto + "\n");
            txtTerminal.ScrollToCaret();
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