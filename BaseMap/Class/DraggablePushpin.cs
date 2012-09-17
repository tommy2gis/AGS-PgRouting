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
using Microsoft.Maps.MapControl;

namespace BaseMap.Class
{
    public class DraggablePushpin : Microsoft.Maps.MapControl.Pushpin
    {
        /// <summary>
        /// 是否正在拖放
        /// </summary>
        private bool IsDragging = false;
        /// <summary>
        /// 鼠标拖放
        /// </summary>
        private EventHandler<MapMouseDragEventArgs> MapMouseDragHandler;
        /// <summary>
        /// 鼠标左键弹起
        /// </summary>
        private MouseButtonEventHandler MapMouseLeftButtonUpHandler;
        /// <summary>
        /// 鼠标移动
        /// </summary>
        private MouseEventHandler MapMouseMoveHandler;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //因为图钉是添加在MapLayer上的
            var parentLayer = this.Parent as MapLayer;
            if (parentLayer != null)
            {
                //MapLayer是Map的一个子对象
                var parentMap = parentLayer.ParentMap;
                if (parentMap != null)
                {
                    if (this.MapMouseDragHandler == null)
                    {
                        this.MapMouseDragHandler = new EventHandler<MapMouseDragEventArgs>(ParnetMap_MousePan);
                        parentMap.MousePan += this.MapMouseDragHandler;
                    }

                    if (this.MapMouseLeftButtonUpHandler == null)
                    {
                        this.MapMouseLeftButtonUpHandler = new MouseButtonEventHandler(PrentMap_MouseLeftButtonUp);
                        parentMap.MouseLeftButtonUp += this.MapMouseLeftButtonUpHandler;
                    }

                    if (this.MapMouseMoveHandler == null)
                    {
                        this.MapMouseMoveHandler = new MouseEventHandler(PrentMap_MouseMove);
                        parentMap.MouseMove += this.MapMouseMoveHandler;
                    }
                }
            }
            this.IsDragging = true;

            base.OnMouseLeftButtonDown(e);
        }

        private void ParnetMap_MousePan(object sender, MapMouseDragEventArgs e)
        {
            if (this.IsDragging)
            {
                e.Handled = true;
            }
        }

        private void PrentMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsDragging = false;
        }

        private void PrentMap_MouseMove(object sender, MouseEventArgs e)
        {
            var map = sender as Map;
            if (this.IsDragging)
            {
                var mousePosition = e.GetPosition(map);
                var geoPosition = map.ViewportPointToLocation(mousePosition);
                //重新定位图钉位置
                this.Location = geoPosition;
            }
        }
    }

}
