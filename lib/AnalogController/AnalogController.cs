using System;
using Xamarin.Forms;

namespace AnalogController
{
    public partial class AnalogController : ContentView
    {
        private readonly BoxView _thumbBoxView;
        private Func<double, double, double> _getOffset;

        public AnalogController()
        {
            _getOffset = Device.RuntimePlatform == Device.Android ? (Func<double, double, double>)((a, b) => a) : (a, b) => a - b;

            var rootLayout = new RelativeLayout();

            var backgroundBoxView = new Frame() { BackgroundColor = Color.LightGray };
            rootLayout.Children.Add(
                    backgroundBoxView,
                    Constraint.RelativeToParent(p => 0.0),
                    Constraint.RelativeToParent(p => 0.0),
                    Constraint.RelativeToParent(p =>
                    {
                        backgroundBoxView.CornerRadius = (float)(Math.Min(p.Width, p.Height) / 2.0);
                        return p.Width;
                    }
                        ),
                    Constraint.RelativeToParent(p => p.Height)
                );

            _thumbBoxView = new BoxView() { Color = Color.FromHex("#FFB3B3B3") };
            var thumbDragGestureRecognizer = new PanGestureRecognizer();
            thumbDragGestureRecognizer.PanUpdated += OnThumbDragged;
            _thumbBoxView.GestureRecognizers.Add(thumbDragGestureRecognizer);
            rootLayout.Children.Add(
                    _thumbBoxView,
                    Constraint.RelativeToParent(p => p.Width / 2.0 - (0.1 * p.Width)),
                    Constraint.RelativeToParent(p => p.Height / 2.0 - (0.1 * p.Height)),
                    Constraint.RelativeToParent(p =>
                    {
                        var desiredWidth = 0.2 * p.Width;

                        _thumbBoxView.CornerRadius = desiredWidth / 2.0;
                        return desiredWidth;
                    }),
                    Constraint.RelativeToParent(p => 0.2 * p.Height)
                );

            this.Content = rootLayout;
        }

        private double previousX = 0.0;
        private double previousY = 0.0;

        private void OnThumbDragged(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    _thumbBoxView.Scale = 0.85;
                    break;

                case GestureStatus.Running:
                    _thumbBoxView.TranslationX += _getOffset(e.TotalX, previousX);
                    _thumbBoxView.TranslationY += _getOffset(e.TotalY, previousY);

                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    break;

                case GestureStatus.Completed:
                    _thumbBoxView.Scale = 1.0;
                    break;

                case GestureStatus.Canceled:
                    _thumbBoxView.Scale = 1.0;
                    break;
            }
        }
    }
}