using System;

namespace Timekeeper
{
    public class Methods
    {
        public void SetTime()
        {
            try
            { // Синхронизация Windows с  сервером времени.
                string command = "w32tm /resync";
                System.Diagnostics.Process.Start("cmd.exe", "/C" + command);
            }
            catch (Exception e)
            {
                Console.WriteLine($" ERROR_TIME = {e.Message}  ");
            }
        }
        public void ShowTime(int millisecond,out string timeStr)
        {
            DateTime time = DateTime.Now;
            DateTime timePlus = time.AddMilliseconds(millisecond);
            
            int h = timePlus.Hour;
            int m = timePlus.Minute;
            int s = timePlus.Second;
            int ms = timePlus.Millisecond;

            timeStr = "";

            if (h < 10) { timeStr += "0" + h; }
            else
            { timeStr += h; }

            timeStr += ":";

            if (m < 10) { timeStr += "0" + m; }
            else
            { timeStr += m; }

            timeStr += ":";

            if (s < 10) { timeStr += "0" + s; }
            else
            { timeStr += s; }
            timeStr += ".";
            
            if (ms < 10) { timeStr += "00" + ms; }
            else
            {
                if (ms < 100 && ms > 10) { timeStr += "0" + ms; }
                else { timeStr += ms; }
            }
        }
    }
}
