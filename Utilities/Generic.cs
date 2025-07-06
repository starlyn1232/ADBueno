using System.Threading;

namespace ADBueno.Utilities
{
    public class Generic
    {
        // Wait using milliseconds
        public static void Wait(int milliSeconds)
        {
            Thread.Sleep(milliSeconds);
        }
    }
}
