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

namespace TexEditor
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

        private void BtnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxbFileSection.Text, out var sectionNum))
            {
                MessageBox.Show(this, "Please enter a right num!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filePath = TxbFilePath.Text;

            if (!File.Exists(filePath))
            {
                MessageBox.Show(this, "File NOT Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fileDir = System.IO.Path.GetDirectoryName(filePath);
            var fileText = System.IO.Path.GetFileNameWithoutExtension(filePath) + ".txt";
            var saveTextPath = System.IO.Path.Combine(fileDir, fileText);

            var texSections = LibTex.Deserialize(filePath, sectionNum);
            using var writer = new StreamWriter(saveTextPath, false, Encoding.UTF8);
            writer.Write(texSections.ConvertToText());
        }

        private void TxbFilePath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {

                // Set filter for file extension and default file extension 
                DefaultExt = ".tex",
                Filter = "Firefly Tex Files (*.tex)|*.tex"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog(this);

            // Get the selected file name and display in a TextBox 
            if (result == null || result != true)
            {
                return;
            }

            TxbFilePath.Text = dlg.FileName;
        }
    }
}
