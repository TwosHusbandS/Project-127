﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Runtime.InteropServices;
using WpfAnimatedGif;
using System.Threading;
using System.IO.Compression;

namespace Project_127.SaveFileHandlerStuff
{
    /// <summary>
    /// Class SaveFileHandler.xaml
    /// </summary>
    public partial class SaveFileHandler : Page
    {

        public static MySaveFile CopyCutPasteObject = null;

        public enum PasteKinds
        {
            Copy,
            Cut
        }

        public static PasteKinds PasteKind = PasteKinds.Copy;

        /// <summary>
        /// Constructor of SaveFileHandler
        /// </summary>
        public SaveFileHandler()
        {

            // Initializing all WPF Elements
            InitializeComponent();

            // Used for DataBinding
            this.DataContext = this;

            MouseOverMagic(btn_LeftArrow);
            MouseOverMagic(btn_RightArrow);
            MouseOverMagic(btn_Refresh);

            MySaveFile.CurrentBackupSavesPath = MySaveFile.BackupSavesPath;

            CopyCutPasteObject = null;

            if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
            {
                MySaveFile.CurrGTASavesPath = MySaveFile.GTASavesPathSocialClub;
                btn_GTA_Path_Change.Content = "Load Dr490n Launcher SaveFilePath";
                btn_lbl_GTASavesHeader.Content = "GTA V Save Files (Social Club)";
            }
            else
            {
                MySaveFile.CurrGTASavesPath = MySaveFile.GTASavesPathDragonEmu;
                btn_GTA_Path_Change.Content = "Load Social Club SaveFilePath";
                btn_lbl_GTASavesHeader.Content = "GTA V Save Files (Dr490n Launcher)";
            }
            this.sv_BackupFiles_Loading.Visibility = Visibility.Visible;
            this.sv_BackupFiles.Visibility = Visibility.Hidden;
            this.sv_GTAFiles_Loading.Visibility = Visibility.Visible;
            this.sv_GTAFiles.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Mouse Over Magic
        /// </summary>
        /// <param name="myBtn"></param>
        public static void MouseOverMagic(Button myBtn)
        {
            switch (myBtn.Name)
            {
                case "btn_LeftArrow":
                    if (myBtn.IsMouseOver)
                    {
                        MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowleft_mo.png");
                    }
                    else
                    {
                        MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowleft.png");
                    }
                    break;
                case "btn_RightArrow":
                    if (myBtn.IsMouseOver)
                    {
                        MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowright_mo.png");
                    }
                    else
                    {
                        MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowright.png");
                    }
                    break;
                case "btn_Refresh":
                    if (myBtn.IsMouseOver)
                    {
                        MainWindow.MW.SetControlBackground(myBtn, @"Artwork/refresh_mo.png");
                    }
                    else
                    {
                        MainWindow.MW.SetControlBackground(myBtn, @"Artwork/refresh.png");
                    }
                    break;
            }
        }


        /// <summary>
        /// Click on the Refresh Button. Reads files from disk again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Refresh_Click(object sender = null, RoutedEventArgs e = null)
        {
            Refresh();
        }



        private async void Refresh(DataGrid DataGridToSelect = null)
        {
            this.sv_BackupFiles_Loading.Visibility = Visibility.Visible;
            this.sv_BackupFiles.Visibility = Visibility.Hidden;
            this.sv_GTAFiles_Loading.Visibility = Visibility.Visible;
            this.sv_GTAFiles.Visibility = Visibility.Hidden;

            string TMP_BackupPathDisplay = "";
            int TMP_Fontsize = 20;
            // Set Label and MouseOverLabel
            if (MySaveFile.BackupSavesPath.TrimEnd('\\') == MySaveFile.CurrentBackupSavesPath.TrimEnd('\\'))
            {
                TMP_Fontsize = 20;
                TMP_BackupPathDisplay = "Backup Save Files";
            }
            else
            {
                if (MySaveFile.BackupSavesPath.StartsWith(MySaveFile.CurrentBackupSavesPath))
                {
                    TMP_BackupPathDisplay = MySaveFile.CurrentBackupSavesPath;
                }
                else
                {
                    string result = MySaveFile.CurrentBackupSavesPath;
                    while (result.StartsWith(MySaveFile.BackupSavesPath))
                    {
                        result = result.Substring(MySaveFile.BackupSavesPath.Length);
                    }

                    TMP_BackupPathDisplay = "Backup Save Files" + @"\" + result;
                }
                int myFontSize = 20;
                int AdditionalLength = TMP_BackupPathDisplay.ToString().Length - "Backup Save Files".Length;
                int removeSize = (int)((AdditionalLength / 4) * 2);
                int newFontSize = myFontSize - removeSize;
                if (newFontSize < 8) { newFontSize = 8; }
                if (newFontSize > 30) { newFontSize = 30; }
                TMP_Fontsize = newFontSize;

            }
            btn_lbl_BackupSaves.Content = TMP_BackupPathDisplay;
            btn_lbl_BackupSaves.ToolTip = TMP_BackupPathDisplay;
            btn_lbl_BackupSaves.FontSize = TMP_Fontsize;

            SaveFileHandlerStuff.RefreshTaskObject myTMP = await RefreshLogic();

            // Resetting the Obvservable Collections:
            MySaveFile.BackupSaves = new ObservableCollection<MySaveFile>();
            MySaveFile.GTASaves = new ObservableCollection<MySaveFile>();

            MySaveFile.BackupSaves = myTMP.MyBackupSaves;
            MySaveFile.GTASaves = myTMP.MyGTASaves;

            // Set the ItemSource of Both Datagrids for the DataBinding
            dg_BackupFiles.ItemsSource = MySaveFile.BackupSaves;
            dg_GTAFiles.ItemsSource = MySaveFile.GTASaves;


            if (DataGridToSelect != null)
            {
                HelperClasses.DataGridHelper.SelectFirst(DataGridToSelect);

                try
                {

                    if (GetDataGridCell(DataGridToSelect.SelectedCells[0]) != null)
                    {
                        Keyboard.Focus(GetDataGridCell(DataGridToSelect.SelectedCells[0]));
                    }
                }
                catch { }
            }

            this.sv_BackupFiles_Loading.Visibility = Visibility.Hidden;
            this.sv_BackupFiles.Visibility = Visibility.Visible;
            this.sv_GTAFiles_Loading.Visibility = Visibility.Hidden;
            this.sv_GTAFiles.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets a single DataGridCell from a CellInfo
        /// </summary>
        /// <param name="cellInfo"></param>
        /// <returns></returns>
        private System.Windows.Controls.DataGridCell GetDataGridCell(System.Windows.Controls.DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((System.Windows.Controls.DataGridCell)cellContent.Parent);

            return (null);
        }


        /// <summary>
        /// Refresh Logic. Returns a RefreshTaskObject. Async task
        /// </summary>
        /// <param name="DataGridToSelect"></param>
        /// <returns></returns>
        private async Task<SaveFileHandlerStuff.RefreshTaskObject> RefreshLogic(DataGrid DataGridToSelect = null)
        {
            SaveFileHandlerStuff.RefreshTaskObject TMP = null;

            await Task.Run(() =>
            {
                // Resetting the Obvservable Collections:
                ObservableCollection<MySaveFile> TMP_BackUpSaves = new ObservableCollection<MySaveFile>();
                ObservableCollection<MySaveFile> TMP_GTASaves = new ObservableCollection<MySaveFile>();

                TMP_BackUpSaves.Add(new MySaveFile(HelperClasses.FileHandling.PathSplitUp(MySaveFile.CurrentBackupSavesPath.TrimEnd('\\'))[0], true));
                string[] MySubFolders = HelperClasses.FileHandling.GetSubFolders(MySaveFile.CurrentBackupSavesPath);
                foreach (string MySubFolder in MySubFolders)
                {
                    TMP_BackUpSaves.Add(new MySaveFile(MySubFolder, true));
                }

                // Files in BackupSaves (own File Path)
                string[] MyBackupSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.CurrentBackupSavesPath);
                foreach (string MyBackupSaveFile in MyBackupSaveFiles)
                {
                    if (!MyBackupSaveFile.Contains(".bak"))
                    {
                        TMP_BackUpSaves.Add(new MySaveFile(MyBackupSaveFile));
                    }
                }

                // Files in actual GTAV Save File Locations
                string[] MyGTAVSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.CurrGTASavesPath);
                foreach (string MyGTAVSaveFile in MyGTAVSaveFiles)
                {
                    if (!MyGTAVSaveFile.Contains(".bak") && MyGTAVSaveFile.Contains("SGTA500"))
                    {
                        TMP_GTASaves.Add(new MySaveFile(MyGTAVSaveFile));
                    }
                }

                TMP = new SaveFileHandlerStuff.RefreshTaskObject(TMP_BackUpSaves, TMP_GTASaves);
            });

            return TMP;
        }


