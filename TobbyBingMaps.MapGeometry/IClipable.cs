using Microsoft.Maps.MapControl;

namespace TobbyBingMaps.MapGeometry
{
    public interface IClipable
    {
        LocationRect MBR { get; set; }
    }
}
