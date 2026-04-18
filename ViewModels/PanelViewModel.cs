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

    private List<FileSystemItem> _selectedItems = [];
    public List<FileSystemItem> SelectedItems
    {
        get => _selectedItems;
        set
        {
            _selectedItems = value;
            OnPropertyChanged(nameof(SelectedItems));
        }
    }

    [ObservableProperty]
    private FileSystemItem? _clipboardItem;

    [ObservableProperty]
    private bool _isCutMode;

    private readonly MainWindowViewModel? _mainViewModel;
    private readonly Stack<string> _navigationHistory = new();
    private readonly Stack<string> _forwardHistory = new();

    public PanelViewModel(MainWindowViewModel? mainViewModel = null)
    {
        _mainViewModel = mainViewModel;
        CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Refresh();
    }

    public void NavigateTo(string path)
    {
        if (path != CurrentPath)
        {
            _navigationHistory.Push(CurrentPath);
            _forwardHistory.Clear();
            CurrentPath = path;
            Refresh();
        }
    }

    [RelayCommand]
    public void GoBack()
    {
        if (_navigationHistory.Count > 0)
        {
            _forwardHistory.Push(CurrentPath);
            CurrentPath = _navigationHistory.Pop();
            Refresh();
        }
    }

    [RelayCommand]
    public void GoForward()
    {
        if (_forwardHistory.Count > 0)
        {
            _navigationHistory.Push(CurrentPath);
            CurrentPath = _forwardHistory.Pop();
            Refresh();
        }
    }

    [RelayCommand]
    public void Refresh()
    {
        Items.Clear();

        if (!Directory.Exists(CurrentPath))
            return;

        try
        {
            var itemsList = new List<FileSystemItem>();

            foreach (var dir in Directory.GetDirectories(CurrentPath))
            {
                try { itemsList.Add(new FileSystemItem(dir)); }
                catch { }
            }

            foreach (var file in Directory.GetFiles(CurrentPath))
            {
                try { itemsList.Add(new FileSystemItem(file)); }
                catch { }
            }

            foreach (var item in itemsList)
            {
                Items.Add(item);
            }
        }
        catch { }
    }

    public void Search(string? pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            Refresh();
            return;
        }

        Items.Clear();
        var searchPattern = pattern.ToLower();

        try
        {
            var itemsList = new List<FileSystemItem>();

            foreach (var dir in Directory.GetDirectories(CurrentPath))
            {
                try 
                {
                    var item = new FileSystemItem(dir);
                    if (item.Name.ToLower().Contains(searchPattern))
                        itemsList.Add(item);
                }
                catch { }
            }

            foreach (var file in Directory.GetFiles(CurrentPath))
            {
                try 
                {
                    var item = new FileSystemItem(file);
                    if (item.Name.ToLower().Contains(searchPattern))
                        itemsList.Add(item);
                }
                catch { }
            }

            foreach (var item in itemsList)
            {
                Items.Add(item);
            }

            if (itemsList.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"Ничего не найдено по запросу '{pattern}'");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка поиска: {ex.Message}");
        }
    }

    [RelayCommand]
    public void GoUp()
    {
        var parent = Directory.GetParent(CurrentPath);
        if (parent != null)
        {
            NavigateTo(parent.FullName);
        }
    }

    [RelayCommand]
    public void EnterDirectory()
    {
        if (SelectedItem?.IsDirectory == true)
        {
            NavigateTo(SelectedItem.FullPath);
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
        if (_mainViewModel == null) return;
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count > 0)
        {
            _mainViewModel.ClipboardItem = items.First();
            _mainViewModel.IsCutMode = false;
        }
    }

    [RelayCommand]
    public void Cut()
    {
        if (_mainViewModel == null) return;
        var items = SelectedItems.Count > 0 ? SelectedItems : (SelectedItem != null ? [SelectedItem] : []);
        if (items.Count > 0)
        {
            _mainViewModel.ClipboardItem = items.First();
            _mainViewModel.IsCutMode = true;
        }
    }

    public async Task Paste()
    {
        if (_mainViewModel?.ClipboardItem == null)
            return;

        var targetPath = Path.Combine(CurrentPath, _mainViewModel.ClipboardItem.Name);

        try
        {
            if (_mainViewModel.ClipboardItem.IsDirectory)
            {
                if (_mainViewModel.IsCutMode)
                {
                    CopyDirectory(_mainViewModel.ClipboardItem.FullPath, targetPath);
                    Directory.Delete(_mainViewModel.ClipboardItem.FullPath, true);
                }
                else
                {
                    CopyDirectory(_mainViewModel.ClipboardItem.FullPath, targetPath);
                }
            }
            else
            {
                if (_mainViewModel.IsCutMode)
                {
                    File.Move(_mainViewModel.ClipboardItem.FullPath, targetPath, true);
                }
                else
                {
                    File.Copy(_mainViewModel.ClipboardItem.FullPath, targetPath, true);
                }
            }
            
            _mainViewModel.IsCutMode = false;
            Refresh();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task PasteAsync()
    {
        await Paste();
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

    [RelayCommand]
    public void ClearSelection()
    {
        SelectedItem = null;
        SelectedItems = [];
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