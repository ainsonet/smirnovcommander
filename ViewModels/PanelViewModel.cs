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

    [ObservableProperty]
    private bool _isCutMode;

    public PanelViewModel()
    {
        CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Refresh();
    }

    [RelayCommand]
    public void Refresh()
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
    public void GoUp()
    {
        var parent = Directory.GetParent(CurrentPath);
        if (parent != null)
        {
            CurrentPath = parent.FullName;
            Refresh();
        }
    }

    [RelayCommand]
    public void EnterDirectory()
    {
        if (SelectedItem?.IsDirectory == true)
        {
            CurrentPath = SelectedItem.FullPath;
            Refresh();
        }
    }

    [RelayCommand]
    public void Open()
    {
        if (SelectedItem == null)
            return;

        if (SelectedItem.IsDirectory)
        {
            EnterDirectory();
        }
        else
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = SelectedItem.FullPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка открытия файла: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    public void Copy()
    {
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count > 0)
        {
            ClipboardItem = items.First();
            IsCutMode = false;
        }
    }

    [RelayCommand]
    public void Cut()
    {
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count > 0)
        {
            ClipboardItem = items.First();
            IsCutMode = true;
        }
    }

    [RelayCommand]
    public async Task Paste()
    {
        if (ClipboardItem == null)
            return;

        var targetPath = Path.Combine(CurrentPath, ClipboardItem.Name);

        try
        {
            if (ClipboardItem.IsDirectory)
            {
                if (IsCutMode)
                {
                    CopyDirectory(ClipboardItem.FullPath, targetPath);
                    Directory.Delete(ClipboardItem.FullPath, true);
                }
                else
                {
                    CopyDirectory(ClipboardItem.FullPath, targetPath);
                }
            }
            else
            {
                if (IsCutMode)
                {
                    File.Move(ClipboardItem.FullPath, targetPath, true);
                }
                else
                {
                    File.Copy(ClipboardItem.FullPath, targetPath, true);
                }
            }
            
            IsCutMode = false;
            Refresh();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    [RelayCommand]
    public void Delete()
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

    [RelayCommand]
    public void CreateFolder()
    {
        var folderName = $"Новая папка {DateTime.Now:HH.mm.ss}";
        var newPath = Path.Combine(CurrentPath, folderName);
        
        try
        {
            Directory.CreateDirectory(newPath);
            Refresh();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка создания папки: {ex.Message}");
        }
    }

    [RelayCommand]
    public void Rename()
    {
        if (SelectedItem == null)
            return;

        System.Diagnostics.Debug.WriteLine($"Переименовать: {SelectedItem.Name}");
    }

    public bool RenameItem(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return false;

        var targetPath = Path.Combine(CurrentPath, newName);

        try
        {
            if (File.Exists(targetPath) || Directory.Exists(targetPath))
            {
                System.Diagnostics.Debug.WriteLine($"Файл уже существует: {targetPath}");
                return false;
            }

            var oldPath = Path.Combine(CurrentPath, SelectedItem?.Name ?? "");
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, targetPath, true);
            }
            else if (Directory.Exists(oldPath))
            {
                Directory.Move(oldPath, targetPath);
            }
            SelectedItem = null;
            Refresh();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка переименования: {ex.Message}");
            return false;
        }
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