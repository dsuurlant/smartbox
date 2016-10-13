using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace SmartBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void gamepathSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDlg = new VistaFolderBrowserDialog();
            folderDlg.SelectedPath = Properties.Settings.Default.GameCollectionPath;
            folderDlg.ShowNewFolderButton = true;
            if (folderDlg.ShowDialog() == true)
            {
                SmartBox.Properties.Settings.Default.GameCollectionPath = folderDlg.SelectedPath;
            }
        }

        private void gameInstallPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDlg = new VistaFolderBrowserDialog();
            folderDlg.SelectedPath = Properties.Settings.Default.GameInstallPath;
            folderDlg.ShowNewFolderButton = true;
            if (folderDlg.ShowDialog() == true)
            {
                SmartBox.Properties.Settings.Default.GameInstallPath = folderDlg.SelectedPath;
            }
        }

        private void gameRunPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDlg = new VistaFolderBrowserDialog();
            folderDlg.SelectedPath = Properties.Settings.Default.GameRunPath;
            folderDlg.ShowNewFolderButton = true;
            if (folderDlg.ShowDialog() == true)
            {
                SmartBox.Properties.Settings.Default.GameRunPath = folderDlg.SelectedPath;
            }
        }

        private void gameListLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            var loadPath = SmartBox.Properties.Settings.Default.GameCollectionPath;
            var files = Directory.GetFiles(loadPath);
        }
        
    }
}
