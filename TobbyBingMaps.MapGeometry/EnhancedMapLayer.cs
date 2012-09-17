using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TobbyBingMaps.Common;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;
using Location = Microsoft.Maps.MapControl.Location;
using TobbyBingMaps.Common.Entities;


namespace TobbyBingMaps.MapGeometry
{
    public class EnhancedMapLayer : MapLayer, IEnhancedLayer
    {

        private const int clusterwidth = 15; //Cluster region width, all pin within this area are clustered
        private const int clusterheight = 15; //Cluster region height, all pin within this area are clustered

        private readonly BackgroundWorker onMapMovementWorker = new BackgroundWorker();
        private readonly BackgroundWorker dataAddedWorker = new BackgroundWorker();
        private readonly List<MapLayerObject> mapLayerObjects;
        private readonly List<VectorLayerData> layerDataToProcess;
        private readonly List<BaseGeometry> mapLayerGeometries;
        private bool loaded;
        private bool isDirty;
        private int currentSnapLevel;

        private const string cDefaultStyleID = "defaultstyle";
        private DateRange dateRangeDisplay;

        private bool enableBalloon = true;
        private bool enableSelection = true;
        private double maxZoomLevel = 100;
        private double minZoomLevel = 1;
        private int zIndex;

        public EnhancedMapLayer(MapCore map)
        {
            mapLayerObjects = new List<MapLayerObject>();
            layerDataToProcess = new List<VectorLayerData>();
            mapLayerGeometries = new List<BaseGeometry>();
            MapInstance = map;

            onMapMovementWorker.WorkerSupportsCancellation = true;
            onMapMovementWorker.DoWork += OnMapMovementWorkerOnDoWork;
            onMapMovementWorker.RunWorkerCompleted += OnMapMovementWorkerOnRunWorkerCompleted;

            Loaded += EnhancedMapLayer_Loaded;

            dataAddedWorker.WorkerSupportsCancellation = true;
            dataAddedWorker.DoWork += dataAddedWorker_DoWork;
            dataAddedWorker.RunWorkerCompleted += dataAddedWorker_RunWorkerCompleted;
        }

        #region Public Properties

        public MapCore MapInstance { get; set; }

        public double MaxZoomLevel
        {
            get { return maxZoomLevel; }
            set
            {
                maxZoomLevel = value;
                startMapMovementWorker();
            }
        }

        public double MinZoomLevel
        {
            get { return minZoomLevel; }
            set
            {
                minZoomLevel = value;
                startMapMovementWorker();
            }
        }

        private bool enableScaling = true;
        public bool EnableScaling
        {
            get { return enableScaling; }
            set
            {
                //do we need to reset all elements?
                if (!value && enableScaling)
                {
                    foreach (var child in mapLayerObjects)
                    {
                        if (child.Element is IScalable)
                        {
                            ((IScalable)child.Element).Scale = 1;
                        }
                    }
                }
                enableScaling = value;
                startMapMovementWorker();
            }
        }

        private bool enableClustering = true;
        public bool EnableClustering
        {
            get { return enableClustering; }
            set
            {
                enableClustering = value;
                isDirty = true;
                startMapMovementWorker();
            }
        }

        private bool enableReduction = true;
        public bool EnableReduction
        {
            get { return enableReduction; }
            set
            {
                //do we need to reset all elements?
                if (!value && enableScaling)
                {
                    foreach (var child in mapLayerObjects)
                    {
                        if (child.Element is IReduceable && child.Element is MapShapeBase)
                        {
                            ((MapShapeBase)child.Element).Locations = ((IReduceable)child.Element).Original;
                        }
                    }
                }
                else
                {
                    isDirty = true;
                }
                enableReduction = value;
                startMapMovementWorker();
            }
        }

        public bool EnableBalloon
        {
            get { return enableBalloon; }
            set
            {
                enableBalloon = value;
                foreach (var geometry in mapLayerGeometries)
                {
                    if (geometry is InfoGeometry)
                    {
                        ((InfoGeometry)geometry).EnableBalloon = enableBalloon;
                    }
                }
            }
        }

