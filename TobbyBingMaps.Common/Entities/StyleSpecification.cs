using System;
using System.ComponentModel;
using System.Text;

namespace TobbyBingMaps.Common.Entities
{
    public delegate void StyleChangedEventHandler(object sender, StyleChangedEventArgs e);

    public class StyleSpecification : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public StyleSpecification()
        {
            id = "default";
            iconScale = 1.0;
            lineColour = "FFFF0000";
            lineWidth = 2.0;
            polyFillColour = "CC0000FF";
            polygonLineColour = "FFFF0000";
            polygonLineWidth = 2.0;
            showFill = true;
            showLine = true;
            frames = 0;
            frameInterval = -1;
            width = -1;
            height = -1;
        }

        private string id;
        [DefaultValue("default")]
        public string ID
        {
            get { return id; }
            set
            {
                if (id == value) return;
                var oldStyle = Clone();
                id = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("ID");
            }
        }

        private double? iconScale;
        [DefaultValue(1.0)]
        public double? IconScale
        {
            get { return iconScale; }
            set
            {
                if (iconScale == value) return;
                var oldStyle = Clone();
                iconScale = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("IconScale");
            }
        }

        private string iconUrl;
        [DefaultValue("")]
        public string IconURL
        {
            get { return iconUrl; }
            set
            {
                if (iconUrl == value) return;
                var oldStyle = Clone();
                iconUrl = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("IconURL");
            }
        }

        private double iconOffsetX;
        [DefaultValue(0.0)]
        public double IconOffsetX
        {
            get { return iconOffsetX; }
            set
            {
                if (iconOffsetX == value) return;
                var oldStyle = Clone();
                iconOffsetX = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("IconOffsetX");
            }
        }

        private double iconOffsetY;
        [DefaultValue(0.0)]
        public double IconOffsetY
        {
            get { return iconOffsetY; }
            set
            {
                if (iconOffsetY == value) return;
                var oldStyle = Clone();
                iconOffsetY = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("IconOffsetY");
            }
        }

        private string lineColour;
        [DefaultValue("FFFF0000")]
        public string LineColour
        {
            get { return lineColour; }
            set
            {
                if (lineColour == value) return;
                var oldStyle = Clone();
                lineColour = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("LineColour");
            }
        }

        private double lineWidth;
        [DefaultValue(2.0)]
        public double LineWidth
        {
            get { return lineWidth; }
            set
            {
                if (lineWidth == value) return;
                var oldStyle = Clone();
                lineWidth = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("LineWidth");
            }
        }

        private string polyFillColour;
        [DefaultValue("CC0000FF")]
        public string PolyFillColour
        {
            get { return polyFillColour; }
            set
            {
                if (polyFillColour == value) return;
                var oldStyle = Clone();
                polyFillColour = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("PolyFillColour");
            }
        }

        private string polygonLineColour;
        [DefaultValue("FFFF0000")]
        public string PolygonLineColour
        {
            get { return polygonLineColour; }
            set
            {
                if (polygonLineColour == value) return;
                var oldStyle = Clone();
                polygonLineColour = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("PolygonLineColour");
            }
        }

        private double polygonLineWidth;
        [DefaultValue(2.0)]
        public double PolygonLineWidth
        {
            get { return polygonLineWidth; }
            set
            {
                if (polygonLineWidth == value) return;
                var oldStyle = Clone();
                polygonLineWidth = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("PolygonLineWidth");
            }
        }

        private bool showFill;
        [DefaultValue(true)]
        public bool ShowFill
        {
            get { return showFill; }
            set
            {
                if (showFill == value) return;
                var oldStyle = Clone();
                showFill = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("ShowFill");
            }
        }

        private bool showLine;
        [DefaultValue(true)]
        public bool ShowLine
        {
            get { return showLine; }
            set
            {
                if (showLine == value) return;
                var oldStyle = Clone();
                showLine = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("ShowLine");
            }
        }

        //extras, are their KML equivilants?
        private int frames;
        [DefaultValue(0)]
        public int Frames
        {
            get { return frames; }
            set
            {
                if (frames == value) return;
                var oldStyle = Clone();
                frames = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("Frames");
            }
        }

        private int frameInterval;
        [DefaultValue(-1)]
        public int FrameInterval
        {
            get { return frameInterval; }
            set
            {
                if (frameInterval == value) return;
                var oldStyle = Clone();
                frameInterval = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("FrameInterval");
            }
        }

        private int width;
        [DefaultValue(-1)]
        public int Width
        {
            get { return width; }
            set
            {
                if (width == value) return;
                var oldStyle = Clone();
                width = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("Width");
            }
        }

        private int height;
        [DefaultValue(-1)]
        public int Height
        {
            get { return height; }
            set
            {
                if (height == value) return;
                var oldStyle = Clone();
                height = value;
                OnStyleChanged(new StyleChangedEventArgs(oldStyle, this));
                OnPropertyChanged("Height");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region StyleChanged
        public event StyleChangedEventHandler StyleChanged;

        protected virtual void OnStyleChanged(StyleChangedEventArgs e)
        {
            if (StyleChanged != null)
            {
                StyleChanged(this, e);
            }
        }
        #endregion

        public StyleSpecification Clone()
        {
            return new StyleSpecification
            {
                ID = id,
                IconScale = iconScale,
                IconURL = iconUrl,
                LineColour = lineColour,
                LineWidth = lineWidth,
                PolyFillColour = polyFillColour,
                ShowFill = showFill,
                ShowLine = showLine,
                Frames = frames,
                FrameInterval = frameInterval,
                Width = width,
                Height = height,
            };
        }

        public string ToXML()
        {
            var sb = new StringBuilder();
            sb.Append("<Style id=\"");
            sb.Append(ID);
            sb.Append("\"><IconStyle><scale>");
            sb.Append(IconScale);
            sb.Append("</scale><Icon><href>");
            sb.Append(IconURL);
            sb.Append("</href></Icon></IconStyle><LineStyle><color>");
            sb.Append(LineColour);
            sb.Append("</color><width>");
            sb.Append(LineWidth);
            sb.Append("</width></LineStyle><PolyStyle><color>");
            sb.Append(PolyFillColour);
            sb.Append("</color><fill>");
            sb.Append(ShowFill ? "1" : "0");
            sb.Append("</fill><outline>");
            sb.Append(ShowLine ? "1" : "0");
            sb.Append("</outline><LineStyle><color>");
            sb.Append(PolygonLineColour);
            sb.Append("</color><width>");
            sb.Append(PolygonLineWidth);
            sb.Append("</width></LineStyle></PolyStyle></Style>");
            return sb.ToString();
        }
    }

    public class StyleChangedEventArgs : EventArgs
    {
        public StyleSpecification oldStyle;
        public StyleSpecification newStyle;

        public StyleChangedEventArgs(StyleSpecification oldStyle, StyleSpecification newStyle)
        {
            this.oldStyle = oldStyle;
            this.newStyle = newStyle;
        }
    }
}
