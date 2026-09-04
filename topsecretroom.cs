using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ControllerManager; // A tua DLL do XInput

public class TopSecretRoom : Form
{
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

    [DllImport("user32.dll")]
    public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

    private Timer gameTimer;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private Label lblStatus;
    private Button btnSecretoCanto;
    private Button btnInterativo;

    private int faseAtual = 1;

    // Fila para o Konami Code
    private readonly List<Keys> konamiSequence = new List<Keys>
    {
        Keys.Up, Keys.Up, Keys.Down, Keys.Down,
        Keys.Left, Keys.Right, Keys.Left, Keys.Right,
        Keys.B, Keys.A
    };
    private readonly List<Keys> userInputs = new List<Keys>();

    private int cliquesCantos = 0;
    private int contadorGlitch = 0;
    private bool glitchAtivo = false;

    private int cliquesVelocidade = 0;
    private DateTime ultimoClique = DateTime.Now;
    private int contadorVibracaoMorse = 0;
    private int contadorBinario = 0;
    private string inputArmado = "";

    public TopSecretRoom()
    {
        this.Text = "Projeto Top Secret - Zona Restrita v5.3";
        this.Width = 800;
        this.Height = 600;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(15, 15, 15);
        this.KeyPreview = true;

        lblStatus = new Label();
        lblStatus.Text = "=== ETAPA 1: O INÍCIO ===\nO sistema está bloqueado.\nDica: Lembra-te do clássico código de batotice no teclado (Konami Code).";
        lblStatus.ForeColor = Color.OrangeRed;
        lblStatus.Font = new Font("Consolas", 10, FontStyle.Bold);
        lblStatus.AutoSize = false;
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        lblStatus.Dock = DockStyle.Fill;
        this.Controls.Add(lblStatus);

        btnSecretoCanto = new Button();
        btnSecretoCanto.Size = new Size(80, 80);
        btnSecretoCanto.Location = new Point(0, 0);
        btnSecretoCanto.BackColor = Color.Transparent;
        btnSecretoCanto.FlatStyle = FlatStyle.Flat;
        btnSecretoCanto.FlatAppearance.BorderSize = 0;
        btnSecretoCanto.Visible = false;
        btnSecretoCanto.Click += BtnSecretoCanto_Click;
        this.Controls.Add(btnSecretoCanto);
        btnSecretoCanto.BringToFront();

        btnInterativo = new Button();
        btnInterativo.Text = "CLICA-ME RÁPIDO!";
        btnInterativo.Size = new Size(160, 50);
        btnInterativo.Location = new Point(320, 420);
        btnInterativo.BackColor = Color.DarkRed;
        btnInterativo.ForeColor = Color.White;
        btnInterativo.Visible = false;
        btnInterativo.Click += BtnInterativo_Click;
        this.Controls.Add(btnInterativo);
        btnInterativo.BringToFront();

        SetupSystemTray();

        gameTimer = new Timer();
        gameTimer.Interval = 50;
        gameTimer.Tick += GameLoop;
        gameTimer.Start();

        this.KeyDown += TopSecretRoom_KeyDown;
    }

