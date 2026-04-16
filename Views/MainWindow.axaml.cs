using Avalonia.Controls;
using Avalonia.Input;
using SmirnovCommander.ViewModels;

namespace SmirnovCommander.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        LeftPanelList.DoubleTapped += LeftPanelList_DoubleTapped;
        RightPanelList.DoubleTapped += RightPanelList_DoubleTapped;
    }

    private void LeftPanelList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.LeftPanel.SelectedItem?.IsDirectory == true)
        {
            vm.LeftPanel.EnterDirectoryCommand.Execute(null);
        }
    }

    private void RightPanelList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.RightPanel.SelectedItem?.IsDirectory == true)
        {
            vm.RightPanel.EnterDirectoryCommand.Execute(null);
        }
    }
}