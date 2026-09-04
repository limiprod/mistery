using System;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.VisualBasic;
namespace ConfigsEngine
{
    public class Configs
    {
        public static void modify(string chave, string novoValor)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[chave] != null)
            {
                config.AppSettings.Settings[chave].Value = novoValor;
            }
            else
            {
                config.AppSettings.Settings.Add(chave, novoValor);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        public static void delete(string chave)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[chave] != null)
            {
                config.AppSettings.Settings.Remove(chave);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                MessageBox.Show("A chave '" + chave + "' não foi encontrada.", "Aviso");
            }
        }
        public static string read(string chave)
        {
            return ConfigurationManager.AppSettings[chave];
        }
    }
}