using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRC_table
{
    class Program
    {
        static void Main(string[] args)
        {
            var crc = Crc32.Compute(new byte[] { 0, 1, 2 });
            var builder = new StringBuilder();
            builder.Append("{");
            foreach (var val in Crc32.defaultTable)
            {
                builder.AppendFormat("0x{0:X8},", val);
               
            }
            var str = builder.ToString();
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}
