using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timekeeper
{
    public class DataMeasurement
    {
        public int cycle_1hour { get; set; }
        public int number_1hour { get; set; }
        public int interval_1hour { get; set; }

        public int cycle_3hour { get; set; }
        public int number_3hour { get; set; }
        public int interval_3hour { get; set; }

        public int cycle_6hour { get; set; }
        public int number_6hour { get; set; }
        public int interval_6hour { get; set; }

        public int millisecondСorrection { get; set; }

        public DataMeasurement() { }

        public DataMeasurement(int c_1hour,int n_1hour,int i_1hour,
          int c_3hour,int n_3hour,int i_3hour,
          int c_6hour,int n_6hour,int i_6hour,int millCor)
        {
            cycle_1hour = c_1hour;
            number_1hour = n_1hour;
            interval_1hour = i_1hour;
            cycle_3hour = c_3hour;
            number_3hour = n_3hour;
            interval_3hour = i_3hour;
            cycle_6hour = c_6hour;
            number_6hour = n_6hour;
            interval_6hour = i_6hour;
            millisecondСorrection = millCor;
        }
  
    }
}
