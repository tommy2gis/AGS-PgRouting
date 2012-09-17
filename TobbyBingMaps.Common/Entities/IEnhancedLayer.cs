using System;


namespace TobbyBingMaps.Common.Entities
{
    public interface IEnhancedLayer : IDisposable
    {
        string ID { get; set; }
        int ZIndex { get; set; }
    }
}
