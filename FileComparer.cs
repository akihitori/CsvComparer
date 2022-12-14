using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvComparer
{
    public class FileComparer
    {
        public StreamCompare _comparer;

        public FileComparer()
        {
            _comparer = new StreamCompare();
        }

        public FileComparer(uint bufferSize)
        {
            _comparer = new StreamCompare((int)bufferSize);
        }

        public Task<bool> AreEqualAsync(string path1, string path2)
        {
            return AreEqualAsync(path1, path2, CancellationToken.None);
        }

        public async Task<bool> AreEqualAsync(string path1, string path2, CancellationToken cancel)
        {
            path1 = Path.GetFullPath(path1);
            path2 = Path.GetFullPath(path2);

            if (StringComparer.CurrentCulture.Compare(path1, path2) == 0)
            {
                return true;
            }

            var fileInfo1 = new FileInfo(path1);
            var fileInfo2 = new FileInfo(path2);

            if (fileInfo1.Length != fileInfo2.Length)
            {
                return false;
            }

            using (var stream1 = File.OpenRead(path1))
            using (var stream2 = File.OpenRead(path2))
            {
                return await _comparer.AreEqualAsync(stream1, stream2, cancel, false);
            }
        }
    }
}
