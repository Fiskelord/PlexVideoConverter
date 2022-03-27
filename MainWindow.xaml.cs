using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Plex_Video_Converter
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

        private List<string> getVideoFiles()
        {
            string[] allFiles = Directory.GetFiles(txtSource.Text, "*.*", SearchOption.AllDirectories);
            List<string> filesList = new List<string>();

            foreach (string file in allFiles)
            {
                if (file.Split(".").Last() == "mkv" || file.Split(".").Last() == "avi" || file.Split(".").Last() == "mp4" && chkIncludeMP4.IsChecked == true)
                    filesList.Add(file);
            }

            return filesList;
        }

        private void disableAllControls()
        {
            txtSource.IsEnabled = false;
            txtDestination.IsEnabled = false;

            chkIncludeMP4.IsEnabled = false;

            btnBrowseSource.IsEnabled = false;
            btnBrowseDestination.IsEnabled = false;
            btnCheckSource.IsEnabled = false;
            btnStart.IsEnabled = false;
        }

        private void enableAllControls()
        {
            txtSource.IsEnabled = true;
            txtDestination.IsEnabled = true;

            chkIncludeMP4.IsEnabled = true;

            btnBrowseSource.IsEnabled = true;
            btnBrowseDestination.IsEnabled = true;
            btnCheckSource.IsEnabled = true;
            btnStart.IsEnabled = true;
        }

        private void btnCheckSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> files = getVideoFiles();

                System.Windows.Forms.MessageBox.Show(files.Count.ToString() + " files found in \"" + txtSource.Text + "\"");
            }
            catch (DirectoryNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("Source directory not found, or path not formatted correctly");
            }
            catch (ArgumentException)
            {
                System.Windows.Forms.MessageBox.Show("No source directory specified");
            }
        }

        private void btnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSource.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnBrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDestination.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> files = getVideoFiles();
                string dest = txtDestination.Text;

                disableAllControls();
                pbMain.Value = 0;

                Thread t = new Thread(() =>
                {
                    foreach (string file in files)
                    {
                        Process p = new Process();

                        p.StartInfo.FileName = "handbrakecli.exe";
                        p.StartInfo.Arguments = "-Z \"Very Fast 1080p30\" " +
                                                "-i \"" + file + "\" " +
                                                "-o \"" + dest + "\\" + file.Split("\\").Last().Replace(file.Split("\\").Last().Substring(file.Split("\\").Last().Length - 4), "") + ".mp4" + "\"";

                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;

                        p.Start();

                        p.WaitForExit();
                        p.Dispose();

                        Dispatcher.BeginInvoke((Action)(() => pbMain.Value += 1000 / files.Count));
                    }

                    Dispatcher.BeginInvoke((Action)(() => enableAllControls()));
                });

                t.IsBackground = true;
                t.Start();
            }
            catch (DirectoryNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("Source directory not found, or path not formatted correctly");
            }
            catch (ArgumentException)
            {
                System.Windows.Forms.MessageBox.Show("No source directory specified");
            }
        }
    }
}
