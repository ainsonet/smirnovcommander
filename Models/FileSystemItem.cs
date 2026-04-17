using System;
using System.IO;

namespace SmirnovCommander.Models;

public class FileSystemItem
{
    public string Name { get; }
    public string FullPath { get; }
    public bool IsDirectory { get; }
    public long Size { get; }
    public DateTime Modified { get; }

    public FileSystemItem(string path)
    {
        FullPath = path;
        Name = Path.GetFileName(path) ?? Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
        
        var dirInfo = Directory.Exists(path) ? new DirectoryInfo(path) : null;
        var fileInfo = File.Exists(path) ? new FileInfo(path) : null;
        
        IsDirectory = dirInfo != null;
        
        if (IsDirectory && dirInfo != null)
        {
            Modified = dirInfo.LastWriteTime;
        }
        else if (fileInfo != null && fileInfo.Exists)
        {
            Size = fileInfo.Length;
            Modified = fileInfo.LastWriteTime;
        }
    }

    public static FileSystemItem CreateDirectory(string path)
    {
        return new FileSystemItem(path) { _isRoot = true };
    }

    private bool _isRoot;
    public bool IsRoot => _isRoot;

    public string DisplaySize => IsDirectory ? "" : FormatSize(Size);

    private static string FormatSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}
