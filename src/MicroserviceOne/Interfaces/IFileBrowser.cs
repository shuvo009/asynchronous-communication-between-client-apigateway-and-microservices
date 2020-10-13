using System.Collections.Generic;
using MicroserviceOne.Models;

namespace MicroserviceOne.Interfaces
{
    public interface IFileBrowser
    {
        List<FileBrowserResponse> GetFileAndFolders(string root);
    }
}