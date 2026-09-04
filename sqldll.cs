using System;
using System.IO;
using System.Collections.Generic;

namespace Sql
{
    public class BaseDeDados
    {
        private string caminhoFicheiro;

        public BaseDeDados(string nomeFicheiro)
        {
            caminhoFicheiro = nomeFicheiro;
        }

        // Inserir dados
        public void Inserir(string nome, decimal preco)
        {
            using (FileStream fs = new FileStream(caminhoFicheiro, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(nome);
                writer.Write(preco);
            }
        }

        // Ler tudo
        public void LerTudo()
        {
            if (!File.Exists(caminhoFicheiro))
            {
                Console.WriteLine("O ficheiro .db ainda não existe.");
                return;
            }

            using (FileStream fs = new FileStream(caminhoFicheiro, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    string nome = reader.ReadString();
                    decimal preco = reader.ReadDecimal();
                    Console.WriteLine("Item: " + nome + " | Preço: " + preco + "€");
                }
            }
        }

        // 1. Sql.destruir: Apaga o ficheiro .db inteiro do disco
        public void destruir()
        {
            if (File.Exists(caminhoFicheiro))
            {
                File.Delete(caminhoFicheiro);
                Console.WriteLine("Ficheiro .db destruído e apagado do disco.");
            }
            else
            {
                Console.WriteLine("O ficheiro .db não existe para ser destruído.");
            }
        }

        // 2. Sql.delete: Apaga/limpa TODOS os dados de dentro do .db (mantém o ficheiro vazio)
        public void delete()
        {
            // Abre o ficheiro em modo de criação (FileMode.Create), o que apaga o conteúdo anterior e deixa-o vazio
            using (FileStream fs = new FileStream(caminhoFicheiro, FileMode.Create, FileAccess.Write))
            {
                // Apenas abre e fecha vazio
            }
            Console.WriteLine("Todos os dados foram apagados da base de dados (.db limpo).");
        }

        // 3. Sql.remove: Remove apenas a parte/registo específico que quiseres
        public void remove(string nomeAlvo)
        {
            if (!File.Exists(caminhoFicheiro))
            {
                Console.WriteLine("O ficheiro .db não existe.");
                return;
            }

            List<Tuple<string, decimal>> itensRestantes = new List<Tuple<string, decimal>>();

            using (FileStream fs = new FileStream(caminhoFicheiro, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    string nome = reader.ReadString();
                    decimal preco = reader.ReadDecimal();

                    if (string.Compare(nome, nomeAlvo, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        itensRestantes.Add(new Tuple<string, decimal>(nome, preco));
                    }
                }
            }

            using (FileStream fs = new FileStream(caminhoFicheiro, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                foreach (var item in itensRestantes)
                {
                    writer.Write(item.Item1);
                    writer.Write(item.Item2);
                }
            }

            Console.WriteLine("Parte removida com sucesso: '" + nomeAlvo + "'.");
        }
    }
}