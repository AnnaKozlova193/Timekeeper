using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Timekeeper.UI
{
    public partial class PreviewCorrection : Form
    {
        string pathPreview;
        List<string> photoPath;
        int count = 0;

        public PreviewCorrection(string pathPreview)
        {
            InitializeComponent();

            // эту папку удаляем по завершению работы программы
            this.pathPreview = pathPreview; // здесь оригинал для просмотра

            // Заполняем список названиями файлов
            var dir = new DirectoryInfo(pathPreview); // папка с файлами 
            photoPath = new List<string>();
            foreach (FileInfo file in dir.GetFiles())
            {
                photoPath.Add(Path.GetFileNameWithoutExtension(file.FullName));
            }
      
        }
        private Bitmap MyImage;
        public void ShowMyImage(String fileToDisplay, int xSize, int ySize)
        {
            // Устанавливает объект изображения для отображения.
            if (MyImage != null)
            {
                MyImage.Dispose();
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            MyImage = new Bitmap(fileToDisplay);
            pictureBox1.Image = (Image)MyImage;
        }

        // Кнопки перелистывания фотографий
        #region
        private void btn_back_Click(object sender, EventArgs e)
        {
           count--;

            for (int i = 0; i < photoPath.Count; i++)
            {
                if (i == count && i < photoPath.Count)
                {
                    ShowMyImage($@"{pathPreview}\{photoPath[i]}.jpg", 634, 376);
                }
                if (count < 0)
                {
                    count = photoPath.Count -1;
                    ShowMyImage($@"{pathPreview}\{photoPath[photoPath.Count - 1]}.jpg", 634, 376);
                }
            }
       
        }

        private void btn_before_Click(object sender, EventArgs e)
        {
            count++;

            for (int i = 0; i < photoPath.Count; i++)
            {
                if (i == count && i < photoPath.Count)
                {
                   ShowMyImage($@"{pathPreview}\{photoPath[i]}.jpg", 634, 376);
                }
                if (count == photoPath.Count)
                {
                    count = 0;
                    ShowMyImage($@"{pathPreview}\{photoPath[0]}.jpg", 634, 376);
                }
               
            }
    
        }

        #endregion

        private void PreviewCorrection_Load(object sender, EventArgs e)
        {
            ShowMyImage($@"{pathPreview}\{photoPath[0]}.jpg", 634, 376);
        }
    }
}
