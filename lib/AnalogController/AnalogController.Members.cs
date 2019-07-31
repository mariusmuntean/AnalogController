using Xamarin.Forms;

namespace AnalogController
{
    public partial class AnalogController
    {
        public static readonly BindableProperty XAxisProperty = BindableProperty.Create(
            nameof(XAxis),
            typeof(double),
            typeof(AnalogController),
            0.0
        );

        public double XAxis { get => (double)GetValue(XAxisProperty); set => SetValue(XAxisProperty, value); }

        public static readonly BindableProperty YAxisProperty = BindableProperty.Create(
            nameof(YAxis),
            typeof(double),
            typeof(AnalogController),
            0.0
        );

        public double YAxis { get => (double)GetValue(YAxisProperty); set => SetValue(YAxisProperty, value); }
    }
}