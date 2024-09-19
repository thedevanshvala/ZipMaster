using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.IO.Compression; // For zip operations
//using System.Windows.Shapes;

namespace ZipUpzip {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
    }
    private const string SevenZipDllPath = @"C:\pCareRemoteWork_pcpBranch\pCareWpf\bin\x64\Debug\7z.dll";

    [DllImport(SevenZipDllPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern int SevenZipCompress(string inputPath, string outputPath);

    [DllImport(SevenZipDllPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern int SevenZipDecompress(string inputPath, string outputPath);

    private void tbZipFolder_Loaded(object sender, RoutedEventArgs e) {

    }

    private void BrowseFolder_Click(object sender, RoutedEventArgs e) {
      var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog {
        IsFolderPicker = true  // Enable folder selection mode
      };

      if (dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok) {
        tbSelectedFolder.Text = dialog.FileName;  // Set the folder path to your text box
      }
    }

    private void ZipFolder_Click(object sender, RoutedEventArgs e) {
      try {
        string folderPath = tbSelectedFolder.Text;

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) {
          MessageBox.Show("Please select a valid folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        string zipFilePath = Path.Combine(Path.GetDirectoryName(folderPath), Path.GetFileName(folderPath) + ".zip");

        if (File.Exists(zipFilePath)) {
          File.Delete(folderPath); // Overwrite if already exists
        }

        ZipFile.CreateFromDirectory(folderPath, zipFilePath);
        MessageBox.Show($"Folder successfully zipped to {zipFilePath}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
      } catch (Exception ex) {
        MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void UnzipFolder_Click(object sender, RoutedEventArgs e) {
      try {
        string folderPath = tbSelectedFolder.Text;

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) {
          MessageBox.Show("Please select a valid folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        var zipFiles = Directory.GetFiles(folderPath, "*.zip");

        if (!zipFiles.Any()) {
          MessageBox.Show("No .zip files found in the selected folder.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
          return;
        }

        foreach (var zipFile in zipFiles) {
          string extractPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(zipFile));

          if (Directory.Exists(extractPath)) {
            Directory.Delete(extractPath, true); // Overwrite if already exists
          }

          ZipFile.ExtractToDirectory(zipFile, extractPath);
          MessageBox.Show($"File '{Path.GetFileName(zipFile)}' successfully extracted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
      } catch (Exception ex) {
        MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

  }
}
