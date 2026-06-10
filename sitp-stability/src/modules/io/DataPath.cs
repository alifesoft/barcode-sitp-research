//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal class DataPath: CustomPath
    {
        private static readonly DataPath _instance = new DataPath(@".\..\data");
        private DataPath(string rootFolder) : base(rootFolder)
        {
        }

        public static string RootFolder
        {
            get { return _instance._rootFolder; }
            set { _instance.SetRootFolder(value); }
        }
        public static string GetRootFolderPath()
        {
            return _instance.GetRootFolderInternal();
        }

        public static string GetRootFolderPath(string filename)
        {
            var path = Path.Combine(GetRootFolderPath(), filename);
            return path;
        }

        public static string GetSubFoldersPath(params string[] subFolders)
        {
            return _instance.GetSubFoldersInternal(subFolders);
        }

        public static string GetSubFoldersPath(string filename, params string[] subFolders)
        {
            var path = Path.Combine(GetSubFoldersPath(subFolders), filename);
            return path;
        }
    }


    internal class OutputPath : CustomPath
    {
        private static readonly OutputPath _instance = new OutputPath(@".\..\output");
        private OutputPath(string rootFolder) : base(rootFolder)
        {
        }

        public static string RootFolder
        {
            get { return _instance._rootFolder; }
            set { _instance.SetRootFolder(value); }
        }
        public static string GetRootFolderPath()
        {
            return _instance.GetRootFolderInternal();
        }

        public static string GetRootFolderPath(string filename)
        {
            var path = Path.Combine(GetRootFolderPath(), filename);
            return path;
        }

        public static string GetSubFoldersPath(params string[] subFolders)
        {
            return _instance.GetSubFoldersInternal(subFolders);
        }

        public static string GetSubFoldersPath(string filename, params string[] subFolders)
        {
            var path = Path.Combine(GetSubFoldersPath(subFolders), filename);
            return path;
        }
    }


}
