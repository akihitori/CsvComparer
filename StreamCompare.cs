
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CsvComparer
{
    public class StreamCompare
    {
        public static int DefaultBufferSize = 4096;
        public readonly int BufferSize = DefaultBufferSize;
        
        private byte[] _buffer1;
        private byte[] _buffer2;

        public StreamCompare() : this(DefaultBufferSize) { }

        public StreamCompare(int bufferSize)
        {
            BufferSize = bufferSize;

            _buffer1 = new byte[BufferSize];
            _buffer2 = new byte[BufferSize];
        }

        public async Task<bool> AreEqualAsync(Stream stream1, Stream stream2,
            CancellationToken cancel, bool? forceLengthCompare = null)
        {
            if (stream1 == stream2)
            {
                return true;
            }
            //using var nocontext = ChangeContext.NoContext();

            if ((!forceLengthCompare.HasValue && stream1.CanSeek && stream2.CanSeek)
                || (forceLengthCompare.HasValue && forceLengthCompare.Value))
            {
                if (stream1.Length != stream2.Length)
                {
                    return false;
                }
            }

            long offset1 = 0;
            long offset2 = 0;

            while (true)
            {
                var task1 = stream1.ReadAsync(_buffer1, 0, BufferSize, cancel);
                var task2 = stream2.ReadAsync(_buffer2, 0, BufferSize, cancel);
                var bytesRead = await Task.WhenAll(task1, task2);
                var bytesRead1 = bytesRead[0];
                var bytesRead2 = bytesRead[1];

                if (bytesRead1 == 0 && bytesRead2 == 0)
                {
                    break;
                }

                int sharedCount = Math.Min(bytesRead1, bytesRead2);

                if (!Memory.Compare(_buffer1, 0, _buffer2, 0, sharedCount))
                {
                    return false;
                }

                if (bytesRead1 != bytesRead2)
                {
                    var lessCount = 0;
                    var (lessRead, moreRead, moreCount, lessStream, moreStream)
                        = bytesRead1 < bytesRead2
                        ? (_buffer1, _buffer2, bytesRead2 - sharedCount, stream1, stream2)
                        : (_buffer2, _buffer1, bytesRead1 - sharedCount, stream2, stream1);

                    while (moreCount > 0)
                    {
                        lessCount = await lessStream.ReadAsync(lessRead, 0, moreCount, cancel);

                        if (lessCount == 0)
                        {
                            return false;
                        }

                        if (!Memory.Compare(lessRead, 0, moreRead, sharedCount, lessCount))
                        {
                            return false;
                        }

                        moreCount -= lessCount;
                        sharedCount += lessCount;
                    }
                }
                offset1 += sharedCount;
                offset2 += sharedCount;
            }
            return true;
        }
    }
}
