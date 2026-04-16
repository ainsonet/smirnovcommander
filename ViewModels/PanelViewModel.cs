using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmirnovCommander.Models;

namespace SmirnovCommander.ViewModels;

public partial class PanelViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentPath;

    [ObservableProperty]
    private ObservableCollection<FileSystemItem> _items = [];

    [ObservableProperty]
    private FileSystemItem? _selectedItem;

    public PanelViewModel()
    {
        CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Refresh();
    }

    [RelayCommand]
    private void Refresh()
    {
        Items.Clear();

        if (!Directory.Exists(CurrentPath))
            return;

        try
        {
            foreach (var dir in Directory.GetDirectories(CurrentPath))
            {
                try { Items.Add(new FileSystemItem(dir)); }
                catch { }
            }

            foreach (var file in Directory.GetFiles(CurrentPath))
            {
                try { Items.Add(new FileSystemItem(file)); }
                catch { }
            }
        }
        catch { }
    }

    [RelayCommand]
    private void GoUp()
    {
        var parent = Directory.GetParent(CurrentPath);
        if (parent != null)
        {
            CurrentPath = parent.FullName;
            Refresh();
        }
    }

    [RelayCommand]
    private void EnterDirectory()
    {
        if (SelectedItem?.IsDirectory == true)
        {
            CurrentPath = SelectedItem.FullPath;
            Refresh();
        }
    }

    [RelayCommand]
    private void DoubleClick()
    {
        EnterDirectory();
    }
}