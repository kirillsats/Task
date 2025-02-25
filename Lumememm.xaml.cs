using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BohatyrovMobile
{
    public partial class Lumememm : ContentPage
    {
        StackLayout st;
        Label lbl;
        Button btn, btnColor;
        Stepper stp;
        Slider sld, sld1, sld2, sld3;
        Frame box, box2, box3_vedro;
        AbsoluteLayout abs;
        Random rnd;

        public Lumememm(int v)
        {
            BackgroundColor = Colors.White;

            box = new Frame
            {
                CornerRadius = 200,
                BackgroundColor = Colors.White,
                WidthRequest = 130,
                HeightRequest = 130,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HasShadow = false,
                BorderColor = Colors.Transparent
            };

            box2 = new Frame
            {
                CornerRadius = 300,
                BackgroundColor = Colors.White,
                WidthRequest = 180,
                HeightRequest = 180,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HasShadow = false,
                BorderColor = Colors.Transparent
            };

            box3_vedro = new Frame
            {
                BackgroundColor = Colors.Orange,
                WidthRequest = 120,
                HeightRequest = 100,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 20,
                HasShadow = false,
                BorderColor = Colors.Transparent
            };

            lbl = new Label { Text = "Läbipaistvus: 100%" };

            sld = new Slider { Minimum = 0, Maximum = 1, Value = 1, MinimumTrackColor = Colors.White, MaximumTrackColor = Colors.Gray, ThumbColor = Colors.White };
            sld.ValueChanged += Sld_ValueChanged;

            btn = new Button { Text = "Peida", BackgroundColor = Colors.Black, TextColor = Colors.White };
            btn.Clicked += Btn_Clicked;

            btnColor = new Button { Text = "Random värv", BackgroundColor = Colors.Black, TextColor = Colors.White };
            btnColor.Clicked += BtnColor_Clicked;

            stp = new Stepper { Minimum = -50, Maximum = 50, Increment = 5, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.EndAndExpand };
            stp.ValueChanged += Stp_ValueChanged;

            sld1 = new Slider { WidthRequest = 300, Minimum = -200, Maximum = 200, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.EndAndExpand };
            sld1.ValueChanged += Sld1_ValueChanged;

            sld2 = new Slider { WidthRequest = 300, Minimum = -200, Maximum = 200, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.EndAndExpand };
            sld2.ValueChanged += Sld2_ValueChanged;

            sld3 = new Slider { WidthRequest = 300, Minimum = -200, Maximum = 200, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.EndAndExpand };
            sld3.ValueChanged += Sld3_ValueChanged;

            st = new StackLayout { Children = { lbl, sld, btn, btnColor, stp, sld1, sld2, sld3 } };

            abs = new AbsoluteLayout { Children = { st, box2, box, box3_vedro } };

            AbsoluteLayout.SetLayoutBounds(box3_vedro, new Rect(0.5, 0.03, 130, 100));
            AbsoluteLayout.SetLayoutFlags(box3_vedro, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(box, new Rect(0.5, 0.1, 200, 245));
            AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(box2, new Rect(0.5, 0.37, 200, 280));
            AbsoluteLayout.SetLayoutFlags(box2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(st, new Rect(0.5, 0.95, 300, 280));
            AbsoluteLayout.SetLayoutFlags(st, AbsoluteLayoutFlags.PositionProportional);

            Content = abs;
        }

        private void Sld3_ValueChanged(object sender, ValueChangedEventArgs e) => box.TranslationX = e.NewValue;
        private void Sld2_ValueChanged(object sender, ValueChangedEventArgs e) => box2.TranslationX = e.NewValue;
        private void Sld1_ValueChanged(object sender, ValueChangedEventArgs e) => box3_vedro.TranslationX = e.NewValue;

        private void Stp_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            box.TranslationX = e.NewValue;
            box3_vedro.TranslationX = e.NewValue;
            box2.TranslationX = e.NewValue;
        }

        private void BtnColor_Clicked(object sender, EventArgs e)
        {
            rnd = new Random();
            box3_vedro.BackgroundColor = Color.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            box.BackgroundColor = Color.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            box2.BackgroundColor = Color.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
        }

        private void Btn_Clicked(object sender, EventArgs e)
        {
            if (box.IsVisible && box2.IsVisible && box3_vedro.IsVisible)
            {
                box.IsVisible = false;
                box2.IsVisible = false;
                box3_vedro.IsVisible = false;
                sld.Value = 0;
            }
        }

        private void Sld_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            box.IsVisible = true;
            box2.IsVisible = true;
            box3_vedro.IsVisible = true;
            lbl.Text = $"Läbipaistmatus: {e.NewValue * 100:F1}%";
            box3_vedro.Opacity = e.NewValue;
            box.Opacity = e.NewValue;
            box2.Opacity = e.NewValue;
        }
    }
}
