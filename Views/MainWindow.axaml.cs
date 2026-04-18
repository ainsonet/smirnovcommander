using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SmirnovCommander.ViewModels;
using SmirnovCommander.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmirnovCommander.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        LeftPanelList.DoubleTapped += LeftPanelList_DoubleTapped;
        RightPanelList.DoubleTapped += RightPanelList_DoubleTapped;
        LeftPanelList.SelectionChanged += (s, e) => { UpdateActivePanel(); UpdateStatusBars(); SyncSelectedItems(); };
        RightPanelList.SelectionChanged += (s, e) => { UpdateActivePanel(); UpdateStatusBars(); SyncSelectedItems(); };
        
        // Левые кнопки
        LeftBackButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.GoBack(); };
        LeftForwardButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.GoForward(); };
        LeftCopyButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.Copy(); };
        LeftCutButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.Cut(); };
        LeftPasteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.Paste(); };
        LeftDeleteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.Delete(); };
        LeftCreateFolderButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.CreateFolder(); };
        LeftRenameButton.Click += LeftRenameButton_Click;
        LeftGoUpButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.GoUp(); };
        LeftRefreshButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.LeftPanel.Refresh(); };
        
        // Правые кнопки
        RightForwardButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.GoForward(); };
        RightBackButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.GoBack(); };
        RightRefreshButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.Refresh(); };
        RightGoUpButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.GoUp(); };
        RightRenameButton.Click += RightRenameButton_Click;
        RightCreateFolderButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.CreateFolder(); };
        RightDeleteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.Delete(); };
        RightPasteButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.Paste(); };
        RightCutButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.Cut(); };
        RightCopyButton.Click += (s, e) => { if (DataContext is MainWindowViewModel vm) vm.RightPanel.Copy(); };
        
        // Поиск
        SearchButton.Click += SearchButton_Click;
        SearchTextBox.KeyDown += SearchTextBox_KeyDown;
        
        // Подсветка кнопки поиска
        SearchButton.PointerEntered += (s, e) => SearchButton.Background = Avalonia.Media.Brushes.LightGray;
        SearchButton.PointerExited += (s, e) => SearchButton.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CCCCCC"));
        
        KeyDown += MainWindow_KeyDown;
    }

    private void LeftPanelList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.LeftPanel.SelectedItem?.IsDirectory == true)
        {
            vm.LeftPanel.EnterDirectory();
            e.Handled = true;
        }
    }

    private void RightPanelList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.RightPanel.SelectedItem?.IsDirectory == true)
        {
            vm.RightPanel.EnterDirectory();
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

    private void UpdateStatusBars()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Прямо из ListBox читаем выделение
            var leftSelected = LeftPanelList.SelectedItems.Count;
            var rightSelected = RightPanelList.SelectedItems.Count;
            
            var leftDirCount = LeftPanelList.Items.Cast<FileSystemItem>().Count(i => i.IsDirectory);
            var leftFileCount = LeftPanelList.Items.Cast<FileSystemItem>().Count(i => !i.IsDirectory);
            vm.LeftPanelStatusBar = $"{vm.LeftPanel.CurrentPath} | Файлов: {leftFileCount} | Папок: {leftDirCount} | Выделено: {leftSelected}";
            
            var rightDirCount = RightPanelList.Items.Cast<FileSystemItem>().Count(i => i.IsDirectory);
            var rightFileCount = RightPanelList.Items.Cast<FileSystemItem>().Count(i => !i.IsDirectory);
            vm.RightPanelStatusBar = $"{vm.RightPanel.CurrentPath} | Файлов: {rightFileCount} | Папок: {rightDirCount} | Выделено: {rightSelected}";
        }
    }

    private void SyncSelectedItems()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Синхронизируем SelectedItems из ListBox в ViewModel
            vm.LeftPanel.SelectedItems = LeftPanelList.SelectedItems.Cast<FileSystemItem>().ToList();
            vm.RightPanel.SelectedItems = RightPanelList.SelectedItems.Cast<FileSystemItem>().ToList();
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
                    activePanel.Copy();
                    e.Handled = true;
                }
                else if (e.Key == Key.X)
                {
                    activePanel.Cut();
                    e.Handled = true;
                }
                else if (e.Key == Key.V)
                {
                    await activePanel.Paste();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete)
            {
                activePanel.Delete();
                e.Handled = true;
            }
            else if (e.Key == Key.F5)
            {
                activePanel.Refresh();
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

    private void SearchButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            var activePanel = vm.ActivePanel ?? vm.LeftPanel;
            activePanel.Search(SearchTextBox.Text);
            
            // Сфокусироваться на активной панели
            if (vm.ActivePanel == vm.RightPanel)
                RightPanelList.Focus();
            else
                LeftPanelList.Focus();
        }
    }

    private void SearchTextBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
        {
            SearchButton_Click(sender, e);
        }
    }
}