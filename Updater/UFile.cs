namespace Updater
{
    class UFile
    {
        public readonly string fileName;
        public readonly string filePath;
        public readonly int fileSize;

        public UFile(string file, int filesize)
        {
            filePath = file;
            fileName = file.Substring(file.LastIndexOf('/') + 1);
            fileSize = filesize;
        }

    }
}
