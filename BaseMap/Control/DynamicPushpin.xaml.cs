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
    public partial class DynamicPushpin : UserControl
    {
        public bool HasAnimated { get; set; }
        public bool IsAnimating { get; set; }
        System.Threading.Timer StopTimer;

        public DynamicPushpin()
        {
            InitializeComponent();
            this.HasAnimated = false;
            this.IsAnimating = false;
            timer.Duration = new TimeSpan(0, 0, 0, 5);
            timer.Stop();
        }

        public void pulseStoryboard_Completed(object sender, EventArgs e)
        {
            this.IsAnimating = false;

        }
        //动画开始
        public void BeginAnimation()
        {
            this.pulseStoryboard.Begin();
            this.HasAnimated = true;
        }

        //动画开始
        public void BeginAnimation(int iTimeCout, Action<bool> CallBack)
        {
            this.pulseStoryboard.Begin();
            this.HasAnimated = true;
            timer.Duration = new TimeSpan(0, 0, 0, iTimeCout);
            timer.Begin();
            timer.Completed += (os, en) =>
            {
                StopAnimation();
                if (CallBack != null)
                {
                    CallBack(true);
                }
            };

        }
        public void timerCall(object obj)
        {


        }
        //动画结束
        public void StopAnimation()
        {
            this.pulseStoryboard.Stop();
            this.HasAnimated = false;
            timer.Stop();
        }
        //设定中心点大小及动画时长
        public double Magnitude
        {
            get
            {
                return (double)base.GetValue(MagnitudeProperty);
            }
            set
            {
                base.SetValue(MagnitudeProperty, value);
                int seconds = 0;
                double num2 = 0.127;
                if (this.Magnitude <= 2.5)
                {
                    seconds = 1;
                    num2 = 0.05;
                }
                else if (this.Magnitude <= 3.5)
                {
                    seconds = 3;
                    num2 = 0.08;
                }
                else if (this.Magnitude <= 4.5)
                {
                    seconds = 4;
                    num2 = 0.1;
                }
                else if (this.Magnitude <= 5.5)
                {
                    seconds = 6;
                    num2 = 0.127;
                }
                else if (this.Magnitude <= 6.5)
                {
                    seconds = 8;
                    num2 = 0.15;
                }
                else if (this.Magnitude <= 7.5)
                {
                    seconds = 10;
                    num2 = 0.3;
                }
                else if (this.Magnitude <= 8.5)
                {
                    seconds = 12;
                    num2 = 0.4;
                }
                else if (this.Magnitude > 8.5)
                {
                    seconds = 14;
                    num2 = 0.6;
                }
                TimeSpan span = new TimeSpan(0, 0, 0, seconds);
                this.pulseAnim.Duration = (Duration)span;
                this.pulseColour.Duration = (Duration)span;
                this.pulseStoryboard.Duration = (Duration)span.Add(new TimeSpan(0, 0, 0, 1));
                this.ImpactSite.Offset = num2;
                this.PulseRing.Offset = num2 + 0.2;
            }

        }
        public static readonly DependencyProperty MagnitudeProperty = DependencyProperty.Register("Magnitude", typeof(double), typeof(DynamicPushpin), new PropertyMetadata(2.5));

    }
}
