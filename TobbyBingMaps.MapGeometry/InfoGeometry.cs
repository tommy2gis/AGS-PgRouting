using System.Windows;
using System.Windows.Controls;
using TobbyBingMaps.Common.Entities;
using GeoAPI.Geometries;
using Microsoft.Maps.MapControl.Core;


namespace TobbyBingMaps.MapGeometry
{
    public delegate void BalloonEventHandler(object sender, BalloonEventArgs args);

    /// <summary>
    /// Adds a label and a ballon popup to a selectable geometry
    /// Requires at lease one EnhancedMapPoint is created to launch the balloon
    /// All EnhancedMapPoints show the label.
    /// </summary>
    public class InfoGeometry : SelectGeometry
    {
        private readonly EnhancedMapPoint mainPoint;

        public InfoGeometry(IGeometry geometry, StyleSpecification styleSpecification, MapCore map, EnhancedMapLayer layer)
            : base(geometry, styleSpecification, map, layer)
        {
            //must have at least one EnhancedMapPoint to show label and launch balloon.
            foreach (var mapObject in mapObjects)
            {
                if (mapObject is EnhancedMapPoint)
                {
                    var mapPoint = (EnhancedMapPoint)mapObject;
                    if (mainPoint == null)
                    {
                        mainPoint = mapPoint;
                    }
                    mapPoint.Balloon += (o, e) => Balloon(this, new BalloonEventArgs { LayerID = layer.ID, ItemID = ItemID });
                }
            }
            if (mainPoint == null)
            {
                //TODO: create one, invisible at centre of first object
                var point = geometry.Centroid;
                mainPoint = createPoint(point, layer);
                mainPoint.Balloon += (o, e) => Balloon(this, new BalloonEventArgs { LayerID = layer.ID, ItemID = ItemID });
            }
            Selected += InfoGeometry_Selected;
        }

        void InfoGeometry_Selected(object sender, System.EventArgs args)
        {
            mainPoint.ShowBalloon();
        }

        public bool EnableBalloon
        {
            get { return mainPoint.EnableBalloon; }
            set
            {
                foreach (var mapObject in mapObjects)
                {
                    if (mapObject is EnhancedMapPoint)
                    {
                        ((EnhancedMapPoint)mapObject).EnableBalloon = value;

                    }
                }
            }
        }

        public Visibility LabelVisibility
        {
            get { return mainPoint.LabelVisibility; }
            set
            {
                foreach (var mapObject in mapObjects)
                {
                    if (mapObject is EnhancedMapPoint)
                    {
                        ((EnhancedMapPoint)mapObject).LabelVisibility = value;

                    }
                }
            }
        }

        public string Label
        {
            get { return mainPoint.Label; }
            set
            {
                foreach (var mapObject in mapObjects)
                {
                    if (mapObject is EnhancedMapPoint)
                    {
                        ((EnhancedMapPoint)mapObject).Label = value;

                    }
                }
            }
        }

        public event BalloonEventHandler Balloon;

        public UIElementCollection BalloonData
        {
            get { return mainPoint.BalloonData; }
        }
    }
}
