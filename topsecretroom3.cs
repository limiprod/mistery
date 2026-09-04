using System;
using System.Drawing;
using System.Windows.Forms;
using AsciiButtonLang;
using PassMaluco;
using Sql;

public class EscapeRoomForm : Form
{
    private Label lblTitulo;
    private Label lblInstrucao;
    private TextBox txtInput;
    private Button btnSubmeter;
    private int nivelAtual = 1;

    public EscapeRoomForm()
    {
        this.Text = "Escape Room Gráfico - Modo Extremo 10 Níveis";
        this.Size = new Size(580, 450);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(20, 20, 20);

        lblTitulo = new Label();
        lblTitulo.Text = "=== ESCAPE ROOM GUI (NÍVEL 1/10) ===";
        lblTitulo.ForeColor = Color.Cyan;
        lblTitulo.Font = new Font("Arial", 12, FontStyle.Bold);
        lblTitulo.Location = new Point(30, 25);
        lblTitulo.AutoSize = true;
        this.Controls.Add(lblTitulo);

        lblInstrucao = new Label();
        lblInstrucao.Text = "Nível 1: Introduz a password de acesso (Dica: 'raio'):";
        lblInstrucao.ForeColor = Color.White;
        lblInstrucao.Font = new Font("Arial", 10, FontStyle.Regular);
        lblInstrucao.Location = new Point(30, 75);
        lblInstrucao.AutoSize = true;
        this.Controls.Add(lblInstrucao);

        txtInput = new TextBox();
        txtInput.Location = new Point(30, 125);
        txtInput.Size = new Size(360, 25);
        txtInput.Font = new Font("Arial", 10);
        this.Controls.Add(txtInput);

        btnSubmeter = new Button();
        btnSubmeter.Text = "Validar";
        btnSubmeter.Location = new Point(410, 123);
        btnSubmeter.Size = new Size(115, 30);
        btnSubmeter.BackColor = Color.Green;
        btnSubmeter.ForeColor = Color.White;
        btnSubmeter.FlatStyle = FlatStyle.Flat;
        btnSubmeter.Click += new EventHandler(BtnSubmeter_Click);
        this.Controls.Add(btnSubmeter);
    }

    private void BtnSubmeter_Click(object sender, EventArgs e)
    {
        string resposta = txtInput.Text.Trim().ToLower();

        switch (nivelAtual)
        {
            case 1:
                if (resposta == "raio")
                    AvancarNivel(2, "=== ESCAPE ROOM GUI (NÍVEL 2/10) ===", "Nível 2: Firewall Ativa. Qual é o código de erro padrão? ('404'):");
                else
                    DispararErroVisual("Password incorreta! O sistema disparou um alerta.");
                break;

            case 2:
                if (resposta == "404")
                    AvancarNivel(3, "=== ESCAPE ROOM GUI (NÍVEL 3/10) ===", "Nível 3: Protocolo Visual. Qual a cor primária de perigo? ('red'):");
                else
                    MessageBox.Show("Código de firewall recusado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 3:
                if (resposta == "red")
                    AvancarNivel(4, "=== ESCAPE ROOM GUI (NÍVEL 4/10) ===", "Nível 4: Sincronização. Quantos segundos tem um minuto? ('60'):");
                else
                    DispararErroVisual("Cor incorreta detetada pela câmara térmica!");
                break;

            case 4:
                if (resposta == "60")
                    AvancarNivel(5, "=== ESCAPE ROOM GUI (NÍVEL 5/10) ===", "Nível 5: Criptografia. Escreve a palavra-passe quântica ('MATRIX'):");
                else
                    MessageBox.Show("Tempo dessincronizado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 5:
                if (resposta == "matrix")
                    AvancarNivel(6, "=== ESCAPE ROOM GUI (NÍVEL 6/10) ===", "Nível 6: Vetor Cardinal. Qual o oposto do Norte? ('sul'):");
                else
                    MessageBox.Show("Chave quântica inválida!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 6:
                if (resposta == "sul")
                    AvancarNivel(7, "=== ESCAPE ROOM GUI (NÍVEL 7/10) ===", "Nível 7: Teste de Bits. Qual o decimal de '1010'? ('10'):");
                else
                    MessageBox.Show("Coordenada errada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 7:
                if (resposta == "10")
                    AvancarNivel(8, "=== ESCAPE ROOM GUI (NÍVEL 8/10) ===", "Nível 8: Alarme Sonoro. Escreve o código do altifalante ('bipbip'):");
                else
                    MessageBox.Show("Erro de paridade nos bits!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 8:
                if (resposta == "bipbip")
                    AvancarNivel(9, "=== ESCAPE ROOM GUI (NÍVEL 9/10) ===", "Nível 9: Reforço de Núcleo. Insere o código de emergência ('777'):");
                else
                    MessageBox.Show("Frequência acústica rejeitada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 9:
                if (resposta == "777")
                    AvancarNivel(10, "=== ESCAPE ROOM GUI (NÍVEL 10/10 - FINAL) ===", "Nível 10: Aniquilação. Escreve 'DESTRUIR' para rebentar com a BD:");
                else
                    MessageBox.Show("Código do reator incorreto!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;

            case 10:
                if (resposta == "destruir")
                {
                    BaseDeDados db = new BaseDeDados("escape_gui.db");
                    db.Inserir("Estado", 0.0m);
                    db.destruir();

                    MessageBox.Show("PARABÉNS! Superaste todos os 10 níveis gráficos!", "Vitória Definitiva", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("Hesitaste! O sistema bloqueou a interface para sempre.", "Fim de Jogo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
                break;
        }
    }

    private void AvancarNivel(int proximoNivel, string novoTitulo, string novaInstrucao)
    {
        MessageBox.Show("Nível " + (nivelAtual) + " superado com sucesso!", "Painel de Controlo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        nivelAtual = proximoNivel;
        lblTitulo.Text = novoTitulo;
        lblInstrucao.Text = novaInstrucao;
        txtInput.Clear();
    }

    private void DispararErroVisual(string mensagem)
    {
        MessageBox.Show(mensagem, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
        GdiEffects.FlashScreen(2);
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new EscapeRoomForm());
    }
}