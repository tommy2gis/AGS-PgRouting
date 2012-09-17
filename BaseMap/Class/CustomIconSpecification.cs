using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Maps.MapControl;

namespace BaseMap.Class
{

    public static class CustomPushPinExtensions
    {
        /// <summary>
        /// Allows a custom pushpin to be added to a MapLayer
        /// </summary>
        /// <param name="layer">MapLayer to add custom pushpin to</param>
        /// <param name="pushpin">Custom Pushpin to add to layer</param>
        public static void AddChild(this MapLayer layer, CustomPushpin pushpin)
        {
            layer.AddChild(pushpin, pushpin.Location);
        }
        public static void RemoveChild(this MapLayer layer, CustomPushpin pushpin)
        {
            layer.Children.Remove(pushpin);
        }

    }
    public class CustomIconSpecification : INotifyPropertyChanged
    {
        #region Private Variables

        //Default properties
        private double width = 30;
        private double height = 30;
        private int rotateTr = 0;
        private Brush textColor = new SolidColorBrush(Colors.Black);
        private double fontSize = 10;
        private Point iconOffset = new Point(0, 0);
        private Point textOffset = new Point(0, 0);
        private PlaneProjection planeProjection = new PlaneProjection();

        public PlaneProjection PlaneProjection
        {
            get { return planeProjection; }
            set
            {
                planeProjection = value;
                NotifyPropertyChanged("PlaneProjection");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A user can use a UIElement as a pushpin icon
        /// </summary>
        private UIElement _Icon;
        public UIElement Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                _Icon = value;
                NotifyPropertyChanged("Icon");
            }
        }

        /// <summary>
        /// If a uri is specified for an icon and image will be created. This is only used if the Icon property is null.
        /// </summary>
        private Uri _IconUri;
        public Uri IconUri
        {
            get
            {
                return _IconUri;
            }
            set
            {
                _IconUri = value;
                NotifyPropertyChanged("IconUri");
            }
        }

        /// <summary>
        /// Offset used for positioning icon so that pin point is anchored to coordinate
        /// </summary>
        private Point _IconOffsett;
        public Point IconOffset
        {
            get
            {
                return _IconOffsett;
            }
            set
            {
                _IconOffsett = value;
                NotifyPropertyChanged("IconOffset");
            }
        }

        /// <summary>
        /// Height of the pushpin. This is used for positioning Text and for sizing the icon image
        /// </summary>
        private double _Height;
        public double Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
                NotifyPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Width of the pushpin. This is used for positioning Text and for sizing the icon image
        /// </summary>
        private double _Width;
        public double Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
                NotifyPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Width of the pushpin. This is used for positioning Text and for sizing the icon image
        /// </summary>
        private int _RotateTr;
        public int RotateTr
        {
            get
            {
                return _RotateTr;
            }
            set
            {
                _RotateTr = value;
                NotifyPropertyChanged("RotateTr");
            }
        }

        /// <summary>
        /// Custom text to be displayed above the pushpin. 
        /// </summary>
        private string _TextContent;
        public string TextContent
        {
            get
            {
                return _TextContent;
            }
            set
            {
                _TextContent = value;
                NotifyPropertyChanged("TextContent");
            }
        }

        /// <summary>
        /// Color of the custom Text
        /// </summary>
        private Brush _TextColor;
        public Brush TextColor
        {
            get
            {
                return _TextColor;
            }
            set
            {
                _TextColor = value;
                NotifyPropertyChanged("TextColor");
            }
        }


        /// <summary>
        /// Size of the custom Text
        /// </summary>
        private double _FontSize;
        public double FontSize
        {
            get
            {
                return _FontSize;
            }
            set
            {
                _FontSize = value;
                NotifyPropertyChanged("FontSize");
            }
        }

        /// <summary>
        /// Offset used for positioning Text so that it is centered over the right part of the pushpin
        /// </summary>
        private Point _TextOffet;
        public Point TextOffet
        {
            get
            {
                return _TextOffet;
            }
            set
            {
                _TextOffet = value;
                NotifyPropertyChanged("TextOffet");
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
