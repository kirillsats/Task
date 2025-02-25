using Microsoft.Maui.Controls.Shapes;

namespace TaskMaster
{
    public partial class RGB_mudel : ContentPage
    {
        Border ColorBox;
        Border ColorCodeBox;
        Label ColorCodeLabel;
        Frame TitleBar;
        Label RedLabel;
        Slider RedSlider;
        Label GreenLabel;
        Slider GreenSlider;
        Label BlueLabel;
        Slider BlueSlider;
        Label RadiusLabel;
        Slider RadiusSlider;


        AbsoluteLayout MainBody;

        public RGB_mudel(int k)
        {
            Title = "";

            TitleBar = new Frame
            {
                BackgroundColor = Colors.LightGray,
                Content = new Label
                {
                    Text = "RBG mudeli",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                CornerRadius = 0,
                Padding = 0,

            };
            RedLabel = new Label
            {
                BackgroundColor = Color.FromRgb(255, 255, 255),
                Text = "Red = 0.0%",
            };
            RedSlider = new Slider
            {
                Minimum = 0,
                Maximum = 255,
                Value = 0,
            };

            GreenLabel = new Label
            {
                BackgroundColor = Color.FromRgb(255, 255, 255),
                Text = "Green = 0.0%",
            };
            GreenSlider = new Slider
            {
                Minimum = 0,
                Maximum = 255,
                Value = 0,
            };
            BlueLabel = new Label
            {
                BackgroundColor = Color.FromRgb(255, 255, 255),
                Text = "Blue = 0.0%",
            };
            BlueSlider = new Slider
            {
                Minimum = 0,
                Maximum = 255,
                Value = 0,
            };

            RadiusLabel = new Label
            {
                BackgroundColor = Color.FromRgb(255, 255, 255),
                Text = "Radius",
            };
            RadiusSlider = new Slider
            {
                Minimum = 0,
                Maximum = 150,
                Value = 0,
            };

            ColorBox = new Border
            {
                BackgroundColor = Color.FromRgb(0, 0, 0),
                Stroke = Color.FromRgb(0, 0, 0),
                StrokeThickness = 4,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(0, 0, 0, 0)
                },
            };

            ColorCodeLabel = new Label
            {
                Text = "#000000",
                TextColor = Colors.Black,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                Padding = 4,
            };

            ColorCodeBox = new Border
            {
                Content = ColorCodeLabel,
                BackgroundColor = Colors.LightGray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(4, 4, 4, 4)
                },
            };

            RedSlider.ValueChanged += Sl_Value_Changed;
            GreenSlider.ValueChanged += Sl_Value_Changed;
            BlueSlider.ValueChanged += Sl_Value_Changed;
            RadiusSlider.ValueChanged += Sl_Value_Changed;

            MainBody = new AbsoluteLayout { Children = { RedLabel, RedSlider, GreenLabel, GreenSlider, BlueLabel, BlueSlider, ColorCodeBox, ColorBox, RadiusLabel, RadiusSlider, TitleBar } };

            int CenterPoint = 40;

            AbsoluteLayout.SetLayoutBounds(TitleBar, new Rect(0, 0, 400, 50));

            AbsoluteLayout.SetLayoutBounds(ColorBox, new Rect(CenterPoint, 100, 300, 300));
            AbsoluteLayout.SetLayoutBounds(ColorCodeBox, new Rect(150, 400, 100, 40));

            AbsoluteLayout.SetLayoutBounds(RedLabel, new Rect(CenterPoint, 450, 300, 60));
            AbsoluteLayout.SetLayoutBounds(RedSlider, new Rect(CenterPoint, 450, 300, 60));

            AbsoluteLayout.SetLayoutBounds(GreenLabel, new Rect(CenterPoint, 500, 300, 60));
            AbsoluteLayout.SetLayoutBounds(GreenSlider, new Rect(CenterPoint, 500, 300, 60));

            AbsoluteLayout.SetLayoutBounds(BlueLabel, new Rect(CenterPoint, 550, 300, 60));
            AbsoluteLayout.SetLayoutBounds(BlueSlider, new Rect(CenterPoint, 550, 300, 60));

            AbsoluteLayout.SetLayoutBounds(RadiusLabel, new Rect(CenterPoint, 600, 300, 60));
            AbsoluteLayout.SetLayoutBounds(RadiusSlider, new Rect(CenterPoint, 600, 300, 60));


            Content = MainBody;
        }
        private void Sl_Value_Changed(object sender, ValueChangedEventArgs e)
        {
            if (sender == RedSlider) { RedLabel.Text = String.Format("Red = {0:F1}%", ((float)e.NewValue / 255.0) * 100); }
            if (sender == GreenSlider) { GreenLabel.Text = String.Format("Green = {0:F1}%", ((float)e.NewValue / 255.0) * 100); }
            if (sender == BlueSlider) { BlueLabel.Text = String.Format("Blue = {0:F1}%", ((float)e.NewValue / 255.0) * 100); }
            if (sender == RadiusSlider)
            {
                ColorBox.StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius((int)e.NewValue, (int)e.NewValue, (int)e.NewValue, (int)e.NewValue)
                };
            }
            int based = 16;

            string r = Convert.ToString((int)RedSlider.Value, based);
            string g = Convert.ToString((int)GreenSlider.Value, based);
            string b = Convert.ToString((int)BlueSlider.Value, based);

            if (r.Length == 1) { r = "0" + r; }
            if (g.Length == 1) { g = "0" + g; }
            if (b.Length == 1) { b = "0" + b; }

            string hexText = "#" + r + g + b;

            //Console.WriteLine(DeviceDisplay.Current.MainDisplayInfo.Width);

            ColorCodeLabel.Text = hexText;
            ColorBox.BackgroundColor = Color.FromRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
        }
    }
}