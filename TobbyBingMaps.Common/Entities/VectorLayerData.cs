using System;


namespace TobbyBingMaps.Common.Entities
{
    public class VectorLayerData
    {
        public byte[] Geo { get; set; }
        public string ID { get; set; }
        
        public string Label { get; set; }
        public string Style { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
