using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace AvaloniaShowcase.Views;

public partial class PlaygroundPage : UserControl
{
    private CancellationTokenSource? _debounce;

    public PlaygroundPage()
    {
        InitializeComponent();

        RunButton.Click += (_, _) => ParseAndRender();
        ClearButton.Click += (_, _) =>
        {
            CodeEditor.Text = "";
            PreviewHost.Content = null;
            ErrorBar.IsVisible = false;
            EmptyState.IsVisible = true;
            PreviewStatus.Text = "";
        };

        CodeEditor.TextChanged += OnCodeChanged;

        // Load a default template so preview isn't empty
        CodeEditor.Text = GetTemplate("button");
    }

    private void OnCodeChanged(object? sender, EventArgs e)
    {
        if (AutoRunCheck.IsChecked != true) return;

        // Debounce: wait 400ms after last keystroke before parsing
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;

        DispatcherTimer.RunOnce(() =>
        {
            if (!token.IsCancellationRequested)
                ParseAndRender();
        }, TimeSpan.FromMilliseconds(400));
    }

    private void ParseAndRender()
    {
        var code = CodeEditor.Text;
        if (string.IsNullOrWhiteSpace(code))
        {
            PreviewHost.Content = null;
            ErrorBar.IsVisible = false;
            EmptyState.IsVisible = true;
            PreviewStatus.Text = "";
            return;
        }

        try
        {
            var xaml = WrapNamespacesCheck.IsChecked == true
                ? WrapWithNamespaces(code)
                : code;

            var result = AvaloniaRuntimeXamlLoader.Parse(xaml);

            if (result is Control control)
            {
                PreviewHost.Content = control;
                EmptyState.IsVisible = false;
                ErrorBar.IsVisible = false;
                PreviewStatus.Text = $"Rendered: {control.GetType().Name}";
            }
            else if (result != null)
            {
                PreviewHost.Content = new TextBlock
                {
                    Text = $"Parsed object of type: {result.GetType().Name}\n(Not a visual Control — cannot display)",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Opacity = 0.6
                };
                EmptyState.IsVisible = false;
                ErrorBar.IsVisible = false;
                PreviewStatus.Text = $"Parsed: {result.GetType().Name} (non-visual)";
            }
        }
        catch (Exception ex)
        {
            ErrorBar.IsVisible = true;
            ErrorText.Text = ex.Message;
            PreviewStatus.Text = "Error";
        }
    }

    private static string WrapWithNamespaces(string code)
    {
        var trimmed = code.TrimStart();

        // If code already has a namespace declaration, don't wrap
        if (trimmed.Contains("xmlns="))
            return code;

        // Find the first tag name to inject namespaces into
        // e.g. <Button ... /> or <StackPanel>
        if (trimmed.StartsWith("<"))
        {
            var firstSpace = trimmed.IndexOf(' ');
            var firstClose = trimmed.IndexOf('>');
            var firstSlash = trimmed.IndexOf('/');

            int insertPos;
            if (firstSpace > 0 && firstSpace < firstClose)
                insertPos = firstSpace;
            else if (firstSlash > 0 && firstSlash < firstClose)
                insertPos = firstSlash;
            else
                insertPos = firstClose;

            if (insertPos > 0)
            {
                var tagName = trimmed[1..insertPos];
                // Build a wrapped version with proper namespaces
                return
$@"<{tagName}
    xmlns=""https://github.com/avaloniaui""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:FluentAvalonia.UI.Controls""
{trimmed[(1 + tagName.Length)..]}";
            }
        }

        // If it doesn't start with '<', wrap it in a Border
        return
$@"<Border xmlns=""https://github.com/avaloniaui""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:FluentAvalonia.UI.Controls"">
{code}
</Border>";
    }

