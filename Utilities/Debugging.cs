using System;

namespace ADBueno.Utilities
{
    public static class Debugging
    {
        public delegate void DebugDelegate(string message);

        // Custom debugging event
        private static DebugDelegate DebugEvent = null;

        // Debugging mode
        public static bool DEBUGGING = false;

        public static void Aux<T>(T value)
        {
            if (!DEBUGGING)
                return;

            if (DebugEvent == null)
                Console.WriteLine(value.ToString());
            else
                DebugEvent(value.ToString());
        }

        public static void UpdateAux(DebugDelegate customAux)
        {
            DebugEvent = customAux;
        }
    }
}
