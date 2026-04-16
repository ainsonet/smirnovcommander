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
        
        LeftPanelList.AddHandler(InputElement.TappedEvent, OnLeftPanelTapped);
        RightPanelList.AddHandler(InputElement.TappedEvent, OnRightPanelTapped);
        
        LeftRenameButton.Click += LeftRenameButton_Click;
        RightRenameButton.Click += RightRenameButton_Click;
    }

    private void LeftRenameButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var panel = DataContext is MainWindowViewModel vm ? vm.LeftPanel : null;
        ShowRenameDialog(panel);
    }

    private void RightRenameButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var panel = DataContext is MainWindowViewModel vm ? vm.RightPanel : null;
        ShowRenameDialog(panel);
    }

    private void OnLeftPanelTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.LeftPanel.SelectedItem?.IsDirectory == true)
        {
            vm.LeftPanel.EnterDirectoryCommand.Execute(null);
        }
    }

    private void OnRightPanelTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.RightPanel.SelectedItem?.IsDirectory == true)
        {
            vm.RightPanel.EnterDirectoryCommand.Execute(null);
        }
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
            else if (e.Key == Key.F2 && activePanel.SelectedItem != null)
            {
                ShowRenameDialog(activePanel);
                e.Handled = true;
            }
        }
    }

    private void ShowRenameDialog(PanelViewModel? panel)
    {
        if (panel == null || panel.SelectedItem == null)
            return;

        var dialog = new RenameDialog(panel.SelectedItem.Name);
        dialog.ShowDialog(this);
        
        if (!string.IsNullOrEmpty(dialog.Result))
        {
            panel.RenameItem(dialog.Result);
        }
    }
}