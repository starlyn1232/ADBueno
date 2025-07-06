using System.Collections.Generic;
using System.IO;

namespace ADBueno.Utilities
{
    public static class Constants
    {
        // General constants
        public const int SECOND = 1000;

        // ADB MAGICS
        public const string ADB_DETECT_HEADER = "List of devices attached";
        public const string ADB_PUSH_SUCCESS = "file pushed, 0 skipped";
        public const string ADB_PULL_SUCCESS = "file pulled";
        public const string ADB_SUCCESS = "success";
        public const string ADB_APK_GENERIC = "package:";
        public const string ADB_APK_DISABLED = "new state: disabled-user";
        public const string ADB_APK_ENABLED = "new state: default";
        public const string ADB_DD_OKAY = "copied,";
        public const string ADB_ROOT_LEVEL = "root";
        public const string ADB_SIDELOAD_SUCCESS = "total xfer:";

        // ADB Detection wrong modes
        public const string ADB_DETECT_UNAUTHORIZED = "unauthorized";
        public const string ADB_DETECT_OFFLINE = "offline";

        // Errors
        public const string ADB_ERROR = "error:";
        public const string ADB_ERROR_NOT_FOUND = "no such file or directory";
        public const string ADB_ERROR_PERMISSION_MISSING = "Permission denied";
        public const string ADB_ACTIVITY_MISSING = "does not exist";
        public const string ADB_ACTIVITY_CANNOT_START = "Activity not started";
        public const string ADB_SERVICE_CANNOT_START = "no service started.";

        // Paths
        public static string ADB_EXE_PATH =
            $"{Directory.GetCurrentDirectory()}\\adb.exe";

        // Usefull ADB Props
        public readonly static Dictionary<string, string> ADB_PROPS = new Dictionary<string, string>()
        {
            { "model", "ro.product.model" },
            { "brand", "ro.product.brand" },
            { "version", "ro.build.version.release" },
            { "sdk", "ro.build.version.release" },
            { "patch", "ro.build.version.security_patch" },
            { "manufacturer", "ro.product.manufacturer" },
            { "build", "ro.build.display.id" },
            { "name", "ro.product.name" },
            { "carrier", "ro.product.carrier" },
            { "network", "gsm.network.type" },
            { "sim", "gsm.sim.state" },
            { "language", "persist.sys.locale" },
            { "horary", "persist.sys.timezone" },
            { "booted", "sys.boot_completed" }
        };

        // Functions
        public static void UpdateAdbExe(string path)
        {
            ADB_EXE_PATH = path;
        }

    }
}
