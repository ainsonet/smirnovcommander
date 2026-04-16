using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SmirnovCommander.Views;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        InitializeComponent();
        OkButton.Click += OkButton_Click;
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}