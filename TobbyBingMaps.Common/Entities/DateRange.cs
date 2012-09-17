using System;
using System.ComponentModel;

namespace TobbyBingMaps.Common.Entities
{
    public class DateRange : INotifyPropertyChanged
    {
        private DateTime valueLow;
        public DateTime ValueLow
        {
            get { return valueLow; }
            set
            {
                valueLow = value;
                UpdateProperty("ValueLow");
            }
        }

        private DateTime valueHigh;
        public DateTime ValueHigh
        {
            get { return valueHigh; }
            set
            {
                valueHigh = value;
                UpdateProperty("ValueHigh");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void UpdateProperty(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public DateRange Clone()
        {
            return new DateRange { ValueHigh = ValueHigh, ValueLow = ValueLow };
        }
    }
}
