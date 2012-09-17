using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Maps.MapControl;

namespace TobbyBingMaps.MapGeometry
{
    public interface IClusterable
    {
        event EventHandler ProjectionComplete;

        Location Location { get; set; }
        bool Initalized { get; set; }
        Dictionary<int, Point> ProjectedPoints { get; set; }
        List<IClusterable> ClusteredElements { get; set; }
        bool IsCluster { get; set; }
        bool IsClustered { get; set; }
    }
}
