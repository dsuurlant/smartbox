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
using SQLite;
using SmartBox.Properties;
using System.Diagnostics;
using SmartBox.Entity;
using System.Data;

namespace SmartBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection db;
        private static string dbName = "gamecollection.db"; //!IMPORTANT must match conn string in App.config

        public MainWindow()
        {
            InitializeComponent();
            if (initializeDatabase() == false)
            {
                MessageBox.Show("An error has occurred whilst initializing the application database.", "Database initialization error", MessageBoxButton.OK);
            }
            if (initializeDataGrid() == false)
            {
                MessageBox.Show("An error has occurred whilst initializing the game collection list.", "Grid initialization error", MessageBoxButton.OK);
            }
        }

        private bool initializeDatabase()
        {
            try
            {
                if (File.Exists(dbName) == false)
                {
                    System.Data.SQLite.SQLiteConnection.CreateFile(dbName);
                }
                db = new SQLiteConnection(dbName);

                var mappings = db.TableMappings;
                if (mappings.Count() == 0)
                {
                    db.CreateTable<Game>();
                    db.CreateTable<Template>();
                }

                return true;
            } catch (SQLiteException e)
            {
                // @TODO exception handling
                return false;
            }
        }

        private bool initializeDataGrid()
        {
            try
            {
                var games = db.Table<Game>();
                gameCollectionGrid.DataContext = games;
                return true;
            } catch (Exception e)
            {
                // @TODO exception handling
                return false;
            }
        }

        private void gamepathSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDlg = new VistaFolderBrowserDialog();
            folderDlg.SelectedPath = Properties.Settings.Default.GameCollectionPath;
            folderDlg.ShowNewFolderButton = true;
            if (folderDlg.ShowDialog() == true)
            {
                Settings.Default.GameCollectionPath = folderDlg.SelectedPath;
            }
        }

        private void gameInstallPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDlg = new VistaFolderBrowserDialog();
            folderDlg.SelectedPath = Properties.Settings.Default.GameInstallPath;
            folderDlg.ShowNewFolderButton = true;
            if (folderDlg.ShowDialog() == true)
            {
                Settings.Default.GameInstallPath = folderDlg.SelectedPath;
            }
        }

        private void gameRunPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDlg = new VistaFolderBrowserDialog();
            folderDlg.SelectedPath = Properties.Settings.Default.GameRunPath;
            folderDlg.ShowNewFolderButton = true;
            if (folderDlg.ShowDialog() == true)
            {
                Settings.Default.GameRunPath = folderDlg.SelectedPath;
            }
        }

        private void gameListQuickLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            var loadPath = Settings.Default.GameCollectionPath;
            if (loadPath == "")
            {
                // display error, return.
                MessageBox.Show("Configure the game collection load path first!", "Unable to Quick Load", MessageBoxButton.OK);
                return;
            }
            else
            {
                var files = Directory.GetFiles(loadPath);
                // @TODO overwrite or append etc.

                foreach (string filename in files)
                {
                    Game game = new Game();
                    game.fileName = filename;
                    db.Insert(game);
                }

                var games = db.Query<Game>("SELECT * FROM Game");
                quickLoadMsg.Content = games.Count() + " loaded.";
            }
        }
    }
}
