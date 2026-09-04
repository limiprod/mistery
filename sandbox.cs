using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using Point = System.Drawing.Point;
using UserControl = System.Windows.Controls.UserControl;

namespace TopSecretSandbox
{
    public class EnigmaObj
    {
        public string Id;
        public string Pergunta;
        public string Resposta;
        public bool ResolvidoPeloUtilizador;
    }

    public class TargetObj
    {
        public string Nome;
        public string EnigmaId;
    }

    class Program
    {
        public static Dictionary<string, string> virtualFiles = new Dictionary<string, string>();
        public static Dictionary<string, string> webPages = new Dictionary<string, string>();
        public static Dictionary<string, string> superiaCustomCommands = new Dictionary<string, string>();
        public static Dictionary<string, string> auraCustomCommands = new Dictionary<string, string>();

        public static Dictionary<string, EnigmaObj> riddles = new Dictionary<string, EnigmaObj>();
        public static Dictionary<string, TargetObj> targets = new Dictionary<string, TargetObj>();

        static string currentDirectory = "/home/operator/";
        static Random rand = new Random();

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "TOP SECRET ROOMS - CONSOLE SANDBOX";
            Console.ForegroundColor = ConsoleColor.Green;

            InicializarFicheirosVirtuais();
            InicializarServidorWeb();

            Console.WriteLine("==================================================");
            Console.WriteLine(" [!] TOP SECRET ROOMS - CONSOLE SANDBOX v8.0 (UNIFIED AI)");
            Console.WriteLine(" [!] Digite 'help' para ver a lista de comandos.");
            Console.WriteLine("==================================================\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(currentDirectory + " $ ");
                Console.ForegroundColor = ConsoleColor.Green;

                string comandoBruto = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(comandoBruto)) continue;

                string[] partes = comandoBruto.Trim().Split(new char[] { ' ' }, 3);
                string cmd = partes[0].ToLower();
                string arg1 = partes.Length > 1 ? partes[1] : "";
                string arg2 = partes.Length > 2 ? partes[2] : "";

