using System;
using Xamarin.Forms;

namespace AnalogController
{
    public class AnalogController : ContentView
    {
        BoxView thumbBoxView;

        public AnalogController()
        {

            var rootLayout = new RelativeLayout();

            var backgroundBoxView = new BoxView() { BackgroundColor = Color.LightGray };
            rootLayout.Children.Add(
                    backgroundBoxView,
                    Constraint.RelativeToParent(p => 0.0),
                    Constraint.RelativeToParent(p => 0.0),
                    Constraint.RelativeToParent(p =>
                    {
                        backgroundBoxView.CornerRadius = Math.Min(p.Width, p.Height) / 2.0;
                        return p.Width;
                    }
                        ),
                    Constraint.RelativeToParent(p => p.Height)
                );

            thumbBoxView = new BoxView() { Color = Color.FromHex("#FFB3B3B3") };
            var thumbDragGestureRecognizer = new PanGestureRecognizer();
            thumbDragGestureRecognizer.PanUpdated += OnThumbDragged;
            thumbBoxView.GestureRecognizers.Add(thumbDragGestureRecognizer);
            rootLayout.Children.Add(
                    thumbBoxView,
                    Constraint.RelativeToParent(p => p.Width / 2.0 - (0.1 * p.Width)),
                    Constraint.RelativeToParent(p => p.Height / 2.0 - (0.1 * p.Height)),
                    Constraint.RelativeToParent(p =>
                    {
                        var desiredWidth = 0.2 * p.Width;

                        thumbBoxView.CornerRadius = desiredWidth / 2.0;
                        return desiredWidth;
                    }),
                    Constraint.RelativeToParent(p => 0.2 * p.Height)
                );

            this.Content = rootLayout;
        }

        double previousX = 0.0;
        double previousY = 0.0;

        private void OnThumbDragged(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    thumbBoxView.Scale = 0.85;
                    break;
                case GestureStatus.Running:
                    thumbBoxView.TranslationX += e.TotalX - previousX;
                    thumbBoxView.TranslationY += e.TotalY - previousY;


                    previousX = e.TotalX;
                    previousY = e.TotalY;
                    break;
                case GestureStatus.Completed:
                    thumbBoxView.Scale = 1.0;
                    break;
                case GestureStatus.Canceled:
                    thumbBoxView.Scale = 1.0;
                    break;
            }
        }
    }
}