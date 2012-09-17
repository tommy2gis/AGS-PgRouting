using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Shapes;
using TobbyBingMaps.Common.Converters;
using GisSharpBlog.NetTopologySuite.Simplify;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace TobbyBingMaps.MapGeometry
{
    public abstract class EnhancedMapShapeBase : MapShapeBase, IDisposable, IScalable, IReduceable, IClipable
    {
        private double scale;
        private double strokeThickness;
        private readonly BackgroundWorker reductionWorker = new BackgroundWorker();

        public event EventHandler ReductionComplete;

        protected EnhancedMapShapeBase(Shape shape)
            : base(shape)
        {
            Scale = 1;
            Reduced = new Dictionary<int, LocationCollection>();
            reductionWorker.WorkerSupportsCancellation = true;
            reductionWorker.DoWork += reductionWorkerDoWork;
            reductionWorker.RunWorkerCompleted += reductionWorkerOnCompleted;
        }

        private void reductionWorkerDoWork(object sender, DoWorkEventArgs args)
        {
            var worker = sender as BackgroundWorker;
            var lineSimplify = new DouglasPeuckerLineSimplifier(CoordinateConvertor.LocationCollectionToCoordinates(Original));
            for (int i = 21; i > 0; i--)
            {

                if (worker != null && worker.CancellationPending)
                {
                    args.Cancel = true;
                    break;
                }

                var simplificationDistance = (1 / (Math.Pow(2, i - 1)));
                lineSimplify.DistanceTolerance = simplificationDistance;
                Reduced.Add(i, CoordinateConvertor.CoordinatesToLocationCollection(lineSimplify.Simplify()));
            }

            Initalized = true;
        }

        private void startReductionWorker()
        {
            //only be running once instance, if already running we cancel and wait.
            if (reductionWorker.WorkerSupportsCancellation && reductionWorker.IsBusy)
            {
                reductionWorker.CancelAsync();
            }
            else
            {
                reductionWorker.RunWorkerAsync();
            }
        }

        private void reductionWorkerOnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                startReductionWorker();
            }
            else if (ReductionComplete != null)
            {
                ReductionComplete(this, new EventArgs());
            }
        }

        public new LocationCollection Locations
        {
            get { return base.Locations; }
            set
            {
                Original = value;
                MBR = new LocationRect(value);
                base.Locations = new LocationCollection { new Location(0, 0) };
                Initalized = false;
                Reduced.Clear();
                startReductionWorker();
            }
        }

        public new double StrokeThickness
        {
            get { return strokeThickness; }
            set
            {
                strokeThickness = value;
                updateThickness();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            reductionWorker.RunWorkerCompleted -= reductionWorkerOnCompleted;
            if (reductionWorker.WorkerSupportsCancellation && reductionWorker.IsBusy)
            {
                reductionWorker.CancelAsync();
            }
            Reduced.Clear();
            Reduced = null;
        }

        #endregion

        #region IScalable Members

        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                updateThickness();
            }
        }

        #endregion

        private void updateThickness()
        {
            base.StrokeThickness = StrokeThickness * Scale;
        }

        public LocationCollection Original { get; set; }
        public Dictionary<int, LocationCollection> Reduced { get; set; }
        public int CurrentDisplayed { get; set; }
        public bool Initalized { get; set; }
        public LocationRect MBR { get; set; }
    }
}
