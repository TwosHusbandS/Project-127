using CefSharp.DevTools.Page;
using GSF.IO;
using GSF.Units;
using Project_127.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Project_127.Popups.PopupInstallComponent;
using static Project_127.Popups.PopupProgress;
using static System.Windows.Forms.LinkLabel;

namespace Project_127.Popups
{
    /// <summary>
    /// I cant be arsed
    /// going through the entire codebase
    /// and checking from which thread i could be calling a popup
    /// im wrapping all of them inside DispatcherInvoke
    /// Its not performant, but its a fucking popup
    /// So doesnt matter
    /// and cant be arsed putting in the time otherwise
    /// </summary>
    public class PopupWrapper
    {
        public static List<HelperClasses.MyFileOperation> PopupProgress(ProgressTypes pProgressType, string pParam1, List<HelperClasses.MyFileOperation> pMyFileOperations = null, string zipExtractionPath = "", bool betterPercentage = false)
        {
            List<HelperClasses.MyFileOperation> rtrn = new List<HelperClasses.MyFileOperation>();
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupProgress PP = new PopupProgress(pProgressType, pParam1, pMyFileOperations, zipExtractionPath, betterPercentage);
                PP.ShowDialog();
                rtrn = PP.RtrnMyFileOperations;
            });
            return rtrn;
        }


        public static string PopupProgressMD5(string pFilePath)
        {
            string rtrn = "";
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupProgress PP = new PopupProgress(ProgressTypes.MD5, pFilePath, null);
                PP.ShowDialog();
                rtrn = PP.RtrnMD5;
            });
            return rtrn;
        }


        public static string PopupTextbox(string Message, string TextBoxDefault, int pFontSize = 18)
        {
            string rtrn = "";
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupTextbox pt = new PopupTextbox(Message, TextBoxDefault, pFontSize);
                pt.ShowDialog();
                if (pt.DialogResult == true)
                {
                    rtrn = pt.MyReturnString;
                }
                else
                {
                    rtrn = "CANCEL";
                }    
            });
            return rtrn;
        }

        public static bool PopupInstallComponent(ComponentManager.Components pComponent, ComponentActions pComponentAction)
        {
            bool rtrn = false;
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupInstallComponent PIC = new PopupInstallComponent(pComponent, pComponentAction);
                PIC.ShowDialog();
                rtrn = PIC.rtrn;
            });
            return rtrn;
        }

        public static void PopupPatchEditor(string input)
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupPatchEditor PPE = new PopupPatchEditor(input);
                PPE.ShowDialog();
            });
        }


        public static void PopupSpecialPatcher()
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupSpecialPatcher PSP = new PopupSpecialPatcher();
                PSP.ShowDialog();
            });
        }

        public static void PopupMode()
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupMode pm = new PopupMode();
                pm.ShowDialog();
            });
        }

        public static string PopupDownload(string pLink, string pFilePath, string pFileNameForUi, bool pReturnHash = false)
        {
            string rtrn = "";
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                PopupDownload pd = new PopupDownload(pLink, pFilePath, pFileNameForUi, pReturnHash);
                pd.ShowDialog();
                if (pReturnHash)
                {
                    rtrn = pd.HashString;
                }
            });
            return rtrn;
        }

        public static void PopupError(string pMsg, int pFontSize = 18)
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                new Popup(Popup.PopupWindowTypes.PopupOkError, pMsg, pFontSize).ShowDialog();
            });
        }

        public static bool PopupYesNo(string pMsg, int pFontSize = 18)
        {
            bool rtrn = true;
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, pMsg, pFontSize);
                yesno.ShowDialog();
                if (yesno.DialogResult == true)
                {
                    rtrn = true;
                }
                else
                {
                    rtrn = false;
                }
            });
            return rtrn;
        }


        public static void PopupOk(string pMsg, int pFontSize = 18)
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                new Popup(Popup.PopupWindowTypes.PopupOk, pMsg, pFontSize).ShowDialog();
            });
        }
    }
}
