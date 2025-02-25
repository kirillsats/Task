namespace TaskMaster;
public partial class Valgusfoor : ContentPage 
{
    bool trafficLight = false;
    List<BoxView> lights;
    Label statusLbl;
    String turnONTxt = "Lülitage esmalt valgusfoor sisse!";
    public Valgusfoor(int k)
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

        
        for (int i = 0; i < 3; i++)
        {
            lights.Add(new BoxView
            {
                Color = Colors.Gray,
                WidthRequest = 100,
                HeightRequest = 100,
                CornerRadius = 50,
            });
        };


        BoxView redLight = lights[0];
        BoxView yellowLight = lights[1];
        BoxView greenLight = lights[2];

        var redTapGestureRecognizer = new TapGestureRecognizer(); 
        redTapGestureRecognizer.Tapped += (s, e) => OnLightTapped("red");

        var yellowTapGestureRecognizer = new TapGestureRecognizer();
        yellowTapGestureRecognizer.Tapped += (s, e) => OnLightTapped("yellow");

        var greenTapGestureRecognizer = new TapGestureRecognizer();
        greenTapGestureRecognizer.Tapped += (s, e) => OnLightTapped("green");

        redLight.GestureRecognizers.Add(redTapGestureRecognizer); 
        yellowLight.GestureRecognizers.Add(yellowTapGestureRecognizer);
        greenLight.GestureRecognizers.Add(greenTapGestureRecognizer);


        StackLayout lightStack = new StackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 10,
            WidthRequest = 120,
            HeightRequest = 340,
            BackgroundColor = Colors.Black,
            Padding = 10,
            Margin = 10,
            Children = { redLight, yellowLight, greenLight },
        };

        Button on_btn = new Button
        {
            Text = "ON",
            Margin = 5,
        };
        on_btn.Clicked += on_btn_clicked;

        Button off_btn = new Button
        {
            Text = "OFF",
            Margin = 5,
        };

        off_btn.Clicked += Off_btn_Clicked;

        Button auto_btn = new Button
        {
            Text = "AUTO",
            Margin = 5,
        };
        auto_btn.Clicked += auto_btn_clicked;


        StackLayout mainL = new StackLayout
        {
            Children = { statusLbl, lightStack, on_btn, off_btn },
            Padding = new Thickness(20) 
        };

        Content = mainL;
    }
    private void Off_btn_Clicked(object? sender, EventArgs e)
    {
        trafficLight = false;
        foreach (var light in lights)
        {
            light.Color = Colors.Gray;
        }
        statusLbl.Text = turnONTxt;
    }

    private async void on_btn_clicked(object? sender, EventArgs e)
    {
        trafficLight = true;
        lights[0].Color = Colors.Red;
        lights[1].Color = Colors.Yellow;
        lights[2].Color = Colors.Green;

        statusLbl.Text = "Klõpsake värvil";
    }

    private async void auto_btn_clicked(object? sender, EventArgs e)
    {
        trafficLight = true;

        
    }
    

    private void OnLightTapped(string color)
    {
        if (!trafficLight)
        {
            statusLbl.Text = turnONTxt;
            return;
        }

        switch (color)
        {
            case "red":
                statusLbl.Text = "Peatus";
                break;
            case "yellow":
                statusLbl.Text = "Oota!";
                break;
            case "green":
                statusLbl.Text = "Sa võid minna";
                break;
        }
    }
}
