//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal abstract class BasePath
    {
        protected virtual string GetBaseDirectory()
        {
            return AppContext.BaseDirectory;
        }

        protected virtual string ResolveFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Path is empty.", nameof(folder));

            if (Path.IsPathFullyQualified(folder))
                return Path.GetFullPath(folder);

            return Path.GetFullPath(Path.Combine(GetBaseDirectory(), folder));
        }

        protected virtual string ResolveFolderWithSubFolders(string folder, params string[] subFolders)
        {
            string _folder = ResolveFolder(folder);
            var _folders = new List<string>() { _folder};
            _folders.AddRange(subFolders);

            return Path.GetFullPath(Path.Combine(_folders.ToArray()));
        }

    }

    internal abstract class CustomPath: BasePath
    {
        protected string _rootFolder = string.Empty;
        protected CustomPath(string rootFolder)
        {
            //check folder
            ResolveFolder(rootFolder);
            _rootFolder = rootFolder;
        }

        protected void SetRootFolder(string rootFolder)
        {
            //check folder
            ResolveFolder(rootFolder);
            _rootFolder = rootFolder;
        }

        protected string GetRootFolderInternal()
        {
            var path = ResolveFolder(_rootFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        protected string GetSubFoldersInternal(params string[] subFolders)
        {
            var path = ResolveFolderWithSubFolders(_rootFolder, subFolders);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

}
