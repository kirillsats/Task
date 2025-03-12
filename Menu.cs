using BohatyrovMobile;
using Microsoft.Maui.Controls;

namespace TaskMaster;

internal class Menu : ContentPage
{
    List<ContentPage> pages = new List<ContentPage>() { new MainPage(0), new Valgusfoor(1), new RGB_mudel(2), new Lumememm(3), new KN(4) };
    List<string> txt = new List<string> { "Kodu", "Valgusfoor", "RGB Mudel", "Lumememm", "Trips traps trull" };
    List<Button> btns = new List<Button>();
    public Menu()
    {
        StackLayout stackLayout = new StackLayout
        {
            Padding = 20,
        };

        for (int i = 0; i < pages.Count; i++)
        {
            Button btn = new Button
            {
                Text = txt[i],
                BackgroundColor = Colors.Blue,
                TextColor = Colors.White,
                FontSize = 28,
                CornerRadius = 5,
                Margin = 5,
            };
            btn.BindingContext = i; // stores the index of each btn
            btns.Add(btn);
            btn.Clicked += Btn_Clicked;

            stackLayout.Children.Add(btn);
        }

        Content = stackLayout;
    }
    private async void Btn_Clicked(object? sender, EventArgs e)
    {
        Button button = (Button)sender;
        int i = (int)button.BindingContext; //takes index from BindingContent
        await Navigation.PushAsync(pages[i]);
    }
}