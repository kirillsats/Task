namespace TaskMaster;

public partial class MainPage : ContentPage
{
    public MainPage(int k)
    {
        Title = "Home";
        Content = new Label
        {
            Text = "Welcome to home!",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
    }
}
