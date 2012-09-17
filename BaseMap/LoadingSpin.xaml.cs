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

namespace BaseMap
{
    public partial class LoadingSpin : UserControl
    {
        private Storyboard intervalTimer;
        private List<Storyboard> animations;
        private int nextAnimation;
        public event EventHandler<EventArgs> ExitCurrentProcess;
        public string StopMuemCaption { get; set; }
        public Visibility StopMuemVisibility { get; set; }
        public LoadingSpin()
        {
            InitializeComponent();
            this.FlashText.Begin();
            this.intervalTimer = new Storyboard()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(100))
            };
            this.intervalTimer.Completed += new EventHandler(this.OnIntervalTimerCompleted);

            this.animations = new List<Storyboard>()
			{
				CycleAnimation0,
				CycleAnimation1,
				CycleAnimation2,
				CycleAnimation3,
				CycleAnimation4,
				CycleAnimation5,
				CycleAnimation6,
				CycleAnimation7,
				CycleAnimation8,
				CycleAnimation9
			};
            this.Loaded += new RoutedEventHandler(LoadingSpin_Loaded);
        }

        void LoadingSpin_Loaded(object sender, RoutedEventArgs e)
        {
            //this.StopCurrent.Visibility = StopMuemVisibility;
            //this.StopCurrent.Header = StopMuemCaption;
        }

        /// <summary>
        /// Start the animation.
        /// </summary>
        public void Begin()
        {
            this.Visibility = Visibility.Visible;
            AppearAnimation.Begin();
            this.intervalTimer.Begin();
        }

        /// <summary>
        /// Start the animation.
        /// </summary>
        public void Begin(string strMsg)
        {
            this.Visibility = Visibility.Visible;
            waitingText.Text = strMsg;
            AppearAnimation.Begin();
            this.intervalTimer.Begin();
        }

        /// <summary>
        /// Stop the animation.
        /// </summary>
        public void Stop()
        {
            this.Visibility = Visibility.Collapsed;
            AppearAnimation.Stop();
            this.nextAnimation = 0;
            this.intervalTimer.Stop();
            foreach (var item in this.animations)
            {
                item.Stop();
            }
        }

        private void OnIntervalTimerCompleted(object sender, EventArgs e)
        {
            this.animations[nextAnimation].Begin();

            // Cycle:
            this.nextAnimation = this.nextAnimation > 8 ? 0 : this.nextAnimation + 1;

            this.intervalTimer.Begin();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
