using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvComparer
{
    public class Memory
    {
        public static bool Compare(byte[] range1, int offset1, byte[] range2, int offset2, int count)
        {
            var span1 = range1.AsSpan(offset1, count);
            var span2 = range2.AsSpan(offset2, count);

            return span1.SequenceEqual(span2);
        }
    }
}
