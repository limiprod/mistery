using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;

namespace AsciiButtonLang
{
    // ==========================================
    // CLASSE 1: GDI32 E USER32 - A "PENA" EXPANDIDA
    // ==========================================
    public static class GdiEffects
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool PatBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, uint dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const uint PATINVERT = 0x005A0049;

        /// <summary>
        /// 1. Flash estroboscópico de inversão de cores no ecrã inteiro.
        /// </summary>
        public static void FlashScreen(int count)
        {
            IntPtr hwnd = GetDesktopWindow();
            IntPtr hdc = GetWindowDC(hwnd);
            int width = 1920;
            int height = 1080;

            for (int i = 0; i < count; i++)
            {
                PatBlt(hdc, 0, 0, width, height, PATINVERT);
                Thread.Sleep(30);
            }
            ReleaseDC(hwnd, hdc);
        }

        /// <summary>
        /// 2. Desenha caixas/retângulos caóticos diretamente sobre os pixéis do ecrã (GDI puro).
        /// </summary>
        public static void DrawRandomBoxes(int iterations)
        {
            IntPtr hwnd = GetDesktopWindow();
            IntPtr hdc = GetWindowDC(hwnd);
            Random rand = new Random();

            for (int i = 0; i < iterations; i++)
            {
                IntPtr hPen = CreatePen(0, 3, (uint)rand.Next(0x00FFFFFF));
                SelectObject(hdc, hPen);

                int x1 = rand.Next(0, 1500);
                int y1 = rand.Next(0, 900);
                Rectangle(hdc, x1, y1, x1 + rand.Next(50, 200), y1 + rand.Next(50, 200));

                DeleteObject(hPen);
                Thread.Sleep(20);
            }
            ReleaseDC(hwnd, hdc);
        }

        /// <summary>
        /// 3. Teleporta o cursor do rato aleatoriamente pelo ecrã.
        /// </summary>
        public static void GlitchCursor(int shakes)
        {
            Random rand = new Random();
            for (int i = 0; i < shakes; i++)
            {
                SetCursorPos(rand.Next(100, 1200), rand.Next(100, 800));
                Thread.Sleep(40);
            }
        }

