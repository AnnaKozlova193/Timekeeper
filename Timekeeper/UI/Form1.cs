using System;
using System.IO;
using System.Windows.Forms;
using Timekeeper.UI;

namespace Timekeeper
{
    public partial class Form1 : Form
    {
        Clock clock = new Clock();
        Methods methods = new Methods();

        private static string dateNow = DateTime.Now.ToShortDateString();
        private static string path = @"D:\Timekeeper BOX";
        private static string pathDir = $@"{path}\{dateNow}";
        private static string pathText = $@"{path}\{dateNow}\report.txt";
        public Form1()
        {
            InitializeComponent();
         
            methods.SetTime();

            // Создаем папки для хранения снимков. 
            if (!Directory.Exists(pathDir))
            {
                DirectoryInfo dir = Directory.CreateDirectory(pathDir);
            }

            textBox_temperature.MaxLength = 5;
            textBox_humidity.MaxLength = 5;
            textBox_pressure.MaxLength = 7;

        }

        private void btn_remember_Click(object sender, EventArgs e)
        {
            // Проверяем допуски атмосферных показателей.
            double number_t = 0;
            bool t = true;
            if (Double.TryParse(textBox_temperature.Text.Replace(".", ","), out number_t))
            {
                if (number_t >= 20 && number_t <= 25)
                {
                    clock.temperature = number_t;

                }
                else
                {
                    MessageBox.Show("Допустимая температура : от 20 до 25");
                    t = false;
                    textBox_temperature.Clear();
                    textBox_temperature.Focus();
                }
            }
            double number_h = 0;
            bool h = true;
            if (Double.TryParse(textBox_humidity.Text.Replace(".", ","), out number_h))
            {
                if (number_h >= 30 && number_h <= 90)
                {
                    clock.humidity = number_h;
                }
                else
                {
                    MessageBox.Show("Допустимая влажность : от 30 до 90");
                    h = false;
                    textBox_humidity.Clear();
                    textBox_humidity.Focus();
                }
            }
           double number_p = 0;
            bool p = true;
            if (Double.TryParse(textBox_pressure.Text.Replace(".", ","), out number_p))
            {
                if (number_p >= 80 && number_p <= 110)
                {
                    clock.pressure = number_p;
                }
                else
                {
                    MessageBox.Show("Допустимое давление : от 80 до 110");
                    p = false;
                    textBox_pressure.Clear();
                    textBox_pressure.Focus();
                }
            }

            if (t && h && p)
            {
                // Создаем текстовый документ.
                string text = $"Время создания документа : {dateNow}";
                using (StreamWriter sw = new StreamWriter(pathText, true, System.Text.Encoding.Default))
                {
                    sw.Write($"{text}   {DateTime.Now.ToLongTimeString()}\n");
                    sw.Write(clock.ToString());
                }
                // Переходим в форму для выбора камеры.
                WebCamera cameraForm = new WebCamera();
                cameraForm.Show();
                this.Hide(); // прячем главную форму
            }
     
        }
        // Обработка событий textBox
        #region
        // Записываем данные из формы в переменные.
        private void textBox_customerShotName_TextChanged(object sender, EventArgs e)
        {
            clock.customerShotName = textBox_customerShotName.Text;
        }

        private void textBox_customerFullName_TextChanged(object sender, EventArgs e)
        {
            clock.customerFullName = textBox_customerFullName.Text;
        }

        private void textBox_objectShotName_TextChanged(object sender, EventArgs e)
        {
            clock.shotName = textBox_objectShotName.Text;
        }

        private void textBox_idObject_TextChanged(object sender, EventArgs e)
        {
            clock.strId = textBox_idObject.Text;
        }

        private void textBox_objectFullName_TextChanged(object sender, EventArgs e)
        {
            clock.fullName = textBox_objectFullName.Text;
        }

    
        private void textBox_temperature_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ввод только цифр с одной точкой (запятой)
            if (e.KeyChar == '.'|| e.KeyChar == ',')
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text.Contains(".") || txt.Text.Contains(","))
                {
                    e.Handled = true;
                }
                return;
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }

        }

        private void textBox_humidity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ввод только цифр с одной точкой (запятой)
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text.Contains(".") || txt.Text.Contains(","))
                {
                    e.Handled = true;
                }
                return;
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox_pressure_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ввод только цифр с одной точкой (запятой)
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text.Contains(".") || txt.Text.Contains(","))
                {
                    e.Handled = true;
                }
                return;
            }

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
