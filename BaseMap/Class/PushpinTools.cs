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
using System.Windows.Media.Imaging;

namespace BaseMap
{
    public static class PushpinTools
    {
        /// <summary>
        /// Creates an Image pushpin
        /// </summary>
        /// <param name="imageUri">Uri of the image to use for the pushpin icon</param>
        /// <param name="width">Width of the pushpin</param>
        /// <param name="height">Height of the pushpin</param>
        /// <param name="offset">Offset distance for the pushpin so that the point of the 
        /// pin is aligned with the associated coordinate.</param>
        /// <returns></returns>
        public static UIElement CreateImagePushpin(Uri imageUri, double width, double height, int rotateTr, Point offset, PlaneProjection planeProjection)
        {

            //Source.RenderTransform.r
            //TransformGroup 
            RotateTransform RotateTr = new RotateTransform();
            RotateTr.Angle = rotateTr;



            return new Image()
            {
                Source = new BitmapImage(imageUri),
                //RenderTransform = RotateTr,
                Width = width,
                Height = height,
                //Stretch = System.Windows.Media.Stretch.Uniform,
                //VerticalAlignment = VerticalAlignment.Center,
                //HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(offset.X, offset.Y, 0, 0),
                Projection = planeProjection,

            };
        }

        /// <summary>
        /// Creates a Circle UIElement
        /// </summary>
        /// <param name="radius">Radius of the circle to create</param>
        /// <param name="fillColor">The fill color of the circle</param>
        /// <param name="strokeColor">The stroke color of the circle. 
        /// This is used to create a border around the circle</param>
        /// <returns></returns>
        public static UIElement CreateCirclePushpin(double radius, Brush fillColor, Brush strokeColor)
        {
            return new Ellipse()
            {
                Width = radius + radius,
                Height = radius + radius,
                Fill = fillColor,
                Stroke = strokeColor,
                StrokeThickness = 2,
                Margin = new Thickness(-radius, -radius, 0, 0)
            };
        }


    }
}
