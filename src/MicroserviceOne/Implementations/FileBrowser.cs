using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MicroserviceOne.Interfaces;
using MicroserviceOne.Models;
using Microsoft.Extensions.Logging;

namespace MicroserviceOne.Implementations
{
    public class FileBrowser : IFileBrowser
    {
        private readonly ILogger<FileBrowser> _logger;

        public FileBrowser(ILogger<FileBrowser> logger)
        {
            _logger = logger;
        }

        public List<FileBrowserResponse> GetFileAndFolders(string root)
        {
            var paths = string.IsNullOrEmpty(root)
                ? GetDrives()
                : GetDirectoryList(root);
            return paths;
        }

        #region Supported Methods

        private List<FileBrowserResponse> GetDirectoryList(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return new List<FileBrowserResponse>();
                var directoryList = Directory.GetDirectories(path).ToList().Select(x => new FileBrowserResponse
                {
                    Path = x,
                    Name = GetDirectoryName(x),
                    Type = "Folder"
                }).ToList();

                var fileList = Directory.EnumerateFiles(path, "*.csv", SearchOption.TopDirectoryOnly)
                    .Select(x => new FileBrowserResponse
                    {
                        Path = x,
                        Name = Path.GetFileName(x),
                        Type = "File"
                    }).ToList();

                var list = new List<FileBrowserResponse>();
                var blackListFolders = new List<string> { "System Volume Information" };
                list.AddRange(directoryList.Where(x => !String.IsNullOrEmpty(x.Name)).Where(x => blackListFolders.All(s => s != x.Name)));
                list.AddRange(fileList);

                return list;
            }
            catch (Exception e)
            {
                _logger.LogError(1, e, e.Message);
            }
            return new List<FileBrowserResponse>();
        }

        private List<FileBrowserResponse> GetDrives()
        {
            var driveList = DriveInfo.GetDrives().ToList().Select(x => new FileBrowserResponse
            {
                Name = GetDirectoryName(x.Name),
                Path = x.RootDirectory.Name,
                Type = "Folder"
            }).ToList();
            return driveList;
        }

        private string GetDirectoryName(string path)
        {
            var dirName = new DirectoryInfo(path).Name;
            return dirName.StartsWith("$") ? null : dirName;
        }

        #endregion
    }
}