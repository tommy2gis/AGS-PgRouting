
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TobbyBingMaps.Common;
using TobbyBingMaps.Common.Commanding;
using TobbyBingMaps.Common.Entities;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace TobbyBingMaps.MapGeometry
{
    [TemplatePart(Name = PART_ScaleTransform, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = PART_TranslateTransform, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = PART_Image, Type = typeof(Image))]
    [TemplatePart(Name = PART_LabelScaleTransform, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = PART_Label, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_LabelContainer, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_Balloon, Type = typeof(Grid))]
    [TemplatePart(Name = PART_BalloonContent, Type = typeof(Panel))]
    [TemplatePart(Name = PART_BalloonScaleTransform, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = PART_BallonTranslateTransform, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = PART_ShowBalloon, Type = typeof(Storyboard))]
    [TemplatePart(Name = PART_Animate, Type = typeof(Storyboard))]
    public class EnhancedMapPoint : Control, IDisposable, IScalable, IClusterable
    {
        private const string PART_Image = "PART_Image";
        private const string PART_ScaleTransform = "PART_ScaleTransform";
        private const string PART_TranslateTransform = "PART_TranslateTransform";
        private const string PART_Animate = "PART_Animate";
        private const string PART_BallonTranslateTransform = "PART_BallonTranslateTransform";
        private const string PART_Balloon = "PART_Balloon";
        private const string PART_BalloonScaleTransform = "PART_BalloonScaleTransform";
        private const string PART_BalloonContent = "PART_BalloonContent";
        private const string PART_Label = "PART_Label";
        private const string PART_LabelContainer = "PART_LabelContainer";
        private const string PART_LabelScaleTransform = "PART_LabelScaleTransform";
        private const string PART_ShowBalloon = "PART_ShowBalloon";

        private const int mouseDelayMS = 400;
        private const double balloonMinScale = 0.75;
        private const double labelMinScale = 0.5;
        private const int zIndexInternal = 10;

        private Storyboard animateStoryboard;

        private Grid balloonContainer;
        private Panel balloonContent;
        private ScaleTransform balloonScaleTransform;
        private Storyboard balloonShowStoryboard;
        private TranslateTransform balloonTranslateTransform;

        private string label;
        private Canvas labelContainer;
        private TextBlock labelControl;
        private Visibility labelVisibility;
        private ScaleTransform labelScaleTransform;

        private int prevZIndex;
        private DispatcherTimer dt;

        private bool isMouseOver;
        private Image imageControl;
        private double scale;
        private ScaleTransform scaleTransform;
        private TranslateTransform translateTransform;


        private readonly BackgroundWorker projectionWorker = new BackgroundWorker();
        public event EventHandler ProjectionComplete;

        private readonly MapCore mapInstance;

        public EnhancedMapPoint(Location location, MapCore map)
        {
            Offset = new Point(0, 0);
            ClusteredElements = new List<IClusterable>();
            ProjectedPoints = new Dictionary<int, Point>();
            projectionWorker.WorkerSupportsCancellation = true;
            projectionWorker.DoWork += projectionWorkerDoWork;
            projectionWorker.RunWorkerCompleted += projectionWorkerOnCompleted;

            mapInstance = map;
            Location = location;
            DefaultStyleKey = typeof(EnhancedMapPoint);

            MouseLeftButtonDown += EnhancedMapPoint_MouseLeftButtonDown;
            MouseLeave += EnhancedMapPoint_MouseLeave;
            MouseEnter += EnhancedMapPoint_MouseEnter;
        }

        public string ItemID { get; set; }

        void EnhancedMapPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (balloonContainer.Visibility == Visibility.Collapsed)
            {
                ShowBalloon();
            }
        }

        public void ShowBalloon()
        {
            if (dt != null)
            {
                dt.Stop();
                dt.Tick -= dt_Tick;
                dt = null;
            }
            if (EnableBalloon && balloonContainer != null && balloonContainer.Visibility != Visibility.Visible)
            {
                Commands.ClosePopupCommand.Execute();
                Commands.ClosePopupCommand.Executed += ClosePopupCommand_Executed;
                var layer = (EnhancedMapLayer)Parent;
                if (layer != null)
                {
                    //set to top z-order
                    prevZIndex = layer.ZIndex;
                    layer.ZIndex = 1000;
                    SetValue(Canvas.ZIndexProperty, 1000);

                    applyTranslations();

                    balloonShowStoryboard.Begin();
                    balloonContainer.Visibility = Visibility.Visible;
                    OnBalloon(new BalloonEventArgs { LayerID = layer.ID, ItemID = ItemID });
                }
            }
        }

        private void ClosePopupCommand_Executed(object sender, ExecutedEventArgs e)
        {
            HideBalloon();
        }

        public void HideBalloon()
        {
            if (dt != null)
            {
                dt.Stop();
                dt.Tick -= dt_Tick;
                dt = null;
            }
            if (balloonContainer != null && balloonContainer.Visibility == Visibility.Visible)
            {
                Commands.ClosePopupCommand.Executed -= ClosePopupCommand_Executed;
                var layer = (EnhancedMapLayer)Parent;
                if (layer != null)
                {
                    layer.ZIndex = prevZIndex;
                }
                SetValue(Canvas.ZIndexProperty, zIndexInternal);
                balloonContainer.Visibility = Visibility.Collapsed;
                balloonShowStoryboard.Stop();
            }
        }

        private StyleSpecification geometryStyle;
        public StyleSpecification GeometryStyle
        {
            get { return geometryStyle; }
            set
            {
                geometryStyle = value;
                applyStyle();
            }
        }

        public Point Offset { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            MouseLeftButtonDown -= EnhancedMapPoint_MouseLeftButtonDown;
            MouseLeave -= EnhancedMapPoint_MouseLeave;
            MouseEnter -= EnhancedMapPoint_MouseEnter;

            if (balloonContent != null)
            {
                balloonContent.SizeChanged -= balloonContent_SizeChanged;
            }
            if (balloonContainer != null)
            {
                balloonContainer.MouseEnter -= EnhancedMapPoint_MouseEnter;
                balloonContainer.MouseLeave -= EnhancedMapPoint_MouseLeave;
            }

            if (labelContainer != null)
            {
                labelContainer.MouseEnter -= EnhancedMapPoint_MouseEnter;
                labelContainer.MouseLeave -= EnhancedMapPoint_MouseLeave;
            }

            if (animateStoryboard != null)
            {
                animateStoryboard.Stop();
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            scaleTransform = (ScaleTransform)GetTemplateChild(PART_ScaleTransform);
            translateTransform = (TranslateTransform)GetTemplateChild(PART_TranslateTransform);
            imageControl = (Image)GetTemplateChild(PART_Image);
            labelScaleTransform = (ScaleTransform)GetTemplateChild(PART_LabelScaleTransform);
            labelControl = (TextBlock)GetTemplateChild(PART_Label);
            labelContainer = (Canvas)GetTemplateChild(PART_LabelContainer);
            balloonContainer = (Grid)GetTemplateChild(PART_Balloon);
            balloonScaleTransform = (ScaleTransform)GetTemplateChild(PART_BalloonScaleTransform);
            balloonTranslateTransform = (TranslateTransform)GetTemplateChild(PART_BallonTranslateTransform);
            balloonShowStoryboard = (Storyboard)GetTemplateChild(PART_ShowBalloon);
            animateStoryboard = (Storyboard)GetTemplateChild(PART_Animate);
            balloonContent = (Panel)GetTemplateChild(PART_BalloonContent);

            base.OnApplyTemplate();

            applyStyle();

            //balloon content size
            if (balloonContent != null)
            {
                balloonContent.SizeChanged += balloonContent_SizeChanged;
            }
            if (balloonContainer != null)
            {
                balloonContainer.MouseEnter += EnhancedMapPoint_MouseEnter;
                balloonContainer.MouseLeave += EnhancedMapPoint_MouseLeave;
            }

            //reset properties
            if (labelControl != null) labelControl.Text = label;
            if (labelContainer != null)
            {
                labelContainer.Visibility = Visibility == Visibility.Visible ? LabelVisibility : Visibility.Collapsed;
                labelContainer.MouseEnter += EnhancedMapPoint_MouseEnter;
                labelContainer.MouseLeave += EnhancedMapPoint_MouseLeave;
            }

            //kick off animation if found
            if (animateStoryboard != null)
            {
                animateStoryboard.Begin();
            }

            //ensure within the layer that points are above shapes (polygons / polylines)
            SetValue(Canvas.ZIndexProperty, zIndexInternal);
        }

        private void balloonContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (balloonContainer.Visibility == Visibility.Visible)
            {
                applyTranslations();
            }
        }

        private void applyTranslations()
        {
            if (translateTransform != null)
            {
                translateTransform.X = Offset.X * Scale;
                translateTransform.Y = Offset.Y * Scale;
            }
            if (balloonTranslateTransform != null)
            {

                //scale factor to take into account
                var scaleFactor = balloonMinScale + (balloonMinScale * Scale);

                double x = -10;
                double y = -10;
                //avoid balloon going outside the map bounds
                Point point = mapInstance.LocationToViewportPoint((Location)GetValue(MapLayer.PositionProperty));
                if (point.X > mapInstance.ViewportSize.Width / 2)
                {
                    x = (-balloonContent.ActualWidth) + 10;
                }
                if (point.Y > mapInstance.ViewportSize.Height / 2)
                {
                    y = (-balloonContent.ActualHeight) + 10;
                }
                balloonTranslateTransform.X = x * scaleFactor;
                balloonTranslateTransform.Y = y * scaleFactor;
            }
        }

        private void applyStyle()
        {
            applyScale(Scale);

            if (!string.IsNullOrEmpty(GeometryStyle.IconURL) && imageControl != null)
            {
                imageControl.Source = new BitmapImage(new Uri(GeometryStyle.IconURL, UriKind.Absolute));
            }
            Offset = new Point(GeometryStyle.IconOffsetX, GeometryStyle.IconOffsetY);
        }

        private Location location;

        public Location Location
        {
            get { return location; }
            set
            {
                location = value;
                //update projected coordinates.
                startProjectionWorker();
            }
        }

        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                applyScale(value);
            }
        }

        void EnhancedMapPoint_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isMouseOver)
            {
                isMouseOver = false;
                //only if the timer is not already running
                if (EnableBalloon && dt == null)
                {
                    dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, mouseDelayMS) };
                    dt.Tick += dt_Tick;
                    dt.Start();
                }
            }
        }

        void EnhancedMapPoint_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isMouseOver)
            {
                isMouseOver = true;
                //only if the timer is not already running
                if (EnableBalloon && dt == null)
                {
                    dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, mouseDelayMS) };
                    dt.Tick += dt_Tick;
                    dt.Start();
                }
            }
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            //if we are still mouse over.
            if (isMouseOver)
            {
                ShowBalloon();
            }
            else
            {
                HideBalloon();
            }
        }

        //allows you to set the balloon content on demand
        public UIElementCollection BalloonData
        {
            get
            {
                if (balloonContent != null)
                {
                    return balloonContent.Children;
                }
                return null;
            }
        }

        public Visibility LabelVisibility
        {
            get { return labelVisibility; }
            set
            {
                labelVisibility = value;
                if (labelContainer != null && Visibility == Visibility.Visible)
                {
                    labelContainer.Visibility = labelVisibility;
                }
            }
        }

        public string Label
        {
            get { return label; }
            set
            {
                label = value;
                if (labelControl != null)
                {
                    labelControl.Text = label;
                }
            }
        }

        public bool EnableBalloon { get; set; }
        public event BalloonEventHandler Balloon;


        protected virtual void OnBalloon(BalloonEventArgs e)
        {
            if (Balloon != null)
                Balloon(this, e);
        }

        private void applyScale(double value)
        {
            //point
            if (scaleTransform != null)
            {
                var scaleFactor = GeometryStyle.IconScale.GetValueOrDefault();
                if (scaleFactor == 0)
                {
                    scaleFactor = 1;
                }
                scaleTransform.ScaleX = value * scaleFactor;
                scaleTransform.ScaleY = value * scaleFactor;
            }

            //TODO: can these be ignored if not visible / used? (label + balloon)
            //label, scales to 50%
            if (labelScaleTransform != null)
            {
                labelScaleTransform.ScaleX = labelMinScale + (labelMinScale * value);
                labelScaleTransform.ScaleY = labelMinScale + (labelMinScale * value);
            }
            //balloon, scales to 75%
            if (balloonScaleTransform != null)
            {
                balloonScaleTransform.ScaleX = balloonMinScale + (balloonMinScale * value);
                balloonScaleTransform.ScaleY = balloonMinScale + (balloonMinScale * value);
            }
            applyTranslations();
        }

        private void projectionWorkerDoWork(object sender, DoWorkEventArgs args)
        {
            var worker = sender as BackgroundWorker;
            for (int i = 21; i > 0; i--)
            {

                if (worker != null && worker.CancellationPending)
                {
                    args.Cancel = true;
                    break;
                }
                int pixelX;
                int pixelY;
                TileSystem.LatLongToPixelXY(Location.Latitude, Location.Longitude, i, out pixelX, out pixelY);
                ProjectedPoints.Add(i, new Point(pixelX, pixelY));
            }

            Initalized = true;
        }

        private void startProjectionWorker()
        {
            //only be running once instance, if already running we cancel and wait.
            if (projectionWorker.WorkerSupportsCancellation && projectionWorker.IsBusy)
            {
                projectionWorker.CancelAsync();
            }
            else
            {
                projectionWorker.RunWorkerAsync();
            }
        }

        private void projectionWorkerOnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                startProjectionWorker();
            }
            else if (ProjectionComplete != null)
            {
                ProjectionComplete(this, new EventArgs());
            }
        }

        public List<IClusterable> ClusteredElements { get; set; }
        public Dictionary<int, Point> ProjectedPoints { get; set; }

        //TODO: support clustered view
        public bool IsClustered { get; set; }
        public bool Initalized { get; set; }
        public bool IsCluster { get; set; }

    }
}