    private void TemplateCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TemplateCombo.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            CodeEditor.Text = GetTemplate(tag);
            TemplateCombo.SelectedItem = null; // Reset so user can pick same again
        }
    }

    private static string GetTemplate(string key) => key switch
    {
        "button" =>
@"<StackPanel Spacing=""12"" HorizontalAlignment=""Center""
            VerticalAlignment=""Center"">
    <TextBlock Text=""Hello, Playground!""
               FontSize=""24"" FontWeight=""Bold""
               HorizontalAlignment=""Center"" />
    <TextBlock Text=""Edit this XAML and see changes live.""
               Opacity=""0.6"" HorizontalAlignment=""Center"" />
    <StackPanel Orientation=""Horizontal"" Spacing=""8""
                HorizontalAlignment=""Center"">
        <Button Content=""Primary"" Classes=""accent"" />
        <Button Content=""Secondary"" />
        <Button Content=""Disabled"" IsEnabled=""False"" />
    </StackPanel>
</StackPanel>",

        "stack" =>
@"<StackPanel Spacing=""16"" Margin=""20""
            MaxWidth=""400"">
    <TextBlock Text=""Control Stack"" FontSize=""20""
               FontWeight=""SemiBold"" />
    <TextBox Watermark=""Enter your name"" />
    <TextBox Watermark=""Enter your email"" />
    <ComboBox PlaceholderText=""Select a role"" Width=""200"">
        <ComboBoxItem Content=""Developer"" />
        <ComboBoxItem Content=""Designer"" />
        <ComboBoxItem Content=""Manager"" />
    </ComboBox>
    <CheckBox Content=""I agree to the terms"" />
    <ToggleSwitch OnContent=""Notifications On""
                  OffContent=""Notifications Off"" />
    <Slider Minimum=""0"" Maximum=""100"" Value=""50""
            TickFrequency=""10""
            TickPlacement=""BottomRight"" />
    <ProgressBar Value=""65"" Maximum=""100"" />
    <StackPanel Orientation=""Horizontal"" Spacing=""8"">
        <Button Content=""Submit"" Classes=""accent"" />
        <Button Content=""Cancel"" />
    </StackPanel>
</StackPanel>",

        "card" =>
@"<Border Background=""{DynamicResource CardBackgroundFillColorDefaultBrush}""
        BorderBrush=""{DynamicResource CardStrokeColorDefaultBrush}""
        BorderThickness=""1"" CornerRadius=""12""
        Padding=""24"" MaxWidth=""420""
        HorizontalAlignment=""Center""
        VerticalAlignment=""Center"">
    <StackPanel Spacing=""12"">
        <StackPanel Orientation=""Horizontal"" Spacing=""12"">
            <Border Width=""48"" Height=""48""
                    CornerRadius=""24""
                    Background=""{DynamicResource AccentFillColorDefaultBrush}"">
                <TextBlock Text=""AV"" FontSize=""18""
                           FontWeight=""Bold""
                           Foreground=""White""
                           HorizontalAlignment=""Center""
                           VerticalAlignment=""Center"" />
            </Border>
            <StackPanel VerticalAlignment=""Center"">
                <TextBlock Text=""Avalonia UI""
                           FontSize=""18"" FontWeight=""SemiBold"" />
                <TextBlock Text=""Cross-platform .NET UI""
                           FontSize=""13"" Opacity=""0.6"" />
            </StackPanel>
        </StackPanel>
        <TextBlock TextWrapping=""Wrap"" Opacity=""0.8""
            Text=""Build beautiful, cross-platform applications with Avalonia. Supports Windows, macOS, Linux, iOS, Android, and WebAssembly."" />
        <StackPanel Orientation=""Horizontal"" Spacing=""8"">
            <Button Content=""Learn More"" Classes=""accent"" />
            <Button Content=""GitHub"" />
        </StackPanel>
    </StackPanel>
</Border>",

        "form" =>
@"<Border Background=""{DynamicResource CardBackgroundFillColorDefaultBrush}""
        BorderBrush=""{DynamicResource CardStrokeColorDefaultBrush}""
        BorderThickness=""1"" CornerRadius=""12""
        Padding=""32"" MaxWidth=""380""
        HorizontalAlignment=""Center""
        VerticalAlignment=""Center"">
    <StackPanel Spacing=""16"">
        <TextBlock Text=""Sign In"" FontSize=""26""
                   FontWeight=""Bold""
                   HorizontalAlignment=""Center"" />
        <TextBlock Text=""Welcome back! Please enter your details.""
                   Opacity=""0.5"" FontSize=""13""
                   HorizontalAlignment=""Center""
                   Margin=""0,0,0,8"" />
        <StackPanel Spacing=""4"">
            <TextBlock Text=""Email"" FontSize=""13""
                       FontWeight=""SemiBold"" />
            <TextBox Watermark=""you@example.com"" />
        </StackPanel>
        <StackPanel Spacing=""4"">
            <TextBlock Text=""Password"" FontSize=""13""
                       FontWeight=""SemiBold"" />
            <TextBox Watermark=""••••••••""
                     PasswordChar=""*""
                     RevealPassword=""True"" />
        </StackPanel>
        <CheckBox Content=""Remember me"" />
        <Button Content=""Sign In"" Classes=""accent""
                HorizontalAlignment=""Stretch""
                HorizontalContentAlignment=""Center""
                Padding=""0,10"" />
        <TextBlock HorizontalAlignment=""Center""
                   Opacity=""0.5"" FontSize=""12"">
            <Run Text=""Don't have an account? "" />
            <Run Text=""Sign Up"" FontWeight=""SemiBold""
                 Foreground=""{DynamicResource AccentFillColorDefaultBrush}"" />
        </TextBlock>
    </StackPanel>
</Border>",

        "dashboard" =>
@"<StackPanel Spacing=""16"" Margin=""16"">
    <TextBlock Text=""Dashboard"" FontSize=""24""
               FontWeight=""Bold"" />
    <WrapPanel Orientation=""Horizontal"">
        <!-- Tile 1 -->
        <Border Background=""{DynamicResource CardBackgroundFillColorDefaultBrush}""
                BorderBrush=""{DynamicResource CardStrokeColorDefaultBrush}""
                BorderThickness=""1"" CornerRadius=""8""
                Padding=""20"" Margin=""0,0,12,12""
                Width=""180"">
            <StackPanel>
                <TextBlock Text=""Users"" Opacity=""0.5""
                           FontSize=""13"" />
                <TextBlock Text=""1,248"" FontSize=""28""
                           FontWeight=""Bold"" />
                <ProgressBar Value=""72"" Maximum=""100""
                             Margin=""0,8,0,0"" />
            </StackPanel>
        </Border>
        <!-- Tile 2 -->
        <Border Background=""{DynamicResource CardBackgroundFillColorDefaultBrush}""
                BorderBrush=""{DynamicResource CardStrokeColorDefaultBrush}""
                BorderThickness=""1"" CornerRadius=""8""
                Padding=""20"" Margin=""0,0,12,12""
                Width=""180"">
            <StackPanel>
                <TextBlock Text=""Revenue"" Opacity=""0.5""
                           FontSize=""13"" />
                <TextBlock Text=""$48.5k"" FontSize=""28""
                           FontWeight=""Bold"" />
                <ProgressBar Value=""85"" Maximum=""100""
                             Margin=""0,8,0,0"" />
            </StackPanel>
        </Border>
        <!-- Tile 3 -->
        <Border Background=""{DynamicResource CardBackgroundFillColorDefaultBrush}""
                BorderBrush=""{DynamicResource CardStrokeColorDefaultBrush}""
                BorderThickness=""1"" CornerRadius=""8""
                Padding=""20"" Margin=""0,0,12,12""
                Width=""180"">
            <StackPanel>
                <TextBlock Text=""Orders"" Opacity=""0.5""
                           FontSize=""13"" />
                <TextBlock Text=""384"" FontSize=""28""
                           FontWeight=""Bold"" />
                <ProgressBar Value=""45"" Maximum=""100""
                             Margin=""0,8,0,0"" />
            </StackPanel>
        </Border>
        <!-- Tile 4 -->
        <Border Background=""{DynamicResource CardBackgroundFillColorDefaultBrush}""
                BorderBrush=""{DynamicResource CardStrokeColorDefaultBrush}""
                BorderThickness=""1"" CornerRadius=""8""
                Padding=""20"" Margin=""0,0,12,12""
                Width=""180"">
            <StackPanel>
                <TextBlock Text=""Uptime"" Opacity=""0.5""
                           FontSize=""13"" />
                <TextBlock Text=""99.9%"" FontSize=""28""
                           FontWeight=""Bold"" />
                <ProgressBar Value=""99"" Maximum=""100""
                             Margin=""0,8,0,0"" />
            </StackPanel>
        </Border>
    </WrapPanel>
</StackPanel>",

        "gallery" =>
@"<StackPanel Spacing=""16"" Margin=""16"">
    <TextBlock Text=""Color Palette"" FontSize=""20""
               FontWeight=""SemiBold"" />
    <WrapPanel Orientation=""Horizontal"">
        <Border Width=""100"" Height=""100"" CornerRadius=""8""
                Background=""#FF6B6B"" Margin=""0,0,8,8"">
            <TextBlock Text=""Red"" Foreground=""White""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       FontWeight=""SemiBold"" />
        </Border>
        <Border Width=""100"" Height=""100"" CornerRadius=""8""
                Background=""#4ECDC4"" Margin=""0,0,8,8"">
            <TextBlock Text=""Teal"" Foreground=""White""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       FontWeight=""SemiBold"" />
        </Border>
        <Border Width=""100"" Height=""100"" CornerRadius=""8""
                Background=""#45B7D1"" Margin=""0,0,8,8"">
            <TextBlock Text=""Sky"" Foreground=""White""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       FontWeight=""SemiBold"" />
        </Border>
        <Border Width=""100"" Height=""100"" CornerRadius=""8""
                Background=""#96CEB4"" Margin=""0,0,8,8"">
            <TextBlock Text=""Sage"" Foreground=""White""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       FontWeight=""SemiBold"" />
        </Border>
        <Border Width=""100"" Height=""100"" CornerRadius=""8""
                Background=""#FFEAA7"" Margin=""0,0,8,8"">
            <TextBlock Text=""Gold"" Foreground=""#333""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       FontWeight=""SemiBold"" />
        </Border>
        <Border Width=""100"" Height=""100"" CornerRadius=""8""
                Background=""#DDA0DD"" Margin=""0,0,8,8"">
            <TextBlock Text=""Plum"" Foreground=""White""
                       HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""
                       FontWeight=""SemiBold"" />
        </Border>
    </WrapPanel>
    <TextBlock Text=""Try changing the colors above!""
               Opacity=""0.5"" FontSize=""13"" />
</StackPanel>",

        _ => ""
    };
}
