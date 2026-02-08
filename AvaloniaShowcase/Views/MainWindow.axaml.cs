using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace AvaloniaShowcase.Views;

public partial class MainWindow : AppWindow
{
    private readonly HomePage _homePage;
    private readonly ShowcasePage _showcasePage;
    private readonly PlaygroundPage _playgroundPage;

    public MainWindow()
    {
        InitializeComponent();

        // Style the title bar to match the NavigationView pane background
        TitleBar.BackgroundColor = Color.FromRgb(32, 32, 32);
        TitleBar.ForegroundColor = Colors.White;
        TitleBar.InactiveBackgroundColor = Color.FromRgb(32, 32, 32);
        TitleBar.InactiveForegroundColor = Color.FromRgb(160, 160, 160);
        TitleBar.ButtonBackgroundColor = Color.FromRgb(32, 32, 32);
        TitleBar.ButtonForegroundColor = Colors.White;
        TitleBar.ButtonHoverBackgroundColor = Color.FromRgb(50, 50, 50);
        TitleBar.ButtonHoverForegroundColor = Colors.White;
        TitleBar.ButtonPressedBackgroundColor = Color.FromRgb(70, 70, 70);
        TitleBar.ButtonPressedForegroundColor = Colors.White;
        TitleBar.ButtonInactiveBackgroundColor = Color.FromRgb(32, 32, 32);
        TitleBar.ButtonInactiveForegroundColor = Color.FromRgb(160, 160, 160);

        _homePage = new HomePage();
        _showcasePage = new ShowcasePage();
        _playgroundPage = new PlaygroundPage();

        // Select the first item (Home) by default
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            ContentFrame.Content = tag switch
            {
                "HomePage" => _homePage,
                "ShowcasePage" => _showcasePage,
                "PlaygroundPage" => _playgroundPage,
                _ => _homePage
            };
        }
    }
}
