using System;
using System.Threading;
using AsciiButtonLang;
using ControllerManager;
using PassMaluco;
using Sql;
using TileEngineCore;
using ConfigsEngine;
using WhatIsEngine;

class EscapeRoomBizarro
{
    static void Main()
    {
        Console.Title = "Escape Room Bizarro - O Terminal Corrompido (Modo Extremo 14 Níveis)";
        Console.Clear();

        Console.WriteLine("==================================================");
        Console.WriteLine("   BEM-VINDO AO ESCAPE ROOM BIZARRO (14 NÍVEIS)!");
        Console.WriteLine("==================================================");
        Console.WriteLine("Prepara-te. Tens de ultrapassar 14 desafios alucinantes.\n");

        // ---------------------------------------------------------
        // NÍVEL 1: A Password Mascarada (PassMaluco + GDI Flash)
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 1/14] O sistema exige a password de acesso.");
        Console.Write("Introduz a password (dica: 'raio'): ");
        string senha = PassMaluco.Pass.Read("§");

        if (senha.ToLower().Trim() != "raio")
        {
            Console.WriteLine("\n[ERRO] Password incorreta! O sistema entra em pânico visual.");
            GdiEffects.FlashScreen(5);
            return;
        }
        Console.WriteLine("\n[SUCESSO] Nível 1 superado!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 2: O Comando de Jogos (ControllerManager)
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 2/14] Mantém premido o **Botao A** no comando Xbox...");
        bool passouNivel2 = false;
        for (int i = 0; i < 15; i++)
        {
            if (XInputController.EstaConectado(0) && XInputController.EstaPremido(XInputController.BotaoA, 0))
            {
                passouNivel2 = true;
                break;
            }
            Thread.Sleep(1000);
            Console.Write(".");
        }

        if (!passouNivel2)
        {
            Console.WriteLine("\n[ERRO] Comando não detetado ou botão não premido!");
            if (XInputController.EstaConectado(0))
            {
                XInputController.Vibrar(65535, 65535, 0);
                Thread.Sleep(1500);
                XInputController.PararVibracao(0);
            }
            return;
        }
        Console.WriteLine("\n[SUCESSO] Nível 2 superado!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 3: O Labirinto Gráfico (TileEngineCore)
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 3/14] Corredor Laser detetado!");
        Console.WriteLine("A abrir o simulador de fuga tátil... Usa as setas ou WASD.");
        Thread.Sleep(1500);

        int[,] mapaLabirinto = new int[,] {
            { 0, 1, 0, 0, 0 },
            { 0, 1, 0, 2, 0 },
            { 0, 0, 0, 2, 0 },
            { 1, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 0 }
        };

        MotorJogo.IniciarJogo(mapaLabirinto);
        Console.WriteLine("\n[SUCESSO] Travessia do corredor laser concluída!\n");