        /// <summary>
        /// Button Click on the LeftArrow (From GTA Path to Backup Path)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            // Copying a File from the GTA V Saves to the Backup Saves
            // We ask User for Name

            // Null Check
            if (dg_GTAFiles.SelectedItem != null)
            {
                // Get MySaveFile from the selected Item
                MySaveFile tmp = (MySaveFile)dg_GTAFiles.SelectedItem;

                string newName = tmp.FileName;

                if (HelperClasses.FileHandling.doesFileExist(MySaveFile.CurrentBackupSavesPath.TrimEnd('\\') + @"\" + tmp.FileName))
                {
                    // Get the Name for it
                    newName = GetNewFileName(tmp.FileName, MySaveFile.CurrentBackupSavesPath);
                }

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    // Only do if it the name is not "" or null
                    tmp.CopyToBackup(newName);
                    Refresh(dg_GTAFiles);
                }
            }
        }




        /// <summary>
        /// Click on the Right Arrow (From Backup Path to GTA Path)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RightArrow_Click(object sender, RoutedEventArgs e)
        {
            // Copying a File from the Backup Saves to the GTA V Saves
            // Build name which GTA Uses

            // Null checker
            if (dg_BackupFiles.SelectedItem != null)
            {
                // Get MySaveFile from the selected Item
                MySaveFile tmp = (MySaveFile)dg_BackupFiles.SelectedItem;

                // Building the first theoretical FileName
                int i = 0;
                string NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
                string FilePathInsideGTA = MySaveFile.CurrGTASavesPath.TrimEnd('\\') + @"\" + NewFileName;

                // While Loop through all 16 names, breaking out when File does NOT exist or when we reached the manimum
                while (HelperClasses.FileHandling.doesFileExist(FilePathInsideGTA))
                {
                    if (i >= 15)
                    {
                        // Save Files with a Number larger than that will not be read by game
                        PopupWrapper.PopupOk("No free SaveSlot (00-15) inside the GTA Save File Path.\nDelete one and try again");
                        return;
                    }

                    // Build new theoretical FileName
                    i++;
                    NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
                    FilePathInsideGTA = MySaveFile.CurrGTASavesPath.TrimEnd('\\') + @"\" + NewFileName;
                }
                tmp.CopyToGTA(NewFileName);
                Refresh(dg_BackupFiles);
                //PopupWrapper.PopupOk("Copied '" + tmp.FileName + "' to the GTA V Saves Location under the Name '" + NewFileName + "' !").ShowDialog();
            }
        }



