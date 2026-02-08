using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;

namespace AvaloniaShowcase.Views;

public partial class ShowcasePage : UserControl
{
    private int _standardClickCount;
    private string _dialogResult = "none yet";

    private string? _lastButtonControl;
    private string? _lastTextInputControl;
    private string? _lastSelectionControl;
    private string? _lastProgressControl;
    private string? _lastInfoBarControl;
    private string? _lastBadgeControl;
    private string? _lastDateTimeControl;
    private string? _lastFlyoutControl;
    private string? _lastColorControl;
    private string? _lastIconName;
    private string? _lastCommandName;

    private const string Hint = "Interact with any control above to see its code here.";

    public ShowcasePage()
    {
        InitializeComponent();

        AutoCompleteBoxDemo.ItemsSource = new[]
        {
            "Apple", "Banana", "Cherry", "Date", "Elderberry",
            "Fig", "Grape", "Honeydew", "Kiwi", "Lemon",
            "Mango", "Nectarine", "Orange", "Papaya", "Quince",
            "Raspberry", "Strawberry", "Tangerine", "Watermelon"
        };

        SampleDataGrid.ItemsSource = new List<ProductItem>
        {
            new(1, "Wireless Mouse", "Electronics", 29.99m, true),
            new(2, "Mechanical Keyboard", "Electronics", 89.50m, true),
            new(3, "USB-C Hub", "Accessories", 45.00m, false),
            new(4, "Monitor Stand", "Furniture", 120.00m, true),
            new(5, "Webcam HD", "Electronics", 59.99m, true),
            new(6, "Desk Lamp", "Furniture", 34.95m, false),
            new(7, "Mouse Pad XL", "Accessories", 15.00m, true),
            new(8, "Laptop Sleeve", "Accessories", 22.50m, true),
        };

        if (Application.Current?.Styles[0] is FluentAvaloniaTheme theme)
        {
            if (theme.CustomAccentColor.HasValue)
                DemoColorPicker.Color = theme.CustomAccentColor.Value;
            else
                DemoColorPicker.Color = Color.FromRgb(0, 120, 212);
        }

        WireUpEvents();
        WireCopyButtons();
        UpdateSelectionStatus();

        // Initial hints
        ButtonsCode.Text = Hint;
        TextInputCode.Text = Hint;
        SelectionCode.Text = Hint;
        ProgressCode.Text = Hint;
        InfoBarCode.Text = Hint;
        BadgesCode.Text = Hint;
        TabControlCode.Text = Hint;
        DateTimeCode.Text = Hint;
        DialogCode.Text = Hint;
        FlyoutsCode.Text = Hint;
        DataGridCode.Text = Hint;
        ColorPickerCode.Text = Hint;
        IconsCode.Text = Hint;
        CommandBarCode.Text = Hint;

        // Static expander code
        ExpanderCode.Text =
@"<Expander Header=""Click to expand"">
    <TextBlock Text=""Expanded content area."" />
</Expander>

<Expander Header=""Initially expanded"" IsExpanded=""True"">
    <StackPanel Spacing=""8"">
        <TextBlock Text=""Content is visible by default."" />
        <Button Content=""A button inside"" />
    </StackPanel>
</Expander>";
    }

    // ═══════════════════════════════════════════════════
    //  COPY BUTTONS — one per code block
    // ═══════════════════════════════════════════════════

    private void WireCopyButtons()
    {
        WireCopy(CopyButtonsCode, ButtonsCode);
        WireCopy(CopyTextInputCode, TextInputCode);
        WireCopy(CopySelectionCode, SelectionCode);
        WireCopy(CopyProgressCode, ProgressCode);
        WireCopy(CopyInfoBarCode, InfoBarCode);
        WireCopy(CopyBadgesCode, BadgesCode);
        WireCopy(CopyExpanderCode, ExpanderCode);
        WireCopy(CopyTabControlCode, TabControlCode);
        WireCopy(CopyDateTimeCode, DateTimeCode);
        WireCopy(CopyDialogCode, DialogCode);
        WireCopy(CopyFlyoutsCode, FlyoutsCode);
        WireCopy(CopyDataGridCode, DataGridCode);
        WireCopy(CopyColorPickerCode, ColorPickerCode);
        WireCopy(CopyIconsCode, IconsCode);
        WireCopy(CopyCommandBarCode, CommandBarCode);
    }

