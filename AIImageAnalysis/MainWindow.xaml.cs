using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO.Compression;

namespace AIImageAnalysis
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

        /// <summary>
        /// Переменная класса для обработки изображения (получения текстового описания)
        /// </summary>
        private List<ImageAnalysis> ImageAnalysisList;
        private string folderPath;

        private List<string> GetImageNames(string folderPath)
        {
            DirectoryInfo Folder = new DirectoryInfo(folderPath);
            FileInfo[] Images = Folder.GetFiles();
            List<string> imageNames = new List<string>();

            for (int i = 0; i < Images.Length; i++)
            {
                imageNames.Add(Images[i].Name);
            }

            return imageNames;
        }

        private void CreateFolder(string pathToNewFolder)
        {
            Directory.CreateDirectory(pathToNewFolder);
        }
        private void CreateZipFile(string folderPath, string zipPath)
        {
            ZipFile.CreateFromDirectory(folderPath, zipPath);
        }
        private void MoveFile(string path1, string path2)
        {
            File.Move(path1, path2);
        }
        private void MoveFileToArchive(string folderPath, string zipPath)
        {
            using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(folderPath);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        writer.WriteLine("Information about this package.");
                        writer.WriteLine("========================");
                    }
                }
            }
        }


        #region MenuButton
        /// <summary>
        /// Загружаем изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUploadImage(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };

            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                folderPath = commonOpenFileDialog.FileName;
                List<string> imageNames = GetImageNames(folderPath);
                ImageAnalysisList = new List<ImageAnalysis>();
                foreach (var item in imageNames)
                {
                    //ImageView.Source = BitmapFrame.Create(new Uri(string.Format(@"{0}/{1}", folderPath, item)));
                    ImageAnalysisList.Add(new ImageAnalysis(string.Format(@"{0}/{1}", folderPath, item), item));
                }
            }

            //OpenFileDialog openFileDialog = new OpenFileDialog()
            //{
            //    //Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|All(*.*)|*.*",
            //    Filter = "Image|*.png;*.jpg;*.bmp|All (*.*)|*.*",
            //    CheckFileExists = true
            //};
            //if (openFileDialog.ShowDialog() == true)
            //{
            //    ImageView.Source = BitmapFrame.Create(new Uri(openFileDialog.FileName));
            //    imageAnalysis = new ImageAnalysis(openFileDialog.FileName);
            //}
        }
        /// <summary>
        /// Обрабатываем изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetImageAnalysis(object sender, RoutedEventArgs e)
        {
            if (ImageAnalysisList.Count > 0 && GetImageNames(folderPath).Count > 0)
            {
                int count = 0;
                List<string> ImageClassesList = new List<string>();
                foreach (var item in ImageAnalysisList)
                {
                    item.ImageClassification();
                    string imageClass = item.GetPredictedLabel();
                    if (!ImageClassesList.Contains(imageClass))
                    {
                        ImageClassesList.Add(imageClass);
                        CreateFolder(folderPath + "/" + imageClass);
                    }
                    MoveFile(item.filePath, folderPath + "/" + imageClass + "/" + item.fileName);
                    count++;
                }
                //CreateZipFile(folderPath, folderPath + "/" + "SortedImages.zip");

                TextBoxClasses.Text = $"Обработано фотографий: {count}, Создано папок: {ImageClassesList.Count}";
            }
        }
        #endregion
    }
}
