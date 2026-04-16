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
    private string _statusBarText = "Готово";

    public MainWindowViewModel()
    {
        UpdateStatusBar();
        
        LeftPanel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PanelViewModel.Items) || 
                e.PropertyName == nameof(PanelViewModel.SelectedItem))
                UpdateStatusBar();
        };
    }

    private void UpdateStatusBar()
    {
        var selectedCount = LeftPanel.SelectedItems.Count > 0 ? LeftPanel.SelectedItems.Count : 
                           (LeftPanel.SelectedItem != null ? 1 : 0);
        var dirCount = LeftPanel.Items.Count(i => i.IsDirectory);
        var fileCount = LeftPanel.Items.Count(i => !i.IsDirectory);
        
        StatusBarText = $"Файлов: {fileCount} | Папок: {dirCount} | Выделено: {selectedCount}";
    }
}