using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBueno.Classes
{
    public class ScreenSize
    {
        // Attributes
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;

        // Constructor
        public ScreenSize() { }
        public ScreenSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        // Functions
        override public string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
