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
    public partial class ToolBarPanel : UserControl
    {
        public event Click ToolBarItemClick;
        public ToolBarPanel()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ToolBarPanel_Loaded);
        }

        void ToolBarPanel_Loaded(object sender, RoutedEventArgs e)
        {
            this.CreateClickEvent();
        }

        public static DependencyProperty ToolBarItemsProperty = DependencyProperty.Register("ToolBarItems",
        typeof(PresentationFrameworkCollection<ToolBarItem>),
        typeof(ToolBarPanel),
        new PropertyMetadata(null));
        public UIElementCollection ToolBarItems
        {
            get
            {
                return ItemHolder.Children;
            }
        }

        public static DependencyProperty ToolBarItemsOrientation = DependencyProperty.Register("Orientation",
        typeof(Orientation),
        typeof(ToolBarPanel),
        new PropertyMetadata(null));
        public Orientation Orientation
        {
            get { return (ItemHolder.Orientation); }
            set { ItemHolder.Orientation = value; }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemHolder.DataContext = this;
        }

        private void CreateClickEvent()
        {
            foreach (UIElement elem in ItemHolder.Children)
            {
                ToolBarItem item = elem as ToolBarItem;
                if (item != null)
                    item.ToolBarItemClick += new Click(item_ToolBarItemClick);
            }
        }

        void item_ToolBarItemClick(object sender, string key)
        {
            if (ToolBarItemClick != null)
                this.ToolBarItemClick(sender, ((ToolBarItem)sender).ToolBarItemKey);
        }

    }
}
