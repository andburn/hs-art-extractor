namespace HsArtExtractor.Util
{
    public class FilePointer
    {
        public int FileID { get; private set; }
        public long PathID { get; private set; }

        public FilePointer(int file, long path)
        {
            FileID = file;
            PathID = path;
        }

        public override string ToString()
        {
            return FileID + " : " + PathID;
        }
    }

    public class FilePointerWithClass : FilePointer
    {
        public int ClassID { get; private set; }

        public FilePointerWithClass(int classid, int file, long path)
            : base(file, path)
        {
            ClassID = classid;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", ClassID, base.ToString());
        }
    }
}
