using Avalonia.Controls;
using Avalonia.Input;
using SmirnovCommander.ViewModels;

namespace SmirnovCommander.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ListBox_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.DataContext is PanelViewModel panel)
        {
            panel.DoubleClickCommand.Execute(null);
        }
    }
}