        public bool EnableSelection
        {
            get { return enableSelection; }
            set
            {
                enableSelection = value;
                foreach (var geometry in mapLayerGeometries)
                {
                    if (geometry is InfoGeometry)
                    {
                        ((InfoGeometry)geometry).EnableSelection = enableSelection;
                    }
                }
            }
        }

        private Visibility labelVisibility;
        public Visibility LabelVisibility
        {
            get { return labelVisibility; }
            set
            {
                labelVisibility = value;
                foreach (var geometry in mapLayerGeometries)
                {
                    if (geometry is InfoGeometry)
                    {
                        ((InfoGeometry)geometry).LabelVisibility = labelVisibility;
                    }
                }
            }
        }

        public Dictionary<string, StyleSpecification> Styles { get; set; }
        public LayerDefinition LayerDefinition { get; set; }

        public DateRange DateRangeDisplay
        {
            get { return dateRangeDisplay; }
            set
            {
                dateRangeDisplay = value;
                updateObjectsDateRangeVisibility();
            }
        }

        public void Refresh()
        {
            isDirty = true;
            startMapMovementWorker();
        }

        public void HideAll()
        {
            HideAll(new List<string>());
        }

        public void HideAll(List<string> ignoreList)
        {
            //update visibility of children
            foreach (var geometry in mapLayerGeometries)
            {
                //visibility is based on ignorelist supplied
                geometry.Visibility = ignoreList.Contains(geometry.ItemID) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void ShowAll()
        {
            ShowAll(new List<string>());
        }

        public void ShowAll(List<string> ignoreList)
        {
            //update visibility of children
            foreach (var geometry in mapLayerGeometries)
            {
                //visibility is based on ignorelist supplied
                geometry.Visibility = ignoreList.Contains(geometry.ItemID) ? Visibility.Collapsed : Visibility.Visible;
            }

            //confirm daterange
            updateObjectsDateRangeVisibility();
        }

        private void updateObjectsDateRangeVisibility()
        {
            if (dateRangeDisplay != null && LayerDefinition.Temporal)
            {
                //update visibility of children
                foreach (var geometry in mapLayerGeometries)
                {
                    //either not set (null) or falls within range then we show:
                    if (geometry.TimeStamp == DateTime.MinValue || (dateRangeDisplay.ValueLow.CompareTo(geometry.TimeStamp) < 0 &&
                                                                    dateRangeDisplay.ValueHigh.CompareTo(geometry.TimeStamp) > 0))
                    {
                        geometry.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        geometry.Visibility = Visibility.Collapsed;
                    }
                }
            }
            startMapMovementWorker();
        }

        #endregion

        #region IEnhancedLayer Members

        public string ID { get; set; }

        public int ZIndex
        {
            get { return zIndex; }
            set
            {
                zIndex = value;
                if (Parent != null)
                {
                    SetValue(Canvas.ZIndexProperty, zIndex);
                }
            }
        }

        public void Dispose()
        {
            loaded = false;
            onMapMovementWorker.RunWorkerCompleted -= OnMapMovementWorkerOnRunWorkerCompleted;
            dataAddedWorker.RunWorkerCompleted -= dataAddedWorker_RunWorkerCompleted;
            MapInstance.ViewChangeOnFrame -= MapInstance_ViewChangeOnFrame;
            Clear();
        }

        #endregion

        public void Clear()
        {
            if (dataAddedWorker.WorkerSupportsCancellation && dataAddedWorker.IsBusy)
            {
                dataAddedWorker.CancelAsync();
            }
            layerDataToProcess.Clear();

            if (onMapMovementWorker.WorkerSupportsCancellation && onMapMovementWorker.IsBusy)
            {
                onMapMovementWorker.CancelAsync();
            }
            //dispose any objects.
            Children.Clear();
            foreach (var layerObject in mapLayerObjects)
            {
                
                if (layerObject is IDisposable)
                {
                    ((IDisposable)layerObject).Dispose();
                }
            }
            mapLayerObjects.Clear();
            foreach (var geometry in mapLayerGeometries)
            {
                
                geometry.Dispose();
            }
            mapLayerGeometries.Clear();
        }

        public void Add(ObservableCollection<VectorLayerData> layerData)
        {
            layerDataToProcess.AddRange(layerData);
            if (dataAddedWorker.WorkerSupportsCancellation && dataAddedWorker.IsBusy)
            {
                dataAddedWorker.CancelAsync();
            }
            else
            {
                dataAddedWorker.RunWorkerAsync();
            }
        }

        void dataAddedWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (layerDataToProcess.Count > 0)
            {
                dataAddedWorker.RunWorkerAsync();
            }
            updateObjectsDateRangeVisibility();
        }

        private void dataAddedWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            var worker = sender as BackgroundWorker;
            //going to modify the list so can't use a foreach.
            while (true)
            {
                if (layerDataToProcess.Count == 0)
                {
                    break;
                }

                VectorLayerData vectorLayerData = layerDataToProcess[0];
                if (worker != null && worker.CancellationPending)
                {
                    args.Cancel = true;
                    break;
                }

                StyleSpecification style = Styles[cDefaultStyleID];
                string styleID = (!string.IsNullOrEmpty(vectorLayerData.Style))
                                     ? vectorLayerData.Style
                                     : LayerDefinition.LayerStyleName;
                if (Styles.ContainsKey(styleID))
                {
                    style = Styles[styleID];
                }
                var wkbReader = new WKBReader(new GeometryFactory(new PrecisionModel(), 4326));
                try
                {
                    IGeometry geo = wkbReader.Read(vectorLayerData.Geo);
                    VectorLayerData data = vectorLayerData;
                    Dispatcher.BeginInvoke(delegate
                    {
                        var infogeo = new InfoGeometry(geo, style, MapInstance, this)
                        {
                            Label = data.Label,
                            LabelVisibility =
                                (LayerDefinition.LabelOn)
                                    ? Visibility.Visible
                                    : Visibility.Collapsed,
                            ItemID = data.ID,
                            EnableBalloon = EnableBalloon,
                            EnableSelection = EnableSelection,
                            TimeStamp = data.TimeStamp,
                        };
                        infogeo.Balloon += (o, e) =>
                        {
                            if (
                                ((InfoGeometry)o).BalloonData.Count ==
                                0)
                            {
                                Commands.LoadBalloonDataCommand.Execute(e, o);
                            }
                        };
                        infogeo.Selected += (o, e) => Commands.ItemSelectedCommand.Execute(o);
                        if (LayerDefinition.PermissionToEdit)
                        {
                            infogeo.Selected += (o, e) => Commands.EditVectorCommand.Execute(o);
                        }
                        mapLayerGeometries.Add(infogeo);
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                layerDataToProcess.Remove(vectorLayerData);
            }
        }

        private void EnhancedMapLayer_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parent != null)
            {
                SetValue(Canvas.ZIndexProperty, zIndex);
            }

            loaded = true;
            MapInstance.ViewChangeOnFrame += MapInstance_ViewChangeOnFrame;
            startMapMovementWorker();
        }

        private void OnMapMovementWorkerOnDoWork(object sender, DoWorkEventArgs args)
        {
            var currentZoomLevel = (double)args.Argument;
            var worker = sender as BackgroundWorker;

            //make sure we are within rendering levels or else remove everything
            if (currentZoomLevel > MaxZoomLevel || currentZoomLevel < MinZoomLevel)
            {
                //remove all items
                if (!removeItemsNotInView(worker, mapLayerObjects, true))
                {
                    args.Cancel = true;
                }
                return;
            }


            if (EnableScaling)
            {
                updateScale(currentZoomLevel);
            }

            //are we cancelled?
            if (worker != null && worker.CancellationPending)
            {
                args.Cancel = true;
                return;
            }

            //virtualize objects add / remove if in view.
            LocationRect mapview = MapInstance.BoundingRectangle;

            List<MapLayerObject> objectsInView;
            bool noChange;

            //determine what should be in view
            if (!getObjectsInView(worker, mapview, out objectsInView, out noChange))
            {
                args.Cancel = true;
                return;
            }

            //round the current zoom to the nearest full level
            var snapLevel = (int)Math.Round(currentZoomLevel, 0);

            //if nothing has changed then we can simply exit here
            if (!isDirty && noChange && snapLevel == currentSnapLevel)
            {
                return;
            }

            //something has changed must render next time we can:
            isDirty = true;
            currentSnapLevel = snapLevel;

            //remove items no longer in view
            if (!removeItemsNotInView(worker, mapLayerObjects, false))
            {
                args.Cancel = true;
                return;
            }

            //simplify complex shapes in view
            if (EnableReduction)
            {
                if (!reduceComplexObjects(worker, objectsInView, snapLevel))
                {
                    args.Cancel = true;
                    return;
                }
            }

            //cluster points in view
            if (EnableClustering)
            {
                if (!clusterObjects(worker, objectsInView, snapLevel, mapview))
                {
                    args.Cancel = true;
                    return;
                }
            }

            //add final objects to view
            if (!addObjectsInViewToUI(worker, objectsInView))
            {
                args.Cancel = true;
                return;
            }

            //update the scale again as new objects could be added.
            if (EnableScaling)
            {
                updateScale(currentZoomLevel);
            }

            isDirty = false;
        }

        private bool addObjectsInViewToUI(BackgroundWorker worker, IEnumerable<MapLayerObject> objectsInView)
        {
            foreach (var loopLayerObject in objectsInView)
            {
                //if the item isn't already on display then we add it
                if (loopLayerObject.ToBeRendered && !loopLayerObject.InUse)
                {
                    var layerObject = loopLayerObject;
                    if (layerObject.Element is MapShapeBase)
                    {
                        Dispatcher.BeginInvoke(() => Children.Add(layerObject.Element));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => AddChild(layerObject.Element, layerObject.Location, PositionOrigin.Center));
                    }
                    layerObject.InUse = true;
                }

                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }
            return true;
        }

