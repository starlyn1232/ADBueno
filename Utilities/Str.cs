using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ADBueno.ADB;

namespace ADBueno.Utilities
{
    public static class Str
    {
        // Parse bool android properties
        public static string BoolStr(string value)
        {
            return value == "true" || value == "1" ? "YES" : "NO";
        }

        // Parse android properties
        public static string ParseProperty(string value)
        {
            var help = value.ToLower();

            if (help == "0" || help == "1" ||
                help == "false" || help == "true")
            {
                return BoolStr(value);
            }
            else
            {
                return value;
            }
        }

        // String to ADBModes
        public static ADBModes Str2ADBModes(string mode)
        {
            mode = mode.ToLower();
            ADBModes checking;

            switch (mode)
            {
                case "device":
                    checking = ADBModes.Device;
                    break;
                case "recovery":
                    checking = ADBModes.Recovery;
                    break;
                case "sideload":
                    checking = ADBModes.Sideload;
                    break;
                case "offline":
                    checking = ADBModes.Offline;
                    break;
                case "unauthorized":
                    checking = ADBModes.Unauthorized;
                    break;
                default:
                    checking = ADBModes.Any;
                    break;
            }

            return checking;
        }

        // ADBModes to string
        public static string ADBModes2Str(ADBModes mode)
        {
            string checking;
            switch (mode)
            {
                case ADBModes.Device:
                    checking = "device";
                    break;
                case ADBModes.Recovery:
                    checking = "recovery";
                    break;
                case ADBModes.Sideload:
                    checking = "sideload";
                    break;
                case ADBModes.Offline:
                    checking = "offline";
                    break;
                case ADBModes.Unauthorized:
                    checking = "unauthorized";
                    break;
                default:
                    checking = "any";
                    break;
            }
            return checking;
        }
    }
}
