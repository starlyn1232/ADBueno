using System.Threading;

namespace ADBueno.Utils
{
    public class Util
    {
        // Wait using milliseconds
        public static void Wait(int milliSeconds)
        {
            Thread.Sleep(milliSeconds);
        }
    }
}
