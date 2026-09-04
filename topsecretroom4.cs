using System;
using System.Threading;

namespace InfinityHardcoreGame
{
    class Program
    {
        static bool firewallBypassed = false;
        static bool enigma1Solved = false;
        static bool enigma2Solved = false;
        static bool hasRoot = false;

        static void Main(string[] args)
        {
            Console.Title = "INFINITY CORPS SECURE TERMINAL [NIGHTMARE MODE]";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Console.WriteLine("==========================================================");
            Console.WriteLine(" [!] TERMINAL DE SEGURANÇA MÁXIMA - INFINITY CORPS");
            Console.WriteLine(" [!] AVISO: Tentativas de intrusão detetadas e registadas.");
            Console.WriteLine(" [!] Digita 'help' para ver os comandos de exploração.");
            Console.WriteLine("==========================================================\n");

            while (true)
            {
                Console.Write("\n[operator@infinity-core:~]$ ");
                string comando = Console.ReadLine().Trim().ToLower();

                if (string.IsNullOrEmpty(comando)) continue;

                string[] partes = comando.Split(new char[] { ' ' }, 2);
                string cmd = partes[0];
                string arg = partes.Length > 1 ? partes[1].Trim() : "";

                switch (cmd)
                {
                    case "help":
                        MostrarAjuda();
                        break;

                    case "ls":
                        ListarDiretorio();
                        break;

                    case "cat":
                        LerFicheiro(arg);
                        break;

                    case "solve":
                        ResolverEnigma(arg);
                        break;

                    case "hack":
                        ExecutarHack(arg);
                        break;

                    case "override":
                        ExecutarOverride();
                        break;

                    case "matrix":
                        AtivarMatrix();
                        break;

                    case "exit":
                    case "quit":
                        Console.WriteLine("A desligar a ligação... Os teus rastos foram eliminados.");
                        return;

                    default:
                        Console.WriteLine("Comando desconhecido. Tens de pensar como um hacker, não como um script kiddie.");
                        break;
                }
            }
        }

        static void MostrarAjuda()
        {
            Console.WriteLine("\n--- COMANDOS DO SISTEMA ---");
            Console.WriteLine("  ls                  - Lista o diretório atual");
            Console.WriteLine("  cat [ficheiro]      - Lê documentos de texto");
            Console.WriteLine("  solve [enigma_id]   - Submete a resposta a um enigma detetado");
            Console.WriteLine("  hack [alvo]         - Tenta invadir firewalls ou portas");
            Console.WriteLine("  override            - Executa o comando final (exige root total)");
            Console.WriteLine("  matrix              - Ativa o efeito visual");
            Console.WriteLine("  exit                - Sair\n");
        }

        static void ListarDiretorio()
        {
            Console.WriteLine("\n[Diretório: /var/www/infinity/core/]");
            Console.WriteLine("  -rw-r--r--  welcome_sys.txt");
            Console.WriteLine("  -rw-------  encrypted_memo_01.enc");
            Console.WriteLine("  -rw-------  ceo_diary_fragment.log");
            Console.WriteLine("  drwxr-xr-x  restricted_vault/");
        }

        static void LerFicheiro(string nome)
        {
            if (nome == "welcome_sys.txt")
            {
                Console.WriteLine("\n[LEITURA] welcome_sys.txt:");
                Console.WriteLine("Bem-vindo ao núcleo da Infinity Corps. A nossa segurança é impenetrável.");
                Console.WriteLine("O acesso ao 'db_main' requer duas chaves de encriptação ativas.");
                Console.WriteLine("Lê os ficheiros .enc e .log se achas que tens capacidades para lá chegar.");
            }
            else if (nome == "encrypted_memo_01.enc")
            {
                Console.WriteLine("\n[LEITURA] encrypted_memo_01.enc (CIFRA DETETADA):");
                Console.WriteLine(" Mensagem cifrada: 'kdwdfjx_gh_vro'");
                Console.WriteLine(" Pista do Administrador: 'O algoritmo shiftizou cada letra 3 casas à frente no alfabeto.'");
                Console.WriteLine(" Usa o comando 'solve enigma1 [resposta]' para submeter a desencriptação.");
            }
            else if (nome == "ceo_diary_fragment.log")
            {
                Console.WriteLine("\n[LEITURA] ceo_diary_fragment.log:");
                Console.WriteLine(" 'O meu PIN de acesso à base de dados principal é a resposta matemática:");
                Console.WriteLine("  A raiz quadrada de 144, multiplicada pelo número de portas lógicas num computador quântico (8), mais 5.'");
                Console.WriteLine(" Usa o comando 'solve enigma2 [resposta]' para submeter o resultado.");
            }
            else if (nome == "restricted_vault")
            {
                Console.WriteLine("Erro: 'restricted_vault' é um diretório, não um ficheiro. Podes tentar 'hack restricted_vault'.");
            }
            else
            {
                Console.WriteLine("Erro: Ficheiro '" + nome + "' não encontrado ou sem permissões de leitura.");
            }
        }

