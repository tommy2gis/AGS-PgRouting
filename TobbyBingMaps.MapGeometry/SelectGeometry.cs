using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using TobbyBingMaps.Common;
using TobbyBingMaps.Common.Entities;
using GeoAPI.Geometries;
using Microsoft.Maps.MapControl.Core;


namespace TobbyBingMaps.MapGeometry
{
    public delegate void SelectedEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Geometry with mouse over effect a common selected event handler.
    /// </summary>
    public class SelectGeometry : BaseGeometry
    {
        public SelectGeometry(IGeometry geometry, StyleSpecification styleSpecification, MapCore map,
                              EnhancedMapLayer layer)
            : base(geometry, styleSpecification, map, layer)
        {
            //default is the inverse of the style colours + 50% larger icon.
            SelectedSpecification = styleSpecification.Clone();
            SelectedSpecification.IconScale = styleSpecification.IconScale * 1.5;
            SelectedSpecification.LineColour = Utilities.InvertColorFromHexString(styleSpecification.LineColour);
            SelectedSpecification.PolyFillColour = Utilities.InvertColorFromHexString(styleSpecification.PolyFillColour);

            foreach (Control mapObject in mapObjects)
            {
                mapObject.MouseLeftButtonDown += SelectGeometry_MouseLeftButtonDown;
                mapObject.MouseEnter += SelectGeometry_MouseEnter;
                mapObject.MouseLeave += SelectGeometry_MouseLeave;
            }

            mapObjects.CollectionChanged += mapObjects_CollectionChanged;

            EnableSelection = true;
        }

        public StyleSpecification SelectedSpecification { get; set; }
        public bool EnableSelection { get; set; }

        private void mapObjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Control oldItem in e.OldItems)
                {
                    oldItem.MouseLeftButtonDown -= SelectGeometry_MouseLeftButtonDown;
                    oldItem.MouseEnter -= SelectGeometry_MouseEnter;
                    oldItem.MouseLeave -= SelectGeometry_MouseLeave;
                }
            }
            if (e.NewItems != null)
            {
                foreach (Control newItem in e.NewItems)
                {
                    newItem.MouseLeftButtonDown += SelectGeometry_MouseLeftButtonDown;
                    newItem.MouseEnter += SelectGeometry_MouseEnter;
                    newItem.MouseLeave += SelectGeometry_MouseLeave;
                }
            }
        }

        public event SelectedEventHandler Selected;

        protected virtual void OnSelected(EventArgs e)
        {
            if (Selected != null)
                Selected(this, e);
        }


        private void SelectGeometry_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EnableSelection)
            {
                OnSelected(new EventArgs());
            }
        }

        private void SelectGeometry_MouseLeave(object sender, MouseEventArgs e)
        {
            Restore();
        }


        private void SelectGeometry_MouseEnter(object sender, MouseEventArgs e)
        {
            Select();
        }

        public void Select()
        {
            if (EnableSelection && SelectedSpecification != null)
            {
                setStyleOnMapObjects(SelectedSpecification);
            }
        }

        public void Restore()
        {
            setStyleOnMapObjects(StyleSpecification);
        }

        private void setStyleOnMapObjects(StyleSpecification styleSpecification)
        {
            foreach (Control mapObject in mapObjects)
            {
                if (mapObject is EnhancedMapPoint)
                {
                    ((EnhancedMapPoint)mapObject).GeometryStyle = styleSpecification;
                }
                if (mapObject is EnhancedMapPolyline)
                {
                    ((EnhancedMapPolyline)mapObject).GeometryStyle = styleSpecification;
                }
                if (mapObject is EnhancedMapPolygon)
                {
                    ((EnhancedMapPolygon)mapObject).GeometryStyle = styleSpecification;
                }
            }
        }

        private void removeGeometry()
        {
            foreach (Control mapObject in mapObjects)
            {
                mapObject.MouseLeftButtonDown -= SelectGeometry_MouseLeftButtonDown;
                mapObject.MouseEnter -= SelectGeometry_MouseEnter;
                mapObject.MouseLeave -= SelectGeometry_MouseLeave;
            }
        }

        public new void Dispose()
        {
            removeGeometry();
            base.Dispose();
        }
    }
}
