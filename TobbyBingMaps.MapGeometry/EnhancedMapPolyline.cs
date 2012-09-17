using System.Windows.Browser;
using System.Windows.Media;
using System.Windows.Shapes;
using TobbyBingMaps.Common;
using TobbyBingMaps.Common.Entities;

namespace TobbyBingMaps.MapGeometry
{
    public class EnhancedMapPolyline : EnhancedMapShapeBase
    {
        public EnhancedMapPolyline()
            : base(new Polyline())
        {
        }

        // Properties
        [ScriptableMember]
        public FillRule FillRule
        {
            get
            {
                return (FillRule)EncapsulatedShape.GetValue(Polyline.FillRuleProperty);
            }
            set
            {
                EncapsulatedShape.SetValue(Polyline.FillRuleProperty, value);
            }
        }

        protected override PointCollection ProjectedPoints
        {
            get
            {
                return ((Polyline)EncapsulatedShape).Points;
            }
            set
            {
                ((Polyline)EncapsulatedShape).Points = value;
            }
        }

        private StyleSpecification geometryStyle;
        public StyleSpecification GeometryStyle
        {
            get { return geometryStyle; }
            set
            {
                geometryStyle = value;
                Stroke = new SolidColorBrush(Utilities.ColorFromHexString(geometryStyle.LineColour));
                StrokeThickness = geometryStyle.LineWidth;
            }
        }
    }
}