        static void ResolverEnigma(string argumento)
        {
            if (string.IsNullOrEmpty(argumento))
            {
                Console.WriteLine("Erro: Especifica qual o enigma e a resposta (ex: solve enigma1 palavra).");
                return;
            }

            string[] partes = argumento.Split(new char[] { ' ' }, 2);
            string id = partes[0].ToLower();
            string resposta = partes.Length > 1 ? partes[1].Trim().ToLower() : "";

            if (id == "enigma1")
            {
                // 'kdwdfjx_gh_vro' rot-3 desfeito -> 'haddock_de_sol' ou 'hancock_do_sol' (vamos usar 'hancock_do_sol') -> wait, k-3=h, d-3=a, w-3=t, etc.
                // Vamos simplificar para: 'atractor_do_sol' ou 'tarefa_do_sol' (t->w, a->d, r->u, e->h, f->i, a->d) -> 'tatafa'
                // Vamos fixar uma resposta direta: 'tarefa_do_sol' (cifra rot3 de 'wdvhlfa_gr_vro' -> 'tarefa_do_sol')
                if (resposta == "tarefa_do_sol")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[+] Enigma 1 resolvido! Chave criptográfica A obtida.");
                    Console.ForegroundColor = ConsoleColor.Green;
                    enigma1Solved = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[-] Resposta errada para o Enigma 1. Recua 3 letras em cada caractere da cifra.");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            else if (id == "enigma2")
            {
                // Raiz de 144 = 12. 12 * 8 = 96. 96 + 5 = 101.
                if (resposta == "101")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[+] Enigma 2 resolvido! Chave criptográfica B obtida (PIN do CEO correto).");
                    Console.ForegroundColor = ConsoleColor.Green;
                    enigma2Solved = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[-] PIN incorreto! Calcula com calma: (Raiz de 144 * 8) + 5.");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            else
            {
                Console.WriteLine("Erro: ID de enigma desconhecido. Usa 'enigma1' ou 'enigma2'.");
            }
        }

        static void ExecutarHack(string alvo)
        {
            if (alvo == "firewall" || alvo == "external_firewall")
            {
                Console.WriteLine("[*] A bombardear o cluster de firewalls da Infinity Corps...");
                Thread.Sleep(1500);
                Console.WriteLine("[+] Firewall externa superada. Mas a base de dados interna continua blindada.");
                firewallBypassed = true;
            }
            else if (alvo == "db_main" || alvo == "database")
            {
                if (!firewallBypassed)
                {
                    Console.WriteLine("[-] ALERTA: A firewall externa está ativa! Não podes acender à base de dados diretamente.");
                }
                else if (!enigma1Solved || !enigma2Solved)
                {
                    Console.WriteLine("[-] ACESSO NEGADO: O núcleo 'db_main' exige que resolvas AMBOS os enigmas (enigma1 e enigma2) primeiro para obteres as chaves.");
                }
                else
                {
                    Console.WriteLine("[*] A injetar Chave A e PIN (Chave B) no túnel do 'db_main'...");
                    Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[+] SUCESSO! Barreira da base de dados quebrada. Privilégios de Administrador desbloqueados!");
                    Console.ForegroundColor = ConsoleColor.Green;
                    hasRoot = true;
                }
            }
            else
            {
                Console.WriteLine("Erro: Alvo desconhecido. Alvos válidos detetados na rede: 'firewall', 'db_main'.");
            }
        }

        static void ExecutarOverride()
        {
            if (!hasRoot)
            {
                Console.WriteLine("[-] ERRO CRÍTICO: Não tens permissões de root. Tens de hackear o 'db_main' resolvendo os enigmas e bypassando a firewall.");
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==========================================================");
                Console.WriteLine(" [VITÓRIA ABSOLUTA] INFINITY CORPS COMPLETAMENTE DERRUBADA!");
                Console.WriteLine("==========================================================");
                Console.WriteLine("Superaste todos os firewalls, decifraste os códigos e extraíste os dados.");
                Console.WriteLine("A E-Corp e a Infinity Corps perderam o controlo total.");
                Console.WriteLine("Pressiona [ENTER] para sair do terminal de elite...");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        static void AtivarMatrix()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Random r = new Random();
            for (int i = 0; i < 80; i++)
            {
                Console.Write(r.Next(0, 2) + " ");
                Thread.Sleep(10);
            }
            Console.WriteLine("\n[+] Ecrã Matrix limpo.");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}