using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Timekeeper.UI
{
    public partial class SeePhoto : Form
    {
        string nameCycle;
        string path1h;
        List<string> photoPath;
        int count = 0;
        public SeePhoto(string path1h, string nameCycle)
        {
            InitializeComponent();
            this.path1h = path1h;
            this.nameCycle = nameCycle;
            // Заполняем список названиями файлов
            var dir = new DirectoryInfo(path1h); // папка с файлами 
            photoPath = new List<string>();
            foreach (FileInfo file in dir.GetFiles())
            {
                photoPath.Add(Path.GetFileNameWithoutExtension(file.FullName));
            }
            Text = nameCycle;
            ShowMyImage($@"{path1h}\{photoPath[0]}.jpg", 634, 376);
        }

        private Bitmap MyImage;
        public void ShowMyImage(String fileToDisplay, int xSize, int ySize)
        {
            if (MyImage != null)
            {
                MyImage.Dispose();
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            MyImage = new Bitmap(fileToDisplay);
            pictureBox1.Image = (Image)MyImage;
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            count--;

            for (int i = 0; i < photoPath.Count; i++)
            {
                if (i == count && i < photoPath.Count)
                {
                    ShowMyImage($@"{path1h}\{photoPath[i]}.jpg", 634, 376);
                }
                if (count < 0)
                {
                    count = photoPath.Count - 1;
                    ShowMyImage($@"{path1h}\{photoPath[photoPath.Count - 1]}.jpg", 634, 376);
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
                    ShowMyImage($@"{path1h}\{photoPath[i]}.jpg", 634, 376);
                }
                if (count == photoPath.Count)
                {
                    count = 0;
                    ShowMyImage($@"{path1h}\{photoPath[0]}.jpg", 634, 376);
                }

            }
  
        }

    }
}