    private void SetupSystemTray()
    {
        trayMenu = new ContextMenuStrip();

        trayMenu.Items.Add("Inserir Chave de Acesso (F3)", null, (s, e) => {
            if (faseAtual < 3) { MostrarAvisoUnico("Acesso Negado! Completa as etapas anteriores."); return; }
            string input = Microsoft.VisualBasic.Interaction.InputBox("Insere o código visual do glitch:", "Terminal de Cifragem", "");
            if (input == "7749" && faseAtual == 3)
            {
                faseAtual = 4;
                lblStatus.Text = "=== ETAPA 4: CALIBRAGEM DE RATOS ===\nClica 3 vezes no BOTÃO SECRETO no CANTO SUPERIOR ESQUERDO.";
                lblStatus.ForeColor = Color.Cyan;
                this.BackColor = Color.FromArgb(0, 0, 40);
                btnSecretoCanto.Visible = true;
                MostrarAvisoUnico("Código aceite! Avançaste para a Etapa 4.");
            }
            else { MostrarAvisoUnico("Código incorreto."); }
        });

        trayMenu.Items.Add("Verificar Ficheiro Root (F5)", null, (s, e) => {
            if (faseAtual < 5) { MostrarAvisoUnico("Ainda não chegaste a esta etapa!"); return; }
            if (File.Exists("root.txt") && File.ReadAllText("root.txt").Trim().Equals("OVERRIDE", StringComparison.OrdinalIgnoreCase))
            {
                faseAtual = 6;
                lblStatus.Text = "=== ETAPA 6: TESTE DE REFLEXOS ===\nClica 5 vezes no botão vermelho em baixo em menos de 3 segundos!";
                lblStatus.ForeColor = Color.Magenta;
                this.BackColor = Color.FromArgb(40, 0, 40);
                btnInterativo.Visible = true;
                MostrarAvisoUnico("Ficheiro root validado com sucesso!");
            }
            else { MostrarAvisoUnico("Ficheiro 'root.txt' em falta ou sem o texto 'OVERRIDE'."); }
        });

        trayMenu.Items.Add("Descodificar Frequência Fantasma (F7)", null, (s, e) => {
            if (faseAtual < 7) { MostrarAvisoUnico("Ainda não chegaste a esta etapa!"); return; }
            string input = Microsoft.VisualBasic.Interaction.InputBox("Qual era a palavra escondida na opacidade?", "Terminal Ghost", "");
            if (input.Equals("GHOST", StringComparison.OrdinalIgnoreCase))
            {
                faseAtual = 8;
                this.Opacity = 1.0;
                lblStatus.Text = "=== ETAPA 8: MORSE DE VIBRAÇÃO ===\nSente a vibração rítmica no comando da Xbox!\n(Dica: Fogo em inglês)";
                lblStatus.ForeColor = Color.Red;
                this.BackColor = Color.Black;
                MostrarAvisoUnico("Palavra fantasma correta!");
            }
            else { MostrarAvisoUnico("Palavra incorreta."); }
        });

        trayMenu.Items.Add("Chave Final do Comando (F8)", null, (s, e) => {
            if (faseAtual < 8) { MostrarAvisoUnico("Ainda não chegaste a esta etapa!"); return; }
            string input = Microsoft.VisualBasic.Interaction.InputBox("Qual é a mensagem enviada pelo motor de vibração?", "Terminal Morse", "");
            if (input.Equals("FIRE", StringComparison.OrdinalIgnoreCase))
            {
                faseAtual = 9;
                lblStatus.Text = "=== ETAPA 9: MENSAGEM BINÁRIA ===\nObserva o TÍTULO DA JANELA a piscar!\nConverte o binário para texto e insere a chave no Tray.";
                lblStatus.ForeColor = Color.Yellow;
                this.BackColor = Color.FromArgb(20, 20, 0);
                MostrarAvisoUnico("Mensagem de vibração aceite!");
            }
            else { MostrarAvisoUnico("Chave de vibração incorreta."); }
        });

        trayMenu.Items.Add("Descodificar Binário (F9)", null, (s, e) => {
            if (faseAtual < 9) { MostrarAvisoUnico("Ainda não chegaste a esta etapa!"); return; }
            string input = Microsoft.VisualBasic.Interaction.InputBox("Insere a palavra decodificada do binário:", "Terminal Binário", "");
            if (input.Equals("SECRET", StringComparison.OrdinalIgnoreCase))
            {
                faseAtual = 10;
                lblStatus.Text = "=== ETAPA 10: KILL SWITCH (CHANCE ÚNICA) ===\nO sistema está armado!\nTens todo o tempo necessário, mas **SÓ TENS UMA CHANCE**.\nEscreve 'OVERRIDE-CORE-99' diretamente no teclado e prime [ENTER].\n\nInput atual: ";
                lblStatus.ForeColor = Color.DarkRed;
                this.BackColor = Color.Maroon;
                MostrarAvisoUnico("CUIDADO: Entraste no modo de Tentativa Única da Fase Final!");
            }
            else { MostrarAvisoUnico("Palavra binária incorreta."); }
        });

        trayMenu.Items.Add("Sair", null, (s, e) => { Application.Exit(); });

        trayIcon = new NotifyIcon();
        trayIcon.Icon = SystemIcons.Shield;
        trayIcon.Text = "Escape Room Engine v5.3";
        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.Visible = true;
    }

