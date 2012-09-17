using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TobbyBingMaps.Common.Converters;
using TobbyBingMaps.Common.Entities;
using GeoAPI.Geometries;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace TobbyBingMaps.MapGeometry
{
    /// <summary>
    /// A geometry object on the map.
    /// Creates and manages a collection of points, lines and polygons (mapObjects) based on the IGeometry type.
    /// Provides a common object to apply style and metadata onto.
    /// </summary>
    public class BaseGeometry : IDisposable
    {
        private Visibility visibility;
        private readonly MapCore mapInstance;
        private readonly EnhancedMapLayer mapLayer;

        public BaseGeometry(IGeometry geometry, StyleSpecification styleSpecification, MapCore map, EnhancedMapLayer layer)
        {
            Geometry = geometry;
            StyleSpecification = styleSpecification;
            mapInstance = map;
            mapLayer = layer;
            mapObjects = new ObservableCollection<Control>();
            if (mapLayer != null)
            {
                createGeometry(mapLayer, Geometry);
            }
        }

        internal ObservableCollection<Control> mapObjects { get; set; }

        public string ItemID { get; set; }

        public EnhancedMapLayer MapLayer { get { return mapLayer; } }

        public IGeometry Geometry { get; set; }

        public StyleSpecification StyleSpecification { get; set; }

        public DateTime TimeStamp { get; set; }

        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    foreach (var mapObject in mapObjects)
                    {
                        mapObject.Visibility = visibility;
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            removeGeometry();
        }

        #endregion

        private void removeGeometry()
        {
            foreach (var mapObject in mapObjects)
            {
                mapLayer.Remove(mapObject);
            }
            mapObjects.Clear();
        }

        protected void createGeometry(MapLayerBase layer, IGeometry geo)
        {
            if (geo is IPoint)
            {
                var point = (IPoint)geo;
                createPoint(point, layer);
            }
            if (geo is ILineString)
            {
                var lineString = (ILineString)geo;
                createPolyLine(lineString, layer);
            }
            if (geo is IPolygon)
            {
                var polygon = (IPolygon)geo;
                createPolygon(polygon, layer);
            }
            if (geo is IGeometryCollection)
            {
                var geometryCollection = (IGeometryCollection)geo;
                foreach (IGeometry subgeometry in geometryCollection.Geometries)
                {
                    createGeometry(layer, subgeometry);
                }
            }
        }

        protected EnhancedMapPoint createPoint(IGeometry point, MapLayerBase layer)
        {
            var location = CoordinateConvertor.ConvertBack(point.Coordinate);
            var mapPoint = new EnhancedMapPoint(location, mapInstance) { GeometryStyle = StyleSpecification };
            layer.AddChild(mapPoint, location);
            mapObjects.Add(mapPoint);
            mapPoint.Visibility = Visibility;
            return mapPoint;
        }

        protected EnhancedMapPolyline createPolyLine(IGeometry lineString, MapLayerBase layer)
        {
            var line = new EnhancedMapPolyline
            {
                Locations = CoordinateConvertor.CoordinatesToLocationCollection(lineString.Coordinates),
                GeometryStyle = StyleSpecification,
                Visibility = Visibility
            };
            layer.AddChild(line, CoordinateConvertor.ConvertBack(lineString.Centroid.Coordinate));
            mapObjects.Add(line);
            return line;
        }

        protected EnhancedMapPolygon createPolygon(IGeometry polygon, MapLayerBase layer)
        {
            var poly = new EnhancedMapPolygon
            {
                Locations = CoordinateConvertor.CoordinatesToLocationCollection(polygon.Coordinates),
                GeometryStyle = StyleSpecification,
                Visibility = Visibility
            };
            layer.AddChild(poly, CoordinateConvertor.ConvertBack(polygon.Centroid.Coordinate));
            mapObjects.Add(poly);
            return poly;
        }
    }
}
