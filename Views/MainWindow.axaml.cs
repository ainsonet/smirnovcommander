using Avalonia.Controls;
using Avalonia.Input;
using SmirnovCommander.ViewModels;
using System.Threading.Tasks;

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
        
        // Левые кнопки
        LeftCopyButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.CopyCommand.Execute(null); };
        LeftCutButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.CutCommand.Execute(null); };
        LeftPasteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.PasteCommand.Execute(null); };
        LeftDeleteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.DeleteCommand.Execute(null); };
        LeftCreateFolderButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.CreateFolderCommand.Execute(null); };
        LeftRenameButton.Click += LeftRenameButton_Click;
        LeftGoUpButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.GoUpCommand.Execute(null); };
        LeftRefreshButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.RefreshCommand.Execute(null); };
        
        // Правые кнопки
        RightRefreshButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.RefreshCommand.Execute(null); };
        RightGoUpButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.GoUpCommand.Execute(null); };
        RightRenameButton.Click += RightRenameButton_Click;
        RightCreateFolderButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.CreateFolderCommand.Execute(null); };
        RightDeleteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.DeleteCommand.Execute(null); };
        RightPasteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.PasteCommand.Execute(null); };
        RightCutButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.CutCommand.Execute(null); };
        RightCopyButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.CopyCommand.Execute(null); };
        
        KeyDown += MainWindow_KeyDown;
    }

    private void LeftPanelList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.LeftPanel.SelectedItem?.IsDirectory == true)
        {
            vm.LeftPanel.EnterDirectoryCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void RightPanelList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.RightPanel.SelectedItem?.IsDirectory == true)
        {
            vm.RightPanel.EnterDirectoryCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void UpdateActivePanel()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.ActivePanel = LeftPanelList.IsFocused ? vm.LeftPanel : vm.RightPanel;
        }
    }

    private async void LeftRenameButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var panel = DataContext is MainWindowViewModel vm ? vm.LeftPanel : null;
        await ShowRenameDialog(panel);
    }

    private async void RightRenameButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var panel = DataContext is MainWindowViewModel vm ? vm.RightPanel : null;
        await ShowRenameDialog(panel);
    }

    private async void MainWindow_KeyDown(object? sender, KeyEventArgs e)
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
                await ShowRenameDialog(activePanel);
                e.Handled = true;
            }
        }
    }

    private async Task ShowRenameDialog(PanelViewModel? panel)
    {
        if (panel == null || panel.SelectedItem == null)
            return;

        var dialog = new RenameDialog(panel.SelectedItem.Name);
        await dialog.ShowDialog(this);
        
        if (!string.IsNullOrEmpty(dialog.Result))
        {
            if (!panel.RenameItem(dialog.Result))
            {
                // Показать ошибку
                var errorDialog = new MessageBox();
                errorDialog.Title = "Ошибка переименования";
                errorDialog.MessageTextBlock.Text = "Не удалось переименовать файл/папку. Возможно такой файл уже существует.";
                await errorDialog.ShowDialog(this);
            }
        }
    }
}