        private bool clusterObjects(BackgroundWorker worker, IList<MapLayerObject> objectsInView, int zoomLevel, LocationRect bounds)
        {
            //step one, get list of those to be clustered and reset their cluster status
            var clusterableMapLayerObjects = new List<IClusterable>();
            foreach (var layerObject in objectsInView)
            {
                if (layerObject.Element is IClusterable)
                {
                    var clusterable = ((IClusterable)layerObject.Element);
                    clusterable.IsCluster = false;
                    clusterable.IsClustered = false;
                    clusterable.ClusteredElements.Clear();
                    layerObject.ToBeRendered = false;
                    clusterableMapLayerObjects.Add(clusterable);
                }
            }

            //quick exit if nothing to cluster
            if (clusterableMapLayerObjects.Count == 0)
            {
                return true;
            }

            //exit loop if thread cancelled
            if (worker != null && worker.CancellationPending)
            {
                return false;
            }

            //step 2 apply the clustering algorithm of your choice here. 
            //Note you want an algorithm that returns consistant results or else things shift to much

            //if < 2500 items we can use the QT clustering, slow but accurate
            if (objectsInView.Count < 2500)
            {
                if (!QTCluster(clusterableMapLayerObjects, worker, zoomLevel)) return false;
            }
            else
            {
                //else fall back to simple grid - fast but snaps to a predefined grid which looks average.
                if (!GridCluster(clusterableMapLayerObjects, worker, zoomLevel, bounds)) return false;
            }

            //step three, set clusters and singles to be visible
            foreach (var layerObject in objectsInView)
            {
                if (layerObject.Element is IClusterable)
                {
                    if (!((IClusterable)layerObject.Element).IsClustered)
                    {
                        layerObject.ToBeRendered = true;
                    }
                }
            }

            //step four remove items no longer in view
            return removeItemsNotInView(worker, objectsInView, false);

        }

