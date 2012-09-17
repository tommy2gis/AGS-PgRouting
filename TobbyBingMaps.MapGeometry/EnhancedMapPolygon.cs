using System.Windows.Browser;
using System.Windows.Media;
using System.Windows.Shapes;
using TobbyBingMaps.Common;
using TobbyBingMaps.Common.Entities;

namespace TobbyBingMaps.MapGeometry
{
    public class EnhancedMapPolygon : EnhancedMapShapeBase
    {
        // Methods
        public EnhancedMapPolygon()
            : base(new Polygon())
        {
        }

        // Properties
        [ScriptableMember]
        public FillRule FillRule
        {
            get
            {
                return (FillRule)EncapsulatedShape.GetValue(Polygon.FillRuleProperty);
            }
            set
            {
                EncapsulatedShape.SetValue(Polygon.FillRuleProperty, value);
            }
        }

        protected override PointCollection ProjectedPoints
        {
            get
            {
                return ((Polygon)EncapsulatedShape).Points;
            }
            set
            {
                ((Polygon)EncapsulatedShape).Points = value;
            }
        }

        private StyleSpecification geometryStyle;
        public StyleSpecification GeometryStyle
        {
            get { return geometryStyle; }
            set
            {
                geometryStyle = value;
                Fill =
                    geometryStyle.ShowFill
                        ? new SolidColorBrush(Utilities.ColorFromHexString(geometryStyle.PolyFillColour))
                        : new SolidColorBrush(Colors.Transparent);
                if (geometryStyle.ShowLine)
                {
                    Stroke = new SolidColorBrush(Utilities.ColorFromHexString(geometryStyle.PolygonLineColour));
                    StrokeThickness = geometryStyle.PolygonLineWidth;
                }
                else
                {
                    Stroke = new SolidColorBrush(Colors.Transparent);
                    StrokeThickness = 0;
                }
            }
        }
    }
}
