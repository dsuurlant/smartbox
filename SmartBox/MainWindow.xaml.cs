﻿using System;
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
using System.Text.RegularExpressions;

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
                MessageBox.Show("Configure the game collection load path first!", "Unable to Quick Load", MessageBoxButton.OK);
                return;
            }
            else if (quickAppendGames(loadPath) == false)
            {
                MessageBox.Show("An error occurred whilst quickloading.", "Unable to Quick Load", MessageBoxButton.OK);
            }            
        }

        private bool quickAppendGames(string gameFolder)
        {
            try
            {
                var files = Directory.GetFiles(gameFolder);
                var games = db.Table<Game>();
                List<string> gameList = new List<string>();
                List<Game> gamesToLoad = new List<Game>();

                if (games.Count() > 0)
                {
                    foreach (Game game in games)
                    {
                        gameList.Add(game.ToString());
                    }
                }

                foreach (string filename in files)
                {
                    if (gameList.Contains(filename) == false)
                    {
                        Game game = new Game();
                        game.fileName = filename;
                        gamesToLoad.Add(game);
                    }
                }

                if (gamesToLoad.Count() > 0)
                {
                    db.InsertAll(gamesToLoad);
                }

                var newTotal = db.ExecuteScalar<int>("SELECT count(id) FROM Game");
                quickLoadMsg.Content = gamesToLoad.Count() + " imported. " + newTotal + " games total.";
                initializeDataGrid(); // @TODO datagrid should auto-update with table Game changes. issue#11

                return true;

            } catch (Exception e)
            {
                // @TODO Exception handling
                return false;
            }
        }


        private bool analyzeGames()
        {
            try
            {
                var games = db.Table<Game>();
                var total = games.Count();
                if (games.Count() == 0)
                {
                    MessageBox.Show("You have no games to analyze!", "Unable to Analyze", MessageBoxButton.OK);
                    return true;
                }
                
                string pattern = @"(?<gameName>[0-9a-zA-Z\s\!\?:.,&\-\+]+)\((?<year>[0-9]{4})\)\((?<publisher>[0-9a-zA-Z\s\!\?:.,&\-\+]+)\)(\((?<revision>[0-9a-zA-Z\s]+)\))?\.(?<extension>[0-9a-zA-Z\s]+)";
                Regex analyzer = new Regex(pattern);

                int failedCount = 0;
                foreach (Game game in games)
                {
                    var match = analyzer.Match(game.fileName);
                    if (match.Success)
                    {
                        game.name = match.Groups["gameName"].Value;
                        game.publisher = match.Groups["publisher"].Value;
                        string year = match.Groups["year"].Value;
                        if (year != "") { game.year = int.Parse(year); }
                        game.revision = match.Groups["revision"].Value;
                        game.extension = match.Groups["extension"].Value;
                        db.Update(game);
                    } else
                    {
                        failedCount++;
                    }
                }

                quickLoadMsg.Content = total + " games analyzed, " + failedCount + " games failed to parse.";
                initializeDataGrid(); // @TODO auto-update datagrid on table changes issue#11

                return true;
            }
            catch (Exception e)
            {
                // @TODO Exception handling
                return false;
            }
        }

        private void analyzeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (analyzeGames() == false)
            {
                MessageBox.Show("An error occurred analyzing your game collection list.", "Unable to Analyze", MessageBoxButton.OK);
            }
        }
    }
}
