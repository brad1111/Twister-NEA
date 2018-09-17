using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Nea_Prototype.Level;

namespace Nea_Prototype
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            LevelIO.CreateJSON();
            LevelIO.ReadJSON("testing.json");
        }
    }
}
