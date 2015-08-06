using System.IO;

namespace Updater
{
    public delegate void ProgressChangeDelegate(long TotalCopyed, ref bool Cancel);
    public delegate void Completedelegate();

    class Copy
    {
        public Copy(string Source, string Dest)
        {
            SourceFilePath = Source;
            DestFilePath = Dest;

            OnProgressChanged += delegate { };
            OnComplete += delegate { };
        }


        public void StartCopy()
        {
            byte[] buffer = new byte[1024]; // 1kB buffer
            bool cancelFlag = false;

            using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                long fileLength = source.Length;
                using (FileStream dest = new FileStream(DestFilePath, FileMode.CreateNew, FileAccess.Write))
                {
                    long totalBytes = 0;
                    int currentBlockSize = 0;

                    while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytes += currentBlockSize;
                        //double persentage = (double)fileLength * 100.0 / totalBytes;
                        

                        dest.Write(buffer, 0, currentBlockSize);

                        cancelFlag = false;
                        OnProgressChanged(totalBytes, ref cancelFlag);

                        if (cancelFlag)
                        {
                            // Delete dest file here
                            break;
                        }
                    }
                }
            }

            OnComplete();
        }

        public string SourceFilePath { get; set; }
        public string DestFilePath { get; set; }

        public event ProgressChangeDelegate OnProgressChanged;
        public event Completedelegate OnComplete;
    }
}
