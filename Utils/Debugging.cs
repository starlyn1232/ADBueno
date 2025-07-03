using System;

namespace ADBueno.Utils
{
    public static class Debugging
    {
        // Debugging mode
        public static bool DEBUGGING = false;

        public static void Aux<T>(T value)
        {
            if (!DEBUGGING)
                return;

            Console.WriteLine(value.ToString());
        }
    }
}