        // ---------------------------------------------------------
        // NÍVEL 4: O Cofre de Configurações (ConfigsEngine)
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 4/14] Painel de Segurança do Elevador.");
        Console.Write("Injeta a chave de configuração 'NivelAcesso' ('admin123'): ");
        string codigoOverride = Console.ReadLine();

        Configs.modify("NivelAcesso", codigoOverride);
        string valorLido = Configs.read("NivelAcesso");

        if (valorLido != "admin123")
        {
            Console.WriteLine("\n[ERRO] Código de override recusado.");
            return;
        }
        Console.WriteLine("\n[SUCESSO] Chave de sistema injetada!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 5: O Teste de Memória Binária
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 5/14] Firewall de Memória Ativa.");
        Console.Write("Qual é o valor padrão da memória base? (Dica: '404'): ");
        string memCheck = Console.ReadLine();

        if (memCheck != "404")
        {
            Console.WriteLine("\n[ERRO] Estouro de pilha! O sistema bloqueou o terminal.");
            GdiEffects.FlashScreen(3);
            return;
        }
        Console.WriteLine("\n[SUCESSO] Firewall ultrapassada!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 6: O Alarme de Frequência Sonora
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 6/14] Sensor Acústico de Proximidade.");
        Console.Write("Escreve a palavra-passe sonora ('bipbip'): ");
        string somPass = Console.ReadLine();

        if (somPass != "bipbip")
        {
            Console.WriteLine("\n[ERRO] Alarme disparado!");
            return;
        }
        Console.Beep(800, 200);
        Console.Beep(1200, 200);
        Console.WriteLine("\n[SUCESSO] Ressonância aceite!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 7: O Desarmamento do Reator
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 7/14] Corredor do Reator Nuclear.");
        Console.Write("Insere o código de emergência ('777') em menos de 5 segundos: ");

        DateTime inicio = DateTime.Now;
        string reatorCod = Console.ReadLine();
        DateTime fim = DateTime.Now;

        if (reatorCod != "777" || (fim - inicio).TotalSeconds > 5.0)
        {
            Console.WriteLine("\n[ERRO] Demoraste demais ou o código falhou!");
            return;
        }
        Console.WriteLine("\n[SUCESSO] Reator estabilizado!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 8: O Teste de Paridade
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 8/14] Conversor de Bits Corrompidos.");
        Console.Write("Qual é o decimal correspondente a '1010'? (Dica: '10'): ");
        string binAns = Console.ReadLine();

        if (binAns != "10")
        {
            Console.WriteLine("\n[ERRO] Erro de paridade nos bits!");
            return;
        }
        Console.WriteLine("\n[SUCESSO] Bits alinhados com sucesso!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 9: O Hack de Cores da Consola
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 9/14] Protocolo Visual de Autenticação.");
        Console.Write("Qual a cor primária associada ao perigo crítico em inglês? ('red'): ");
        string corSec = Console.ReadLine();

        if (corSec.ToLower().Trim() != "red")
        {
            Console.WriteLine("\n[ERRO] Câmara detetou intruso não autorizado!");
            GdiEffects.FlashScreen(2);
            return;
        }
        Console.WriteLine("\n[SUCESSO] Assinatura térmica mascarada!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 10: O Código do Relógio
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 10/14] Sincronização Temporal do Cofre.");
        Console.Write("Quantos segundos tem um minuto? ('60'): ");
        string tempoSeq = Console.ReadLine();

        if (tempoSeq != "60")
        {
            Console.WriteLine("\n[ERRO] Fuso horário dessincronizado!");
            return;
        }
        Console.WriteLine("\n[SUCESSO] Relógio sincronizado!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 11: A Porta de Criptografia Avançada
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 11/14] Camada de Criptografia Quântica.");
        Console.Write("Escreve a palavra-passe de desativação ('MATRIX'): ");
        string quantPass = Console.ReadLine();

        if (quantPass.ToUpper().Trim() != "MATRIX")
        {
            Console.WriteLine("\n[ERRO] Campo de força intacto.");
            return;
        }
        Console.WriteLine("\n[SUCESSO] Campo de força desativado!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 12: A Porta dos Vetores (NOVO!)
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 12/14] Antena de Rádio Direcional.");
        Console.Write("Qual é a coordenada cardinal oposta ao Norte? ('sul'): ");
        string direcao = Console.ReadLine();

        if (direcao.ToLower().Trim() != "sul")
        {
            Console.WriteLine("\n[ERRO] Coordenada de calibração errada!");
            return;
        }
        Console.WriteLine("\n[SUCESSO] Antena calibrada com sucesso!\n");
        Thread.Sleep(800);

        // ---------------------------------------------------------
        // NÍVEL 13: O Enigma Quântico da DLL WhatIs (NOVO OBRIGATÓRIO!)
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 13/14] Motor Quântico 'WhatIs' Ativo.");
        Console.WriteLine("O núcleo executa a operação estruturada: WhatIs w1 + WhatIs w2.");
        Console.Write("Qual é o resultado textual retornado por esta sobrecarga de operador? (Dica: 'positive'): ");
        string whatIsInput = Console.ReadLine();

        // Validação usando formalmente a WhatIsEngine
        WhatIs wA = new WhatIs("alfa");
        WhatIs wB = new WhatIs("beta");
        WhatIs wResultado = wA + wB; // Produz um WhatIs com o texto "positive"[cite: 11]

        if (whatIsInput.ToLower().Trim() != wResultado.ToString())
        {
            Console.WriteLine("\n[ERRO] Colapso da função quântica WhatIs!");
            GdiEffects.FlashScreen(3);
            return;
        }
        Console.WriteLine("\n[SUCESSO] Estado quântico estabilizado pela WhatIsEngine!\n");
        Thread.Sleep(1000);

        // ---------------------------------------------------------
        // NÍVEL 14: Destruição do Sistema (Sql) - A Grande Final
        // ---------------------------------------------------------
        Console.WriteLine("[NÍVEL 14/14] Fase Final Absoluta: A Aniquilação Total dos Dados.");
        BaseDeDados db = new BaseDeDados("escape_core.db");
        db.Inserir("CodigoSaida", 999.99m);

        Console.Write("Escreve 'DESTRUIR' para eliminar a base de dados central e abrir a saída definitiva: ");
        string escolha = Console.ReadLine();

        if (escolha == "DESTRUIR")
        {
            db.destruir();
            GdiEffects.DrawRandomBoxes(25);

            Console.WriteLine("\n==================================================");
            Console.WriteLine("   IMPARÁVEL! ESCAPASTE DOS 14 NÍVEIS DO LABIRINTO!");
            Console.WriteLine("==================================================");
        }
        else
        {
            Console.WriteLine("\n[FALHA] Hesitaste no último segundo. Prisioneiro para sempre.");
        }

        Console.WriteLine("\nPressiona [ENTER] para sair.");
        Console.ReadLine();
    }
}