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
        Name = Path.GetFileName(path);
        
        var info = new FileInfo(path);
        IsDirectory = info.Attributes.HasFlag(FileAttributes.Directory);
        
        if (!IsDirectory && info.Exists)
        {
            Size = info.Length;
            Modified = info.LastWriteTime;
        }
        else if (IsDirectory && Directory.Exists(path))
        {
            var dirInfo = new DirectoryInfo(path);
            Modified = dirInfo.LastWriteTime;
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
