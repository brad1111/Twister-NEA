using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Twister.Level;

namespace Twister
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // /// <summary>
        // /// Overriding main so that you can use parameters to launch the program
        // /// </summary>
        // /// <param name="inputStrings"></param>
        // [STAThread]
        // public static void Main(params string[] inputStrings)
        // {
        //     //Creates a new json file
        //     if (inputStrings.Length > 0 && inputStrings[0] == "create")
        //     {
        //         LevelIO.CreateJSON();
        //     }
        //     else
        //     {
        //         //starts the WPF UI
        //         App app = new App();
        //         app.InitializeComponent();
        //         app.Run();
        //     }
        // }

        public static string AppDir { get; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }
}
