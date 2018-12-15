using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// Overriding main so that you can use parameters to launch the program
        /// </summary>
        /// <param name="inputStrings"></param>
        [STAThread]
        public static void Main(params string[] inputStrings)
        {
            //Creates a new json file
            if (inputStrings.Length > 0 && inputStrings[0] == "create")
            {
                LevelIO.CreateJSON();
            }
            else
            {
                //starts the WPF UI
                Nea_Prototype.App app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }

        public static string AppDir { get; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }
}
