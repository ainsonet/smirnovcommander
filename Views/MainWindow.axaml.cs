using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using SmirnovCommander.ViewModels;

namespace SmirnovCommander.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        LeftPanelList.DoubleTapped += LeftPanelList_DoubleTapped;
        RightPanelList.DoubleTapped += RightPanelList_DoubleTapped;
        
        KeyDown += MainWindow_KeyDown;
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

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Определяем активную панель
            var activePanel = e.KeyModifiers == KeyModifiers.None ? vm.LeftPanel : vm.LeftPanel;

            if (e.KeyModifiers == KeyModifiers.Control)
            {
                if (e.Key == Key.C)
                {
                    vm.LeftPanel.CopyCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.X)
                {
                    vm.LeftPanel.CutCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.V)
                {
                    vm.LeftPanel.PasteCommand.Execute(null);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete)
            {
                vm.LeftPanel.DeleteCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.F5)
            {
                vm.LeftPanel.RefreshCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                vm.LeftPanel.SelectedItem = null;
                vm.LeftPanel.SelectedItems = [];
                e.Handled = true;
            }
        }
    }
}