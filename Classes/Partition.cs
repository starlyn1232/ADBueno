using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBueno.Classes
{
    public class Partition
    {
        // Attributes
        public string Name { get; set; }
        public string realAddress { get; set; }
        public long Size { get; set; }

        // Constructor
        public Partition() { }

        public Partition(string name, string realPath, long size)
        {
            Name = name;
            realAddress = realPath;
            Size = size;
        }

        // Functions
        override public string ToString()
        {
            return $"{Name} ({Size} bytes) at {realAddress}";
        }
    }
}
