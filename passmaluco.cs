using System;
namespace PassMaluco
{
    public class Pass
    {
        public static string Read(string caractere)
        {
            string password = "";
            ConsoleKeyInfo tecla;

            do
            {
                tecla = Console.ReadKey(true); // Lê a tecla sem a mostrar no ecrã

                // Se o utilizador carregar no Enter, paramos
                if (tecla.Key == ConsoleKey.Enter)
                {
                    break;
                }
                // Se carregar no Backspace (apagar), removemos o último caratere
                else if (tecla.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        string apagar = "";
                        for (int i = 0; i < caractere.Length; i++)
                        {
                            apagar += "\b \b";
                        }
                        Console.Write(apagar);
                    }
                }
                // Para qualquer outro caratere válido, adicionamos e mostramos um '*'
                else if (!char.IsControl(tecla.KeyChar))
                {
                    password += tecla.KeyChar;
                    Console.Write(caractere);
                }

            } while (true);

            Console.WriteLine(); // Quebra a linha quando o utilizador prime Enter
            return password;
        }
    }
}