using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WPFFrameworkApp
{
    /// <summary>
    /// TrashBacket.xaml etkileşim mantığı
    /// </summary>
    public partial class TrashBacket : Window
    {
        public TrashBacket()
        {
            InitializeComponent();
            ReloadTrashBacket();
            Show();
        }

        private void ReloadTrashBacket()
        {
            trashPanel.Children.Clear();
            string[] options = { "Shred", "Rescue" };
            IEnumerable<string> trashes = Directory.EnumerateFileSystemEntries(MainWindow.TrashPath);
            foreach (string trash in trashes)
            {
                string trashname = Path.GetFileName(trash);
                Button app = RoutineLogics.CreateButton(trashname);
                TextBlock appname = RoutineLogics.CreateTextBlock(trashname);
                Image image = RoutineLogics.CreateImage();
                StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Vertical };

                if (trashname.EndsWith(SupportedFiles.TXT))
                {
                    RoutineLogics.Appearence(image, stackpanel, app, appname, ImagePaths.TXT_IMG);
                    AddListener(app, trash, trashname, options, ImagePaths.TXT_IMG);
                }
                else if (trashname.EndsWith(SupportedFiles.RTF))
                {
                    RoutineLogics.Appearence(image, stackpanel, app, appname, ImagePaths.RTF_IMG);
                    AddListener(app, trash, trashname, options, ImagePaths.RTF_IMG);
                }
                else if (trashname.EndsWith(SupportedFiles.EXE))
                {
                    RoutineLogics.Appearence(image, stackpanel, app, appname, ImagePaths.EXE_IMG);
                    AddListener(app, trash, trashname, options, ImagePaths.EXE_IMG);
                }
                else if (trashname.EndsWith(SupportedFiles.MP3))
                {
                    RoutineLogics.Appearence(image, stackpanel, app, appname, ImagePaths.MP3_IMG);
                    AddListener(app, trash, trashname, options, ImagePaths.MP3_IMG);
                }
                else if (trashname.EndsWith(SupportedFiles.WAV))
                {
                    RoutineLogics.Appearence(image, stackpanel, app, appname, ImagePaths.WAV_IMG);
                    AddListener(app, trash, trashname, options, ImagePaths.WAV_IMG);
                }
                trashPanel.Children.Add(app);
            }
        }
        private void AddListener(Button app, string trash, string trashname, string[] options, string Image)
        {
            app.Click += (sender, e) =>
            {
                try
                {
                    switch(QueryDialog.ShowQueryDialog(trashname, "Deleted Item Options", options, Image, ImagePaths.FULL_IMG)) 
                    {
                        case 0:
                            File.Delete(trash); // delete selected
                            ReloadTrashBacket();
                            break;
                        case 1:
                            File.Move(trash, Path.Combine(MainWindow.CDesktopPath, trashname)); // rescue selected
                            ReloadTrashBacket();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    RoutineLogics.ErrorMessage(Errors.DEL_ERR, Errors.DEL_ERR_MSG, trashname, "\n", ex.Message);
                }
            };
        }

        private void TrashBacketReload_Wanted(object sender, RoutedEventArgs e)
        {
            ReloadTrashBacket();
        }

        private void RescueAll(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to rescue all files from TrashBacket?", "Empty Trash", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                IEnumerable<string> trashes = Directory.EnumerateFileSystemEntries(MainWindow.TrashPath);
                foreach (string trash in trashes)
                {
                    File.Move(trash, Path.Combine(MainWindow.CDesktopPath, Path.GetFileName(trash)));
                }
                trashPanel.Children.Clear();
            }
        }

        private void EmptyTrash(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to empty the TrashBacket?\nYou can not take back again.", "Empty Trash", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                IEnumerable<string> trashes = Directory.EnumerateFileSystemEntries(MainWindow.TrashPath);
                foreach(string trash in trashes)
                {
                    File.Delete(trash);
                }
                trashPanel.Children.Clear();
            }
        }
    }
}
