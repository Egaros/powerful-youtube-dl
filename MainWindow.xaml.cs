﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
//using MahApps.Metro.Controls;

namespace powerful_youtube_dl
{
    public partial class MainWindow 
    {
        public static string ytDlPath = "";
        public static string downloadPath = "";

        public MainWindow()
        {
            InitializeComponent();
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Powerful YouTube Dl", true);
            ytDlPath = key.GetValue("ytdlexe", "").ToString();
            if (ytDlPath == "")
                ytDLabel.Content = "Wybierz plik youtube-dl.exe";
            else
                ytDLabel.Content = Path.GetFileName(ytDlPath);

            downloadPath = key.GetValue("dlpath", "").ToString();
            if (downloadPath == "")
                localization.Content = "Wybierz lokalizację pobierania";
            else
                localization.Content = downloadPath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".exe";
            dialog.Filter = "Exe Files (*.exe)|*.exe";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                ytDlPath = dialog.FileName;
                ytDLabel.Content = dialog.SafeFileName;
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Powerful YouTube Dl", true);
                key.SetValue("ytdlexe", ytDlPath);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                localization.Content = dialog.SelectedPath;
                downloadPath = dialog.SelectedPath;
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Powerful YouTube Dl", true);
                key.SetValue("dlpath", downloadPath);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string url = link.Text;
            if (url.Contains(" "))
                Error("Podany link jest nieprawidłowy!");
            else if (url.Contains("channel") || url.Contains("user"))
            {
                int wynik = Dialog.Prompt("Co dokładnie ma zostać pobrane:", "Powerful YouTube DL", "Wszystkie playlisty użytkownika", "Wszystkie materiały dodane przez użytkownika");
                if (wynik == 0)
                    new User(url);
                else if (wynik == 1)
                    new PlayList(url);         ///////////////////////// ZROBIĆ POBIERANIE WSZYSTKICH DODANYCH PLIKÓW 
                else
                    System.Windows.MessageBox.Show("Powerful YouTube DL", "Wystąpił błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (url.Contains("watch"))
            {
                int wynik = -1;
                if (url.Contains("list"))
                {
                    wynik = Dialog.Prompt("Co dokładnie ma zostać pobrane:", "Powerful YouTube DL", "Tylko piosenka, bez playlisty", "Cała playlista na której umieszczona jest piosenka");
                    if (wynik == 0)
                        new Video(url);
                    else if (wynik == 1)
                        new PlayList(url);
                    else
                        System.Windows.MessageBox.Show("Powerful YouTube DL", "Wystąpił błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    new Video(url);
            }
            else if (url.Contains("playlist") || url.Contains("list"))
            {
                new PlayList(url);
            }
            else
                Error("Podany link jest nieprawidłowy!");
        }

        public static void Error(string err)
        {
            System.Windows.Forms.MessageBox.Show(err, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void link_GotFocus(object sender, RoutedEventArgs e)
        {
            if (link.Text == "Link do kanału, playlisty lub video")
                link.Text = "";
            link.SelectAll();
        }

        private void playlist_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int index = playlist.SelectedIndex;
            Video._listOfVideosCheckBox.Clear();
            foreach (System.Windows.Controls.CheckBox chec in PlayList._listOfPlayLists[index]._listOfVideosInPlayListCheckBox)
                Video._listOfVideosCheckBox.Add(chec);
            videos.ScrollIntoView(videos.Items[0]);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Download.Load();
            tabs.SelectedIndex = 1;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Download.Delete(kolejka.SelectedIndex);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (buttonDownload.Content.ToString() != "Stop")
            {
                buttonDownload.Content = "Stop";
                Download.DownloadQueue();
            }
            else
            {
                buttonDownload.Content = "Pobierz";
            }
        }

        public void addVideoToQueue(ListViewItemMy video)
        {
            kolejka.Items.Add(video);
        }

        public void deleteVideoFromQueue(int index)
        {
            kolejka.Items.RemoveAt(index);
        }

        public void deleteVideoFromQueue(ListViewItemMy pos)
        {
            int ind = kolejka.Items.IndexOf(pos);
            kolejka.Items.RemoveAt(ind);
        }
    }
}
