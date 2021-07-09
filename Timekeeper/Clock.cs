using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timekeeper
{
    class Clock
    {
        public string shotName { get; set; }
        public string fullName { get; set; }
        public string strId { get; set; }
        public string customerShotName { get; set; }
        public string customerFullName { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
        public double pressure { get; set; }

        public Clock() { }

        public Clock(string shotname, string fullname, string strid, string custshotname,
            string custfullname, double temper, double hum, double press)
        {
            shotName = shotname;
            fullName = fullname;
            strId = strid;
            customerShotName = custshotname;
            customerFullName = custfullname;
            temperature = temper;
            humidity = hum;
            pressure = press;

        }

        public override string ToString()
        {
            return ($" Заказчик_коротко : {customerShotName}\n" +
                $" Заказчик_полное название : {customerFullName}\n" +
                $" Название изделия : {shotName}\n" +
                $" Полное описание изделия: {fullName}\n" +
                $" Серийный номер изделия : {strId}\n" +
                $" Температура: {temperature}\n" +
                $" Влажность : {humidity}\n" +
                $" Давление : { pressure}\n\n");


        }
    }
}
