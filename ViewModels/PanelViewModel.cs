using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    [ObservableProperty]
    private List<FileSystemItem> _selectedItems = [];

    [ObservableProperty]
    private FileSystemItem? _clipboardItem;

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

    [RelayCommand]
    private void Copy()
    {
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count > 0)
        {
            ClipboardItem = items.First(); // Для простоты копируем первый элемент
            // В будущем можно добавить поддержку нескольких элементов
        }
    }

    [RelayCommand]
    private void Cut()
    {
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count > 0)
        {
            ClipboardItem = items.First();
        }
    }

    [RelayCommand]
    private async Task Paste()
    {
        if (ClipboardItem == null)
            return;

        var targetPath = Path.Combine(CurrentPath, ClipboardItem.Name);

        try
        {
            if (ClipboardItem.IsDirectory)
            {
                CopyDirectory(ClipboardItem.FullPath, targetPath);
            }
            else
            {
                File.Copy(ClipboardItem.FullPath, targetPath, true);
            }
            Refresh();
        }
        catch (Exception ex)
        {
            // В будущем показать уведомление
            System.Diagnostics.Debug.WriteLine($"Ошибка копирования: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Delete()
    {
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count == 0)
            return;

        foreach (var item in items)
        {
            try
            {
                if (item.IsDirectory)
                {
                    Directory.Delete(item.FullPath, true);
                }
                else
                {
                    File.Delete(item.FullPath);
                }
            }
            catch { }
        }
        SelectedItems = [];
        SelectedItem = null;
        Refresh();
    }

    private void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, destDir);
        }
    }
}