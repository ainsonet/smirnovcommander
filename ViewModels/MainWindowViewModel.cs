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

    [ObservableProperty]
    private PanelViewModel _activePanel;

    public MainWindowViewModel()
    {
        ActivePanel = LeftPanel;
        UpdateStatusBar();
        
        LeftPanel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PanelViewModel.Items) || 
                e.PropertyName == nameof(PanelViewModel.SelectedItem))
                UpdateStatusBar();
        };
        
        RightPanel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PanelViewModel.Items) || 
                e.PropertyName == nameof(PanelViewModel.SelectedItem))
                UpdateStatusBar();
        };
    }

    private void UpdateStatusBar()
    {
        var panel = ActivePanel ?? LeftPanel;
        var selectedCount = panel.SelectedItems.Count > 0 ? panel.SelectedItems.Count : 
                           (panel.SelectedItem != null ? 1 : 0);
        var dirCount = panel.Items.Count(i => i.IsDirectory);
        var fileCount = panel.Items.Count(i => !i.IsDirectory);
        
        StatusBarText = $"Файлов: {fileCount} | Папок: {dirCount} | Выделено: {selectedCount}";
    }
}