        /// <summary>
        /// 4. Minimiza ou Restaura uma janela do Windows pelo nome exato do título.
        /// </summary>
        public static void ToggleWindowVisibility(string windowTitle, bool hide)
        {
            IntPtr hwnd = FindWindow(null, windowTitle);
            if (hwnd != IntPtr.Zero)
            {
                // 0 = Esconder (SW_HIDE), 5 = Mostrar (SW_SHOW)
                ShowWindow(hwnd, hide ? 0 : 5);
            }
        }
    }

    // ==========================================
    // CLASSE 2: SERVIDOR TCP REAL (Leitura e Escrita)
    // ==========================================
    public static class AsciiTcpServer
    {
        private static TcpListener listener;
        private static Thread serverThread;
        private static bool isRunning = false;
        private static List<TcpClient> connectedClients = new List<TcpClient>();
        private static string lastMessage = string.Empty;

        public static void Start(int port)
        {
            if (isRunning) return;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                isRunning = true;

                serverThread = new Thread(new ThreadStart(() =>
                {
                    while (isRunning)
                    {
                        try
                        {
                            if (!listener.Pending())
                            {
                                Thread.Sleep(50);
                                continue;
                            }

                            TcpClient client = listener.AcceptTcpClient();
                            lock (connectedClients)
                            {
                                connectedClients.Add(client);
                            }

                            Thread clientThread = new Thread(() => HandleClient(client));
                            clientThread.IsBackground = true;
                            clientThread.Start();
                        }
                        catch { }
                    }
                }));
                serverThread.IsBackground = true;
                serverThread.Start();
                Console.WriteLine("[TCP Real] Servidor a escutar na porta " + port);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[TCP Erro] " + ex.Message);
            }
        }

        private static void HandleClient(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    writer.WriteLine("CONECTADO AO ASCII BUTTON TCP SERVER. Envia mensagens ou comandos.");

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lastMessage = line;
                        Console.WriteLine("\n[TCP Recebido] " + line);
                        writer.WriteLine("ECO_GDI_CONFIRMADO: " + line);
                    }
                }
            }
            catch { }
            finally
            {
                lock (connectedClients)
                {
                    connectedClients.Remove(client);
                }
                try { client.Close(); } catch { }
            }
        }

        public static void Broadcast(string message)
        {
            lock (connectedClients)
            {
                foreach (var client in connectedClients)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                        writer.WriteLine(message);
                    }
                    catch { }
                }
            }
            Console.WriteLine("[TCP Broadcast] Enviado para " + connectedClients.Count + " clientes.");
        }

        public static string GetLastMessage() { return lastMessage; }

        public static void Stop()
        {
            isRunning = false;
            try
            {
                lock (connectedClients)
                {
                    foreach (var c in connectedClients) c.Close();
                    connectedClients.Clear();
                }
                if (listener != null) listener.Stop();
            }
            catch { }
            Console.WriteLine("[TCP Real] Servidor parado.");
        }
    }

    // ==========================================
    // CLASSE 3: MOTOR DE BOTÕES E TERMINAL
    // ==========================================
    public class ConsoleButton
    {
        public string Text { get; set; }
        public string AppPath { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
    }

    public static class AsciiButtonEngine
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadConsoleInput(IntPtr hConsoleInput, ref INPUT_RECORD lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_MOUSE_INPUT = 0x0010;
        const uint ENABLE_PROCESSED_INPUT = 0x0001;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public ushort EventType;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEY_EVENT_RECORD
        {
            public int bKeyDown;
            public ushort wRepeatCount;
            public ushort wVirtualKeyCode;
            public ushort wVirtualScanCode;
            public char uChar;
            public uint dwControlKeyState;
        }

        const ushort MOUSE_EVENT = 0x0002;
        const ushort KEY_EVENT = 0x0001;
        const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;

        private static List<ConsoleButton> activeButtons = new List<ConsoleButton>();
        private static IntPtr hConsoleInput;
        private static uint originalConsoleMode;
        private static StringBuilder currentInput = new StringBuilder();

        private static void EnableConsoleModes()
        {
            hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(hConsoleInput, out originalConsoleMode);

            uint consoleMode = originalConsoleMode;
            consoleMode |= ENABLE_MOUSE_INPUT;
            consoleMode &= ~ENABLE_PROCESSED_INPUT;
            consoleMode &= ~ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hConsoleInput, consoleMode);
        }

        private static void RestoreConsoleModes()
        {
            try { SetConsoleMode(hConsoleInput, originalConsoleMode); } catch { }
        }

        public static void StartTerminal(string initialScriptPath = null)
        {
            Console.Title = "AsciiButton Ultimate Modular Terminal";
            EnableConsoleModes();

            if (!string.IsNullOrEmpty(initialScriptPath) && File.Exists(initialScriptPath))
            {
                RunScript(File.ReadAllText(initialScriptPath));
            }

            StartInteractiveLoop();
            RestoreConsoleModes();
        }

        private static void StartInteractiveLoop()
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("   Terminal GDI+TCP (Comandos GDI Avançados)!");
            Console.WriteLine("   Comandos extra: 'gdi_flash <n>', 'gdi_boxes <n>', 'gdi_cursor <n>', 'sair'");
            Console.WriteLine("==================================================\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("asciibutton> ");
            Console.ResetColor();

            while (true)
            {
                uint eventsRead = 0;
                INPUT_RECORD record = new INPUT_RECORD();

                if (ReadConsoleInput(hConsoleInput, ref record, 1, out eventsRead) && eventsRead > 0)
                {
                    if (record.EventType == MOUSE_EVENT)
                    {
                        if ((record.MouseEvent.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) != 0 &&
                            record.MouseEvent.dwEventFlags == 0)
                        {
                            int mouseX = record.MouseEvent.dwMousePosition.X;
                            int mouseY = record.MouseEvent.dwMousePosition.Y;

                            lock (activeButtons)
                            {
                                foreach (var btn in activeButtons)
                                {
                                    if (mouseY == btn.Top && mouseX >= btn.Left && mouseX <= btn.Right)
                                    {
                                        try
                                        {
                                            Process.Start(new ProcessStartInfo
                                            {
                                                FileName = btn.AppPath,
                                                UseShellExecute = true
                                            });
                                        }
                                        catch { }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (record.EventType == KEY_EVENT && record.KeyEvent.bKeyDown != 0)
                    {
                        char c = record.KeyEvent.uChar;
                        ushort vk = record.KeyEvent.wVirtualKeyCode;

                        if (vk == 0x0D) // Enter
                        {
                            Console.WriteLine();
                            string input = currentInput.ToString().Trim();
                            currentInput.Clear();

                            if (!string.IsNullOrEmpty(input))
                            {
                                string lowerInput = input.ToLower();

                                if (lowerInput == "sair")
                                {
                                    AsciiTcpServer.Stop();
                                    break;
                                }
                                else if (lowerInput.StartsWith("gdi_flash "))
                                {
                                    int count;
                                    if (int.TryParse(input.Substring(10).Trim(), out count))
                                        GdiEffects.FlashScreen(count);
                                }
                                else if (lowerInput.StartsWith("gdi_boxes "))
                                {
                                    int count;
                                    if (int.TryParse(input.Substring(10).Trim(), out count))
                                        GdiEffects.DrawRandomBoxes(count);
                                }
                                else if (lowerInput.StartsWith("gdi_cursor "))
                                {
                                    int count;
                                    if (int.TryParse(input.Substring(11).Trim(), out count))
                                        GdiEffects.GlitchCursor(count);
                                }
                                else
                                {
                                    RunScript(input);
                                }
                            }

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("asciibutton> ");
                            Console.ResetColor();
                        }
                        else if (vk == 0x08) // Backspace
                        {
                            if (currentInput.Length > 0)
                            {
                                currentInput.Remove(currentInput.Length - 1, 1);
                                int currentLeft = Console.CursorLeft;
                                int currentTop = Console.CursorTop;
                                if (currentLeft > 0)
                                {
                                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                                    Console.Write(" ");
                                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                                }
                            }
                        }
                        else if (c != '\0' && !char.IsControl(c))
                        {
                            currentInput.Append(c);
                            Console.Write(c);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public static void RunScript(string code)
        {
            EnableConsoleModes();
            MatchCollection matches = Regex.Matches(code, @"(t\s*\(\s*""([^""]*)""\s*\)|b\s*\(\s*""([^""]*)""\s*,\s*""([^""]*)""\s*\)|n\s*\(\s*\)|p\s*\(\s*\))");

            foreach (Match match in matches)
            {
                string fullCommand = match.Value.Trim();

                if (fullCommand.StartsWith("t"))
                {
                    string text = Regex.Match(fullCommand, @"t\s*\(\s*""([^""]*)""\s*\)").Groups[1].Value;
                    Console.Write(text);
                }
                else if (fullCommand.StartsWith("b"))
                {
                    Match btnMatch = Regex.Match(fullCommand, @"b\s*\(\s*""([^""]*)""\s*,\s*""([^""]*)""\s*\)");
                    string btnText = btnMatch.Groups[1].Value;
                    string btnApp = btnMatch.Groups[2].Value;

                    int startX = Console.CursorLeft;
                    int startY = Console.CursorTop;
                    string buttonLabel = " [ " + btnText + " ] ";
                    int endX = startX + buttonLabel.Length;

                    lock (activeButtons)
                    {
                        activeButtons.Add(new ConsoleButton
                        {
                            Text = btnText,
                            AppPath = btnApp,
                            Left = startX,
                            Right = endX,
                            Top = startY
                        });
                    }

                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write(buttonLabel);
                    Console.ResetColor();
                    Console.Write(" ");
                }
                else if (fullCommand.StartsWith("n"))
                {
                    Console.WriteLine();
                }
                else if (fullCommand.StartsWith("p"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("\n[Pausa ativada. Prime ENTER ou clica num botão...] ");
                    Console.ResetColor();

                    while (true)
                    {
                        uint eventsRead = 0;
                        INPUT_RECORD record = new INPUT_RECORD();
                        if (ReadConsoleInput(hConsoleInput, ref record, 1, out eventsRead) && eventsRead > 0)
                        {
                            if (record.EventType == MOUSE_EVENT)
                            {
                                if ((record.MouseEvent.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) != 0 && record.MouseEvent.dwEventFlags == 0)
                                {
                                    int mouseX = record.MouseEvent.dwMousePosition.X;
                                    int mouseY = record.MouseEvent.dwMousePosition.Y;
                                    lock (activeButtons)
                                    {
                                        foreach (var btn in activeButtons)
                                        {
                                            if (mouseY == btn.Top && mouseX >= btn.Left && mouseX <= btn.Right)
                                            {
                                                try { Process.Start(new ProcessStartInfo { FileName = btn.AppPath, UseShellExecute = true }); } catch { }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (record.EventType == KEY_EVENT && record.KeyEvent.bKeyDown != 0)
                            {
                                if (record.KeyEvent.wVirtualKeyCode == 0x0D || record.KeyEvent.uChar == '\r' || record.KeyEvent.uChar == '\n')
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}