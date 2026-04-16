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
        LeftPanelList.SelectionChanged += (s, e) => UpdateActivePanel();
        RightPanelList.SelectionChanged += (s, e) => UpdateActivePanel();
        
        KeyDown += MainWindow_KeyDown;
    }

    private void UpdateActivePanel()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Определяем активную панель по фокусу
            if (FocusManager.GetFocusedElement() is Control control)
            {
                vm.ActivePanel = LeftPanelList.IsFocused ? vm.LeftPanel : vm.RightPanel;
            }
        }
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
            var activePanel = vm.ActivePanel ?? vm.LeftPanel;

            if (e.KeyModifiers == KeyModifiers.Control)
            {
                if (e.Key == Key.C)
                {
                    activePanel.CopyCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.X)
                {
                    activePanel.CutCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.V)
                {
                    activePanel.PasteCommand.Execute(null);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete)
            {
                activePanel.DeleteCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.F5)
            {
                activePanel.RefreshCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                activePanel.SelectedItem = null;
                activePanel.SelectedItems = [];
                e.Handled = true;
            }
        }
    }
}