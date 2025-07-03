using System.IO;

namespace ADBueno.Utils
{
    internal static class Constants
    {
        // ADB MAGICS
        internal const string ADB_DETECT_HEADER = "List of devices attached";
        internal const string ADB_PUSH_SUCCESS = "file pushed, 0 skipped";
        internal const string ADB_PULL_SUCCESS = "file pulled";
        internal const string ADB_SUCCESS = "success";
        internal const string ADB_APK_GENERIC = "package:";
        internal const string ADB_APK_DISABLED = "new state: disabled-user";
        internal const string ADB_APK_ENABLED = "new state: default";
        internal const string ADB_DD_OKAY = "copied,";

        // ADB Detection wrong modes
        internal const string ADB_DETECT_UNAUTHORIZED = "unauthorized";
        internal const string ADB_DETECT_OFFLINE = "offline";

        // Errors
        internal const string ADB_ERROR = "error:";
        internal const string ADB_ERROR_NOT_FOUND = "no such file or directory";
        internal const string ADB_ERROR_NO_ROOT = "/system/bin/sh:";
        internal const string ADB_ERROR_PERMISSION_MISSING = "Permission denied";

        // Paths
        internal static readonly string ADB_EXE_PATH =
            $"{Directory.GetCurrentDirectory()}\\adb.exe";

        // Usefull ADB Props
        internal const string ADB_PROP_BOOTCOMPLETED = "sys.boot_completed";
    }
}