    private void WireCopy(Button btn, TextBlock codeBlock)
    {
        btn.Click += async (_, _) =>
        {
            var text = codeBlock.Text;
            if (string.IsNullOrWhiteSpace(text) || text == Hint) return;
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(text);
                btn.Content = "Copied!";
                DispatcherTimer.RunOnce(() => btn.Content = "Copy", TimeSpan.FromSeconds(2));
            }
        };
    }

    // ═══════════════════════════════════════════════════
    //  EVENT WIRING
    // ═══════════════════════════════════════════════════

    private void WireUpEvents()
    {
        // === BUTTONS ===
        StandardButton.Click += (_, _) =>
        {
            _standardClickCount++;
            _lastButtonControl = "standard";
            ButtonStatus.Text = $"Standard button clicked {_standardClickCount} time(s).";
            UpdateButtonsCode();
        };
        AccentButton.Click += async (_, _) =>
        {
            _lastButtonControl = "accent";
            ButtonStatus.Text = "Accent button clicked — action triggered!";
            UpdateButtonsCode();
            var dialog = new ContentDialog { Title = "Accent Action", Content = "You clicked the accent button.", PrimaryButtonText = "Got it", DefaultButton = ContentDialogButton.Primary };
            var tl = TopLevel.GetTopLevel(this);
            if (tl != null) await dialog.ShowAsync(tl);
        };
        HyperlinkBtn.Click += async (_, _) =>
        {
            _lastButtonControl = "hyperlink";
            ButtonStatus.Text = "Hyperlink button opened a dialog.";
            UpdateButtonsCode();
            var dialog = new ContentDialog { Title = "Hyperlink Navigation", Content = "In a real app this would navigate to a URL.\n\nhttps://avaloniaui.net", PrimaryButtonText = "OK", DefaultButton = ContentDialogButton.Primary };
            var tl = TopLevel.GetTopLevel(this);
            if (tl != null) await dialog.ShowAsync(tl);
        };
        DemoToggleButton.IsCheckedChanged += (_, _) =>
        {
            _lastButtonControl = "toggle";
            ButtonStatus.Text = $"Toggle button is now: {(DemoToggleButton.IsChecked == true ? "ON" : "OFF")}";
            UpdateButtonsCode();
        };
        DemoSplitButton.Click += (_, _) => { _lastButtonControl = "split"; ButtonStatus.Text = "Split button main action clicked."; UpdateButtonsCode(); };
        SplitOptA.Click += (_, _) => { _lastButtonControl = "split_a"; ButtonStatus.Text = "Split menu: Option A selected."; UpdateButtonsCode(); };
        SplitOptB.Click += (_, _) => { _lastButtonControl = "split_b"; ButtonStatus.Text = "Split menu: Option B selected."; UpdateButtonsCode(); };
        SplitOptC.Click += (_, _) => { _lastButtonControl = "split_c"; ButtonStatus.Text = "Split menu: Option C selected."; UpdateButtonsCode(); };
        DropItem1.Click += (_, _) => { _lastButtonControl = "drop_1"; ButtonStatus.Text = "Dropdown: Item 1 selected."; UpdateButtonsCode(); };
        DropItem2.Click += (_, _) => { _lastButtonControl = "drop_2"; ButtonStatus.Text = "Dropdown: Item 2 selected."; UpdateButtonsCode(); };
        DropItem3.Click += (_, _) => { _lastButtonControl = "drop_3"; ButtonStatus.Text = "Dropdown: Item 3 selected."; UpdateButtonsCode(); };

        // === TEXT INPUT ===
        DemoTextBox.TextChanged += (_, _) => { _lastTextInputControl = "textbox"; CharCountLabel.Text = $"Characters: {DemoTextBox.Text?.Length ?? 0}"; UpdateTextInputCode(); };
        DemoPasswordBox.TextChanged += (_, _) => { _lastTextInputControl = "password"; UpdateTextInputCode(); };
        DemoMultilineBox.TextChanged += (_, _) => { _lastTextInputControl = "multiline"; UpdateTextInputCode(); };
        AutoCompleteBoxDemo.TextChanged += (_, _) =>
        {
            _lastTextInputControl = "autocomplete";
            AutoCompleteStatus.Text = string.IsNullOrEmpty(AutoCompleteBoxDemo.SelectedItem?.ToString())
                ? $"Typed: {AutoCompleteBoxDemo.Text}" : $"Selected fruit: {AutoCompleteBoxDemo.SelectedItem}";
            UpdateTextInputCode();
        };
        AutoCompleteBoxDemo.SelectionChanged += (_, _) =>
        {
            _lastTextInputControl = "autocomplete";
            var sel = AutoCompleteBoxDemo.SelectedItem?.ToString();
            AutoCompleteStatus.Text = string.IsNullOrEmpty(sel) ? "No fruit selected." : $"Selected fruit: {sel}";
            UpdateTextInputCode();
        };
        DemoNumberBox.ValueChanged += (_, _) => { _lastTextInputControl = "numberbox"; NumberBoxStatus.Text = $"Value: {DemoNumberBox.Value}"; UpdateTextInputCode(); };

        // === SELECTION ===
        Check1.IsCheckedChanged += (_, _) => { _lastSelectionControl = "checkbox"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        Check2.IsCheckedChanged += (_, _) => { _lastSelectionControl = "checkbox"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        Check3.IsCheckedChanged += (_, _) => { _lastSelectionControl = "checkbox"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        RadioA.IsCheckedChanged += (_, _) => { _lastSelectionControl = "radio"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        RadioB.IsCheckedChanged += (_, _) => { _lastSelectionControl = "radio"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        RadioC.IsCheckedChanged += (_, _) => { _lastSelectionControl = "radio"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        DemoToggleSwitch.IsCheckedChanged += (_, _) => { _lastSelectionControl = "toggleswitch"; UpdateSelectionStatus(); UpdateSelectionCode(); };
        DemoComboBox.SelectionChanged += (_, _) =>
        {
            _lastSelectionControl = "combobox";
            if (DemoComboBox.SelectedItem is ComboBoxItem item) ComboStatus.Text = $"Selected: {item.Content}";
            UpdateSelectionCode();
        };
        DemoListBox.SelectionChanged += (_, _) => { _lastSelectionControl = "listbox"; ListBoxStatus.Text = $"Selected: {DemoListBox.SelectedItems?.Count ?? 0} item(s)"; UpdateSelectionCode(); };
        DemoSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property.Name == "Value")
            {
                _lastSelectionControl = "slider";
                var v = (int)DemoSlider.Value;
                SliderValueLabel.Text = v.ToString();
                SliderPreview.Opacity = v / 100.0;
                UpdateSelectionCode();
            }
        };

        // === PROGRESS ===
        ProgressSlider.PropertyChanged += (_, e) => { if (e.Property.Name == "Value") { _lastProgressControl = "determinate"; DemoProgressBar.Value = ProgressSlider.Value; UpdateProgressCode(); } };
        IndeterminateToggle.IsCheckedChanged += (_, _) => { _lastProgressControl = "indeterminate"; IndeterminateBar.IsIndeterminate = IndeterminateToggle.IsChecked == true; UpdateProgressCode(); };
        RingIndeterminateBtn.Click += (_, _) => { _lastProgressControl = "ring_indeterminate"; UpdateProgressCode(); };
        RingDeterminateBtn.Click += (_, _) => { _lastProgressControl = "ring_determinate"; UpdateProgressCode(); };

        // === INFO BAR ===
        ResetInfoBarsButton.Click += (_, _) => { _lastInfoBarControl = "reset"; InfoBarInfo.IsOpen = true; InfoBarSuccess.IsOpen = true; InfoBarWarning.IsOpen = true; InfoBarError.IsOpen = true; UpdateInfoBarCode(); };
        InfoBarInfo.CloseButtonClick += (_, _) => { _lastInfoBarControl = "info"; UpdateInfoBarCode(); };
        InfoBarSuccess.CloseButtonClick += (_, _) => { _lastInfoBarControl = "success"; UpdateInfoBarCode(); };
        InfoBarWarning.CloseButtonClick += (_, _) => { _lastInfoBarControl = "warning"; UpdateInfoBarCode(); };
        InfoBarError.CloseButtonClick += (_, _) => { _lastInfoBarControl = "error"; UpdateInfoBarCode(); };

        // === TOOLTIPS (Show Code buttons) ===
        ShowSimpleTooltipCode.Click += (_, _) =>
        {
            _lastBadgeControl = "tooltip_simple";
            UpdateBadgesCode();
        };
        ShowRichTooltipCode.Click += (_, _) =>
        {
            _lastBadgeControl = "tooltip_rich";
            UpdateBadgesCode();
        };

        // === BADGES ===
        BadgeIncrement.Click += (_, _) => { _lastBadgeControl = "increment"; DemoBadge.Value = Math.Min(99, DemoBadge.Value + 1); UpdateBadgesCode(); };
        BadgeDecrement.Click += (_, _) => { _lastBadgeControl = "decrement"; DemoBadge.Value = Math.Max(0, DemoBadge.Value - 1); UpdateBadgesCode(); };

        // === TAB CONTROL ===
        DemoTabControl.SelectionChanged += (_, _) => { if (DemoTabControl.SelectedItem is TabItem tab) TabStatus.Text = $"Active tab: {tab.Header}"; UpdateTabControlCode(); };

        // === DATE / TIME ===
        DemoCalendarPicker.SelectedDateChanged += (_, _) =>
        {
            _lastDateTimeControl = "calendar";
            DateTimeStatus.Text = DemoCalendarPicker.SelectedDate.HasValue ? $"Calendar: {DemoCalendarPicker.SelectedDate.Value:yyyy-MM-dd}" : "Calendar: (cleared)";
            UpdateDateTimeCode();
        };
        DemoDatePicker.SelectedDateChanged += (_, _) =>
        {
            _lastDateTimeControl = "datepicker";
            DateTimeStatus.Text = DemoDatePicker.SelectedDate.HasValue ? $"DatePicker: {DemoDatePicker.SelectedDate.Value:yyyy-MM-dd}" : "DatePicker: (cleared)";
            UpdateDateTimeCode();
        };
        DemoTimePicker.SelectedTimeChanged += (_, _) =>
        {
            _lastDateTimeControl = "timepicker";
            DateTimeStatus.Text = DemoTimePicker.SelectedTime.HasValue ? $"TimePicker: {DemoTimePicker.SelectedTime.Value:hh\\:mm}" : "TimePicker: (cleared)";
            UpdateDateTimeCode();
        };

        // === CONTENT DIALOG ===
        ShowDialogButton.Click += async (_, _) =>
        {
            var dialog = new ContentDialog { Title = "Sample Dialog", Content = "This is a ContentDialog from FluentAvalonia.\nChoose an option below.", PrimaryButtonText = "OK", SecondaryButtonText = "Maybe", CloseButtonText = "Cancel", DefaultButton = ContentDialogButton.Primary };
            var tl = TopLevel.GetTopLevel(this);
            if (tl != null) { var result = await dialog.ShowAsync(tl); _dialogResult = result.ToString(); DialogStatus.Text = $"Dialog result: {result}"; UpdateDialogCode(); }
        };

        // === FLYOUTS ===
        FlyoutActionBtn.Click += (_, _) => { _lastFlyoutControl = "flyout"; FlyoutStatus.Text = "Flyout action button clicked — flyout closed."; FlyoutBase.GetAttachedFlyout(FlyoutActionBtn)?.Hide(); UpdateFlyoutsCode(); };
        CtxCut.Click += (_, _) => { _lastFlyoutControl = "Cut"; FlyoutStatus.Text = "Menu action: Cut"; UpdateFlyoutsCode(); };
        CtxCopy.Click += (_, _) => { _lastFlyoutControl = "Copy"; FlyoutStatus.Text = "Menu action: Copy"; UpdateFlyoutsCode(); };
        CtxPaste.Click += (_, _) => { _lastFlyoutControl = "Paste"; FlyoutStatus.Text = "Menu action: Paste"; UpdateFlyoutsCode(); };
        CtxSelectAll.Click += (_, _) => { _lastFlyoutControl = "Select All"; FlyoutStatus.Text = "Menu action: Select All"; UpdateFlyoutsCode(); };

        // === DATA GRID ===
        SampleDataGrid.SelectionChanged += (_, _) =>
        {
            if (SampleDataGrid.SelectedItem is ProductItem p) DataGridStatus.Text = $"Selected: {p.Name} | {p.Price:C} | {(p.InStock ? "In Stock" : "Out of Stock")}";
            else DataGridStatus.Text = "No row selected.";
            UpdateDataGridCode();
        };

        // === COLOR PICKER ===
        DemoColorPicker.ColorChanged += (_, _) =>
        {
            _lastColorControl = "picker";
            var c = DemoColorPicker.Color;
            if (Application.Current?.Styles[0] is FluentAvaloniaTheme t) t.CustomAccentColor = Color.FromRgb(c.R, c.G, c.B);
            ColorStatus.Text = $"Accent color: #{c.R:X2}{c.G:X2}{c.B:X2}";
            UpdateColorPickerCode();
        };
        ResetAccentButton.Click += (_, _) =>
        {
            _lastColorControl = "reset";
            if (Application.Current?.Styles[0] is FluentAvaloniaTheme t) t.CustomAccentColor = null;
            ColorStatus.Text = "Accent color: reset to system default.";
            UpdateColorPickerCode();
        };

        // === ICONS ===
        WireIconButton(IconHome, "Home"); WireIconButton(IconSetting, "Setting"); WireIconButton(IconMail, "Mail");
        WireIconButton(IconSave, "Save"); WireIconButton(IconDelete, "Delete"); WireIconButton(IconEdit, "Edit");
        WireIconButton(IconFind, "Find"); WireIconButton(IconPeople, "People"); WireIconButton(IconStar, "Star");
        WireIconButton(IconAlert, "Alert");

        // === COMMAND BAR ===
        CmdNew.Click += (_, _) => { _lastCommandName = "New"; CommandBarStatus.Text = "Command: New"; UpdateCommandBarCode(); };
        CmdOpen.Click += (_, _) => { _lastCommandName = "Open"; CommandBarStatus.Text = "Command: Open"; UpdateCommandBarCode(); };
        CmdSave.Click += (_, _) => { _lastCommandName = "Save"; CommandBarStatus.Text = "Command: Save"; UpdateCommandBarCode(); };
        CmdDelete.Click += (_, _) => { _lastCommandName = "Delete"; CommandBarStatus.Text = "Command: Delete"; UpdateCommandBarCode(); };
        CmdSettings.Click += (_, _) => { _lastCommandName = "Settings"; CommandBarStatus.Text = "Command: Settings"; UpdateCommandBarCode(); };
        CmdAbout.Click += async (_, _) =>
        {
            _lastCommandName = "About"; CommandBarStatus.Text = "Command: About"; UpdateCommandBarCode();
            var dialog = new ContentDialog { Title = "About", Content = "Avalonia UI Showcase App\nBuilt with Avalonia 11 + FluentAvalonia", PrimaryButtonText = "Close", DefaultButton = ContentDialogButton.Primary };
            var tl = TopLevel.GetTopLevel(this);
            if (tl != null) await dialog.ShowAsync(tl);
        };
    }

    private void WireIconButton(Button btn, string symbolName)
    {
        btn.Click += async (_, _) =>
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(symbolName);
                _lastIconName = symbolName;
                IconStatus.Text = $"Copied \"{symbolName}\" to clipboard!";
                UpdateIconsCode();
            }
        };
    }

    private void UpdateSelectionStatus()
    {
        var items = new List<string>();
        if (Check1.IsChecked == true) items.Add("Option 1");
        if (Check2.IsChecked == true) items.Add("Option 2");
        if (Check3.IsChecked == true) items.Add("Indeterminate");
        var radio = RadioA.IsChecked == true ? "A" : RadioB.IsChecked == true ? "B" : RadioC.IsChecked == true ? "C" : "?";
        var sw = DemoToggleSwitch.IsChecked == true ? "On" : "Off";
        SelectionStatus.Text = $"Checked: {(items.Count > 0 ? string.Join(", ", items) : "none")} | Radio: Choice {radio} | Switch: {sw}";
    }

    // ═══════════════════════════════════════════════════
    //  DYNAMIC CODE — pure XAML, playground-compatible
    // ═══════════════════════════════════════════════════

    private void UpdateButtonsCode()
    {
        ButtonsCode.Text = _lastButtonControl switch
        {
            "standard" =>
$@"<!-- Standard Button — clicked {_standardClickCount} time(s) -->
<Button Content=""Standard"" Margin=""0,0,8,8"" />",

            "accent" =>
@"<!-- Accent Button -->
<Button Content=""Accent"" Classes=""accent"" Margin=""0,0,8,8"" />",

            "hyperlink" =>
@"<!-- HyperlinkButton -->
<HyperlinkButton Content=""Hyperlink Button"" Margin=""0,0,8,8"" />",

            "toggle" =>
$@"<!-- ToggleButton — currently {(DemoToggleButton.IsChecked == true ? "ON" : "OFF")} -->
<ToggleButton Content=""Toggle""
    IsChecked=""{(DemoToggleButton.IsChecked == true ? "True" : "False")}""
    Margin=""0,0,8,8"" />",

            "split" or "split_a" or "split_b" or "split_c" =>
@"<!-- SplitButton with menu -->
<SplitButton Content=""Split Button"" Margin=""0,0,8,8"">
    <SplitButton.Flyout>
        <MenuFlyout>
            <MenuItem Header=""Option A"" />
            <MenuItem Header=""Option B"" />
            <MenuItem Header=""Option C"" />
        </MenuFlyout>
    </SplitButton.Flyout>
</SplitButton>",

            "drop_1" or "drop_2" or "drop_3" =>
@"<!-- DropDownButton with menu -->
<DropDownButton Content=""Dropdown"" Margin=""0,0,8,8"">
    <DropDownButton.Flyout>
        <MenuFlyout>
            <MenuItem Header=""Item 1"" />
            <MenuItem Header=""Item 2"" />
            <MenuItem Header=""Item 3"" />
        </MenuFlyout>
    </DropDownButton.Flyout>
</DropDownButton>",

            _ => Hint
        };
    }

    private void UpdateTextInputCode()
    {
        TextInputCode.Text = _lastTextInputControl switch
        {
            "textbox" =>
$@"<!-- TextBox — {DemoTextBox.Text?.Length ?? 0} character(s) -->
<TextBox Text=""{Esc(DemoTextBox.Text ?? "")}""
    Watermark=""Standard TextBox"" />",

            "password" =>
$@"<!-- Password TextBox — {DemoPasswordBox.Text?.Length ?? 0} character(s) -->
<TextBox Watermark=""Password""
    PasswordChar=""*""
    RevealPassword=""True"" />",

            "multiline" =>
$@"<!-- Multiline TextBox — {DemoMultilineBox.Text?.Length ?? 0} character(s) -->
<TextBox Watermark=""Multiline TextBox""
    AcceptsReturn=""True""
    TextWrapping=""Wrap""
    MinHeight=""80"" />",

            "autocomplete" =>
$@"<!-- AutoCompleteBox — typed: ""{Esc(AutoCompleteBoxDemo.Text ?? "")}"" -->
<AutoCompleteBox
    Watermark=""AutoCompleteBox (type a fruit)"" />",

            "numberbox" =>
$@"<!-- NumberBox — value: {DemoNumberBox.Value} -->
<ui:NumberBox Value=""{DemoNumberBox.Value}""
    PlaceholderText=""NumberBox""
    SpinButtonPlacementMode=""Inline""
    Minimum=""0"" Maximum=""100"" />",

            _ => Hint
        };
    }

    private void UpdateSelectionCode()
    {
        SelectionCode.Text = _lastSelectionControl switch
        {
            "checkbox" =>
$@"<!-- CheckBox -->
<StackPanel Spacing=""4"">
    <CheckBox Content=""Option 1"" IsChecked=""{B(Check1.IsChecked == true)}"" />
    <CheckBox Content=""Option 2"" IsChecked=""{B(Check2.IsChecked == true)}"" />
    <CheckBox Content=""Indeterminate"" IsThreeState=""True"" />
</StackPanel>",

            "radio" =>
$@"<!-- RadioButton — selected: Choice {SelRadio2()} -->
<StackPanel Spacing=""4"">
    <RadioButton Content=""Choice A"" GroupName=""Demo""
        IsChecked=""{B(RadioA.IsChecked == true)}"" />
    <RadioButton Content=""Choice B"" GroupName=""Demo""
        IsChecked=""{B(RadioB.IsChecked == true)}"" />
    <RadioButton Content=""Choice C"" GroupName=""Demo""
        IsChecked=""{B(RadioC.IsChecked == true)}"" />
</StackPanel>",

            "toggleswitch" =>
$@"<!-- ToggleSwitch — {(DemoToggleSwitch.IsChecked == true ? "On" : "Off")} -->
<ToggleSwitch OnContent=""On"" OffContent=""Off""
    IsChecked=""{B(DemoToggleSwitch.IsChecked == true)}"" />",

            "combobox" =>
$@"<!-- ComboBox — selected index: {DemoComboBox.SelectedIndex} -->
<ComboBox SelectedIndex=""{DemoComboBox.SelectedIndex}"" Width=""200"">
    <ComboBoxItem Content=""Option 1"" />
    <ComboBoxItem Content=""Option 2"" />
    <ComboBoxItem Content=""Option 3"" />
    <ComboBoxItem Content=""Option 4"" />
</ComboBox>",

            "listbox" =>
$@"<!-- ListBox — {DemoListBox.SelectedItems?.Count ?? 0} item(s) selected -->
<ListBox SelectionMode=""Multiple""
    MaxHeight=""120"" Width=""200"">
    <ListBoxItem Content=""Item A"" />
    <ListBoxItem Content=""Item B"" />
    <ListBoxItem Content=""Item C"" />
    <ListBoxItem Content=""Item D"" />
    <ListBoxItem Content=""Item E"" />
</ListBox>",

            "slider" =>
$@"<!-- Slider — value: {(int)DemoSlider.Value} -->
<Slider Minimum=""0"" Maximum=""100""
    Value=""{(int)DemoSlider.Value}""
    TickFrequency=""10"" IsSnapToTickEnabled=""True""
    TickPlacement=""BottomRight"" Width=""300"" />",

            _ => Hint
        };
    }

    private void UpdateProgressCode()
    {
        ProgressCode.Text = _lastProgressControl switch
        {
            "determinate" =>
$@"<!-- ProgressBar — {(int)DemoProgressBar.Value}% -->
<StackPanel Spacing=""4"" Width=""400"">
    <ProgressBar Value=""{(int)DemoProgressBar.Value}"" Maximum=""100"" />
    <Slider Value=""{(int)ProgressSlider.Value}""
        Minimum=""0"" Maximum=""100"" />
</StackPanel>",

            "indeterminate" =>
$@"<!-- Indeterminate ProgressBar — {(IndeterminateToggle.IsChecked == true ? "animating" : "paused")} -->
<ProgressBar IsIndeterminate=""{B(IndeterminateToggle.IsChecked == true)}"" Width=""400"" />",

            "ring_indeterminate" =>
@"<!-- ProgressRing — indeterminate (spinning) -->
<ui:ProgressRing IsIndeterminate=""True""
    Width=""40"" Height=""40"" />",

            "ring_determinate" =>
@"<!-- ProgressRing — determinate -->
<ui:ProgressRing IsIndeterminate=""False""
    Width=""40"" Height=""40"" />",

            _ => Hint
        };
    }

    private void UpdateInfoBarCode()
    {
        Dispatcher.UIThread.Post(() =>
        {
            InfoBarCode.Text = _lastInfoBarControl switch
            {
                "info" =>
$@"<!-- InfoBar: Informational — IsOpen={InfoBarInfo.IsOpen} -->
<ui:InfoBar Title=""Informational""
    Message=""This is an informational message.""
    Severity=""Informational""
    IsOpen=""{B(InfoBarInfo.IsOpen)}"" IsClosable=""True"" />",

                "success" =>
$@"<!-- InfoBar: Success — IsOpen={InfoBarSuccess.IsOpen} -->
<ui:InfoBar Title=""Success""
    Message=""Operation completed successfully.""
    Severity=""Success""
    IsOpen=""{B(InfoBarSuccess.IsOpen)}"" IsClosable=""True"" />",

                "warning" =>
$@"<!-- InfoBar: Warning — IsOpen={InfoBarWarning.IsOpen} -->
<ui:InfoBar Title=""Warning""
    Message=""Please review before proceeding.""
    Severity=""Warning""
    IsOpen=""{B(InfoBarWarning.IsOpen)}"" IsClosable=""True"" />",

                "error" =>
$@"<!-- InfoBar: Error — IsOpen={InfoBarError.IsOpen} -->
<ui:InfoBar Title=""Error""
    Message=""An error has occurred.""
    Severity=""Error""
    IsOpen=""{B(InfoBarError.IsOpen)}"" IsClosable=""True"" />",

                "reset" =>
@"<!-- All InfoBars reopened -->
<StackPanel Spacing=""8"">
    <ui:InfoBar Title=""Informational"" Severity=""Informational""
        IsOpen=""True"" IsClosable=""True"" />
    <ui:InfoBar Title=""Success"" Severity=""Success""
        IsOpen=""True"" IsClosable=""True"" />
    <ui:InfoBar Title=""Warning"" Severity=""Warning""
        IsOpen=""True"" IsClosable=""True"" />
    <ui:InfoBar Title=""Error"" Severity=""Error""
        IsOpen=""True"" IsClosable=""True"" />
</StackPanel>",

                _ => Hint
            };
        }, DispatcherPriority.Background);
    }

    private void UpdateBadgesCode()
    {
        var val = DemoBadge.Value;
        BadgesCode.Text = _lastBadgeControl switch
        {
            "tooltip_simple" =>
@"<!-- Simple ToolTip -->
<Button Content=""Hover me for ToolTip"">
    <ToolTip.Tip>
        <TextBlock Text=""This is a ToolTip!"" />
    </ToolTip.Tip>
</Button>",

            "tooltip_rich" =>
@"<!-- Rich ToolTip with multiple elements -->
<Button Content=""Rich ToolTip"">
    <ToolTip.Tip>
        <StackPanel>
            <TextBlock Text=""Rich ToolTip"" FontWeight=""Bold"" />
            <TextBlock Text=""Supports any content inside."" />
        </StackPanel>
    </ToolTip.Tip>
</Button>",

            "increment" or "decrement" =>
$@"<!-- InfoBadge — value: {val} -->
<StackPanel Orientation=""Horizontal"" Spacing=""8"">
    <Button Content=""-"" Width=""32"" />
    <StackPanel Orientation=""Horizontal"" Spacing=""4"">
        <TextBlock Text=""Notifications""
            VerticalAlignment=""Center"" />
        <ui:InfoBadge Value=""{val}"" />
    </StackPanel>
    <Button Content=""+"" Width=""32"" />
</StackPanel>",

            _ => Hint
        };
    }

    private void UpdateTabControlCode()
    {
        var tab = (DemoTabControl.SelectedItem as TabItem)?.Header?.ToString() ?? "Tab 1";
        var idx = DemoTabControl.SelectedIndex;
        TabControlCode.Text =
$@"<!-- TabControl — active: ""{tab}"" (index {idx}) -->
<TabControl SelectedIndex=""{idx}"">
    <TabItem Header=""Tab 1"">
        <StackPanel Margin=""12"">
            <TextBlock Text=""Content of Tab 1"" />
            <TextBlock Text=""Each tab can hold different content.""
                Opacity=""0.6"" />
        </StackPanel>
    </TabItem>
    <TabItem Header=""Tab 2"">
        <StackPanel Margin=""12"">
            <TextBlock Text=""Content of Tab 2"" />
            <Button Content=""Action"" Margin=""0,8,0,0"" />
        </StackPanel>
    </TabItem>
    <TabItem Header=""Tab 3"">
        <TextBlock Text=""Content of Tab 3"" Margin=""12"" />
    </TabItem>
</TabControl>";
    }

    private void UpdateDateTimeCode()
    {
        DateTimeCode.Text = _lastDateTimeControl switch
        {
            "calendar" =>
$@"<!-- CalendarDatePicker — {(DemoCalendarPicker.SelectedDate.HasValue ? DemoCalendarPicker.SelectedDate.Value.ToString("yyyy-MM-dd") : "(not set)")} -->
<CalendarDatePicker Width=""300"" />",

            "datepicker" =>
$@"<!-- DatePicker — {(DemoDatePicker.SelectedDate.HasValue ? DemoDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd") : "(not set)")} -->
<DatePicker Width=""300"" />",

            "timepicker" =>
$@"<!-- TimePicker — {(DemoTimePicker.SelectedTime.HasValue ? DemoTimePicker.SelectedTime.Value.ToString(@"hh\:mm") : "(not set)")} -->
<TimePicker Width=""300"" />",

            _ => Hint
        };
    }

    private void UpdateDialogCode()
    {
        DialogCode.Text =
$@"<!-- ContentDialog preview — last result: {_dialogResult} -->
<Border Background=""{{DynamicResource CardBackgroundFillColorDefaultBrush}}""
    BorderBrush=""{{DynamicResource CardStrokeColorDefaultBrush}}""
    BorderThickness=""1"" CornerRadius=""8""
    Padding=""24"" MaxWidth=""400""
    HorizontalAlignment=""Center"">
    <StackPanel Spacing=""12"">
        <TextBlock Text=""Sample Dialog""
            FontSize=""20"" FontWeight=""SemiBold"" />
        <TextBlock Text=""This is a ContentDialog from FluentAvalonia.""
            TextWrapping=""Wrap"" />
        <StackPanel Orientation=""Horizontal"" Spacing=""8""
            HorizontalAlignment=""Right"">
            <Button Content=""OK"" Classes=""accent"" />
            <Button Content=""Maybe"" />
            <Button Content=""Cancel"" />
        </StackPanel>
    </StackPanel>
</Border>";
    }

    private void UpdateFlyoutsCode()
    {
        FlyoutsCode.Text = _lastFlyoutControl switch
        {
            "flyout" =>
@"<!-- Button with Flyout -->
<Button Content=""Button with Flyout"">
    <Button.Flyout>
        <Flyout>
            <StackPanel Spacing=""8"" Width=""200"">
                <TextBlock Text=""Flyout Content""
                    FontWeight=""SemiBold"" />
                <TextBlock Text=""This flyout appears on click.""
                    TextWrapping=""Wrap"" />
                <Button Content=""Close Flyout"" Classes=""accent""
                    HorizontalAlignment=""Stretch"" />
            </StackPanel>
        </Flyout>
    </Button.Flyout>
</Button>",

            _ when _lastFlyoutControl is "Cut" or "Copy" or "Paste" or "Select All" =>
$@"<!-- Button with context MenuFlyout — last: ""{_lastFlyoutControl}"" -->
<Button Content=""Right-click for MenuFlyout"">
    <Button.ContextFlyout>
        <MenuFlyout>
            <MenuItem Header=""Cut"" />
            <MenuItem Header=""Copy"" />
            <MenuItem Header=""Paste"" />
            <Separator />
            <MenuItem Header=""Select All"" />
        </MenuFlyout>
    </Button.ContextFlyout>
</Button>",

            _ => Hint
        };
    }

    private void UpdateDataGridCode()
    {
        if (SampleDataGrid.SelectedItem is ProductItem p)
        {
            DataGridCode.Text =
$@"<!-- DataGrid — selected: {p.Name} (${p.Price}) -->
<Border BorderBrush=""{{DynamicResource CardStrokeColorDefaultBrush}}""
    BorderThickness=""1"" CornerRadius=""4"" Padding=""16"">
    <StackPanel Spacing=""4"">
        <TextBlock Text=""Selected Product"" FontWeight=""SemiBold"" />
        <TextBlock Text=""ID: {p.Id}"" />
        <TextBlock Text=""Name: {p.Name}"" />
        <TextBlock Text=""Category: {p.Category}"" />
        <TextBlock Text=""Price: {p.Price:C}"" />
        <TextBlock Text=""In Stock: {p.InStock}"" />
    </StackPanel>
</Border>";
        }
        else
        {
            DataGridCode.Text = Hint;
        }
    }

    private void UpdateColorPickerCode()
    {
        ColorPickerCode.Text = _lastColorControl switch
        {
            "picker" =>
$@"<!-- ColorPicker — current: #{DemoColorPicker.Color.R:X2}{DemoColorPicker.Color.G:X2}{DemoColorPicker.Color.B:X2} -->
<ColorPicker HorizontalAlignment=""Left"" />",

            "reset" =>
@"<!-- Reset accent color -->
<Button Content=""Reset to System Default"" />",

            _ => Hint
        };
    }

    private void UpdateIconsCode()
    {
        if (_lastIconName != null)
        {
            IconsCode.Text =
$@"<!-- SymbolIcon: {_lastIconName} -->
<StackPanel HorizontalAlignment=""Center"">
    <ui:SymbolIcon Symbol=""{_lastIconName}""
        FontSize=""28"" HorizontalAlignment=""Center"" />
    <TextBlock Text=""{_lastIconName}""
        FontSize=""11"" HorizontalAlignment=""Center""
        Opacity=""0.6"" />
</StackPanel>";
        }
        else
        {
            IconsCode.Text = Hint;
        }
    }

    private void UpdateCommandBarCode()
    {
        if (_lastCommandName != null)
        {
            var icon = _lastCommandName switch { "New" => "Add", "Open" => "OpenFolder", "Save" => "Save", "Delete" => "Delete", _ => "" };
            var isPrimary = _lastCommandName is "New" or "Open" or "Save" or "Delete";

            CommandBarCode.Text = isPrimary
?
$@"<!-- CommandBar — clicked: {_lastCommandName} -->
<ui:CommandBar DefaultLabelPosition=""Right"">
    <ui:CommandBar.PrimaryCommands>
        <ui:CommandBarButton Label=""New"" IconSource=""Add"" />
        <ui:CommandBarButton Label=""Open"" IconSource=""OpenFolder"" />
        <ui:CommandBarButton Label=""Save"" IconSource=""Save"" />
        <ui:CommandBarSeparator />
        <ui:CommandBarButton Label=""Delete"" IconSource=""Delete"" />
    </ui:CommandBar.PrimaryCommands>
</ui:CommandBar>"
:
$@"<!-- CommandBar — clicked: {_lastCommandName} (secondary) -->
<ui:CommandBar>
    <ui:CommandBar.SecondaryCommands>
        <ui:CommandBarButton Label=""Settings"" />
        <ui:CommandBarButton Label=""About"" />
    </ui:CommandBar.SecondaryCommands>
</ui:CommandBar>";
        }
        else
        {
            CommandBarCode.Text = Hint;
        }
    }

    // Helpers
    private static string SelRadio() => "A"; // placeholder, actual is inline
    private static string B(bool v) => v ? "True" : "False";
    private static string Esc(string s) { var d = s.Length > 40 ? s[..40] + "..." : s; return d.Replace("\"", "&quot;"); }

    private string SelRadio2() => RadioA.IsChecked == true ? "A" : RadioB.IsChecked == true ? "B" : RadioC.IsChecked == true ? "C" : "?";
}

public record ProductItem(int Id, string Name, string Category, decimal Price, bool InStock);
