# Avalonia UI Showcase

A cross-platform desktop application built with **Avalonia UI** and **FluentAvalonia**, styled to match the WinUI 3 / Fluent Design language. The app serves as both a visual reference and an interactive playground for Avalonia controls.

## Features

### Showcase Tab
An interactive catalog of UI controls, each with live XAML code generation:

- **Buttons** -- Standard, Accent, Hyperlink, Toggle, Split, and Dropdown
- **Text Input** -- TextBox, Password, Multiline, AutoCompleteBox, NumberBox
- **Selection Controls** -- CheckBox, RadioButton, ToggleSwitch, ComboBox, ListBox, Slider
- **Progress Indicators** -- Determinate/Indeterminate ProgressBar and ProgressRing
- **InfoBar** -- Informational, Success, Warning, and Error severities
- **ToolTips and Badges** -- Simple/Rich ToolTips with InfoBadge counter
- **Expander** -- Collapsible content sections
- **TabControl** -- Multi-tab navigation
- **Date and Time Pickers** -- CalendarDatePicker, DatePicker, TimePicker
- **ContentDialog** -- Modal dialogs with multiple button options
- **Flyouts** -- Button Flyout and context MenuFlyout
- **DataGrid** -- Tabular data with row selection
- **ColorPicker** -- Live accent color customization
- **SymbolIcon** -- Icon gallery with clipboard copy
- **CommandBar** -- Primary and secondary command buttons

Every control updates a code preview showing **valid standalone XAML** that can be copied and pasted directly into the Playground.

### Playground Tab
A live XAML editor where you can type or paste XAML markup and see it rendered in real-time:

- Auto-run mode with debounced parsing
- Automatic namespace injection (Avalonia + FluentAvalonia)
- Pre-built templates (Button, Stack of Controls, Card Layout, Login Form, Dashboard Tiles, Image Gallery)
- Error display for invalid markup

## Tech Stack

| Component | Version |
|-----------|---------|
| .NET | 10.0 |
| Avalonia | 11.3.11 |
| FluentAvaloniaUI | 2.5.0 |
| Avalonia.Markup.Xaml.Loader | 11.3.11 |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or later)

## Building and Running

```bash
cd AvaloniaShowcase
dotnet build
dotnet run
```

For a release build:

```bash
dotnet publish -c Release
```

## Project Structure

```
AvaloniaShowcase/
├── App.axaml / App.axaml.cs          # Application entry, global theme
├── Views/
│   ├── MainWindow.axaml / .cs        # Shell window with NavigationView sidebar
│   ├── HomePage.axaml / .cs          # Empty home tab
│   ├── ShowcasePage.axaml / .cs      # Interactive control showcase
│   └── PlaygroundPage.axaml / .cs    # Live XAML editor + preview
└── AvaloniaShowcase.csproj           # Project file and NuGet dependencies
```

## License

This project is provided as a reference/demo application.
