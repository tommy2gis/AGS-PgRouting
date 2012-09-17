using System.Windows;
using Microsoft.Maps.MapControl;

namespace TobbyBingMaps.MapGeometry
{
    public class MapLayerObject
    {
        public UIElement Element { get; set; }
        public Location Location { get; set; }
        public bool InUse { get; set; }
        public bool ToBeRendered { get; set; }
        public bool InMapView { get; set; }
        public LocationRect MBR { get; set; }
        public bool Visible { get; set; }
    }
}
