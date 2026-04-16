using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Linq;

namespace SmirnovCommander.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public PanelViewModel LeftPanel { get; } = new();
    public PanelViewModel RightPanel { get; } = new();

    [ObservableProperty]
    private string _leftPanelStatusBar = "";

    [ObservableProperty]
    private string _rightPanelStatusBar = "";

    [ObservableProperty]
    private PanelViewModel _activePanel;

    public MainWindowViewModel()
    {
        ActivePanel = LeftPanel;
        UpdateStatusBars();
        
        LeftPanel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PanelViewModel.Items) || 
                e.PropertyName == nameof(PanelViewModel.SelectedItem))
                UpdateStatusBars();
        };
        
        RightPanel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PanelViewModel.Items) || 
                e.PropertyName == nameof(PanelViewModel.SelectedItem))
                UpdateStatusBars();
        };
    }

    private void UpdateStatusBars()
    {
        var leftSelected = LeftPanel.SelectedItems.Count > 0 ? LeftPanel.SelectedItems.Count : 
                          (LeftPanel.SelectedItem != null ? 1 : 0);
        var leftDirCount = LeftPanel.Items.Count(i => i.IsDirectory);
        var leftFileCount = LeftPanel.Items.Count(i => !i.IsDirectory);
        LeftPanelStatusBar = $"{LeftPanel.CurrentPath} | Файлов: {leftFileCount} | Папок: {leftDirCount} | Выделено: {leftSelected}";
        
        var rightSelected = RightPanel.SelectedItems.Count > 0 ? RightPanel.SelectedItems.Count : 
                           (RightPanel.SelectedItem != null ? 1 : 0);
        var rightDirCount = RightPanel.Items.Count(i => i.IsDirectory);
        var rightFileCount = RightPanel.Items.Count(i => !i.IsDirectory);
        RightPanelStatusBar = $"{RightPanel.CurrentPath} | Файлов: {rightFileCount} | Папок: {rightDirCount} | Выделено: {rightSelected}";
    }
}