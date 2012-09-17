using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TobbyBingMaps.Common.Entities
{
    public enum AttributeType
    {
        KeyValuePair = 0,
        Chart = 1,
        Gauge = 2
    }

    public class LayerObjectAttributeDefinition
    {
        public string ObjectAttributeID { get; set; }
        public string ObjectAttributeTemplate { get; set; }
        public AttributeType ObjectAttributeType { get; set; }

#if SILVERLIGHT

#else
        //not public
        public string ObjectAttributeURI { get; set; }
		public string Description { get; set; }
		public string ConnectionString { get; set; }
		public int DisplayOrder { get; set; }
#endif

    }
}
