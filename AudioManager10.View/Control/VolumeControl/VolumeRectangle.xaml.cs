using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AudioManager10.View.Control.VolumeControl
{
    /// <summary>
    /// Interaction logic for VolumeRectangle.xaml
    /// </summary>
    public partial class VolumeRectangle : UserControl
    {
        private Color _defaultSoftBrush;
        private readonly double _threshold;
        private readonly double _width;

        #region Dependency Properties

        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(double), typeof(VolumeRectangle), new PropertyMetadata(4d));

        public new double BorderThickness
        {
            get { return (double) GetValue(BorderThicknessProperty); }
            set
            {
                SetValue(BorderThicknessProperty, value);
                AdjustWidth();
                AdjustHeight();
            }
        }

        public static readonly DependencyProperty VisualVolumeImpactProperty = DependencyProperty.Register(
            "VisualVolumeImpact", typeof(double), typeof(VolumeRectangle), new PropertyMetadata(15d));

        public double VisualVolumeImpact
        {
            get { return (double) GetValue(VisualVolumeImpactProperty); }
            set { SetValue(VisualVolumeImpactProperty, value); }
        }

        #endregion Dependency Properties

        public VolumeRectangle(double threshold)
        {
            InitializeComponent();

            _width = Width;
            _threshold = threshold;
            InitializeColors();
            SizeChanged += delegate { AdjustWidth(); };
            AdjustWidth();
        }

        #region Visuals

        private void InitializeColors()
        {
            _defaultSoftBrush = VolumeBrush.GradientStops[1].Color;
            DataContextChanged += delegate { AdjustVisuals(); };
            SystemParameters.StaticPropertyChanged += delegate { AdjustVisuals(); };
            AdjustVisuals();
        }

        private void AdjustVisuals()
        {
            var oldColor = VolumeBrush.GradientStops[1].Color.ToString();
            var newVolume = (float) DataContext;
            var volumeIncreased = newVolume >= _threshold;

            VolumeBrush.GradientStops[1].Color = volumeIncreased
                ? SystemParameters.WindowGlassColor
                : _defaultSoftBrush;

            var newColor = VolumeBrush.GradientStops[1].Color.ToString();

            if (oldColor == newColor) return;
            if (volumeIncreased) InvokeVolumeIncreasedAnimation();
            else InvokeVolumeDecreasedAnimation();
        }

        private void AdjustHeight()
        {
            BorderRect.Height = Height;
            VolumeRect.Height = Height - 2 * BorderThickness;
        }

        private void AdjustWidth()
        {
            BorderRect.Width = Width;
            VolumeRect.Width = Width - BorderThickness;
        }

        #endregion Visuals

        #region Comforting Animation

        private void InvokeVolumeIncreasedAnimation()
        {
            Dispatcher.Invoke(() =>
            {
                var widthAnimation = new DoubleAnimation
                {
                    From = _width,
                    To = _width + VisualVolumeImpact,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150)),
                    EasingFunction = new BounceEase()
                    {
                        Bounciness = 15,
                        Bounces = 1
                    },
                    AutoReverse = true
                };
                BeginAnimation(WidthProperty, widthAnimation);
            });
        }

        private void InvokeVolumeDecreasedAnimation()
        {
            Dispatcher.Invoke(() =>
            {
                var widthAnimation = new DoubleAnimation
                {
                    From = _width,
                    To = _width - VisualVolumeImpact,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150)),
                    EasingFunction = new BounceEase()
                    {
                        Bounciness = 15,
                        Bounces = 1
                    },
                    AutoReverse = true
                };
                BeginAnimation(WidthProperty, widthAnimation);
            });
        }

        #endregion Comforting Animation
    }
}