        /// <summary>
        /// Quality threshold clustering, loop through set finding the object with the most items
        /// Make it a cluster and then remove it and its children and repeat until done
        /// Very compute intensive but great repeatable results
        /// </summary>
        /// <param name="clusterableMapLayerObjects">items to cluster</param>
        /// <param name="worker">thread to check in cancelled</param>
        /// <param name="zoomLevel">zoom level of map</param>
        /// <returns>Was the action cancelled?</returns>
        private bool QTCluster(List<IClusterable> clusterableMapLayerObjects, BackgroundWorker worker, int zoomLevel)
        {
            var threshold = clusterableMapLayerObjects.Count / 50;

            //get count of every combination of overlap (expensive)
            //if an item exceeds threashold we make cluster early
            foreach (var clusterableMapObject in clusterableMapLayerObjects)
            {
                if (clusterableMapObject.Initalized && !clusterableMapObject.IsCluster && !clusterableMapObject.IsClustered)
                {
                    foreach (var otherClusterableObject in clusterableMapLayerObjects)
                    {
                        if (otherClusterableObject.Initalized && !otherClusterableObject.IsCluster &&
                            !otherClusterableObject.IsClustered && clusterableMapObject != otherClusterableObject &&
                            otherClusterableObject.ProjectedPoints.ContainsKey(zoomLevel) &&
                            Math.Abs(clusterableMapObject.ProjectedPoints[zoomLevel].X -
                                     otherClusterableObject.ProjectedPoints[zoomLevel].X) < clusterwidth &&
                            Math.Abs(clusterableMapObject.ProjectedPoints[zoomLevel].Y -
                                     otherClusterableObject.ProjectedPoints[zoomLevel].Y) < clusterheight)
                        //within the same y range = cluster needed
                        {
                            //add to cluster
                            clusterableMapObject.ClusteredElements.Add(otherClusterableObject);
                        }
                    }
                    if (clusterableMapObject.ClusteredElements.Count > threshold)
                    {
                        assignCluster(clusterableMapLayerObjects, clusterableMapObject);
                    }
                }
                else
                {
                    //listen for event when it is ready
                    clusterableMapObject.ProjectionComplete += (o, e) =>
                    {
                        isDirty = true;
                        startMapMovementWorker();
                    };
                }
                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }

            //step two - loop this until end:
            bool finished = false;
            int index = 0;
            while (!finished)
            {
                //sort by number of items in cluster
                var clusterComparer = new ClusterCountComparer();
                clusterableMapLayerObjects.Sort(clusterComparer);

                //rather then loop through to next record to process can't we keep an index to start from
                for (var i = index; i < clusterableMapLayerObjects.Count; i++)
                {
                    var clusterableMapObject = clusterableMapLayerObjects[i];
                    index++;
                    finished = true;
                    if (clusterableMapObject.Initalized && !clusterableMapObject.IsCluster && !clusterableMapObject.IsClustered)
                    {
                        //quick exit if count = 0, no cluster and we're done
                        if (clusterableMapObject.ClusteredElements.Count == 0)
                        {
                            break;
                        }

                        //this is a new cluster
                        assignCluster(clusterableMapLayerObjects, clusterableMapObject);

                        finished = false;
                        break;

                    }
                }

                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }
            return true;
        }

