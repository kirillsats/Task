namespace TaskMaster;
public partial class Valgusfoor : ContentPage
{
    bool trafficLight = false;
    List<BoxView> lights;
    Label statusLbl;
    string turnONTxt = "Lülitage esmalt valgusfoor sisse!";
    bool isDayMode = true; // Флаг для дневного/ночного режима

    public Valgusfoor(int v)
    {
        Title = "Valgusfoor";
        lights = new List<BoxView>(3);

        statusLbl = new Label
        {
            Text = turnONTxt,
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start
        };

        StackLayout lightStack = new StackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 10,
            WidthRequest = 120,
            HeightRequest = 340,
            BackgroundColor = Colors.Black,
            Padding = 10,
            Margin = 10
        };

        // Создание ламп со вставкой в рамку
        for (int i = 0; i < 3; i++)
        {
            var light = new BoxView
            {
                Color = Colors.Gray,
                WidthRequest = 100,
                HeightRequest = 100,
                CornerRadius = 50,
            };

            lights.Add(light);

            var frame = new Frame
            {
                Content = light,
                Padding = 5,
                BorderColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                HasShadow = false,
                CornerRadius = 55
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnLightTapped(light);
            light.GestureRecognizers.Add(tapGesture);

            lightStack.Children.Add(frame);
        }

        Button on_btn = new Button { Text = "Sisse", Margin = 5 };
        on_btn.Clicked += OnBtnClicked;

        Button off_btn = new Button { Text = "Välja", Margin = 5 };
        off_btn.Clicked += OffBtnClicked;

        Button mode_btn = new Button { Text = "Režiim", Margin = 5 };
        mode_btn.Clicked += ModeBtnClicked;

        StackLayout mainL = new StackLayout
        {
            Children = { statusLbl, lightStack, on_btn, off_btn, mode_btn },
            Padding = new Thickness(20),
            BackgroundColor = Colors.LightGray // Фон по умолчанию
        };

        Content = mainL;
    }

    private void OffBtnClicked(object sender, EventArgs e)
    {
        trafficLight = false;
        foreach (var light in lights)
        {
            light.Color = Colors.Gray;
        }
        statusLbl.Text = turnONTxt;
    }

    private void OnBtnClicked(object sender, EventArgs e)
    {
        trafficLight = true;
        lights[0].Color = Colors.Red;
        lights[1].Color = Colors.Yellow;
        lights[2].Color = Colors.Green;
        statusLbl.Text = "Klõpsake värvil";
    }

    private void OnLightTapped(BoxView light)
    {
        if (!trafficLight)
        {
            statusLbl.Text = turnONTxt;
            return;
        }

        if (light.Color == Colors.Red)
            statusLbl.Text = "Seisa!";
        else if (light.Color == Colors.Yellow)
            statusLbl.Text = "Oota!";
        else if (light.Color == Colors.Green)
            statusLbl.Text = "Mine!";
    }

    private async void ModeBtnClicked(object sender, EventArgs e)
    {
        // Начинаем цикл включения светофора с задержкой
        trafficLight = true;
        statusLbl.Text = "Režiim käivitub...";

        // Включаем только красный
        lights[0].Color = Colors.Red;
        lights[1].Color = Colors.Gray;
        lights[2].Color = Colors.Gray;
        await Task.Delay(3000);  // Красный горит 3 секунды

        // Включаем красный + желтый
        lights[1].Color = Colors.Yellow;
        await Task.Delay(2000);  // Красный + желтый горят 2 секунды

        // Включаем зеленый
        lights[0].Color = Colors.Gray;
        lights[1].Color = Colors.Gray;
        lights[2].Color = Colors.Green;
        await Task.Delay(3000);  // Зеленый горит 3 секунды

        // После этого возвращаемся к обычному состоянию
        lights[0].Color = Colors.Gray;
        lights[1].Color = Colors.Gray;
        lights[2].Color = Colors.Gray;
        statusLbl.Text = turnONTxt;
    }
}
