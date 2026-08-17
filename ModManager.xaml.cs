using CefSharp.DevTools.IndexedDB;
using GSF.IO;
using GSF.Parsing;
using Project_127.HelperClasses;
using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Project_127.HelperClasses.DownloadManager;

namespace Project_127
{
    /// <summary>
    /// Interaction logic for ModManager.xaml
    /// </summary>
    public partial class ModManager : Page
    {
        public ModManager()
        {
            InitializeComponent();

            // Hi
            // Feel free to modify UI

            // no promises that ill merge any changes

            // Below is some example of useful stuff below

            // Feel free to ask on discord if you have questions
            // also ask on discord if you are looking / searching for something in code base that you need
            // chances are its already implemented somewhere

            // If you do file operations, please do them like this
            // this way its logged, its on a nice progress popup, its try/catched, error messages pop up, folders get created if they dont exist and so on
            // I think the parameters are self explainatory
            // MyFileOperation.FileOperations has: Copy,Move,Hardlink,Delete,Create
            // MyFileOperation.FileOrFolder has: File,Folder
            // If you only need one parameter (like creating a folder), you can just pass "" for second param
            List<MyFileOperation> MFOs = new List<MyFileOperation>();
            MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, @"C:\temp\FileA.txt", @"C:\temp\FileB.txt", "Copy file from A to B, this text is on UI and in log", 0, MyFileOperation.FileOrFolder.File));
            MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, @"C:\temp\FileC.txt", @"C:\temp\FileD.txt", "Copy file from C to D, this text is on UI and in log", 0, MyFileOperation.FileOrFolder.File));
            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Managing some files...", MFOs);


            // if you need a yes/no popup, do it like this
            bool result = PopupWrapper.PopupYesNo("This is my popup message for a yes/no popup");
            // Or like this, if its just an OK type message
            PopupWrapper.PopupOk("This is a popup user can just say 'OK' to");
            // Or like this, if its just an Error type message
            PopupWrapper.PopupError("This is a popup user can just say 'OK' to");


            // File picker
            HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.File, "Pick a file");
            // File picker with multi select and filter
            HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.File, "Pick a file, but can multi select", "", true, "TXT Files|*.txt*");
            // Folder picker
            HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.Folder, "Pick Folder");

            /*
            LauncherLogic.GameState; // to get if game is running or not
            LauncherLogic.InstallationState; // to get if we are currently upgraded or downgraded
            BuildVersionTable.GetGameVersionOfBuild(Globals.GTABuild) == new Version(1, 24); // for detecting which version of a game is currently there, might be more accurate (do differ between 1.24, 1.27 and 1.29)
            LauncherLogic.GTAVFilePath; // duh
            LauncherLogic.ModManagerFilePath; // newly created, its inside the Project_127_Files folder which is by default inside gta installation
            */

            // if you need to store something persistent
            /*
            JavaScriptSerializer json = new JavaScriptSerializer();
            HelperClasses.RegeditHandler.SetValue("ModManagerSettings", json.Serialize(object_instance));
            object_instance = json.Deserialize<object_datatype>(HelperClasses.RegeditHandler.GetValue("ModManagerSettings"));
            */

            // i havent implemented the popups you have in your app yet
            // just copy paste some popup component, rename it, and adapt from there
            // also please add to PopupWrapper.cs, because we need to call UI stuff via dispatcher invoke
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ApplyPreset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Presets_Plus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Presets_Minus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_PresetFiles_Plus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_PresetFiles_Minus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_PresetFiles_Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dg_Presets_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void dg_Presets_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void dg_Presets_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dg_Presets_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void dg_PresetFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void dg_PresetFiles_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void dg_PresetFiles_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dg_PresetFiles_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