                switch (cmd)
                {
                    case "help":
                        MostrarAjuda();
                        break;
                    case "ls":
                        ListarFicheiros();
                        break;
                    case "cat":
                        LerFicheiro(arg1);
                        break;
                    case "touch":
                        CriarFicheiro(arg1);
                        break;
                    case "write":
                        EscreverFicheiro(arg1, arg2);
                        break;
                    case "rm":
                        DestruirFicheiro(arg1);
                        break;
                    case "mv":
                    case "rename":
                        RenomearFicheiro(arg1, arg2);
                        break;
                    case "eniadd":
                        AdicionarEnigmaCmd(comandoBruto);
                        break;
                    case "enirem":
                        RemoverEnigmaCmd(arg1);
                        break;
                    case "solve":
                        ResolverEnigmaCmd(comandoBruto);
                        break;
                    case "addopon":
                        AdicionarPessoaCmd(arg1);
                        break;
                    case "secureopon":
                        SecundarPessoaCmd(arg1, arg2);
                        break;
                    case "remopon":
                        RemoverPessoaCmd(arg1);
                        break;
                    case "hack":
                        HackearPessoaCmd(arg1);
                        break;
                    case "supadd":
                        AdicionarComandoSuperia(arg1, arg2);
                        break;
                    case "suprem":
                        RemoverComandoSuperia(arg1);
                        break;
                    case "aura":
                        AtivarAura();
                        break;
                    case "iad":
                        AdicionarComandoAura(arg1, arg2);
                        break;
                    case "iar":
                        RemoverComandoAura(arg1);
                        break;
                    case "addurl":
                        AdicionarUrl(arg1, arg2);
                        break;
                    case "remurl":
                        RemoverUrl(arg1);
                        break;
                    case "browser":
                        AbrirJanelaBrowser(arg1);
                        break;
                    case "xamleditor":
                        AbrirEditorXaml();
                        break;
                    case "matrix":
                        EfeitoMatrix();
                        break;
                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;
                    case "whoami":
                        Console.WriteLine("Utilizador atual: root_sandbox [Privilégios Totais]");
                        break;
                    case "ping":
                        SimularPing(arg1);
                        break;
                    case "exit":
                    case "quit":
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Comando desconhecido: '" + cmd + "'. Digite 'help' para ajuda.");
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }
                Console.WriteLine();
            }
        }

        static List<string> ParseArgs(string line)
        {
            var list = new List<string>();
            bool inQuotes = false;
            string current = "";
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        list.Add(current);
                        current = "";
                    }
                }
                else
                {
                    current += c;
                }
            }
            if (current.Length > 0) list.Add(current);
            return list;
        }

        static void ExecutarCalculoInterno(string expressao)
        {
            try
            {
                var dt = new DataTable();
                var v = dt.Compute(expressao, "");
                Console.WriteLine("[Aura]: 'O resultado do cálculo é: " + v + "'");
            }
            catch
            {
                Console.WriteLine("[Aura]: 'Não consegui calcular essa expressão matemática.'");
            }
        }

        static void PesquisaInternaAura(string termo)
        {
            Console.WriteLine("[Aura]: 'A pesquisar ficheiros por: " + termo + "'");
            bool encontrado = false;
            foreach (var file in virtualFiles.Keys)
            {
                if (file.Contains(termo.ToLower()))
                {
                    Console.WriteLine("  -> Ficheiro encontrado: " + file);
                    encontrado = true;
                }
            }
            if (!encontrado)
            {
                Console.WriteLine("[Aura]: 'Nenhum ficheiro correspondente foi encontrado na base de dados virtual.'");
            }
        }

        static void SuperiaInternaAura(string pergunta)
        {
            if (string.IsNullOrEmpty(pergunta))
            {
                Console.WriteLine("[Aura/Superia]: 'Por favor, indique a pergunta para a Super IA (ex: superia [pergunta]).'");
                return;
            }

            string q = pergunta.ToLower();
            foreach (var regra in superiaCustomCommands)
            {
                if (q.Contains(regra.Key))
                {
                    Console.WriteLine("[Aura/Superia]: '" + regra.Value + "'");
                    return;
                }
            }

            Console.WriteLine("[Aura/Superia]: 'Analisei a consulta avançada, mas não encontrei nenhum registo correspondente.'");
        }

        static void AtivarAura()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n[AURA AI]: 'Olá! Chat ativo. Podes usar: pesquisa: [termo], calc [expressão], superia [pergunta], ou conversar normalmente. Digite 'sair' para voltar.'");
            while (true)
            {
                Console.Write("Aura > ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                string inputLower = input.ToLower();
                if (inputLower == "sair" || inputLower == "exit") break;

                // 1. Pesquisa de ficheiros interna ("pesquisa: [termo]")
                if (inputLower.StartsWith("pesquisa:"))
                {
                    string termo = input.Substring(9).Trim();
                    PesquisaInternaAura(termo);
                    continue;
                }

                // 2. Cálculos matemáticos internos ("calc [expressão]")
                if (inputLower.StartsWith("calc "))
                {
                    string expr = input.Substring(5).Trim();
                    ExecutarCalculoInterno(expr);
                    continue;
                }

                // 3. Super IA integrada ("superia [pergunta]")
                if (inputLower.StartsWith("superia "))
                {
                    string perg = input.Substring(8).Trim();
                    SuperiaInternaAura(perg);
                    continue;
                }

                // 4. Verificar comandos personalizados da Aura (iad)
                string respostaEncontrada = null;
                foreach (var par in auraCustomCommands)
                {
                    if (inputLower.Contains(par.Key))
                    {
                        respostaEncontrada = par.Value;
                        break;
                    }
                }

                if (respostaEncontrada != null)
                {
                    Console.WriteLine("[Aura]: '" + respostaEncontrada + "'");
                    continue;
                }

                // 5. Conhecimentos básicos integrados da Aura
                if (inputLower.Contains("olá") || inputLower.Contains("ola") || inputLower.Contains("hi") || inputLower.Contains("hello"))
                {
                    Console.WriteLine("[Aura]: 'Olá, operador! Em que posso ser útil hoje?'");
                }
                else if (inputLower.Contains("quem és") || inputLower.Contains("quem es") || inputLower.Contains("teu nome"))
                {
                    Console.WriteLine("[Aura]: 'Eu sou a Aura, a tua assistente virtual com pesquisa, cálculos e Super IA integrados.'");
                }
                else if (inputLower.Contains("como estás") || inputLower.Contains("como estas"))
                {
                    Console.WriteLine("[Aura]: 'Os meus sistemas operacionais estão a 100% e plenamente operacionais.'");
                }
                else if (inputLower.Contains("ajuda") || inputLower.Contains("help"))
                {
                    Console.WriteLine("[Aura]: 'Comandos disponíveis aqui: pesquisa: [termo], calc [conta], superia [pergunta].'");
                }
                else
                {
                    Console.WriteLine("[Aura]: 'Compreendido.'");
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }

        static void AdicionarEnigmaCmd(string linhaBruta)
        {
            var tokens = ParseArgs(linhaBruta);
            if (tokens.Count < 4)
            {
                Console.WriteLine("Uso correto: eniadd [id] \"[pergunta]\" \"[resposta]\"");
                return;
            }
            string id = tokens[1].ToLower();
            string pergunta = tokens[2];
            string resposta = tokens[3];

            riddles[id] = new EnigmaObj { Id = id, Pergunta = pergunta, Resposta = resposta, ResolvidoPeloUtilizador = false };
            Console.WriteLine("[+] Enigma '" + id + "' adicionado com sucesso.");
        }

        static void RemoverEnigmaCmd(string arg1)
        {
            if (string.IsNullOrEmpty(arg1))
            {
                Console.WriteLine("Uso correto: enirem [id]");
                return;
            }
            string id = arg1.ToLower();
            if (riddles.ContainsKey(id))
            {
                riddles.Remove(id);
                foreach (var t in targets.Values)
                {
                    if (t.EnigmaId == id) t.EnigmaId = "";
                }
                Console.WriteLine("[+] Enigma '" + id + "' removido.");
            }
            else
            {
                Console.WriteLine("[-] Enigma não encontrado.");
            }
        }

        static void ResolverEnigmaCmd(string linhaBruta)
        {
            var tokens = ParseArgs(linhaBruta);
            if (tokens.Count < 3)
            {
                Console.WriteLine("Uso correto: solve [id] \"[resposta]\"");
                return;
            }
            string id = tokens[1].ToLower();
            string tentativa = tokens[2];

            if (riddles.ContainsKey(id))
            {
                var eni = riddles[id];
                if (eni.Resposta.Equals(tentativa, StringComparison.OrdinalIgnoreCase))
                {
                    eni.ResolvidoPeloUtilizador = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] Enigma '" + id + "' resolvido com sucesso! Acesso concedido para alvos associados.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[-] Resposta incorreta para o enigma '" + id + "'.");
                }
            }
            else
            {
                Console.WriteLine("[-] Enigma não encontrado.");
            }
        }

        static void AdicionarPessoaCmd(string nome)
        {
            if (string.IsNullOrEmpty(nome))
            {
                Console.WriteLine("Uso correto: addopon [nome]");
                return;
            }
            string n = nome.ToLower();
            if (!targets.ContainsKey(n))
            {
                targets[n] = new TargetObj { Nome = nome, EnigmaId = "" };
                Console.WriteLine("[+] Alvo/Pessoa '" + nome + "' adicionado com sucesso.");
            }
            else
            {
                Console.WriteLine("[-] Esse alvo já existe.");
            }
        }

        static void SecundarPessoaCmd(string nome, string eniId)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(eniId))
            {
                Console.WriteLine("Uso correto: secureopon [nome] [enigma_id]");
                return;
            }
            string n = nome.ToLower();
            string id = eniId.ToLower();

            if (!targets.ContainsKey(n))
            {
                Console.WriteLine("[-] Alvo não encontrado. Adicione com addopon primeiro.");
                return;
            }
            if (!riddles.ContainsKey(id))
            {
                Console.WriteLine("[-] Enigma não encontrado. Adicione com eniadd primeiro.");
                return;
            }

            targets[n].EnigmaId = id;
            Console.WriteLine("[+] Alvo '" + nome + "' securizado com o enigma '" + id + "'.");
        }

        static void RemoverPessoaCmd(string nome)
        {
            if (string.IsNullOrEmpty(nome))
            {
                Console.WriteLine("Uso correto: remopon [nome]");
                return;
            }
            string n = nome.ToLower();
            if (targets.ContainsKey(n))
            {
                targets.Remove(n);
                Console.WriteLine("[+] Alvo '" + nome + "' removido.");
            }
            else
            {
                Console.WriteLine("[-] Alvo não encontrado.");
            }
        }

        static void HackearPessoaCmd(string nome)
        {
            if (string.IsNullOrEmpty(nome))
            {
                Console.WriteLine("Uso correto: hack [nome]");
                return;
            }
            string n = nome.ToLower();
            if (!targets.ContainsKey(n))
            {
                Console.WriteLine("[-] Alvo não encontrado.");
                return;
            }

            var target = targets[n];
            if (string.IsNullOrEmpty(target.EnigmaId))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Alvo '" + target.Nome + "' hackeado com sucesso! (Sem proteção)");
            }
            else
            {
                string eniId = target.EnigmaId;
                if (riddles.ContainsKey(eniId) && riddles[eniId].ResolvidoPeloUtilizador)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] Alvo '" + target.Nome + "' protegido pelo enigma '" + eniId + "', mas você já o resolveu. Hack bem-sucedido!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[-] ACESSO NEGADO! O alvo '" + target.Nome + "' está protegido pelo enigma '" + eniId + "'.");
                    if (riddles.ContainsKey(eniId))
                    {
                        Console.WriteLine("    [Pergunta do Enigma]: " + riddles[eniId].Pergunta);
                        Console.WriteLine("    [Dica]: Resolva primeiro usando o comando: solve " + eniId + " [resposta]");
                    }
                }
            }
        }

        static void InicializarFicheirosVirtuais()
        {
            virtualFiles["/home/operator/welcome.txt"] = "Bem-vindo ao terminal sandbox. Podes criar, editar e apagar ficheiros livremente.";
            virtualFiles["/home/operator/passwords.cfg"] = "infinity_admin=root_2026\nnetsecure_sys=alpha_beta_99";
            virtualFiles["/etc/network.conf"] = "IP: 192.168.1.150\nGateway: 192.168.1.1\nStatus: Secure";
        }

        static void InicializarServidorWeb()
        {
            webPages["infinitycorps.local"] = "<html><body style='background:#121212;color:#4ec9b0;font-family:sans-serif;padding:30px;'><h1>Infinity Corps Portal</h1><p>O futuro digital pertence à Infinity Corps.</p><hr/><a href='http://net-secure.local/index.html' style='color:#569cd6;font-size:16px;'>Ir para NetSecure Corp Firewall</a></body></html>";
            webPages["net-secure.local"] = "<html><body style='background:#000000;color:#00ff00;font-family:monospace;padding:30px;'><h1>NetSecure Corp Firewall</h1><p>Sistema protegido contra intrusões de nível militar.</p><hr/><a href='http://infinitycorps.local/index.html' style='color:#ffff00;font-size:16px;'>Voltar para Infinity Corps</a></body></html>";
        }

        static void MostrarAjuda()
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("          TOP SECRET ROOMS - HELP MANUAL          ");
            Console.WriteLine("==================================================");
            Console.WriteLine("\n--- Gestão de Ficheiros Virtuais ---");
            Console.WriteLine("  ls                        - Lista os ficheiros virtuais");
            Console.WriteLine("  cat [ficheiro]            - Lê o conteúdo de um ficheiro");
            Console.WriteLine("  touch [ficheiro]          - Cria um novo ficheiro vazio");
            Console.WriteLine("  write [fich] [conteúdo]   - Escreve conteúdo num ficheiro");
            Console.WriteLine("  rm [ficheiro]             - Apaga um ficheiro");

            Console.WriteLine("\n--- Sistema de Enigmas e Hacks ---");
            Console.WriteLine("  eniadd [id] [perg] [resp] - Adiciona um enigma");
            Console.WriteLine("  enirem [id]               - Remove um enigma");
            Console.WriteLine("  solve [id] [resp]         - Resolve um enigma");
            Console.WriteLine("  addopon [nome]            - Adiciona uma pessoa/alvo de hack");
            Console.WriteLine("  secureopon [nome] [id]    - Protege a pessoa com um enigma");
            Console.WriteLine("  remopon [nome]            - Remove uma pessoa/alvo");
            Console.WriteLine("  hack [nome]               - Tenta hackear uma pessoa");

            Console.WriteLine("\n--- Inteligências Artificiais ---");
            Console.WriteLine("  supadd [gatilho] [resp]   - Adiciona regra à Super IA");
            Console.WriteLine("  suprem [gatilho]          - Remove regra da Super IA");
            Console.WriteLine("  aura                      - Entra no chat da Aura (com pesquisa, calc e superia integrados)");
            Console.WriteLine("  iad [palavra] [resp]      - Adiciona palavra à Aura");
            Console.WriteLine("  iar [palavra]             - Remove palavra da Aura");

            Console.WriteLine("\n--- Interfaces Gráficas & Outros ---");
            Console.WriteLine("  xamleditor                - Abre o Editor XAML");
            Console.WriteLine("  browser [url]             - Abre o navegador web gráfico");
            Console.WriteLine("  matrix                    - Efeito visual Matrix");
            Console.WriteLine("  clear                     - Limpa o terminal");
            Console.WriteLine("  exit                      - Sai da aplicação");
            Console.WriteLine("==================================================");
        }

        static void AdicionarComandoSuperia(string gatilho, string resposta)
        {
            if (string.IsNullOrEmpty(gatilho) || string.IsNullOrEmpty(resposta))
            {
                Console.WriteLine("Uso correto: supadd [gatilho] [resposta]");
                return;
            }
            superiaCustomCommands[gatilho.ToLower()] = resposta;
            Console.WriteLine("[+] Regra adicionada à Super IA para o gatilho: '" + gatilho + "'");
        }

        static void RemoverComandoSuperia(string gatilho)
        {
            if (string.IsNullOrEmpty(gatilho))
            {
                Console.WriteLine("Uso correto: suprem [gatilho]");
                return;
            }
            string chave = gatilho.ToLower();
            if (superiaCustomCommands.ContainsKey(chave))
            {
                superiaCustomCommands.Remove(chave);
                Console.WriteLine("[+] Gatilho removido da Super IA.");
            }
            else
            {
                Console.WriteLine("[-] Esse gatilho não existe.");
            }
        }

        static void AdicionarComandoAura(string palavra, string resposta)
        {
            if (string.IsNullOrEmpty(palavra) || string.IsNullOrEmpty(resposta))
            {
                Console.WriteLine("Uso correto: iad [palavra] [resposta]");
                return;
            }
            auraCustomCommands[palavra.ToLower()] = resposta;
            Console.WriteLine("[+] Resposta adicionada à Aura para a palavra: '" + palavra + "'");
        }

        static void RemoverComandoAura(string palavra)
        {
            if (string.IsNullOrEmpty(palavra))
            {
                Console.WriteLine("Uso correto: iar [palavra]");
                return;
            }
            string chave = palavra.ToLower();
            if (auraCustomCommands.ContainsKey(chave))
            {
                auraCustomCommands.Remove(chave);
                Console.WriteLine("[+] Palavra removida da Aura.");
            }
            else
            {
                Console.WriteLine("[-] Essa palavra não existe.");
            }
        }

        static void AdicionarUrl(string url, string htmlConteudo)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(htmlConteudo))
            {
                Console.WriteLine("Uso: addurl [url] [conteudo_html]");
                return;
            }
            url = url.Replace("http://", "").Replace("https://", "").ToLower();
            webPages[url] = htmlConteudo;
            Console.WriteLine("[+] URL '" + url + "' adicionado com sucesso.");
        }

        static void RemoverUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Console.WriteLine("Uso: remurl [url]");
                return;
            }
            url = url.Replace("http://", "").Replace("https://", "").ToLower();
            if (webPages.ContainsKey(url))
            {
                webPages.Remove(url);
                Console.WriteLine("[+] URL '" + url + "' removido.");
            }
            else
            {
                Console.WriteLine("[-] URL não encontrada.");
            }
        }

        static string ResolverCaminho(string nome)
        {
            if (nome.StartsWith("/")) return nome;
            return currentDirectory + nome;
        }

        static void ListarFicheiros()
        {
            Console.WriteLine("\n[Ficheiros Virtuais em " + currentDirectory + "]");
            foreach (var file in virtualFiles.Keys)
            {
                if (file.StartsWith(currentDirectory))
                {
                    string nomeRelativo = file.Substring(currentDirectory.Length);
                    Console.WriteLine("  -rw-r--r--  " + nomeRelativo);
                }
            }
        }

        static void LerFicheiro(string nome)
        {
            string caminho = ResolverCaminho(nome);
            if (virtualFiles.ContainsKey(caminho))
            {
                Console.WriteLine("\n--- Conteúdo de " + nome + " ---");
                Console.WriteLine(virtualFiles[caminho]);
                Console.WriteLine("---------------------------------");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erro: Ficheiro '" + nome + "' não encontrado.");
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }

        static void CriarFicheiro(string nome)
        {
            if (string.IsNullOrEmpty(nome)) { Console.WriteLine("Uso: touch [nome_do_ficheiro]"); return; }
            string caminho = ResolverCaminho(nome);
            if (!virtualFiles.ContainsKey(caminho))
            {
                virtualFiles[caminho] = "";
                Console.WriteLine("[+] Ficheiro criado.");
            }
            else
            {
                Console.WriteLine("[-] O ficheiro já existe.");
            }
        }

        static void EscreverFicheiro(string nome, string conteudo)
        {
            if (string.IsNullOrEmpty(nome)) { Console.WriteLine("Uso: write [ficheiro] [conteúdo]"); return; }
            string caminho = ResolverCaminho(nome);
            virtualFiles[caminho] = conteudo;
            Console.WriteLine("[+] Conteúdo gravado.");
        }

        static void DestruirFicheiro(string nome)
        {
            if (string.IsNullOrEmpty(nome)) { Console.WriteLine("Uso: rm [nome_do_ficheiro]"); return; }
            string caminho = ResolverCaminho(nome);
            if (virtualFiles.ContainsKey(caminho))
            {
                virtualFiles.Remove(caminho);
                Console.WriteLine("[+] Ficheiro removido.");
            }
            else
            {
                Console.WriteLine("[-] Ficheiro não encontrado.");
            }
        }

        static void RenomearFicheiro(string antigo, string novo)
        {
            if (string.IsNullOrEmpty(antigo) || string.IsNullOrEmpty(novo)) { Console.WriteLine("Uso: mv [antigo] [novo]"); return; }
            string caminhoAntigo = ResolverCaminho(antigo);
            string caminhoNovo = ResolverCaminho(novo);
            if (virtualFiles.ContainsKey(caminhoAntigo))
            {
                string conteudo = virtualFiles[caminhoAntigo];
                virtualFiles.Remove(caminhoAntigo);
                virtualFiles[caminhoNovo] = conteudo;
                Console.WriteLine("[+] Ficheiro renomeado.");
            }
            else
            {
                Console.WriteLine("[-] Origem não encontrada.");
            }
        }

        static void AbrirJanelaBrowser(string url)
        {
            Thread t = new Thread(() =>
            {
                BrowserForm form = new BrowserForm(url);
                System.Windows.Forms.Application.Run(form);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        static void AbrirEditorXaml()
        {
            Thread t = new Thread(() =>
            {
                XamlEditorForm form = new XamlEditorForm();
                System.Windows.Forms.Application.Run(form);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        static void EfeitoMatrix()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(rand.Next(100000, 999999) + " 01011001 110010");
                Thread.Sleep(25);
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }

        static void SimularPing(string host)
        {
            if (string.IsNullOrEmpty(host)) host = "infinitycorps.local";
            Console.WriteLine("\nA fazer ping a " + host + "...");
            Thread.Sleep(400);
            Console.WriteLine("Resposta de " + host + ": bytes=32 tempo=12ms TTL=118");
        }
    }

    public class BrowserForm : Form
    {
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Label lblStatus;

        public BrowserForm(string urlInicial)
        {
            this.Text = "Browser Window";
            this.Width = 850;
            this.Height = 620;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 20);

            lblStatus = new System.Windows.Forms.Label();
            lblStatus.Text = "Pronto";
            lblStatus.Location = new Point(10, 10);
            lblStatus.Width = 810;
            lblStatus.ForeColor = Color.FromArgb(0, 200, 255);
            this.Controls.Add(lblStatus);

            webBrowser = new System.Windows.Forms.WebBrowser();
            webBrowser.Location = new Point(10, 40);
            webBrowser.Width = 815;
            webBrowser.Height = 520;
            webBrowser.ScriptErrorsSuppressed = true;

            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(Browser_Navigating);

            this.Controls.Add(webBrowser);

            if (string.IsNullOrEmpty(urlInicial)) CarregarPagina("infinitycorps.local");
            else CarregarPagina(urlInicial);
        }

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url != null)
            {
                string urlDestino = e.Url.ToString();

                foreach (var pagina in Program.webPages.Keys)
                {
                    if (urlDestino.ToLower().Contains(pagina))
                    {
                        e.Cancel = true;
                        webBrowser.DocumentText = Program.webPages[pagina];
                        lblStatus.Text = "Acedido: " + pagina;
                        return;
                    }
                }
            }
        }

        private void CarregarPagina(string url)
        {
            url = url.ToLower().Replace("http://", "").Replace("https://", "").Trim('/');
            foreach (var pagina in Program.webPages.Keys)
            {
                if (url.Contains(pagina))
                {
                    webBrowser.DocumentText = Program.webPages[pagina];
                    lblStatus.Text = "Acedido: " + pagina;
                    return;
                }
            }
            webBrowser.DocumentText = "<html><body style='background:#121212;color:#ff5555;font-family:sans-serif;padding:20px;'><h1>404 Not Found</h1><p>A página solicitada não existe no servidor virtual.</p></body></html>";
            lblStatus.Text = "Erro 404";
        }
    }

    public class XamlEditorForm : Form
    {
        private System.Windows.Forms.TextBox txtEditor;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Integration.ElementHost elementHost;

        public XamlEditorForm()
        {
            this.Text = "Editor XAML";
            this.Width = 1050;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            txtEditor = new System.Windows.Forms.TextBox();
            txtEditor.Location = new Point(15, 45);
            txtEditor.Width = 480;
            txtEditor.Height = 540;
            txtEditor.Multiline = true;
            txtEditor.Text = "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock Text=\"Teste\" Foreground=\"White\"/></Grid>";
            this.Controls.Add(txtEditor);

            System.Windows.Forms.Button btnRun = new System.Windows.Forms.Button();
            btnRun.Text = "▶ EXECUTAR";
            btnRun.Location = new Point(15, 595);
            btnRun.Width = 480;
            btnRun.Height = 45;
            btnRun.Click += new EventHandler((s, e) => ExecutarXaml());
            this.Controls.Add(btnRun);

            elementHost = new System.Windows.Forms.Integration.ElementHost();
            elementHost.Location = new Point(515, 45);
            elementHost.Width = 505;
            elementHost.Height = 540;
            this.Controls.Add(elementHost);

            lblError = new System.Windows.Forms.Label();
            lblError.Location = new Point(515, 595);
            lblError.Width = 505;
            lblError.Height = 45;
            this.Controls.Add(lblError);

            ExecutarXaml();
        }

        private void ExecutarXaml()
        {
            try
            {
                object parsedObj = XamlReader.Parse(txtEditor.Text);
                if (parsedObj is UIElement)
                {
                    elementHost.Child = (UIElement)parsedObj;
                    lblError.Text = "Sucesso!";
                }
                else
                {
                    lblError.Text = "Erro: Raiz deve ser UIElement.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Erro: " + ex.Message;
            }
        }
    }
}