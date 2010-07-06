using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OutlookKolab
{
    public class FileTransaction : IDisposable
    {
        string _destpath;
        string _filename;

        public FileTransaction(string fullFileName)
            : this(Path.GetDirectoryName(fullFileName), Path.GetFileName(fullFileName))
        {
        }

        public FileTransaction(string destPath, string filename)
        {
            FixBrokenTransaction(destPath, filename);

            _destpath = destPath;
            _filename = filename;
        }

        public static void FixBrokenTransaction(string fullFileName)
        {
            var file = fullFileName;
            var tmp = fullFileName + ".tmp";
            if (System.IO.File.Exists(tmp) && !System.IO.File.Exists(file))
            {
                System.IO.File.Move(tmp, file);
            }
            else if (System.IO.File.Exists(tmp) && System.IO.File.Exists(file))
            {
                System.IO.File.Delete(tmp);
            }

        }

        public static void FixBrokenTransaction(string destPath, string filename)
        {
            FixBrokenTransaction(Path.Combine(destPath, filename));
        }

        public void Dispose()
        {
            Abort();
        }

        public string FullFileName
        {
            get
            {
                return Path.Combine(_destpath, _filename);
            }
        }

        public string FullTempFileName
        {
            get
            {
                return Path.Combine(_destpath, _filename + ".tmp");
            }
        }

        public void Commit()
        {
            if (System.IO.File.Exists(FullFileName))
            {
                System.IO.File.Delete(FullFileName);
            }
            System.IO.File.Move(FullTempFileName, FullFileName);
        }

        public void Abort()
        {
            if (System.IO.File.Exists(FullTempFileName))
            {
                System.IO.File.Delete(FullTempFileName);
            }
        }
    }
}