        /// <summary>
        /// Rename Button for the Files in the "our" Folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Rename_Click(object sender, RoutedEventArgs e)
        {
            // we use this to refresh the correct DataGrid after operation
            DataGrid tmp_dg = new DataGrid();

            // Getting the Selecting SaveFile, checking if it aint null
            MySaveFile tmp = GetSelectedSaveFile();
            if (tmp != null)
            {
                if (tmp.FileOrFolder == MySaveFile.FileOrFolders.Folder)
                {
                    PopupWrapper.PopupOk("Cant rename a Folder.");
                    return;
                }
                    
                // If its inside GTA Save directory
                if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
                {
                    tmp_dg = dg_GTAFiles;

                    // Asking user if he really wants to rename since its fucked...
                    bool yesno = PopupWrapper.PopupYesNo("Re-Naming something inside the GTA V Saves Location makes no sense,\nsince it wont get recognized by the game.\nStill want to continue?");
                    if (yesno == false)
                    {
                        // If user decides against renaming, return and end function
                        return;
                    }
                }
                else
                {
                    tmp_dg = dg_BackupFiles;
                }

                // Popup for new name of File
                string newName = GetNewFileName(tmp.FileName, tmp.Path);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    // Rename File, Sort Datagrid
                    tmp.Rename(newName);
                }
            }

            Refresh(tmp_dg);
        }



