using System;
using System.Collections.Generic;
using Microsoft.Maps.MapControl;

namespace TobbyBingMaps.MapGeometry
{
    public interface IReduceable
    {
        event EventHandler ReductionComplete;

        LocationCollection Original { get; set; }
        Dictionary<int, LocationCollection> Reduced { get; set; }
        int CurrentDisplayed { get; set; }
        bool Initalized { get; set; }
    }
}
