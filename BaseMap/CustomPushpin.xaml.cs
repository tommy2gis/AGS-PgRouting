using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;
using System.ComponentModel;
using BaseMap.Class;

namespace BaseMap
{
    public partial class CustomPushpin : UserControl
    {
        public CustomPushpin()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CustomPushpin_Loaded);
        }

        void CustomPushpin_Loaded(object sender, RoutedEventArgs e)
        {
           
        }


          private string _Id;
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");
            }
        }



        private string _Describe;
        public string Describe
        {
            get
            {
                return _Describe;
            }
            set
            {
                _Describe = value;
                NotifyPropertyChanged("Describe");
            }
        }


        private string _carName;
        public string carName
        {
            get
            {
                return _carName;
            }
            set
            {
                _carName = value;
                NotifyPropertyChanged("carName");
            }
        }

        private string _carStatus;
        public string carStatus
        {
            get
            {
                return _carStatus;
            }
            set
            {
                _carStatus = value;
                NotifyPropertyChanged("carStatus");
            }
        }


        private DateTime _GpsDate;
        public DateTime GpsDate
        {
            get
            {
                return _GpsDate;
            }
            set
            {
                _GpsDate = value;
                NotifyPropertyChanged("GpsDate");
            }
        }

    

        #region Public Properties

        /// <summary>
        /// Specification to create the custom pushpin
        /// </summary>
        private CustomIconSpecification _iconSpec;
        public CustomIconSpecification IconSpecification
        {
            get
            {
                return _iconSpec;
            }
            set
            {
                _iconSpec = value;

                PushpinBase.Children.Clear();

                if (value.Icon != null)
                {
                    PushpinBase.Children.Add(value.Icon);
                }
                else if (value.IconUri != null)
                {
                    PushpinBase.Children.Add(PushpinTools.CreateImagePushpin(value.IconUri, value.Width, value.Height, value.RotateTr, value.IconOffset, value.PlaneProjection));
                }
                RotateTransform.Angle = value.RotateTr;
                if (!string.IsNullOrEmpty(value.TextContent))
                {
                    lblCarName.Children.Clear();
                    TextBlock text = new TextBlock();
                    text.Width = value.Width;
                    text.Text = value.TextContent;
                    text.TextAlignment = TextAlignment.Center;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.Foreground = value.TextColor;
                    text.FontSize = value.FontSize;
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Margin = new Thickness(value.TextOffet.X, value.TextOffet.Y, 0, 0);
                    if (text.Width > value.Width)
                        lblCarName.Width = value.Width;
                    lblCarName.Children.Add(text);
                }
                NotifyPropertyChanged("IconSpecification");
            }
        }





        /// <summary>
        /// The Location in which to display the pushpin
        /// </summary>
        private Location _Location;
        public Location Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                NotifyPropertyChanged("Location");
            }
        }


        private bool _isVisiable;
        public bool isVisiable
        {
            get
            {
                return _isVisiable;
            }
            set
            {
                _isVisiable = value;
                NotifyPropertyChanged("isVisiable");
            }
        }


        private Map _map;
        public Map MapInstance
        {
            get
            {
                return _map;
            }
            set
            {
                _map = value;
                _map.ViewChangeOnFrame += MapViewChangeOnFrame;
                ApplyPowerLawScaling(_map.ZoomLevel);
                NotifyPropertyChanged("MapInstance");
            }
        }



        void MapViewChangeOnFrame(object sender, MapEventArgs e)
        {
            ApplyPowerLawScaling(MapInstance.ZoomLevel);
        }

        private void ApplyPowerLawScaling(double currentZoomLevel)
        {
            double scale = Math.Pow(0.05 * (currentZoomLevel + 1), 2) + 0.01;
            if (scale > 1) scale = 1;
            if (scale < 0.125) scale = 0.125;

            PinScaleTransform.ScaleX = scale;
            PinScaleTransform.ScaleY = scale;
        }



        /// <summary>
        /// Optional metadata that can be used to store data for a pushpin
        /// </summary>
        private object _MetaData;
        public object MetaData
        {
            get
            {
                return _MetaData;
            }
            set
            {
                _MetaData = value;
                NotifyPropertyChanged("MetaData");
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
    
}
