using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SmirnovCommander.Views;

public partial class RenameDialog : Window
{
    public string Result { get; private set; } = "";

    public RenameDialog(string currentName)
    {
        InitializeComponent();
        
        NameTextBox.Text = currentName;
        NameTextBox.SelectAll();
        
        OkButton.Click += OkButton_Click;
        CancelButton.Click += CancelButton_Click;
        
        NameTextBox.KeyDown += (s, e) =>
        {
            if (e.Key == Avalonia.Input.Key.Enter)
                OkButton_Click(null, null!);
            else if (e.Key == Avalonia.Input.Key.Escape)
                CancelButton_Click(null, null!);
        };
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            Result = NameTextBox.Text;
            Close();
        }
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}