    private void MostrarAvisoUnico(string mensagem)
    {
        MessageBox.Show(mensagem, "Sistema de Segurança", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void GameLoop(object sender, EventArgs e)
    {
        if (glitchAtivo)
        {
            contadorGlitch++;
            Random rnd = new Random();
            if (contadorGlitch % 3 == 0)
            {
                IntPtr hRgn = CreateRectRgn(rnd.Next(0, 50), rnd.Next(0, 50), this.Width - rnd.Next(0, 50), this.Height - rnd.Next(0, 50));
                SetWindowRgn(this.Handle, hRgn, true);
            }
            if (contadorGlitch > 30)
            {
                glitchAtivo = false;
                contadorGlitch = 0;
                SetWindowRgn(this.Handle, IntPtr.Zero, true);
            }
        }

        if (faseAtual == 9)
        {
            contadorBinario++;
            if (contadorBinario % 40 == 0)
            {
                this.Text = (contadorBinario % 80 == 0) ? "01010011 01000101 01000011 01010010 01000101 01010100" : "[!] DECODIFICA O BINÁRIO [!]";
            }
        }

        try
        {
            if (XInputController.EstaConectado(0))
            {
                if (faseAtual == 2)
                {
                    if (XInputController.EstaPremido(XInputController.BotaoA, 0))
                    {
                        XInputController.Vibrar(65535, 32768, 0);
                        glitchAtivo = true;
                        lblStatus.Text = "=== ETAPA 3 ===\nErro GDI32 Detetado!\nCódigo Oculto: [ 7 7 4 9 ]\n[Vai ao System Tray]";
                        this.BackColor = Color.DarkSlateGray;
                        faseAtual = 3;
                    }
                    else { XInputController.PararVibracao(0); }
                }
                else if (faseAtual == 8)
                {
                    contadorVibracaoMorse++;
                    if (contadorVibracaoMorse % 60 < 10) { XInputController.Vibrar(65535, 0, 0); }
                    else if (contadorVibracaoMorse % 60 > 20 && contadorVibracaoMorse % 60 < 25) { XInputController.Vibrar(20000, 0, 0); }
                    else { XInputController.PararVibracao(0); }
                }
            }
        }
        catch { }
    }

    // Interceção direta de comandos de teclado de nível superior (Resolve o Enter e as letras/números colados)
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (faseAtual == 10)
        {
            if (keyData == Keys.Enter)
            {
                if (inputArmado.Equals("OVERRIDE-CORE-99", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("PARABÉNS, AGENTE SUPREMO! Desarmaste o Kill Switch à primeira e venceste todas as 10 camadas do Escape Room!", "VITÓRIA LENDÁRIA", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("CÓDIGO INCORRETO! A tentativa única expirou e o sistema de segurança ativou o bloqueio permanente.", "FALHA CRÍTICA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
                return true;
            }
            else if (keyData == Keys.Back)
            {
                if (inputArmado.Length > 0)
                {
                    inputArmado = inputArmado.Substring(0, inputArmado.Length - 1);
                    AtualizarDisplayEtapa10();
                }
                return true;
            }
            else
            {
                // Converte a tecla premida no caractere legível exato (letras, números reais e hífen)
                char c = ObterCaractereDaTecla(keyData);
                if (c != '\0')
                {
                    inputArmado += c;
                    AtualizarDisplayEtapa10();
                    return true;
                }
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    // Tradutor fiável para evitar "NumPad9", "D9" ou "OemMinus"
    private char ObterCaractereDaTecla(Keys keyData)
    {
        // Tratar letras de A a Z
        if (keyData >= Keys.A && keyData <= Keys.Z)
        {
            return (char)('A' + (keyData - Keys.A));
        }
        // Tratar números do teclado principal (D0 a D9) e NumPad (NumPad0 a NumPad9)
        else if ((keyData >= Keys.D0 && keyData <= Keys.D9))
        {
            return (char)('0' + (keyData - Keys.D0));
        }
        else if ((keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9))
        {
            return (char)('0' + (keyData - Keys.NumPad0));
        }
        // Tratar o sinal de menos / hífen (-)
        else if (keyData == Keys.OemMinus || keyData == Keys.Subtract)
        {
            return '-';
        }
        return '\0';
    }

    private void TopSecretRoom_KeyDown(object sender, KeyEventArgs e)
    {
        if (faseAtual == 1)
        {
            userInputs.Add(e.KeyCode);
            if (userInputs.Count > konamiSequence.Count) userInputs.RemoveAt(0);

            bool match = true;
            if (userInputs.Count == konamiSequence.Count)
            {
                for (int i = 0; i < konamiSequence.Count; i++)
                {
                    if (userInputs[i] != konamiSequence[i]) { match = false; break; }
                }

                if (match)
                {
                    faseAtual = 2;
                    lblStatus.Text = "=== ETAPA 2: HARDWARE ===\nLiga o comando e prime [ BOTÃO A ].";
                    lblStatus.ForeColor = Color.LimeGreen;
                    this.BackColor = Color.FromArgb(0, 30, 0);
                    userInputs.Clear();
                }
            }
        }
    }

    private void AtualizarDisplayEtapa10()
    {
        lblStatus.Text = "=== ETAPA 10: KILL SWITCH (CHANCE ÚNICA) ===\nTens todo o tempo necessário, mas SÓ TENS UMA CHANCE.\nEscreve 'OVERRIDE-CORE-99' e prime [ENTER].\n\nInput atual: " + inputArmado;
    }

    private void BtnSecretoCanto_Click(object sender, EventArgs e)
    {
        if (faseAtual == 4)
        {
            cliquesCantos++;
            if (cliquesCantos >= 3)
            {
                faseAtual = 5;
                btnSecretoCanto.Visible = false;
                lblStatus.Text = "=== ETAPA 5: FIREWALL DE DISCO ===\nCria um ficheiro 'root.txt' com 'OVERRIDE'.\n[Vai ao System Tray]";
                lblStatus.ForeColor = Color.Gold;
                this.BackColor = Color.FromArgb(40, 20, 0);
                MostrarAvisoUnico("Calibragem de cliques concluída!");
            }
            else
            {
                lblStatus.Text = "=== ETAPA 4 ===\nClica mais " + (3 - cliquesCantos) + " vez(es).";
            }
        }
    }

    private void BtnInterativo_Click(object sender, EventArgs e)
    {
        if (faseAtual == 6)
        {
            DateTime agora = DateTime.Now;
            if ((agora - ultimoClique).TotalMilliseconds > 1200) { cliquesVelocidade = 0; }
            ultimoClique = agora;
            cliquesVelocidade++;

            if (cliquesVelocidade >= 5)
            {
                faseAtual = 7;
                btnInterativo.Visible = false;
                this.Opacity = 0.15;
                lblStatus.Text = "=== ETAPA 7: MODO FANTASMA ===\nA janela ficou translúcida.\nA palavra escondida é GHOST.\n[Vai ao System Tray]";
                lblStatus.ForeColor = Color.White;
                MostrarAvisoUnico("Teste de reflexos concluído com sucesso!");
            }
            else
            {
                lblStatus.Text = "=== ETAPA 6 ===\nRápido! Faltam " + (5 - cliquesVelocidade) + " cliques!";
            }
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        trayIcon.Visible = false;
        trayIcon.Dispose();
        base.OnFormClosed(e);
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TopSecretRoom());
    }
}