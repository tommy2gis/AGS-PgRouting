using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BaseMap.Control
{
    public delegate void Click(object sender, string key);
    public partial class ToolBarItem : Button
    {
        public event Click ToolBarItemClick;
        public ToolBarItem()
        {
            InitializeComponent();
            this.Click += new RoutedEventHandler(ToolBarItem_Click);
        }

        void ToolBarItem_Click(object sender, RoutedEventArgs e)
        {
            if (ToolBarItemClick != null)
                this.ToolBarItemClick(sender, (string)GetValue(ToolBarItemKeyProperty));
        }

        public static DependencyProperty ToolBarItemImageProperty = DependencyProperty.Register("ToolBarItemImage",
        typeof(ImageSource),
        typeof(ToolBarItem),
        new PropertyMetadata(null));
        public ImageSource ToolBarItemImage
        {
            get { return (ImageSource)GetValue(ToolBarItemImageProperty); }
            set { SetValue(ToolBarItemImageProperty, value); }
        }

        public static DependencyProperty ToolBarItemKeyProperty = DependencyProperty.Register("ToolBarItemKey",
        typeof(String),
        typeof(ToolBarItem),
        new PropertyMetadata(null));
        public string ToolBarItemKey
        {
            get { return (string)GetValue(ToolBarItemKeyProperty); }
            set { SetValue(ToolBarItemKeyProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetDataContext();
        }
        private void SetDataContext()
        {
            Border border = GetTemplateChild("imgBorder") as Border;
            border.DataContext = this;
            TextBlock txtCaption = GetTemplateChild("caption") as TextBlock;
            txtCaption.DataContext = this;

            if (this.Content == null)
            {
                txtCaption.Margin = new Thickness(0);
                border.Margin = new Thickness(3);
            }
            else
            {
                txtCaption.Margin = new Thickness(3);
            }
        }


    }
}
