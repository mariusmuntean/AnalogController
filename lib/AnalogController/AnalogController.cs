using System;
using Xamarin.Forms;

namespace AnalogController
{
    public partial class AnalogController : ContentView
    {
        private const string ThumbToCenterAnimation = "AnimateThumbToCenter";
        private readonly BoxView _thumbBoxView;
        private readonly Func<double, double, double> _getOffset;

        public AnalogController()
        {
            _getOffset = Device.RuntimePlatform == Device.Android ? (Func<double, double, double>)((a, b) => a) : (a, b) => a - b;

            var rootLayout = new RelativeLayout();

            _backgroundBoxView = new Frame() { BackgroundColor = Color.LightGray };
            rootLayout.Children.Add(
                    _backgroundBoxView,
                    Constraint.RelativeToParent(p => p.Width / 2.0 - Math.Min(p.Width, p.Height) / 2.0),
                    Constraint.RelativeToParent(p => p.Height / 2.0 - Math.Min(p.Width, p.Height) / 2.0),
                    Constraint.RelativeToParent(p =>
                    {
                        _backgroundBoxView.CornerRadius = (float)(Math.Min(p.Width, p.Height) / 2.0);
                        return Math.Min(p.Width, p.Height);
                    }
                        ),
                    Constraint.RelativeToParent(p => Math.Min(p.Width, p.Height))
                );

            _thumbBoxView = new BoxView() { Color = Color.FromHex("#FFB3B3B3") };
            var thumbDragGestureRecognizer = new PanGestureRecognizer();
            thumbDragGestureRecognizer.PanUpdated += OnThumbDragged;
            _thumbBoxView.GestureRecognizers.Add(thumbDragGestureRecognizer);
            rootLayout.Children.Add(
                    _thumbBoxView,
                    Constraint.RelativeToParent(p => p.Width / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                    Constraint.RelativeToParent(p => p.Height / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                    Constraint.RelativeToParent(p =>
                    {
                        var desiredWidth = 0.2 * Math.Min(p.Width, p.Height);

                        _thumbBoxView.CornerRadius = desiredWidth / 2.0;
                        return desiredWidth;
                    }),
                    Constraint.RelativeToParent(p => 0.2 * Math.Min(p.Width, p.Height))
                );

            this.Content = rootLayout;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
        }

        private double previousX = 0.0;
        private double previousY = 0.0;
        private readonly Frame _backgroundBoxView;

        private void OnThumbDragged(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:

                    this.AbortAnimation(ThumbToCenterAnimation);

                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    _thumbBoxView.Scale = 0.85;
                    break;

                case GestureStatus.Running:
                    var newX = _thumbBoxView.TranslationX + _getOffset(e.TotalX, previousX);
                    var newY = _thumbBoxView.TranslationY + _getOffset(e.TotalY, previousY);

                    var newRadius = Math.Sqrt(Math.Pow(newX, 2) + Math.Pow(newY, 2));
                    var newTheta = Math.Atan2(newY, newX);

                    newRadius = Math.Min(_backgroundBoxView.Width / 2.0, newRadius);

                    _thumbBoxView.TranslationX = newRadius * Math.Cos(newTheta);
                    _thumbBoxView.TranslationY = newRadius * Math.Sin(newTheta);

                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    break;

                case GestureStatus.Completed:
                    _thumbBoxView.Scale = 1.0;
                    AnimateThumbToCenter();
                    break;

                case GestureStatus.Canceled:
                    _thumbBoxView.Scale = 1.0;
                    AnimateThumbToCenter();
                    break;
            }
        }

        private void AnimateThumbToCenter()
        {
            var centerAnimation = new Animation(d =>
            {
                _thumbBoxView.TranslationX = d * _thumbBoxView.TranslationX;
                _thumbBoxView.TranslationY = d * _thumbBoxView.TranslationY;
            }, 1.0, 0.0, Easing.CubicIn);

            centerAnimation.Commit(this, ThumbToCenterAnimation, length: 150);
        }
    }
}