        /// <summary>
        /// Delete Button for the files in the GTAV Location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Gets selected File, checks if its not null
            MySaveFile tmp = GetSelectedSaveFile();
            if (tmp != null)
            {
                if (tmp.FileOrFolder == MySaveFile.FileOrFolders.File)
                {
                    // Ask user if he wants to remove it
                    bool yesno = PopupWrapper.PopupYesNo("Are you sure you want to delete this SaveFile?");
                    if (yesno == true)
                    {
                        // Deleting it and sorting them
                        tmp.Delete();
                        if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.Backup)
                        {
                            Refresh(dg_BackupFiles);
                        }
                        else
                        {
                            Refresh(dg_GTAFiles);
                        }
                    }
                }
                else
                {
                    if (!MySaveFile.CurrentBackupSavesPath.StartsWith(tmp.FilePath))
                    {
                        HelperClasses.FileHandling.DeleteFolder(tmp.FilePath);
                        Refresh(dg_BackupFiles);
                    }
                    else
                    {
                        PopupWrapper.PopupOk("Im not letting you delete the Parent-Folder,\nI know what im doing.");
                    }

                }
            }
        }

        /// <summary>
        /// Gets the single selected File from Both Grids
        /// </summary>
        /// <returns></returns>
        private MySaveFile GetSelectedSaveFile()
        {
            // Returns the Selected SaveFile (since it is only one), else returns null
            if (dg_BackupFiles.SelectedItem != null)
            {
                return (MySaveFile)dg_BackupFiles.SelectedItem;
            }
            else if (dg_GTAFiles.SelectedItem != null)
            {
                return (MySaveFile)dg_GTAFiles.SelectedItem;
            }
            return null;
        }


        /// <summary>
        /// Mouse Enter Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseOverMagic((Button)sender);
        }

        /// <summary>
        /// Mouse Leave Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseOverMagic((Button)sender);
        }

        /// <summary>
        /// New Name Popup Logic
        /// </summary>
        /// <param name="pMySaveFile"></param>
        /// <param name="pDestination"></param>
        /// <returns></returns>
        private string GetNewFileName(string pMySaveFileName, string pPathToCheck)
        {
            // Asking for Name 
            string newName = PopupWrapper.PopupTextbox("Enter new Name for the SaveFile:\n'" + pMySaveFileName + "'", pMySaveFileName);

            // While name was give OR fikle exists
            while (String.IsNullOrWhiteSpace(newName) || HelperClasses.FileHandling.doesFileExist(pPathToCheck.TrimEnd('\\') + @"\" + newName) || newName.ToLower() == "cancel")
            {
                if (newName.ToLower() == "cancel")
                {
                    return "";
                }

                // Not a Valid FilePath
                bool yesno = PopupWrapper.PopupYesNo("File does already exists or is not a valid FileName.\nClick yes if you want to try again.");
                if (yesno == false)
                {
                    // When you wanna exit
                    return "";
                }
                else
                {
                    // When you wanna stay in while loop
                    newName = PopupWrapper.PopupTextbox("Enter new Name for the SaveFile:\n'" + pMySaveFileName + "'", pMySaveFileName);
                }
            }
            return newName;
        }

        /// <summary>
        /// New Folder Name Popup Logic
        /// </summary>
        /// <param name="pMySaveFile"></param>
        /// <param name="pDestination"></param>
        /// <returns></returns>
        private string GetNewFolderName(string pPathToCheck)
        {
            // Asking for Name 
            string newName = PopupWrapper.PopupTextbox("Enter new Folder-Name:", "");

            // While name was give OR fikle exists
            while (String.IsNullOrWhiteSpace(newName) || HelperClasses.FileHandling.doesPathExist(pPathToCheck.TrimEnd('\\') + @"\" + newName) || newName.ToLower() == "cancel")
            {
                if (newName.ToLower() == "cancel")
                {
                    return "";
                }

                // Not a Valid FilePath
                bool yesno = PopupWrapper.PopupYesNo("Folder does already exists or is not a valid Folder-Name.\nClick yes if you want to try again.");
                if (yesno == false)
                {
                    // When you wanna exit
                    return "";
                }
                else
                {
                    // When you wanna stay in while loop
                    newName = PopupWrapper.PopupTextbox("Enter new Folder-Name:", "");
                }
            }
            return newName;
        }

        /// <summary>
        /// Enables the scrolling behaviour of the DataGrid for Backup Save-Files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_BackupFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv_BackupFiles.ScrollToVerticalOffset(sv_BackupFiles.VerticalOffset - e.Delta / 3);
        }

        /// <summary>
        /// Enables the scrolling behaviour of the DataGrid for GTA Save-Files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_GTAFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv_GTAFiles.ScrollToVerticalOffset(sv_GTAFiles.VerticalOffset - e.Delta / 3);
        }

        /// <summary>
        /// Reset Selection when the other datagrid was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_GTAFiles_GotFocus(object sender, RoutedEventArgs e)
        {
            // if GTA Datagrid got Focus, we are removing selection from Backup Datagrid
            dg_BackupFiles.SelectedItem = null;
        }

        /// <summary>
        /// Reset Selection when the other datagrid was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_BackupFiles_GotFocus(object sender, RoutedEventArgs e)
        {
            // if got Backup Datagrid Focus, we are removing selection from GTA Datagrid 
            dg_GTAFiles.SelectedItem = null;
        }

        /// <summary>
        /// KeyDown Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // This should now just be easy and useable with keyboard only.
            // KeyDown and KeyUp are already implemented basic DataGrid behaviour
            if (e.Key == Key.Delete)
            {
                btn_Delete_Click(null, null);
            }
            else if (e.Key == Key.N)
            {
                MI_NewFolder_Click(null, null);
            }
            else if (e.Key == Key.E)
            {
                MI_ExportFolder_Click(null, null);
            }
            else if (e.Key == Key.F2)
            {
                btn_Rename_Click(null, null);
            }
            else if (e.Key == Key.Right)
            {
                btn_RightArrow_Click(null, null);
            }
            else if (e.Key == Key.Left)
            {
                btn_LeftArrow_Click(null, null);
            }
            else if (e.Key == Key.F5)
            {
                Refresh();
            }
            else if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                Row_DoubleClick(null, null);
                e.Handled = true;
            }
        }


        /// <summary>
        /// Importing multiple savefiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            string MySelectedFilesRtrn = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Pick the SaveFiles (or a ZIP File) you want to import", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                            @"\Rockstar Games\GTA V\Profiles", true);

            // Close this if return is empty or ""
            if (String.IsNullOrEmpty(MySelectedFilesRtrn))
            {
                return;
            }

            // List of all Selected Files
            List<string> MySelectedFiles = MySelectedFilesRtrn.Split(',').ToList();
            List<string> MySelectedFilesUnique = new List<string>();

            // Looping through all Selected Files
            for (int i = 0; i <= MySelectedFiles.Count - 1; i++)
            {
                // if is .zip
                if (MySelectedFiles[i].Substring(MySelectedFiles[i].Length - 4) == ".zip")
                {
                    PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.ZIPFile, MySelectedFiles[i], null, MySaveFile.CurrentBackupSavesPath);
                }
                else
                {
                    // if is .bak
                    if (MySelectedFiles[i].Substring(MySelectedFiles[i].Length - 4) == ".bak")
                    {
                        string correspondingNonBakFile = MySelectedFiles[i].Substring(0, MySelectedFiles[i].Length - 4);
                        if (!HelperClasses.FileHandling.doesFileExist(correspondingNonBakFile))
                        {
                            // Create correspondingNonBakFile if it doesnt exist
                            List<MyFileOperation> tmp = new List<MyFileOperation>();
                            tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, MySelectedFiles[i], correspondingNonBakFile, "Copying SaveFiles in Source Folder", 1, MyFileOperation.FileOrFolder.File));
                            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Copying", tmp);
                        }
                    }
                    // Non .bak
                    else
                    {
                        string correspondingBakFile = MySelectedFiles[i] + ".bak";
                        if (!HelperClasses.FileHandling.doesFileExist(correspondingBakFile))
                        {
                            // Create correspondingBakFile if it doesnt exist
                            List<MyFileOperation> tmp = new List<MyFileOperation>();
                            tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, MySelectedFiles[i], correspondingBakFile, "Copying SaveFiles in Source Folder", 1, MyFileOperation.FileOrFolder.File));
                            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Copying", tmp);
                        }
                    }

                    // Making the FileName non Bak
                    MySelectedFiles[i] = HelperClasses.FileHandling.TrimEnd(MySelectedFiles[i], ".bak");

                    // Adding it to new list if it doesnt contain it yet
                    if (!MySelectedFilesUnique.Contains(MySelectedFiles[i]))
                    {
                        string FileName = MySelectedFiles[i].Substring(MySelectedFiles[i].LastIndexOf('\\') + 1);
                        if (HelperClasses.FileHandling.doesFileExist(MySaveFile.CurrGTASavesPath.TrimEnd('\\') + @"\" + FileName))
                        {
                            FileName = GetNewFileName(FileName, MySaveFile.CurrentBackupSavesPath);
                        }
                        MySaveFile.Import(MySelectedFiles[i], FileName);

                        MySelectedFilesUnique.Add(MySelectedFiles[i]);
                    }
                }
            }

            Refresh();
        }

        /// <summary>
        /// Double click on a dataGrid Row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            MySaveFile MSV = GetSelectedSaveFile();
            if (MSV != null)
            {
                if (MSV.FileOrFolder == MySaveFile.FileOrFolders.Folder)
                {
                    MySaveFile.CurrentBackupSavesPath = MSV.FilePath;
                    Refresh(dg_BackupFiles);
                }
            }
        }

        /// <summary>
        /// Rightclick on the Datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid myDataGrid = (DataGrid)sender;

            if (myDataGrid != null)
            {
                // this is so that rightclick on non selected shit works...

                e.Handled = false;
                // making the selection 0
                myDataGrid.SelectedItem = null;

                // then simulating leftclick where the mouse is
                HelperClasses.MouseSender.DoMouseClick();

                // Generate COntext Menu
                GenerateContextMenu(myDataGrid);
                myDataGrid.Focus();
            }
        }

        /// <summary>
        /// Generates the ContextMenu
        /// </summary>
        /// <param name="myDG"></param>
        private async void GenerateContextMenu(DataGrid myDG)
        {
            await Task.Delay(50);

            MySaveFile MSV = GetSelectedSaveFile();

            if (myDG == dg_BackupFiles)
            {
                if (MSV == null)
                {
                    ContextMenu cm = new ContextMenu();

                    MenuItem mi = new MenuItem();
                    mi.Header = "Create (N)ew Folder";
                    mi.Click += MI_NewFolder_Click;
                    cm.Items.Add(mi);

                    if (CopyCutPasteObject != null)
                    {
                        MenuItem mi2 = new MenuItem();
                        mi2.Header = "Paste (V)";
                        mi2.Click += MI_PasteIntoBackup_Click;
                        cm.Items.Add(mi2);
                    }

                    cm.IsOpen = true;
                }
                else
                {
                    if (MSV.FileOrFolder == MySaveFile.FileOrFolders.Folder)
                    {
                        ContextMenu cm = new ContextMenu();

                        MenuItem mi = new MenuItem();
                        mi.Header = "Create (N)ew Folder";
                        mi.Click += MI_NewFolder_Click;
                        cm.Items.Add(mi);

                        MenuItem mi2 = new MenuItem();
                        mi2.Header = "(Del)ete Folder";
                        mi2.Click += MI_DeleteFolder_Click;
                        cm.Items.Add(mi2);

                        MenuItem mi3 = new MenuItem();
                        mi3.Header = "(E)xport Folder";
                        mi3.Click += MI_ExportFolder_Click;
                        cm.Items.Add(mi3);

                        if (CopyCutPasteObject != null)
                        {
                            MenuItem mi4 = new MenuItem();
                            mi4.Header = "Paste (V)";
                            mi4.Click += MI_PasteIntoBackupFolder_Click;
                            cm.Items.Add(mi4);
                        }

                        cm.IsOpen = true;
                    }
                    else
                    {
                        ContextMenu cm = new ContextMenu();

                        MenuItem mi = new MenuItem();
                        mi.Header = "Create (N)ew Folder";
                        mi.Click += MI_NewFolder_Click;
                        cm.Items.Add(mi);

                        MenuItem mi2 = new MenuItem();
                        mi2.Header = "Move to GTA Saves (->)";
                        mi2.Click += MI_MoveToGTA_Click;
                        cm.Items.Add(mi2);

                        MenuItem mi3 = new MenuItem();
                        mi3.Header = "Rename SaveFile (F2)";
                        mi3.Click += MI_Rename_Click;
                        cm.Items.Add(mi3);

                        MenuItem mi4 = new MenuItem();
                        mi4.Header = "(Del)ete SaveFile";
                        mi4.Click += MI_Delete_Click;
                        cm.Items.Add(mi4);

                        MenuItem mi5 = new MenuItem();
                        mi5.Header = "(C)opy";
                        mi5.Click += MI_Copy_Click;
                        cm.Items.Add(mi5);

                        MenuItem mi6 = new MenuItem();
                        mi6.Header = "Cut (X)";
                        mi6.Click += MI_Cut_Click;
                        cm.Items.Add(mi6);

                        cm.IsOpen = true;


                        // Copy File
                        // Cut File
                    }
                }
            }
            else if (myDG == dg_GTAFiles)
            {
                if (MSV == null)
                {
                    if (CopyCutPasteObject != null)
                    {
                        ContextMenu cm = new ContextMenu();

                        MenuItem mi = new MenuItem();
                        mi.Header = "Paste (V)";
                        mi.Click += MI_PasteIntoGTA_Click;
                        cm.Items.Add(mi);

                        cm.IsOpen = true;
                    }
                }
                else
                {
                    ContextMenu cm = new ContextMenu();

                    MenuItem mi = new MenuItem();
                    mi.Header = "Move to Backup Saves (<-)";
                    mi.Click += MI_MoveToBackup_Click;
                    cm.Items.Add(mi);

                    MenuItem mi2 = new MenuItem();
                    mi2.Header = "Paste (V)";
                    mi2.Click += MI_PasteIntoGTA_Click;
                    cm.Items.Add(mi2);

                    cm.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Context Menu Click (Cut)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_Cut_Click(object sender, RoutedEventArgs e)
        {
            MySaveFile tmp = GetSelectedSaveFile();
            if (tmp != null)
            {
                PasteKind = PasteKinds.Cut;
                CopyCutPasteObject = tmp;
            }
        }

        /// <summary>
        /// Context Menu Click (Copy)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_Copy_Click(object sender, RoutedEventArgs e)
        {
            MySaveFile tmp = GetSelectedSaveFile();
            if (tmp != null)
            {
                PasteKind = PasteKinds.Copy;
                CopyCutPasteObject = tmp;
            }
        }

        /// <summary>
        /// Context Menu Click (PasteIntoGTA)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_PasteIntoGTA_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            string NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
            string FilePathInsideGTA = MySaveFile.CurrGTASavesPath.TrimEnd('\\') + @"\" + NewFileName;

            // While Loop through all 16 names, breaking out when File does NOT exist or when we reached the manimum
            while (HelperClasses.FileHandling.doesFileExist(FilePathInsideGTA))
            {
                if (i >= 15)
                {
                    // Save Files with a Number larger than that will not be read by game
                    PopupWrapper.PopupOk("No free SaveSlot (00-15) inside the GTA Save File Path.\nDelete one and try again");
                    return;
                }

                // Build new theoretical FileName
                i++;
                NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
                FilePathInsideGTA = MySaveFile.CurrGTASavesPath.TrimEnd('\\') + @"\" + NewFileName;
            }

            if (CopyCutPasteObject != null)
            {
                if (PasteKind == PasteKinds.Copy)
                {
                    CopyCutPasteObject.CopyToGTA(NewFileName);
                }
                else
                {
                    CopyCutPasteObject.CopyToGTA(NewFileName);
                    CopyCutPasteObject.Delete();
                }
                Refresh(dg_BackupFiles);
            }
        }

        /// <summary>
        /// Context Menu Click (PasteIntoBackup)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_PasteIntoBackup_Click(object sender, RoutedEventArgs e)
        {
            if (CopyCutPasteObject != null)
            {
                if (PasteKind == PasteKinds.Copy)
                {
                    CopyCutPasteObject.CopyTo(MySaveFile.CurrentBackupSavesPath);
                }
                else
                {
                    CopyCutPasteObject.MoveTo(MySaveFile.CurrentBackupSavesPath);
                }

                Refresh(dg_BackupFiles);
            }
        }

        /// <summary>
        /// Context Menu Click (PasteIntoBackupFolder)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_PasteIntoBackupFolder_Click(object sender, RoutedEventArgs e)
        {
            MySaveFile tmp = GetSelectedSaveFile();
            if (tmp != null && CopyCutPasteObject != null)
            {
                if (tmp.FileOrFolder == MySaveFile.FileOrFolders.Folder)
                {
                    if (PasteKind == PasteKinds.Copy)
                    {
                        CopyCutPasteObject.CopyTo(tmp.FilePath);
                    }
                    else
                    {
                        CopyCutPasteObject.MoveTo(tmp.FilePath);
                    }

                    Refresh(dg_BackupFiles);
                }
            }
        }

        /// <summary>
        /// Context Menu Click (NewFolder)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            string rtrn = GetNewFolderName(MySaveFile.CurrentBackupSavesPath);
            if (!string.IsNullOrWhiteSpace(rtrn))
            {
                HelperClasses.FileHandling.createPath(MySaveFile.CurrentBackupSavesPath.TrimEnd('\\') + @"\" + rtrn);
                Refresh(dg_BackupFiles);
            }
        }

        /// <summary>
        /// Context Menu Click (DeleteFolder)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            btn_Delete_Click(null, null);
        }

        /// <summary>
        /// Context Menu Click (ExportFolder)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_ExportFolder_Click(object sender, RoutedEventArgs e)
        {
            MySaveFile tmp = GetSelectedSaveFile();
            if (tmp != null)
            {
                if (tmp.FileOrFolder == MySaveFile.FileOrFolders.Folder)
                {
                    // SaveFile ZIP...get zip savepath
                    string outputPath = HelperClasses.FileHandling.SaveFileDialog("Select ZIP File Save Location", "ZIP file (*.zip)|*.zip");

                    if (!String.IsNullOrEmpty(outputPath))
                    {
                        if (HelperClasses.FileHandling.doesFileExist(outputPath))
                        {
                            bool yesno = PopupWrapper.PopupYesNo("File already exists.\nDo you want to replace the existing File?");
                            if (yesno == true)
                            {
                                HelperClasses.FileHandling.deleteFile(outputPath);
                            }
                            else
                            {
                                return;
                            }
                        }
                        ZipFile.CreateFromDirectory(tmp.FilePath, outputPath);
                        PopupWrapper.PopupOk("ZIP file created in:\n'" + outputPath + "'");
                    }
                }
            }
        }

        /// <summary>
        /// Context Menu Click (MoveToGTA)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_MoveToGTA_Click(object sender, RoutedEventArgs e)
        {
            btn_RightArrow_Click(null, null);
        }

        /// <summary>
        /// Context Menu Click (MoveToBackup)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_MoveToBackup_Click(object sender, RoutedEventArgs e)
        {
            btn_LeftArrow_Click(null, null);
        }

        /// <summary>
        /// Context Menu Click (Rename)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_Rename_Click(object sender, RoutedEventArgs e)
        {
            btn_Rename_Click(null, null);
        }

        /// <summary>
        /// Context Menu Click (Delete)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_Delete_Click(object sender, RoutedEventArgs e)
        {
            btn_Delete_Click(null, null);
        }

        /// <summary>
        /// When the Page is loaded, throw a Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10);
            Refresh();
        }

        private void btn_lbl_BackupSaves_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateHeaderContextMenu(MySaveFile.SaveFileKinds.Backup);
        }

        private void btn_lbl_GTASavesHeader_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateHeaderContextMenu(MySaveFile.SaveFileKinds.GTAV);
        }

        public void CreateHeaderContextMenu(MySaveFile.SaveFileKinds mySFK)
        {

            ContextMenu cm = new ContextMenu();
            cm.Name = mySFK.ToString();

            MenuItem mi = new MenuItem();
            mi.Header = "Open Path in Explorer";
            mi.Click += MI_OpenPath_Click;
            cm.Items.Add(mi);

            MenuItem mi2 = new MenuItem();
            mi2.Header = "Clear";
            mi2.Click += MI_Clear_Click;
            cm.Items.Add(mi2);


            cm.IsOpen = true;

        }


        private void MI_OpenPath_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            ContextMenu cm = (ContextMenu)mi.Parent;
            if (cm.Name == "GTAV")
            {
                ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: MySaveFile.CurrGTASavesPath);
            }
            else
            {
                ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: MySaveFile.CurrentBackupSavesPath);
            }
        }

        private void MI_Clear_Click(object sender, RoutedEventArgs e)
        {
            bool yesno = PopupWrapper.PopupYesNo("Do you want to delete every file in the entire Tab?");
            if (yesno == true)
            {
                HelperClasses.Logger.Log("User wants to delete everything in: '" + MySaveFile.CurrGTASavesPath + "' from SaveFileHandler");
                MenuItem mi = (MenuItem)sender;
                ContextMenu cm = (ContextMenu)mi.Parent;
                if (cm.Name == "GTAV")
                {
                    foreach (string myFile in FileHandling.GetFilesFromFolderAndSubFolder(MySaveFile.CurrGTASavesPath))
                    {
                        if (myFile.Contains("SGTA500"))
                        {
                            FileHandling.deleteFile(myFile);
                        }
                    }
                    Refresh(dg_GTAFiles);
                }
                else
                {

                    foreach (string myFile in FileHandling.GetFilesFromFolderAndSubFolder(MySaveFile.CurrentBackupSavesPath))
                    {
                        FileHandling.deleteFile(myFile);
                    }
                    foreach (string myFolder in FileHandling.GetSubFolders(MySaveFile.CurrentBackupSavesPath))
                    {
                        FileHandling.DeleteFolder(myFolder);
                    }
                    Refresh(dg_BackupFiles);
                }
            }
        }

        private void btn_GTA_Path_Change_Click(object sender, RoutedEventArgs e)
        {
            if (MySaveFile.CurrGTASavesPath == MySaveFile.GTASavesPathDragonEmu)
            {
                MySaveFile.CurrGTASavesPath = MySaveFile.GTASavesPathSocialClub;
                btn_GTA_Path_Change.Content = "Load Dr490n Launcher SaveFilePath";
                btn_lbl_GTASavesHeader.Content = "GTA V Save Files (Social Club)";

            }
            else
            {
                MySaveFile.CurrGTASavesPath = MySaveFile.GTASavesPathDragonEmu;
                btn_GTA_Path_Change.Content = "Load Social Club SaveFilePath";
                btn_lbl_GTASavesHeader.Content = "GTA V Save Files (Dr490n Launcher)";
            }
            Refresh();
        }

        private void btn_GTA_Path_Change_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = new ContextMenu();

            MenuItem mi = new MenuItem();
            mi.Header = "Help";
            mi.Click += MI_Help_Click;
            cm.Items.Add(mi);

            MenuItem mi2 = new MenuItem();
            mi2.Header = "Custom Path";
            mi2.Click += MI_CustomPath_Click;
            cm.Items.Add(mi2);

            cm.IsOpen = true;
        }


        /// <summary>
        /// Context Menu Path Button Click (Help)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_Help_Click(object sender, RoutedEventArgs e)
        {
            string msg = "If you enabled 'Launch through Socialclub' the 'default'\n";
            msg += "Path where SaveFiles are saved is the same as 'normal' GTA.\n";
            msg += "If you have NOT enabled that and are launching through P127 Emu\n";
            msg += "with P127 Auth, your SaveFiles will be stored in a different Location\n";
            msg += "\nClicking this Button will toggle what Path is used for the SaveFileHandler.\n";
            msg += "The correct Path (based on your P127 Settings)\nwill be loaded when you open the SafeFileHandler.";
            PopupWrapper.PopupOk(msg, 18);
        }

        /// <summary>
        /// Context Menu Path Button Click (Custom Path)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_CustomPath_Click(object sender, RoutedEventArgs e)
        {
            string rtrn = HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.Folder, "Select the Folder you want P127 to consider as your GTA Saves Location", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (HelperClasses.FileHandling.doesPathExist(rtrn))
            {
                MySaveFile.CurrGTASavesPath = rtrn;
                Refresh();
            }
        }

    } // End of Class
} // End of Namespace
