using System;
using Xamarin.Forms;

namespace AnalogController
{
    public partial class AnalogController : ContentView
    {
        private const string ThumbToCenterAnimation = "AnimateThumbToCenter";
        private readonly Frame _thumbFrame;
        private readonly Func<double, double, double> _getOffset;

        public AnalogController()
        {
            _getOffset = Device.RuntimePlatform == Device.Android ? (Func<double, double, double>)((a, b) => a) : (a, b) => a - b;

            var rootLayout = new RelativeLayout();

            _backgroundFrame = new Frame { BackgroundColor = Color.LightGray };
            rootLayout.Children.Add(
                    _backgroundFrame,
                    Constraint.RelativeToParent(p => p.Width / 2.0 - Math.Min(p.Width, p.Height) / 2.0),
                    Constraint.RelativeToParent(p => p.Height / 2.0 - Math.Min(p.Width, p.Height) / 2.0),
                    Constraint.RelativeToParent(p =>
                    {
                        _backgroundFrame.CornerRadius = (float)(Math.Min(p.Width, p.Height) / 2.0);
                        return Math.Min(p.Width, p.Height);
                    }
                        ),
                    Constraint.RelativeToParent(p => Math.Min(p.Width, p.Height))
                );

            _thumbFrame = new Frame { BackgroundColor = Color.FromHex("#FFB3B3B3") };
            var thumbDragGestureRecognizer = new PanGestureRecognizer();
            thumbDragGestureRecognizer.PanUpdated += OnThumbDragged;
            _thumbFrame.GestureRecognizers.Add(thumbDragGestureRecognizer);
            rootLayout.Children.Add(
                    _thumbFrame,
                    Constraint.RelativeToParent(p => p.Width / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                    Constraint.RelativeToParent(p => p.Height / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                    Constraint.RelativeToParent(p =>
                    {
                        var desiredWidth = 0.2 * Math.Min(p.Width, p.Height);

                        _thumbFrame.CornerRadius = (float)(desiredWidth / 2.0);
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
        private readonly Frame _backgroundFrame;

        private void OnThumbDragged(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:

                    this.AbortAnimation(ThumbToCenterAnimation);

                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    _thumbFrame.Scale = 0.85;
                    break;

                case GestureStatus.Running:
                    var newX = _thumbFrame.TranslationX + _getOffset(e.TotalX, previousX);
                    var newY = _thumbFrame.TranslationY + _getOffset(e.TotalY, previousY);

                    var newRadius = Math.Sqrt(Math.Pow(newX, 2) + Math.Pow(newY, 2));
                    var newTheta = Math.Atan2(newY, newX);

                    newRadius = Math.Min(_backgroundFrame.Width / 2.0, newRadius);

                    _thumbFrame.TranslationX = newRadius * Math.Cos(newTheta);
                    _thumbFrame.TranslationY = newRadius * Math.Sin(newTheta);

                    UpdateControllerOutputs();

                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    break;

                case GestureStatus.Completed:
                    _thumbFrame.Scale = 1.0;
                    AnimateThumbToCenter();
                    break;

                case GestureStatus.Canceled:
                    _thumbFrame.Scale = 1.0;
                    AnimateThumbToCenter();
                    break;
            }
        }

        private void AnimateThumbToCenter()
        {
            var centerAnimation = new Animation(d =>
            {
                _thumbFrame.TranslationX = d * _thumbFrame.TranslationX;
                _thumbFrame.TranslationY = d * _thumbFrame.TranslationY;
            }, 1.0, 0.0, Easing.CubicIn);

            centerAnimation.Commit(this, ThumbToCenterAnimation, length: 150);
        }

        private void UpdateControllerOutputs()
        {
            var smallerDimension = Math.Min(_backgroundFrame.Width, _backgroundFrame.Height);
            XAxis = _thumbFrame.TranslationX / (smallerDimension / 2.0);
            YAxis = _thumbFrame.TranslationY / (smallerDimension / 2.0);
        }
    }
}