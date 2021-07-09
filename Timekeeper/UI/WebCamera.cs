using DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Timekeeper.UI
{
    public partial class WebCamera : Form
    {
        DataMeasurement data = new DataMeasurement();
        Methods methods = new Methods();

        public Image<Bgr, byte> image = null;
        // Здесь храним захваченные данные с камеры.
        public VideoCapture capture = null;
        // Из библиотеки DirectShowLib.
        public DsDevice[] webCams = new DsDevice[] { };

        // Id первой камеры в  списке с видеоустройствами.
        private int selectedCameraId = 0;

        private static System.Timers.Timer timerScreen;
        private static System.Timers.Timer timerScreen3h;
        private static System.Timers.Timer timerScreen6h;
        private static System.Timers.Timer timerScreenPreview;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
  
        public static string dateNow = DateTime.Now.ToShortDateString();
        public static string path = @"D:\Timekeeper BOX";
        public static string pathDir = $@"{path}\{dateNow}";

        public static string pathPreview = $@"{pathDir}\Preview";
        public static string path1h = $@"{pathDir}\hour_1";
        public static string path3h = $@"{pathDir}\hour_3";
        public static string path6h = $@"{pathDir}\hour_6";
      
        public static string pathTampPreview = $@"{pathPreview}\Templates";
        public static string pathTamp1 = $@"{path1h}\Templates_1";
        public static string pathTamp3 = $@"{path3h}\Templates_3";
        public static string pathTamp6 = $@"{path6h}\Templates_6";

        bool plus = false;
        bool interval = false;
        bool cycle = false;
        bool num = false;

        bool interval_3h = false;
        bool cycle_3h = false;
        bool num_3h = false;

        bool interval_6h = false;
        bool cycle_6h = false;
        bool num_6h = false;

        private SynchronizationContext SyncContext = SynchronizationContext.Current;
       
        public WebCamera()
        {
            InitializeComponent();
          
            // Создаем папки для хранения снимков. 
            if (!Directory.Exists(pathTampPreview))
            {
                DirectoryInfo dir = Directory.CreateDirectory(pathTampPreview);
            }

            if (!Directory.Exists(pathTamp1))
            {
                DirectoryInfo dir = Directory.CreateDirectory(pathTamp1);
            }

            if (!Directory.Exists(pathTamp3))
            {
                DirectoryInfo dir = Directory.CreateDirectory(pathTamp3);
            }

            if (!Directory.Exists(pathTamp6))
            {
                DirectoryInfo dir = Directory.CreateDirectory(pathTamp6);
            }
            textBox_millisec.MaxLength = 6;
  
        }
        // Получение видео.
        #region
        private void WebCamera_Load(object sender, EventArgs e)
        {
            // Получили все доступные видеокамеры.
            webCams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            // Заполняем комбобокс доступными камерами. 
            for (int i = 0; i < webCams.Length; i++)
            {
                comboBox1.Items.Add(webCams[i].Name);
            }
            // Прозрачный фон label. 
            lab_time.Parent = pictureBox1;
            lab_time.BackColor = Color.Transparent;
            // Отображение времени в pictureBox1.
            timer.Interval = 1000; // 1 секунда
            timer.Tick += new EventHandler(timer1_Tick);
            timer.Start();

            btn_show1hphoto.Visible = false;
            btn_show3hphoto.Visible = false;
            btn_6hphpto.Visible = false;
            btn_showTest.Visible = false;

        }
        // Выбираем камеру из выпадающего списка.
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraId = comboBox1.SelectedIndex;
        }
       
        private void btn_startCam_Click(object sender, EventArgs e)
        {
            btn_preview.Enabled = true;
            try
            {
                if (webCams.Length == 0)
                {
                    throw new Exception("Нет доступных камер !");
                }
                else if (comboBox1.SelectedItem == null)
                {
                    throw new Exception("Необходимо выбрать камеру !");
                }
                else
                {
                    (sender as Button).Enabled = false;// блокировка нажатия кнопки
                    // Камера с которой захватываем видео.
                    capture = new VideoCapture(selectedCameraId);
                    // Подписываемся на событие захвата картинки.
                    capture.ImageGrabbed += Capture_ImageGrabbed;
                    // Выполняем получение видеоданных.
                    capture.Start();  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR_1!");                                                                                     
            }
     
        }
        // Устанавливаем получаемые кадры.
        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Work(SyncContext);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR_2!");
            }
        }
        // Синхронизируем потоки.
        private void UpdatePictureBox(object state)
        {  // Для хранения изображения.
            Mat mat = new Mat();
            capture.Retrieve(mat);
            pictureBox1.Image = mat.ToImage<Bgr, byte>()
                .Flip(Emgu.CV.CvEnum.FlipType.None).Bitmap;
        }
        
        private void Work(object state)
        {
            SynchronizationContext syncContext = state as SynchronizationContext;
            syncContext.Post(UpdatePictureBox, syncContext);
        }
        
        #endregion
        // События текст-боксов.
        #region
        private void checkBox_minus_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_plus.Checked = false;
            plus = false;
        }

        private void checkBox_plus_CheckedChanged(object sender, EventArgs e)
        {
            plus = true;
            checkBox_minus.Checked = false;
        }
        private void textBox_millisec_TextChanged(object sender, EventArgs e)
        {
            if (textBox_millisec.Text == string.Empty)
            {
                data.millisecondСorrection = 0;
            }
            int numberM = 0;
            if (Int32.TryParse(textBox_millisec.Text, out numberM))
            {
                if (numberM > 0 && numberM < 600000)
                {
                    data.millisecondСorrection = numberM;
                }
                else
                {
                    MessageBox.Show("Введите число 1 - 599999 ");
                    textBox_millisec.Clear();
                    textBox_millisec.Focus();
                }
            }
        }

        private void textBox_millisec_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
        private void tb_cyclr1h_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Блокируем ввод символов кроме цыфр и клавиши удаления Backspace.
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_interval1h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_number1h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_cycle3h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_interval3h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_number3h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_cycle6h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_interval6h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tb_number6h_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
        private void tb_cyclr1h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_cyclr1h.Text, out number))
            {
                if (number > 0 && number < 31)
                {
                    data.cycle_1hour = number;
                    cycle = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемый цикл съёмки: 1 - 30 мин.");
                    tb_cyclr1h.Clear();
                    tb_cyclr1h.Focus();

                }
            }
        }

        private void tb_interval1h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_interval1h.Text, out number))
            {
                if (number > 0 && number < 31)
                {
                    data.interval_1hour = number;
                    interval = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемый интервал: 1 - 30 сек.");
                    tb_interval1h.Clear();
                    tb_interval1h.Focus();

                }
            }
        }
        int maxCountPhoto;
        private void tb_number1h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_number1h.Text, out number))
            {
                if (number > 0 && number < 601)
                {
                    data.number_1hour = number;

                    if (data.interval_1hour > 20)
                    {
                        // Допустимое кол-во фото .
                        maxCountPhoto = data.cycle_1hour * 2;

                        if (data.number_1hour >= maxCountPhoto)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhoto}");
                            tb_number1h.Clear();
                            tb_number1h.Focus();
                        }
                    }
                    else if (data.interval_1hour > 15)
                    {
                        maxCountPhoto = data.cycle_1hour * 3;

                        if (data.number_1hour >= maxCountPhoto)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhoto}");
                            tb_number1h.Clear();
                            tb_number1h.Focus();
                        }
                    }
                    else if (data.interval_1hour > 10)
                    {
                        maxCountPhoto = data.cycle_1hour * 4;

                        if (data.number_1hour >= maxCountPhoto)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhoto}");
                            tb_number1h.Clear();
                            tb_number1h.Focus();
                        }
                    }
                    else if (data.interval_1hour > 5)
                    {
                        maxCountPhoto = data.cycle_1hour * 6;

                        if (data.number_1hour >= maxCountPhoto)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhoto}");
                            tb_number1h.Clear();
                            tb_number1h.Focus();
                        }
                    }
                    else if (data.interval_1hour > 0)
                    {
                        maxCountPhoto = data.cycle_1hour * 12;

                        if (data.number_1hour >= maxCountPhoto)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhoto}");
                            tb_number1h.Clear();
                            tb_number1h.Focus();
                        }
                    }
                    num = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемое кол-во фото: 1 - 600 ");
                    tb_number1h.Clear();
                    tb_number1h.Focus();
                }
            }
        }

        private void tb_cycle3h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_cycle3h.Text, out number))
            {
                if (number > 0 && number < 91)
                {
                    data.cycle_3hour = number;
                    cycle_3h = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемый цикл съёмки: 1 - 90 мин.");
                    tb_cycle3h.Clear();
                    tb_cycle3h.Focus();
                }
            }
        }

        private void tb_interval3h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_interval3h.Text, out number))
            {
                if (number > 0 && number < 301)
                {
                    data.interval_3hour = number;
                    interval_3h = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемый интервал: 1 - 300 сек.");
                    tb_interval3h.Clear();
                    tb_interval3h.Focus();
                }
            }
        }
        int maxCountPhot_3h;
        private void tb_number3h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_number3h.Text, out number))
            {
                if (number > 0 && number < 601)
                {
                    data.number_3hour = number;

                    if (data.interval_3hour > 20)
                    {
                        // Допустимое кол-во фото .
                        maxCountPhot_3h = data.cycle_3hour * 2;

                        if (data.number_3hour >= maxCountPhot_3h)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhot_3h}");
                            tb_number3h.Clear();
                            tb_number3h.Focus();
                        }
                    }
                    else if (data.interval_3hour > 15)
                    {
                        maxCountPhot_3h = data.cycle_3hour * 3;

                        if (data.number_3hour >= maxCountPhot_3h)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhot_3h}");
                            tb_number3h.Clear();
                            tb_number3h.Focus();
                        }
                    }
                    else if (data.interval_3hour > 10)
                    {
                        maxCountPhot_3h = data.cycle_3hour * 4;

                        if (data.number_3hour >= maxCountPhot_3h)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhot_3h}");
                            tb_number3h.Clear();
                            tb_number3h.Focus();
                        }
                    }

                    num_3h = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемое кол-во фото: 1 - 600 ");
                    tb_number3h.Clear();
                    tb_number3h.Focus();
                }
            }
        }

        private void tb_cycle6h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_cycle6h.Text, out number))
            {
                if (number > 0 && number < 121)
                {
                    data.cycle_6hour = number;
                    cycle_6h = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемый цикл съёмки: 1 - 120 мин.");
                    tb_cycle6h.Clear();
                    tb_cycle6h.Focus();
                }
            }
        }

        private void tb_interval6h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_interval6h.Text, out number))
            {
                if (number > 0 && number < 601)
                {
                    data.interval_6hour = number;
                    interval_6h = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемый интервал: 1 - 600 сек.");
                    tb_interval6h.Clear();
                    tb_interval6h.Focus();
                }
            }
        }
        int maxCountPhot_6h;
        private void tb_number6h_TextChanged(object sender, EventArgs e)
        {
            int number = 0;
            if (Int32.TryParse(tb_number6h.Text, out number))
            {
                if (number > 0 && number < 601)
                {
                    data.number_6hour = number;

                    if (data.interval_6hour > 20)
                    {
                        // Допустимое кол-во фото .
                        maxCountPhot_6h = data.cycle_3hour * 2;

                        if (data.number_6hour >= maxCountPhot_6h)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhot_6h}");
                            tb_number6h.Clear();
                            tb_number6h.Focus();
                        }
                    }
                    else if (data.interval_6hour > 15)
                    {
                        maxCountPhot_6h = data.cycle_6hour * 3;

                        if (data.number_6hour >= maxCountPhot_6h)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhot_6h}");
                            tb_number6h.Clear();
                            tb_number6h.Focus();
                        }
                    }
                    else if (data.interval_6hour > 10)
                    {
                        maxCountPhot_6h = data.cycle_6hour * 4;

                        if (data.number_6hour >= maxCountPhot_6h)
                        {
                            MessageBox.Show($"При данных параметрах," +
                           $" \n Рекомендуемое кол-во фото: 1 - {maxCountPhot_6h}");
                            tb_number6h.Clear();
                            tb_number6h.Focus();
                        }
                    }

                    num_6h = true;
                }
                else
                {
                    MessageBox.Show("Рекомендуемое кол-во фото: 1 - 600 ");
                    tb_number6h.Clear();
                    tb_number6h.Focus();
                }
            }
        }
        #endregion
        // Таймеры
        #region
        // Таймер отображаемого времени.
        private void timer1_Tick(object sender, EventArgs e)
        {
            int h = DateTime.Now.Hour;
            int m = DateTime.Now.Minute;
            int s = DateTime.Now.Second;

            string time = "";

            if (h < 10) { time += "0" + h; }
            else
            { time += h; }

            time += ":";

            if (m < 10) { time += "0" + m; }
            else
            { time += m; }

            time += ":";

            if (s < 10) { time += "0" + s; }
            else
            { time += s; }

            lab_time.Text = time;
        }
        // Таймер 1 часового измерения.
        private void SetTimerInterval(int time, double stopSec)
        {
            // Создали таймер .
            timerScreen = new System.Timers.Timer(time);
            // Подключите событие Elapsed к таймеру. 
            timerScreen.Elapsed += TimerScreen_Elapsed;
            timerScreen.AutoReset = true; 
            timerScreen.Enabled = true;
        }
        // Таймер 3 часового измерения.
        private void SetTimerInterval_3h(int time, double stopSec)
        {
            timerScreen3h = new System.Timers.Timer(time);
            timerScreen3h.Elapsed += TimerScreen3h_Elapsed;
            timerScreen3h.AutoReset = true;
            timerScreen3h.Enabled = true;
        }
        // Таймер 6 часового измерения.
        private void SetTimerInterval_6h(int time, double stopSec)
        {
            timerScreen6h = new System.Timers.Timer(time);
            timerScreen6h.Elapsed += TimerScreen6h_Elapsed;
            timerScreen6h.AutoReset = true;
            timerScreen6h.Enabled = true;
        }

        private void TimerScreen_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {   //  Путь к шаблону, путь к оригиналу,время сохранения снимка в секундах.
            SaveScreen(pathTamp1, path1h, out double timeSave);

            if (timeSave >= stopSeconds)
            {
                try
                {
                    if (finishMeasuring == countIteration)
                    {
                        // Доступ в другой поток.Отображаем кнопки.
                        btn_show1hphoto.Invoke(new Action(() =>
                                  btn_show1hphoto.Visible = true));

                        btn_preview.Invoke(new Action(() =>
                              btn_preview.Enabled = true));
                        btn_correction.Invoke(new Action(() =>
                                  btn_correction.Enabled = true));
                        btn_start1h.Invoke(new Action(() =>
                                btn_start1h.Enabled = true));
                        btn_start6h.Invoke(new Action(() =>
                                btn_start6h.Enabled = true));
                        btn_start3h.Invoke(new Action(() =>
                                btn_start3h.Enabled = true));
                        
                        timerScreen.Stop();
                        timerScreen.Dispose();
                    }
                    else
                    {
                        timerScreen.Stop();
                        timerScreen.Dispose();

                        Thread.Sleep(1000 *
                            (60 * data.cycle_1hour - data.interval_1hour * (data.number_1hour - 1))
                            * 1);

                        btn_start1h.Invoke(new Action(() =>
                                    btn_start1h.Enabled = true));

                        // Запуск кнопки "Старт". 
                        btn_start1h.Invoke(new Action(() =>
                                   btn_start1h.PerformClick()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DISPOSE timer_1h {ex.Message}");
                }
            }
        }

        private void TimerScreen3h_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //  Путь к шаблону, путь к оригиналу,время сохранения снимка в секундах.
            SaveScreen(pathTamp3, path3h, out double timeSave);

            if (timeSave >= stopSeconds_3h)
            {
                try
                {
                    if (finishMeasuring_3h == countIteration_3h)
                    {
                        btn_show3hphoto.Invoke(new Action(() =>
                                  btn_show3hphoto.Visible = true));

                        btn_preview.Invoke(new Action(() =>
                              btn_preview.Enabled = true));
                        btn_correction.Invoke(new Action(() =>
                                  btn_correction.Enabled = true));
                        btn_start1h.Invoke(new Action(() =>
                                btn_start1h.Enabled = true));
                        btn_start6h.Invoke(new Action(() =>
                                btn_start6h.Enabled = true));
                        btn_start3h.Invoke(new Action(() =>
                                btn_start3h.Enabled = true));

                        timerScreen3h.Stop();
                        timerScreen3h.Dispose();
                    }
                    else
                    {
                        timerScreen3h.Stop();
                        timerScreen3h.Dispose();

                        Thread.Sleep(1000 *
                            (60 * data.cycle_3hour - data.interval_3hour * (data.number_3hour - 1))
                            * 1);

                        btn_start3h.Invoke(new Action(() =>
                                btn_start3h.Enabled = true));
                      
                        // Запуск кнопки "Старт". 
                        btn_start3h.Invoke(new Action(() =>
                                   btn_start3h.PerformClick()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DISPOSE timer_3h{ex.Message}");
                }
            }
        }

        private void TimerScreen6h_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //  Путь к шаблону, путь к оригиналу,время сохранения снимка в секундах.
            SaveScreen(pathTamp6, path6h, out double timeSave);

            if (timeSave >= stopSeconds_6h)
            {
                try
                {
                    if (finishMeasuring_6h == countIteration_6h)
                    {
                        btn_6hphpto.Invoke(new Action(() =>
                                  btn_6hphpto.Visible = true));

                        btn_preview.Invoke(new Action(() =>
                                btn_preview.Enabled = true));
                        btn_correction.Invoke(new Action(() =>
                                  btn_correction.Enabled = true));
                        btn_start1h.Invoke(new Action(() =>
                                btn_start1h.Enabled = true));
                        btn_start6h.Invoke(new Action(() =>
                                btn_start6h.Enabled = true));
                        btn_start3h.Invoke(new Action(() =>
                                btn_start3h.Enabled = true));

                        timerScreen6h.Stop();
                        timerScreen6h.Dispose();
                     
                    }
                    else
                    {
                        timerScreen6h.Stop();
                        timerScreen6h.Dispose();

                        Thread.Sleep(1000 *
                            (60 * data.cycle_6hour - data.interval_6hour * (data.number_6hour - 1))
                            * 1);

                        btn_start6h.Invoke(new Action(() =>
                                  btn_start6h.Enabled = true));
                       
                        // Запуск кнопки "Старт".
                        btn_start6h.Invoke(new Action(() =>
                                   btn_start6h.PerformClick()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DISPOSE timer_6h{ex.Message}");
                }
            }
        }
        // Таймер предпросмотра фотографий.
        private void CorrectTimerInterval(int time)
        {
            // Создайте таймер 
            timerScreenPreview = new System.Timers.Timer(time);
            timerScreenPreview.Elapsed += TimerScreenPreview_Elapsed;
            timerScreenPreview.AutoReset = true;
            timerScreenPreview.Enabled = true;

        }
     
        private void TimerScreenPreview_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveScreen(pathTampPreview, pathPreview, out double timeSave);
        
            if (timeSave >= stop_Sec)
            {
                try
                {
                    timerScreenPreview.Stop();
                    timerScreenPreview.Dispose();

                    btn_showTest.Invoke(new Action(() =>
                             btn_showTest.Visible = true ));
                    btn_correction.Invoke(new Action(() =>
                             btn_correction.Enabled = true));
                    btn_start1h.Invoke(new Action(() =>
                             btn_start1h.Enabled = true));
                    btn_start3h.Invoke(new Action(() =>
                             btn_start3h.Enabled = true));
                    btn_start6h.Invoke(new Action(() =>
                             btn_start6h.Enabled = true));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DISPOSE TimerScreenPreview - {ex.Message}");
                }
            }
        }
        #endregion
        // Сохранение фото и наложение времени на снимок
        #region
        private void SaveScreen(string filePath, string pathMain, out double timeSave)
        {
            timeSave = TimeSpan.Parse($"{DateTime.Now.ToLongTimeString()}").TotalSeconds;

            // Получаем путь для сохранения временных фото.
            string t = $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
            string filename = $@"{filePath}\{t}_template.jpg";
            try
            {
                // Сохраняем фото.
                bool ui = pictureBox1.InvokeRequired;
                if (ui)
                {
                    pictureBox1.Invoke(new Action(() =>
                    { pictureBox1.Image.Save(filename, ImageFormat.Jpeg); }));
                }
                else
                { pictureBox1.Image.Save(filename, ImageFormat.Jpeg); }
                //  Наложить время на фото.  
                InsertTextIntoPhoto(filename, pathMain);

                //Console.WriteLine($" photo save : {t}.{DateTime.Now.Millisecond}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR SAVE SCREEN_2!");
            }
     
        }

        private void InsertTextIntoPhoto(string pathTamp, string pathMain)
        {
            try
            {
                DateTime time = DateTime.Now;
                string t = $"{time.Hour}-{time.Minute}-{time.Second}";
                // Путь для создаваемого оригинала.
                string filename = $@"{pathMain}\{t}.jpg";
               
                // Создать точку для верхнего левого угла текста.
                float x = 50.0F;
                float y = 50.0F;
                //                              путь к оригиналу
                using (Image img = Bitmap.FromFile($"{pathTamp}"))
                {
                    // Вносим полученное время для коррекции
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        switch (plus)
                        {
                            case true:
                                methods.ShowTime(data.millisecondСorrection, out string timeStr);
                                // Текст на фото, шрифт и его размер.
                                g.DrawString($"{timeStr}", new Font("Verdana", (float)20),
                                    new SolidBrush(Color.Red), x, y);

                       //         Console.WriteLine($" Время на фото с коррекцией(+) - {timeStr}");
                       //         Console.WriteLine($" Время сейчас - {time.Hour}:{time.Minute}:{time.Second}" +
                       //$".{time.Millisecond}");

                                break;

                            case false:
                                methods.ShowTime(-data.millisecondСorrection, out string timeS);
                                // Текст на фото, шрифт и его размер.
                                g.DrawString($"{timeS}", new Font("Verdana", (float)20),
                                    new SolidBrush(Color.Red), x, y);

                       //         Console.WriteLine($" Время на фото с коррекцией(-) - {timeS}");
                       //         Console.WriteLine($" Время сейчас - {time.Hour}:{time.Minute}:{time.Second}" +
                       //$".{time.Millisecond}");

                                break;

                        }
                        // Путь и имя сохранения файла.
                        img.Save($@"{filename}", ImageFormat.Jpeg);

                    }

                }
                //Console.WriteLine($" Адрес фото: {filename}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine("ERROR_InsertTextIntoPhoto", ex.Message);
                MessageBox.Show(ex.Message, "ERROR_InsertTextIntoPhoto!");
            }

        }
        #endregion
        private void btn_correction_Click(object sender, EventArgs e)
        {
            if (plus)
            { MessageBox.Show($"Вносимая поправка : + {data.millisecondСorrection}");}
            else
            { MessageBox.Show($"Вносимая поправка : - {data.millisecondСorrection}"); }
            btn_preview.Enabled = true;
        }
        double stop_Sec;
        double start_Sec;
        private void btn_preview_Click(object sender, EventArgs e)
        {
            btn_correction.Enabled = false;
            btn_start1h.Enabled = false;
            btn_start3h.Enabled = false;
            btn_start6h.Enabled = false;
            (sender as Button).Enabled = false; // блокировка нажатия кнопки
            
            btn_showTest.Visible = false;

            if (comboBox1.SelectedItem != null)
            {
                start_Sec = TimeSpan.Parse($"{DateTime.Now.ToLongTimeString()}").TotalSeconds;
                stop_Sec = start_Sec + 50; // 50 sek - тестовые фото за 1 минуту 5 штук

                SaveScreen(pathTampPreview, pathPreview, out double timeSave); // 1 фото

                CorrectTimerInterval(1000 * 1 * 10);
            }
            else
            { MessageBox.Show("Выберите камеру!"); }

        }

        private void btn_showTest_Click(object sender, EventArgs e)
        {
            btn_preview.Enabled = true;// снятие блокировки нажатия кнопки
            PreviewCorrection showForm = new PreviewCorrection(pathPreview);
            showForm.Show();
        }
        // Запускаем процесс фотосъемки 1ч,3ч и 6ч измерений.
        double startSeconds;
        double stopSeconds;
        int finishMeasuring = 0;
        int countIteration = 0;
        private void btn_start1h_Click(object sender, EventArgs e)
        {
            btn_preview.Enabled = false;
            btn_correction.Enabled = false;
            btn_start6h.Enabled = false;
            btn_start3h.Enabled = false;

            // Синхронизируем время.
            methods.SetTime();

            if (comboBox1.SelectedItem != null)
            {
                if (cycle && interval && num)
                {
                    (sender as Button).Enabled = false;// блокировка нажатия кнопки
                    btn_show1hphoto.Visible = false;
                    countIteration = 60 / data.cycle_1hour;
                 
                    startSeconds = TimeSpan.Parse($"{DateTime.Now.ToLongTimeString()}").TotalSeconds;
                    // Окончание съемки за один период цикла
                    stopSeconds = startSeconds + (data.interval_1hour * (data.number_1hour - 1));
                    SaveScreen(pathTamp1, path1h, out double timeSave);
                    // Делаем снимки с установленным инервалом.
                    SetTimerInterval(1000 * data.interval_1hour * 1, stopSeconds);

                    ++finishMeasuring;
                }
                else { MessageBox.Show("Введите параметры !"); }
            }
            else
            { MessageBox.Show("Выберите камеру!"); }
        }

        double startSeconds_3h;
        double stopSeconds_3h;
        int finishMeasuring_3h = 0;
        int countIteration_3h = 0;
        private void btn_start3h_Click(object sender, EventArgs e)
        {
            btn_preview.Enabled = false;
            btn_correction.Enabled = false;
            btn_start6h.Enabled = false;
            btn_start1h.Enabled = false;
         
            // Синхронизируем время.
            methods.SetTime();

            if (comboBox1.SelectedItem != null)
            {
                if (cycle_3h && interval_3h && num_3h)
                {
                    (sender as Button).Enabled = false;// блокировка нажатия кнопки
                    btn_show3hphoto.Visible = false;
                    countIteration_3h = 180 / data.cycle_3hour;
                    
                    startSeconds_3h = TimeSpan.Parse($"{DateTime.Now.ToLongTimeString()}").TotalSeconds;
                    // Окончание съемки за один период цикла
                    stopSeconds_3h = startSeconds_3h + (data.interval_3hour * (data.number_3hour - 1));
                    SaveScreen(pathTamp3, path3h, out double timeSave);

                    // Делаем снимки с установленным инервалом.
                    SetTimerInterval_3h(1000 * data.interval_3hour * 1, stopSeconds_3h);

                    ++finishMeasuring_3h;
                }
                else { MessageBox.Show("Введите параметры !"); }
            }
            else
            { MessageBox.Show("Выберите камеру!"); }

        }

        double startSeconds_6h;
        double stopSeconds_6h;
        int finishMeasuring_6h = 0;
        int countIteration_6h = 0;
        private void btn_start6h_Click(object sender, EventArgs e)
        {
            btn_preview.Enabled = false;
            btn_correction.Enabled = false;
            btn_start1h.Enabled = false;
            btn_start3h.Enabled = false;
          
            // Синхронизируем время.
            methods.SetTime();

            if (comboBox1.SelectedItem != null)
            {
                if (cycle_6h && interval_6h && num_6h)
                {
                    (sender as Button).Enabled = false;// блокировка нажатия кнопки
                    btn_6hphpto.Visible = false;
                    countIteration_6h = 360 / data.cycle_6hour;
                
                    startSeconds_6h = TimeSpan.Parse($"{DateTime.Now.ToLongTimeString()}").TotalSeconds;
                    // Окончание съемки за один период цикла
                    stopSeconds_6h = startSeconds_6h + (data.interval_6hour * (data.number_6hour - 1));
                    SaveScreen(pathTamp6, path6h, out double timeSave);

                    // Делаем снимки с установленным инервалом.
                    SetTimerInterval_6h(1000 * data.interval_6hour * 1, stopSeconds_6h);
                   
                    ++finishMeasuring_6h;
                }
                else { MessageBox.Show("Введите параметры !"); }
            }
            else
            { MessageBox.Show("Выберите камеру!"); }

        }

        private void btn_show1hphoto_Click(object sender, EventArgs e)
        {
            // Передаём адрес с сохраннеными фотографиями из 1 часового измерения.
            string nameCycle = " 1 часовые измерения";
            SeePhoto showForm = new SeePhoto(path1h, nameCycle);
            showForm.Show();
        }

        private void btn_show3hphoto_Click(object sender, EventArgs e)
        {
            // Передаём адрес с сохраннеными фотографиями из 3 часового измерения. 
            string nameCycle = " 3 часовые измерения";
            SeePhoto showForm = new SeePhoto(path3h, nameCycle);
            showForm.Show();
        }

        private void btn_6hphpto_Click(object sender, EventArgs e)
        {
            // Передаём адрес с сохраннеными фотографиями из 6 часового измерения. 
            string nameCycle = " 6 часовые измерения";
            SeePhoto showForm = new SeePhoto(path6h, nameCycle);
            showForm.Show();
        }

        private void WebCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Удалить папки с шаблонами . 
            if (Directory.Exists(pathPreview))
            {
                Directory.Delete(pathPreview, true);
            }
            if (Directory.Exists(pathTamp1))
            {
                Directory.Delete(pathTamp1, true);
            }
            if (Directory.Exists(pathTamp3))
            {
                Directory.Delete(pathTamp3, true);
            }
            if (Directory.Exists(pathTamp6))
            {
                Directory.Delete(pathTamp6, true);
            }

            pictureBox1.Controls.Clear();
        }
    }
}