        private static void assignCluster(IEnumerable<IClusterable> clusterableMapLayerObjects, IClusterable clusterableMapObject)
        {
            //TODO: find faster way to remove reference from other objects

            clusterableMapObject.IsCluster = true;

            //all its children are clustered
            foreach (var clusteredElement in clusterableMapObject.ClusteredElements)
            {
                clusteredElement.IsClustered = true;
                clusteredElement.ClusteredElements.Clear();
            }

            //remove all allocated elements form remaining list
            foreach (var otherClusterableObject in clusterableMapLayerObjects)
            {
                if (otherClusterableObject.ClusteredElements.Count > 0 && !otherClusterableObject.IsCluster && !otherClusterableObject.IsClustered)
                {
                    //remove cluster if it exist
                    if (otherClusterableObject.ClusteredElements.Contains(clusterableMapObject))
                    {
                        otherClusterableObject.ClusteredElements.Remove(clusterableMapObject);
                    }

                    //remove all clustered
                    foreach (var clusteredElement in clusterableMapObject.ClusteredElements)
                    {
                        if (otherClusterableObject.ClusteredElements.Contains(clusteredElement))
                        {
                            otherClusterableObject.ClusteredElements.Remove(clusteredElement);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Simple fast grid cluster algorithm with no object location shift
        /// Find the first object in each grid cell and make it the cluster
        /// Contains all the other objects in that cell
        /// </summary>
        /// <param name="clusterableMapLayerObjects">items to cluster</param>
        /// <param name="worker">thread to check in cancelled</param>
        /// <param name="zoomLevel">zoom level of map</param>
        /// <param name="bounds">The bounds of the map control</param>
        /// <returns>Was the action cancelled?</returns>
        private bool GridCluster(IEnumerable<IClusterable> clusterableMapLayerObjects, BackgroundWorker worker, int zoomLevel, LocationRect bounds)
        {
            //get width and height from bounds
            int x1;
            int x2;
            int y1;
            int y2;

            TileSystem.LatLongToPixelXY(bounds.North, bounds.West, zoomLevel, out x1, out y1);
            TileSystem.LatLongToPixelXY(bounds.South, bounds.East, zoomLevel, out x2, out y2);

            double width = Math.Abs(x2 - x1);
            // Break the map up into a grid
            var numXCells = (int)Math.Ceiling(width / clusterwidth);
            // Create an array to store the clusters
            var clusters = new Dictionary<int, IClusterable>();
            foreach (var clusterableMapObject in clusterableMapLayerObjects)
            {
                if (clusterableMapObject.Initalized)
                {
                    //what cluster cell does this belong to?
                    var x = (int)Math.Floor((clusterableMapObject.ProjectedPoints[zoomLevel].X - x1) / clusterwidth);
                    var y = (int)Math.Floor((clusterableMapObject.ProjectedPoints[zoomLevel].Y - y1) / clusterheight);
                    var index = x + y * numXCells;
                    //if this is the first it is the cluster
                    if (!clusters.ContainsKey(index))
                    {
                        clusters.Add(index, clusterableMapObject);
                        clusterableMapObject.IsCluster = true;
                    }
                    else
                    {
                        //else add to existing cluster
                        clusters[index].ClusteredElements.Add(clusterableMapObject);
                        clusterableMapObject.IsClustered = true;
                    }
                }
                else
                {
                    //listen for event when it is ready
                    clusterableMapObject.ProjectionComplete += (o, e) =>
                    {
                        isDirty = true;
                        startMapMovementWorker();
                    };
                }

                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }

            return true;
        }

        private void updateScale(double currentZoomLevel)
        {
            double scale = Math.Pow(0.05 * (currentZoomLevel + 1), 2) + 0.01;
            if (scale > 1) scale = 1;
            if (scale < 0.125) scale = 0.125;

            Dispatcher.BeginInvoke(delegate
            {
                foreach (UIElement child in Children)
                {
                    if (child is IScalable)
                    {
                        ((IScalable)child).Scale = scale;
                    }
                }
            });
        }

        private bool getObjectsInView(BackgroundWorker worker, LocationRect mapview, out List<MapLayerObject> objectsInView, out bool noChange)
        {
            objectsInView = new List<MapLayerObject>();
            noChange = true;

            for (int i = 0; i < mapLayerObjects.Count; i++)
            {
                var layerObject = mapLayerObjects[i];
                var inMapView = layerObject.Visible;
                if (inMapView)
                {
                    inMapView = mapview.Intersects(layerObject.MBR);
                }

                //look for no changes to quickly exit the remaining logic.
                if (inMapView != layerObject.InMapView)
                {
                    noChange = false;
                }

                layerObject.InMapView = inMapView;
                layerObject.ToBeRendered = inMapView;

                if (layerObject.InMapView)
                {
                    objectsInView.Add(layerObject);
                }

                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }
            return true;
        }

        private bool removeItemsNotInView(BackgroundWorker worker, IList<MapLayerObject> objectsInView, bool forceAll)
        {
            for (var i = 0; i < objectsInView.Count; i++)
            {
                var layerObject = objectsInView[i];

                //if the item is displayed we remove it
                if (forceAll || !layerObject.ToBeRendered && layerObject.InUse)
                {
                    Dispatcher.BeginInvoke(() => Children.Remove(layerObject.Element));
                    layerObject.InUse = false;
                }

                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }
            return true;
        }

        private bool reduceComplexObjects(BackgroundWorker worker, IEnumerable<MapLayerObject> objectsInView, int snapLevel)
        {
            foreach (var loopLayerObject in objectsInView)
            {
                var layerObject = loopLayerObject;
                //if item can be reduced in complexity
                if (layerObject.Element is IReduceable)
                {
                    var reduceable = ((IReduceable)layerObject.Element);
                    //if the object isn't ready we listen for complete event, then refresh UI.
                    if (!reduceable.Initalized)
                    {
                        reduceable.ReductionComplete += (o, e) =>
                        {
                            isDirty = true;
                            startMapMovementWorker();
                        };
                    }
                    else if (reduceable.CurrentDisplayed != snapLevel && layerObject.Element is MapShapeBase && reduceable.Reduced.ContainsKey(snapLevel))
                    {
                        //reduce the complexity to the cached level
                        Dispatcher.BeginInvoke(() => ((MapShapeBase)layerObject.Element).Locations = reduceable.Reduced[snapLevel]);
                        reduceable.CurrentDisplayed = snapLevel;
                    }
                }
                //exit loop if thread cancelled
                if (worker != null && worker.CancellationPending)
                {
                    return false;
                }
            }
            return true;
        }

        private void MapInstance_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            startMapMovementWorker();
        }

        private void startMapMovementWorker()
        {
            if (loaded)
            {
                //only be running once instance, if already running we cancel and wait.
                if (onMapMovementWorker.WorkerSupportsCancellation && onMapMovementWorker.IsBusy)
                {
                    onMapMovementWorker.CancelAsync();
                }
                else
                {
                    onMapMovementWorker.RunWorkerAsync(MapInstance.ZoomLevel);
                }
            }
        }

        private bool needDataRefresh;

        private void refreshMapMovementWorker()
        {
            if (loaded)
            {
                //only be running once instance, if already running we cancel and wait.
                if (onMapMovementWorker.IsBusy)
                {
                    needDataRefresh = true;
                }
                else
                {
                    startMapMovementWorker();
                }
            }

        }

        private void OnMapMovementWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || needDataRefresh)
            {
                needDataRefresh = false;
                startMapMovementWorker();
            }
        }

        public override void AddChild(UIElement element, Location location)
        {
            LocationRect locationRect;
            if (element is IClipable)
            {
                locationRect = ((IClipable)element).MBR;
            }
            else if (element is MapShapeBase)
            {
                locationRect = new LocationRect(((MapShapeBase)element).Locations);
            }
            else
            {
                locationRect = new LocationRect(new List<Location> { location });
            }
            //TODO: need to keep track of visibility changed.
            mapLayerObjects.Add(new MapLayerObject { Element = element, Location = location, MBR = locationRect, Visible = element.Visibility == Visibility.Visible });
            refreshMapMovementWorker();
        }

        public BaseGeometry GetGeometryByItemID(string itemID)
        {
            foreach (var geometry in mapLayerGeometries)
            {
                if (geometry.ItemID == itemID)
                {
                    return geometry;
                }
            }
            return null;
        }

       

        public LocationRect GetLayerBoundaries()
        {
            double west, north, east, south;
            west = north = east = south = 0.0;
            foreach (var item in mapLayerObjects)
            {
                if (west == 0.0 && north == 0.0 && east == 0.0 && south == 0.0)
                {
                    west = item.MBR.West;
                    north = item.MBR.North;
                    east = item.MBR.East;
                    south = item.MBR.South;
                    continue;
                }

                if (item.MBR.West < east)
                    east = item.MBR.West;

                if (item.MBR.North > north)
                    north = item.MBR.North;

                if (item.MBR.East > east)
                    east = item.MBR.East;

                if (item.MBR.South < south)
                    south = item.MBR.South;
            }


            return new LocationRect(north, west, south, east);

        }

        public void Remove(UIElement element)
        {
            foreach (var layerObject in mapLayerObjects)
            {
                if (layerObject.Element == element)
                {
                    mapLayerObjects.Remove(layerObject);
                    if (layerObject is IDisposable)
                    {
                        ((IDisposable)layerObject).Dispose();
                    }
                    break;
                }
            }
            startMapMovementWorker();
        }

    }
}
