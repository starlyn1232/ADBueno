using ADBueno.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static ADBueno.Utilities.CMDUtils;
using static ADBueno.Utilities.Constants;
using static ADBueno.Utilities.Debugging;
using static ADBueno.Utilities.Generic;
using static ADBueno.Utilities.Str;
using static System.Collections.Specialized.BitVector32;

// ADBueno Library by Starlyn1232

namespace ADBueno
{
    // Avoid ambiguity using class name 'ADB'
    public class ADB : IDisposable
    {
        #region ENUMERATORS
        // Enumerators
        public enum ADBModes
        {
            Any,
            Device,
            Recovery,
            Sideload,
            Offline,
            Unauthorized
        }

        public enum RebootModes
        {
            System,
            Recovery,
            Bootloader,
            EDL,
            Download
        }

        public enum FSMode
        {
            Any,
            Files,
            Folders
        }

        public enum FSPermission
        {
            None = 0,
            ExeOnly = 1,
            WriteOnly = 2,
            WriteExe = 3,
            ReadOnly = 4,
            ReadExe = 5,
            ReadWrite = 6,
            ReadWriteExe = 7
        }

        public enum SettingsSector
        {
            System,
            Secure,
            Global
        }

        public enum SettingsTask
        {
            Get,
            Update,
            Delete
        }

        public enum InputKey
        {
            KEYCODE_0 = 0,
            KEYCODE_SOFT_LEFT = 1,
            KEYCODE_SOFT_RIGHT = 2,
            KEYCODE_HOME = 3,
            KEYCODE_BACK = 4,
            KEYCODE_CALL = 5,
            KEYCODE_ENDCALL = 6,
            KEYCODE_1 = 8,
            KEYCODE_2 = 9,
            KEYCODE_3 = 10,
            KEYCODE_4 = 11,
            KEYCODE_5 = 12,
            KEYCODE_6 = 13,
            KEYCODE_7 = 14,
            KEYCODE_8 = 15,
            KEYCODE_9 = 16,
            KEYCODE_STAR = 17,
            KEYCODE_POUND = 18,
            KEYCODE_DPAD_UP = 19,
            KEYCODE_DPAD_DOWN = 20,
            KEYCODE_DPAD_LEFT = 21,
            KEYCODE_DPAD_RIGHT = 22,
            KEYCODE_DPAD_CENTER = 23,
            KEYCODE_VOLUME_UP = 24,
            KEYCODE_VOLUME_DOWN = 25,
            KEYCODE_POWER = 26,
            KEYCODE_CAMERA = 27,
            KEYCODE_CLEAR = 28,
            KEYCODE_A = 29,
            KEYCODE_B = 30,
            KEYCODE_C = 31,
            KEYCODE_D = 32,
            KEYCODE_E = 33,
            KEYCODE_F = 34,
            KEYCODE_G = 35,
            KEYCODE_H = 36,
            KEYCODE_I = 37,
            KEYCODE_J = 38,
            KEYCODE_K = 39,
            KEYCODE_L = 40,
            KEYCODE_M = 41,
            KEYCODE_N = 42,
            KEYCODE_O = 43,
            KEYCODE_P = 44,
            KEYCODE_Q = 45,
            KEYCODE_R = 46,
            KEYCODE_S = 47,
            KEYCODE_T = 48,
            KEYCODE_U = 49,
            KEYCODE_V = 50,
            KEYCODE_W = 51,
            KEYCODE_X = 52,
            KEYCODE_Y = 53,
            KEYCODE_Z = 54,
            KEYCODE_COMMA = 55,
            KEYCODE_PERIOD = 56,
            KEYCODE_ALT_LEFT = 57,
            KEYCODE_ALT_RIGHT = 58,
            KEYCODE_SHIFT_LEFT = 59,
            KEYCODE_SHIFT_RIGHT = 60,
            KEYCODE_TAB = 61,
            KEYCODE_SPACE = 62,
            KEYCODE_SYM = 63,
            KEYCODE_EXPLORER = 64,
            KEYCODE_ENVELOPE = 65,
            KEYCODE_ENTER = 66,
            KEYCODE_DEL = 67,
            KEYCODE_GRAVE = 68,
            KEYCODE_MINUS = 69,
            KEYCODE_EQUALS = 70,
            KEYCODE_LEFT_BRACKET = 71,
            KEYCODE_RIGHT_BRACKET = 72,
            KEYCODE_BACKSLASH = 73,
            KEYCODE_SEMICOLON = 74,
            KEYCODE_APOSTROPHE = 75,
            KEYCODE_SLASH = 76,
            KEYCODE_AT = 77,
            KEYCODE_NUM = 78,
            KEYCODE_HEADSETHOOK = 79,
            KEYCODE_FOCUS = 80,
            KEYCODE_PLUS = 81,
            KEYCODE_MENU = 82,
            KEYCODE_NOTIFICATION = 83,
            KEYCODE_SEARCH = 84,
            KEYCODE_MEDIA_PLAY_PAUSE = 85,
            KEYCODE_MEDIA_STOP = 86,
            KEYCODE_MEDIA_NEXT = 87,
            KEYCODE_MEDIA_PREVIOUS = 88,
            KEYCODE_MEDIA_REWIND = 89,
            KEYCODE_MEDIA_FAST_FORWARD = 90,
            KEYCODE_MUTE = 91,
            KEYCODE_PAGE_UP = 92,
            KEYCODE_PAGE_DOWN = 93,
            KEYCODE_PICTSYMBOLS = 94
        }
        #endregion

        #region ATTRIBUTES
        // Attributes
        private string serialNumber = string.Empty;
        private bool rootMode = false;
        private Dictionary<string, string> properties;
        private List<Partition> gpt = new List<Partition>();
        private string gptPath = string.Empty;
        public ADBModes mode = ADBModes.Device;
        #endregion

        #region CTOR
        // Constructor
        public ADB(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
                throw new ArgumentNullException("The serialNumber cannot be empty");

            this.serialNumber = serialNumber;
            Aux($"ADB Device serial number: {serialNumber}");
        }

        // Use with any serial number
        public ADB()
        {

        }

        // Any serial but specific mode
        public ADB(ADBModes defaultMode)
        {
            this.mode = defaultMode;
        }
        #endregion

        #region PROPERTIES
        // Properties
        public string GetSerial
        {
            get { return serialNumber; }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                if (!UpdateProps())
                {
                    Aux("Failed to update properties");
                    return null;
                }
                return properties;
            }
        }

        // Check if device is fully booted to the system
        public bool IsBootCompleted
        {
            get
            {
                var checkProp = GetProp(ADB_PROPS["Booted"]);
                Aux($"Result: [{checkProp}]");

                return checkProp == "1";
            }
        }
        #endregion

        #region MAIN FUNCTIONS
        void IDisposable.Dispose()
        {
            properties.Clear();
        }
        // Functions
        // Calling class method from static method :)
        public string Cmd(string cmd, bool checkError = true, int timer = 0)
        {
            return ADBCmd(cmd, serialNumber, checkError);
        }

        public string Shell(string cmd, bool checkError = true, int timer = 0)
        {
            if (this.rootMode)
            {
                cmd = $"su -c {cmd}";
            }

            var result = ADBShell(cmd, serialNumber, checkError, timer: timer);

            // Root mode must grant global access to FS (At least ReadOnly)
            if (rootMode && result.Contains(ADB_ERROR_PERMISSION_MISSING))
                throw new ADBException("Root mode is NOT working properly");

            return result;
        }

        public string RootShell(string cmd, bool checkError = true, int timer = 0)
        {
            if (!RootCheck())
            {
                Aux("To user RootShell, you must have root access enabled");
                return string.Empty;
            }

            cmd = $"su -c {cmd}";

            return ADBShell(cmd, serialNumber, checkError, timer: timer);
        }
        #endregion

        #region ROOT FUNCTIONS
        public bool RootCheck(bool saveRootMode = false)
        {
            if (this.rootMode)
                return this.rootMode;

            var checking = Shell($"su -c whoami");
            var rooted = checking.ToLower()
                .Contains(ADB_ROOT_LEVEL);

            if (saveRootMode)
                this.rootMode = rooted;

            return rooted;
        }

        public string RootInfo()
        {
            return Shell($"su -v");
        }

        public bool RootPersistent(bool value)
        {
            if (value && !RootCheck())
                return false;

            this.rootMode = value;
            return true;
        }
        #endregion

        #region REBOOTING
        // Reboot adb devices in differents modes
        public void Reboot(RebootModes mode)
        {
            var rebootMode = string.Empty;

            switch (mode)
            {
                case RebootModes.Recovery:
                    rebootMode = "recovery";
                    break;
                case RebootModes.Bootloader:
                    rebootMode = "bootloader";
                    break;
                case RebootModes.Download:
                    rebootMode = "download";
                    break;
                case RebootModes.EDL:
                    rebootMode = "edl";
                    break;
            }

            Cmd($"reboot {rebootMode}");
            Aux("device rebooted successfully");
        }

        public void Reboot(string mode)
        {
            var command = "reboot";

            if (!string.IsNullOrEmpty(mode))
                command = $"{command} {mode}";

            Cmd(command);
            Aux("device rebooted successfully");
        }

        public void Reboot()
        {
            Reboot(RebootModes.System);
        }

        // Reboot and wait a fixed time
        public bool RebootAndWait(RebootModes rebootMode, ADBModes detectMode,
            int milliSeconds, bool fullyBooted = false)
        {
            Reboot(rebootMode);
            Aux($"Waiting for reconnection...[Completed boot needed: {fullyBooted}]");
            Wait(milliSeconds);
            return QuickDetect(detectMode, fullyBooted);
        }

        public bool RebootAndWait(RebootModes rebootMode, int milliSeconds,
            bool fullyBooted = false)
        {
            return RebootAndWait(rebootMode, this.mode, milliSeconds, fullyBooted);
        }

        public bool RebootAndWait(int milliSeconds, bool fullyBooted = false)
        {
            return RebootAndWait(RebootModes.System, ADBModes.Any, milliSeconds, fullyBooted);
        }

        // Quick device detect
        public bool QuickDetect(ADBModes mode, bool fullyBooted = false)
        {
            var result = DetectDevices(mode);
            var found = result.Find(device => device.SerialNumber == this.serialNumber);

            // Parse conection status
            Aux(found.Mode);

            if (fullyBooted && found != null)
            {
                return IsBootCompleted;
            }

            return found != null;
        }

        public bool QuickDetect(bool fullyBooted = false)
        {
            return QuickDetect(ADBModes.Any, fullyBooted);
        }
        #endregion

        #region ANDROID PROPERTIES MANAGEMENT
        // Get / Set properties
        public string GetProp(string property)
        {
            return Shell($"getprop {property}").Trim();
        }

        public string SetProp<T>(string property, T value)
        {
            return Shell($"setprop {property} {value.ToString()}");
        }

        public bool UpdateProps()
        {
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            else
            {
                // Clear previous properties
                properties.Clear();
            }

            // Get device properties
            var request = Shell("getprop");
            var reader = new StringReader(request);
            var line = string.Empty;
            Match match = null;
            while ((line = reader.ReadLine()) != null)
            {
                match = Regex.Match(line, @"\[(.*?)\]: \[(.*?)\]");

                if (match.Success)
                {
                    properties.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }

            return properties.Count > 0;
        }
        #endregion

        #region FILE SYSTEM MANAGEMENT
        // List files
        // FS = FileSystem
        public List<string> FSList(string path, FSMode mode,
            out string workingPath,
            string containsItem = "", int deepSearch = 0)
        {
            workingPath = path;
            var list = new List<string>();

            if (!path.StartsWith("/"))
                path = $"/{path}";

            if (!path.EndsWith("/"))
                path += "/";

            var queryToFS = string.Empty;

            // Search files using '^-' regexp
            if (mode == FSMode.Files)
            {
                queryToFS = $"ls -l {path} | grep \'^-\'";
            }
            // Search folders using '^-' regexp
            else if (mode == FSMode.Folders)
            {
                queryToFS = $"ls -l {path} | grep \'^d\'";
            }
            // Search files without filter
            else
            {
                queryToFS = $"ls -l {path}";
            }

            var result = Shell(queryToFS);
            var reader = new StringReader(result);
            var line = string.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim('\n','\r');
                Aux(line);

                // Parse file/folder name
                if (!line.Contains(':') ||
                    line.ToLower().Contains(ADB_ERROR_NOT_FOUND) ||
                    line == "." || line == "..")
                    continue;

                // Example 2025-05-30 11:58 build.prop = (1):(2)5(3)8(4) build.prop
                list.Add(line.Substring(line.LastIndexOf(':') + 4));
            }
            
            if (!string.IsNullOrEmpty(containsItem))
            {
                if (list.Contains(containsItem))
                    return FSList($"{path}{list[0]}", mode, out workingPath);

                // No more folders? Okay we're done...
                if (deepSearch == 0 || list.Count == 0)
                {
                    return new List<string>();
                }

                return FSList($"{path}{list[0]}", mode, out workingPath, 
                    containsItem, deepSearch - 1);
            }

            return list;
        }

        public List<string> FSList(string path, FSMode mode,
            string containsItem = "", int deepSearch = 0)
        {
            return FSList(path, mode, out _, containsItem, deepSearch);
        }

        // Check existing file / folder
        public bool FSExist(string path, string filename, FSMode mode)
        {
            if (!path.Contains('/'))
                throw new ArgumentException($"incorrect format path: {path}");

            return FSList(path, mode).Contains(filename);
        }

        public bool FSExist(string filepath, FSMode mode)
        {
            var path = filepath.Substring(0, filepath.LastIndexOf("/"));
            var file = filepath.Substring(filepath.LastIndexOf("/") + 1);

            if (!path.Contains('/'))
                throw new ArgumentException($"incorrect format path: {path}");

            return FSExist(path, file, mode);
        }

        // Push / Pull filesADB_PUSH_SUCCESS
        public bool FSPush(string filename, string destination)
        {
            return Cmd($"push \"{filename}\" \"{destination}\"", checkError: false)
                .Contains(ADB_PUSH_SUCCESS);
        }

        public bool FSPull(string filename, string saveFile = "")
        {
            Aux($"CMD: [pull \"{filename}\" \"{saveFile}\"]");
            var result = Cmd($"pull \"{filename}\" \"{saveFile}\"", checkError: false);

            return result.Contains(ADB_PULL_SUCCESS) &&
                !result.ToLower().Contains(ADB_ERROR_NOT_FOUND);
        }

        public void FSCreate(FSMode mode, string path)
        {
            if (mode == FSMode.Files)
            {
                Shell($"echo '' > {path}");
            }
            else
            {
                Shell($"mkdir {path}");
            }
        }

        public string FSCat(string filepath)
        {
            return Shell($"cat {filepath}");
        }

        // Remove files / folders
        public bool FSRemove(string filepath, 
            bool mustExist = true, 
            FSMode mode = FSMode.Any,
            bool includeAll = false)
        {
            if (mustExist)
            {
                if (filepath[0] != '/')
                    throw new ArgumentException("the filepath must contains '/' for correct location");

                if (!FSExist(filepath, mode))
                    return false;
            }

            if (mode == FSMode.Folders && includeAll)
            {
                if (!filepath.EndsWith("/"))
                    filepath += "/";

                filepath += "*";
            }

            var removeCmd = mode == FSMode.Folders ? "r" : "rf";
            Shell($"rm -{removeCmd} {filepath}");

            return true;
        }

        public bool FSRemoveFile(string filepath, bool mustExist = true)
        {
            return FSRemove(filepath, mustExist, FSMode.Files);
        }

        public bool FSRemoveFolder(string filepath, bool mustExist = true)
        {
            return FSRemove(filepath, mustExist, FSMode.Folders);
        }

        public bool FSClearFolder(string filepath, bool mustExist = true)
        {
            return FSRemove(filepath, mustExist, FSMode.Folders, true);
        }

        // Copy files / folders
        public bool FSCopyFile(string from, string to, bool mustExist = true)
        {
            if (mustExist && !FSExist(from, FSMode.Files))
                return false;

            var request = Shell($"cp {from} {to}");
            return string.IsNullOrWhiteSpace(request);
        }

        public bool FSCopyAll(string from, string to, bool mustExist = true)
        {
            if (mustExist && !FSExist(from, FSMode.Folders))
                return false;

            var request = Shell($"cp -R {from}* {to}");
            return string.IsNullOrWhiteSpace(request);
        }

        public bool FSCopyFolder(string from, string to, bool mustExist = true)
        {
            if (mustExist && !FSExist(from, FSMode.Folders))
                return false;

            var request = Shell($"cp -R {from} {to}");
            return string.IsNullOrWhiteSpace(request);
        }

        // Move files / folders
        public bool FSMove(string from, string to, bool mustExist = true)
        {
            if (mustExist && !FSExist(from, FSMode.Files))
                return false;

            var request = Shell($"mv {from} {to}");
            return string.IsNullOrWhiteSpace(request);
        }

        public string FSCreatePermission(FSPermission owner, FSPermission group, FSPermission others)
        {
            int ownerPermission = (int)owner;
            int groupPermission = (int)group;
            int othersPermission = (int)others;

            return $"{ownerPermission}{groupPermission}{othersPermission}";
        }

        // FS permissions changer
        public bool FSChmod(
            string filepath, 
            string permissions, 
            bool mustExist = true,
            bool allFiles = false)
        {
            if (mustExist)
            {
                if (!filepath.Contains('/'))
                    throw new ArgumentException("the filepath must contains '/' for correct location");

                if (!FSExist(filepath, FSMode.Any))
                    return false;
            }

            if (allFiles)
            {
                if (!filepath.EndsWith("/"))
                    filepath += "/";

                filepath += "*";
            }

            Shell($"chmod {permissions} {filepath}");
            return true;
        }

        public bool FSUseDD(string from, string to, bool useRoot = false)
        {
            if (useRoot && !RootCheck())
                return false;
            
            var cmd = $"dd if={from} of={to}";
            var result = Shell(cmd);

            return result
                .Contains(ADB_DD_OKAY);
        }

        public bool FSZerout(string filename)
        {
            if (!RootCheck())
            {
                Aux("You must have root access enabled to use this function");
                return false;
            }

            return Shell($"dd if=/dev/zero of={filename}")
                .Contains(ADB_DD_OKAY);
        }

        // Get size using 'blockdev --getsize64 <file>'
        public long FSGetSize(string filepath, bool skipRootCheck = false)
        {
            if (!skipRootCheck && !RootCheck())
            {
                Aux("You must have root access enabled to use this function");
                return -1;
            }
            var result = RootShell($"blockdev --getsize64 {filepath}");

            if (long.TryParse(result.Trim(), out var size))
                return size;

            return -1;

        }
        #endregion

        #region APKS MANAGEMENT
        // Install / uninstall APK(s)
        public bool APKInstall(string apkfile)
        {
            return Cmd($"install \"{apkfile}\"").ToLower()
                .Contains(ADB_SUCCESS);
        }

        public bool APKUninstall(string packagename)
        {
            return PackageManager($"uninstall {packagename}").ToLower()
                .Contains(ADB_SUCCESS);
        }

        public List<string> APKList(string filter = "")
        {
            var list = new List<string>();

            var cmd = "-l";

            // APK Lookup filter
            if (!string.IsNullOrEmpty(filter))
            {
                cmd = $"{cmd} | grep '{filter}'";
            }

            var request = PackageManager(cmd);
            var reader = new StringReader(request);
            var line = string.Empty;
            var subLine = ADB_APK_GENERIC.Length;

            while ((line = reader.ReadLine()) != null)
            {
                if (!line.Contains(ADB_APK_GENERIC))
                    continue;

                // package:com.tcl.systemui.op = com.tcl.systemui.op
                list.Add(line.Substring(subLine));
            }

            return list;
        }

        public bool APKDisabler(string packagename, bool fullJob = true)
        {
            var methodA = fullJob ? PackageManager($"uninstall {packagename}") : string.Empty;
            var methodB = PackageManager($"disable-user {packagename}");

            return methodA
                .ToLower()
                .Contains(ADB_SUCCESS) ||
                methodB
                .ToLower()
                .Contains(ADB_APK_DISABLED);
        }

        public bool APKEnabler(string packagename)
        {
            return PackageManager($"default-state {packagename}")
                .ToLower()
                .Contains(ADB_APK_ENABLED);
        }

        public string APKPath(string packageName)
        {
            return PackageManager($"path {packageName}");
        }

        public List<string> APKData(string packageName)
        {
            if (!RootCheck(true))
            {
                Aux("You need root access.");
                return new List<string>();
            }

            // Get installed apps list
            var apps = APKList();
            var count = apps.Count;
            var found = false;

            for (int i = 0; i < apps.Count; i++)
            {
                if (apps[i] == packageName)
                {
                    found = true;
                    break;
                }
            }

            // Check if desired packaged in found
            if (!found)
            {
                Aux("This package is not installed.");
                return new List<string>();
            }

            // Get package data files
            return FSList($"/data/data/{packageName}", FSMode.Any);
        }
        #endregion

        #region PACKAGE MANAGER
        // Package Manager
        private string PackageManager(string command)
        {
            return Shell($"pm {command}");
        }

        public bool PackageClear(string packageName)
        {
            return PackageManager($"clear {packageName}").ToLower()
                .Contains(ADB_SUCCESS);
        }

        public string PackageDumpInfo(string packageName)
        {
            // Get package info
            return PackageManager($"dump {packageName}");
        }

        public void PackageHide(string packageName)
        {
            // Hide the package
            PackageManager($"hide {packageName}");
        }

        public void PackageUnhide(string packageName)
        {
            // Unhide the package
            PackageManager($"unhide {packageName}");
        }

        public void PackageGrant(string packageName, string permission)
        {
            // Grant the permission to the package
            PackageManager($"grant {packageName} {permission}");
        }

        public void PackageRevoke(string packageName, string permission)
        {
            // Revoke the permission from the package
            PackageManager($"revoke {packageName} {permission}");
        }

        public void PackageResetPermissions(string packageName)
        {
            // Reset the permissions of the package
            PackageManager($"reset-permissions {packageName}");
        }

        public void PackageCreateUser(string userName)
        {
            // Create a new user
            PackageManager($"create-user {userName}");
        }

        public void PackageRemoveUser(int userId)
        {
            // Remove the user
            PackageManager($"remove-user {userId}");
        }

        #endregion

        #region ACTIVITY MANAGER
        // Activity Manager
        private string ActivityManager(string command)
        {
            return Shell($"am {command}");
        }

        public List<string> ActivityList(string filter = "")
        {
            filter = filter.ToLower();

            var filtered = !string.IsNullOrEmpty(filter);
            var result = ActivityManager("stack list");
            var reader = new StringReader(result);
            var line = string.Empty;
            var activities = new HashSet<string>();

            // Regex to match: anyvalue/anyvalue (package/activity)
            var regex = new Regex(@"([a-zA-Z][a-zA-Z0-9_\.]*\/\.?[a-zA-Z0-9_\.]+)\s*", 
                RegexOptions.Compiled);

            while ((line = reader.ReadLine()) != null)
            {
                if (!line.Contains("taskId"))
                    continue;

                var match = regex.Match(line);
                if (match.Success)
                {
                    if (filtered && !match.Groups[1].Value
                        .ToLower().Contains(filter))
                        continue;

                    activities.Add(match.Groups[1].Value);
                }
            }

            return activities.ToList();
        }

        public bool ActivityStart(string packagename, string activityName = "")
        {
            // Check if activity name is valid
            if (string.IsNullOrEmpty(packagename))
                throw new ArgumentException("Activity name cannot be empty");

            if (!string.IsNullOrEmpty(activityName))
            {
                packagename = $"{packagename}/.{activityName}";
            }

            // Start the activity
            var result = ActivityManager($"start -n {packagename}");
            return !result.ToLower()
                .Contains(ADB_ACTIVITY_MISSING);
        }

        public bool ActivityStartAction(string action, string data)
        {
            var result = ActivityManager($"start -a {action} -d \'{data}\'");
            return
                !result.ToLower()
                .Contains(ADB_ACTIVITY_CANNOT_START);
        }

        public bool ActivityStartService(string activity)
        {
            // no service started.
            var result = ActivityManager($"startservice {activity}");
            return
                !result.ToLower()
                .Contains(ADB_SERVICE_CANNOT_START);
        }

        public void ActivityStop(string activity)
        {
            // Stop the service
            ActivityManager($"stopservice {activity}");
        }

        public void ActivityForceStop(string packageName)
        {
            // Stop the service
            ActivityManager($"force-stop {packageName}");
        }

        public void ActivityKill(string packageName)
        {
            // Stop the service
            ActivityManager($"kill {packageName}");
        }

        public void ActivityBroadcast(string action)
        {
            ActivityManager($"broadcast -a \'{action}\'");
        }

        //public bool ActivityStart(string)
        #endregion

        #region INPUT MANAGER
        public string InputManager(string command)
        {
            return Shell($"input {command}");
        }

        public bool InputText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            // Escape spaces
            text = text.Replace(" ", "%s");
            var result = InputManager($"text \"{text}\"");
            return string.IsNullOrWhiteSpace(result);
        }

        public bool InputKeyEvent(InputKey key, bool longPress = false)
        {
            var keyEvent = longPress ? "keyevent --longpress" : "keyevent";
            var result = InputManager($"{keyEvent} {(int)key}");
            return string.IsNullOrWhiteSpace(result);
        }

        public bool InputSwipe(int x1, int y1, int x2, int y2, int duration = 300)
        {
            var result = InputManager($"swipe {x1} {y1} {x2} {y2} {duration}");
            return string.IsNullOrWhiteSpace(result);
        }

        public bool InputDragDrop(int x1, int y1, int x2, int y2, int duration = 300)
        {
            var result = InputManager($"draganddrop {x1} {y1} {x2} {y2} {duration}");
            return string.IsNullOrWhiteSpace(result);
        }

        public bool InputTap(int x, int y)
        {
            var result = InputManager($"tap {x} {y}");
            return string.IsNullOrWhiteSpace(result);
        }
        #endregion

        #region GPT MANAGER
        public bool GPTMount(string partition = "")
        {
            return string.IsNullOrWhiteSpace(RootShell($"mount -o rw,remount /{partition}"));
        }

        public void GPTEngMount()
        {
            Cmd("remount");
        }

        public bool GPTFind(string partitionName, out Partition partition)
        {
            // List GPT
            var gpt = GPTList(out var path, false);
            partition = gpt.Find(p => p.Name == partitionName);
            return partition != null;
        }

        public bool GPTZerout(string partitionName)
        {
            if (!GPTFind(partitionName, out var partition))
                return false;

            return FSUseDD("/dev/zero", partition.realAddress, true);
        }

        public bool GPTDump(string partitionName, string saveFile = "")
        {
            if (!GPTFind(partitionName, out var partition))
                return false;

            // Save at temporal folder if destination is
            // not specified
            if (string.IsNullOrEmpty(saveFile))
                saveFile = $"/data/local/tmp/{partitionName}.bin";

            return FSUseDD(partition.realAddress, saveFile, true);
        }

        public bool GPTWrite(string partitionName, string writeFile)
        {
            if (!GPTFind(partitionName, out var partition))
                return false;

            return FSUseDD(writeFile, partition.realAddress, true);
        }

        public List<Partition> GPTList(out string gptPath, 
            bool getSize = true,
            bool forcedUpdate = false)
        {
            // Check if table was already found
            if (!forcedUpdate && !string.IsNullOrEmpty(this.gptPath))
            {
                gptPath = this.gptPath;
                return this.gpt;
            }

            // Assign generic table path
            gptPath = "/dev/block/bootdevice/by-name/";

            // Lookup partitions
            var list = FSList(gptPath, FSMode.Any);

            var found = false;

            // No found, let's play something called... recursivity!
            if (list.Count == 0)
            {
                // We'll iterate deeper in 10 directories looking for 'by-name' folder
                list = FSList("/dev/block/platform/", FSMode.Any, out gptPath,
                    "by-name", 10);
            }

            // Clear previous table
            this.gpt.Clear();
            int len = list.Count;

            // Check if we found the 'userdata' partition
            for (int i = 0; i < len; i++)
            {
                if (list[i].Contains("userdata ->"))
                {
                    found = true;
                    break;
                }
            }

            // Parse partitions
            if (found)
            {
                bool hasRoot = getSize && RootCheck();

                for (int i = 0; i < len; i++)
                {
                    if (!list[i].Contains(" -> "))
                        continue;
                    var partitionName = list[i].Substring(0, list[i].IndexOf(" -> "));
                    var realPath = list[i].Substring(list[i].IndexOf(" -> ") + 4);
                    this.gpt.Add(new Partition()
                    {
                        Name = partitionName,
                        realAddress = realPath,
                        Size = hasRoot ? FSGetSize(realPath, skipRootCheck: true) : -1
                    });
                }

                this.gptPath = gptPath;
            }

            return this.gpt;
        }
        #endregion

        #region SETTINGS MANAGEMENT
        // Manage device settings
        private string SettingsSectorStr(SettingsSector sector)
        {
            var _namespace = "system";
            switch (sector)
            {
                case SettingsSector.Secure:
                    _namespace = "secure";
                    break;
                case SettingsSector.Global:
                    _namespace = "global";
                    break;
            }
            return _namespace;
        }
        private string SettingsTaskStr(SettingsTask task)
        {
            var _namespace = "get";
            switch (task)
            {
                case SettingsTask.Update:
                    _namespace = "put";
                    break;
                case SettingsTask.Delete:
                    _namespace = "delete";
                    break;
            }
            return _namespace;
        }

        public Dictionary<string, string> SettingsList(SettingsSector sector)
        {
            var _namespace = SettingsSectorStr(sector);
            var result = Shell($"settings list {_namespace}");
            var reader = new StringReader(result);
            var line = string.Empty;
            var settings = new Dictionary<string, string>();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // More robust: allow any key and any value (including empty)
                var match = Regex.Match(line, @"^([^=]+)=(.*)$");
                if (match.Success)
                {
                    settings[match.Groups[1].Value] = match.Groups[2].Value;
                }
            }

            return settings;
        }

        public bool SettingsManager(SettingsSector sector, SettingsTask task, string property,
            string value = "", bool checkExisting = false)
        {
            // Check if property exists at the sector
            if (checkExisting)
            {
                var exists = SettingsList(sector).TryGetValue(property, out var currentValue);

                if (!exists)
                    return exists;
            }

            // Update the property
            var _namespace = SettingsSectorStr(sector);
            var _task = SettingsTaskStr(task);

            // Prepare value
            if (!string.IsNullOrEmpty(value))
                value = $" {value}";

            var command = $"settings {_task} {_namespace} {property}{value}";
            var result = Shell(command);

            return true;
        }

        public bool SettingsUpdate(SettingsSector sector, string property, string value)
        {
            // Update the property
            SettingsManager(sector, SettingsTask.Update, property, value, checkExisting: true);
            // Confirm if the property was updated
            return SettingsList(sector).TryGetValue(property, out var currentValue) &&
                   currentValue == value;
        }

        public string SettingsGet(SettingsSector sector, string property)
        {
            // Check if property exists at the sector, then get it
            var exists = SettingsList(sector).TryGetValue(property, out var currentValue);
            return exists ? currentValue : string.Empty;
        }

        public bool SettingsDelete(SettingsSector sector, string property)
        {
            // Delete the property
            SettingsManager(sector, SettingsTask.Delete, property);
            // Confirm if the property was deleted
            return !SettingsList(sector).TryGetValue(property, out var currentValue);
        }

        // Organized simplified functions

        // Global settings
        public string SettingsGlobalGet(string property)
        {
            return SettingsGet(SettingsSector.Global, property);
        }

        public bool SettingsGlobalUpdate<T>(string property, T value)
        {
            return SettingsUpdate(SettingsSector.Global, property, value.ToString());
        }

        public bool SettingsGlobalDelete(string property)
        {
            return SettingsDelete(SettingsSector.Global, property);
        }

        // Secure settings
        public string SettingsSecureGet(string property)
        {
            return SettingsGet(SettingsSector.Secure, property);
        }

        public bool SettingsSecureUpdate<T>(string property, T value)
        {
            return SettingsUpdate(SettingsSector.Secure, property, value.ToString());
        }

        public bool SettingsSecureDelete(string property)
        {
            return SettingsDelete(SettingsSector.Secure, property);
        }

        // System settings
        public string SettingsSystemGet(string property)
        {
            return SettingsGet(SettingsSector.System, property);
        }

        public bool SettingsSystemUpdate<T>(string property, T value)
        {
            return SettingsUpdate(SettingsSector.System, property, value.ToString());
        }

        public bool SettingsSystemDelete(string property)
        {
            return SettingsDelete(SettingsSector.System, property);
        }
        #endregion

        #region EXTRA FUNCTIONS
        // Get device information
        public Dictionary<string, string> GetDeviceInfo()
        {
            // Check if properties are already loaded
            if (properties == null)
            {
                if (!UpdateProps())
                {
                    Aux("Failed to update properties");
                    return null;
                }
            }

            var info = new Dictionary<string, string>();

            // Get main properties
            foreach (var prop in ADB_PROPS)
            {
                if (properties.TryGetValue(prop.Value, out var value))
                {
                    // Parse property value
                    info.Add(prop.Key, ParseProperty(value));
                }
                else
                {
                    info.Add(prop.Key, "NOT FOUND");
                }
            }

            return info.Count > 0 ? info : null;
        }

        // Remove FRP functions
        public bool RemoveFRP()
        {
            return SettingsSecureUpdate("user_setup_complete", 1);
        }

        // Run Logcat
        public string Logcat(string filter = "", int timer = 3000)
        {
            var result = Shell($"logcat | grep '{filter}'", timer: timer);
            return result;
        }

        public string RunScript(string scriptFile, bool useRoot = false)
        {
            if (useRoot && !RootCheck())
                return string.Empty;

            // Allow execution
            FSChmod(scriptFile,
                FSCreatePermission(
                    FSPermission.ReadWriteExe,
                    FSPermission.ReadWriteExe,
                    FSPermission.ReadWriteExe));

            // Run script
            return Shell($".{scriptFile}");
        }

        public ScreenSize GetScreenSize()
        {
            var request = Shell("wm size");
            request = request.ToLower();

            if (request.Contains("override size:"))
            {
                request = request.Substring(request.IndexOf("override size:"));
                request = request.Replace("override size: ", "").Trim();
            }
            else if (request.Contains("physical size:"))
            {
                request = request.Replace("physical size: ", "").Trim();
            }
            else
            {
                return null;
            }

            var dimensions = request.Split('x');
            if (dimensions.Length == 2 &&
                int.TryParse(dimensions[0], out var width) &&
                int.TryParse(dimensions[1], out var height))
            {
                return new ScreenSize(width, height);
            }

            return null;
        }

        public bool ScreenCapture(string saveFile)
        {
            Cmd($"exec-out screencap -p > \"{saveFile}\"");
            return File.Exists(saveFile);
        }

        public bool ScreenRecording(string saveFile, 
            int durationSeconds,
            long bitRate = 8000000,
            ScreenSize resolution = default(ScreenSize),
            bool useNativeResolution = false)
        {
            if (useNativeResolution)
            {
                var size = GetScreenSize();

                if (size is null)
                {
                    Aux("Failed to get screen size");
                    return false;
                }
            }

            var command = 
                "screenrecord " +
                // Options
                $"--bit-rate {bitRate} " +
                $"--size {resolution} " +
                $"--time-limit{durationSeconds * SECOND} " +
                // Finally the file path
                $"{saveFile}";

            var request = Shell(command);

            // No output means NO ERRORS!!
            return string.IsNullOrWhiteSpace(request);
        }

        public bool Sideload(string uploadFile)
        {
            var request = ADBShell($"sideload \"{uploadFile}\"").ToLower();
            return request.Contains(ADB_SIDELOAD_SUCCESS) && 
                !request.Contains(ADB_ERROR);
        }

        public bool SELinuxSet(bool state)
        {
            var request = RootShell($"setenforce {(state ? "0" : "1")}");
            return string.IsNullOrWhiteSpace(request);
        }

        public string SELinuxGet()
        {
            return Shell("getenforce").Trim();
        }
        #endregion

        #region STATIC FUNCTIONS
        // Static Funtions

        // Run ADB Command
        /// <summary>
        /// Run ADB commands.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="serialNumber"></param>
        /// <param name="checkError"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string ADBCmd(string command, string serialNumber = "",
            bool checkError = true, bool trimResult = false, int timer = 0)
        {
            var useSN = !string.IsNullOrEmpty(serialNumber);

            // Parse serial Number
            if (useSN)
            {
                command = $"-s {serialNumber} {command}";
            }

            var result = CMDRun(ADB_EXE_PATH, command, timer: timer);

            // Make sure device still connected
            if (useSN && result.Contains($"device '{serialNumber}' not found"))
            {
                throw new ADBException("DEVICE UNPLUGGED");
            }

            // Check for errors
            if (checkError && result.Contains(ADB_ERROR))
                throw new ADBException("ERROR FOUND");

            return trimResult ? result.Trim() : result;
        }

        /// <summary>
        /// Run ADB Shell commands
        /// </summary>
        /// <param name="command"></param>
        /// <param name="serialNumber"></param>
        /// <param name="checkError"></param>
        /// <returns></returns>
        public static string ADBShell(string command, string serialNumber = "",
            bool checkError = true, int timer = 0)
        {
            return ADBCmd($"shell \"{command}\"", serialNumber, checkError, timer: timer);
        }

        /// <summary>
        /// Detect ADB devices.
        /// </summary>
        /// <returns></returns>
        public static List<DetectedAdb> DetectDevices(ADBModes mode)
        {
            // Any is the default
            var detectionMode = string.Empty;
            var detectionHelper = string.Empty;
            var devices = new List<DetectedAdb>();

            var request = ADBCmd("devices");
            var line = string.Empty;
            var reader = new StringReader(request);

            // Prepare detection mode
            switch (mode)
            {
                case ADBModes.Device:
                    detectionMode = "device";
                    break;
                case ADBModes.Recovery:
                    detectionMode = "recovery";
                    break;
                case ADBModes.Sideload:
                    detectionMode = "sideload";
                    break;
            }

            // While we have lines to read
            // continue reading!
            while ((line = reader.ReadLine()) != null)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                // Skip header
                else if (line.Contains(ADB_DETECT_HEADER))
                    continue;

                // Parse detection line
                var lastTab = line.LastIndexOf('\t');
                var lastSpace = line.LastIndexOf(' ');

                // 127.0.0.1:47157 device

                // Check for tab
                if (lastTab > -1)
                {
                    detectionHelper = line.Substring(line.LastIndexOf('\t'));
                    line = line.Substring(0, line.LastIndexOf('\t'));
                }
                // Or for space
                else if (lastSpace > -1)
                {
                    detectionHelper = line.Substring(line.LastIndexOf(' '));
                    line = line.Substring(0, line.LastIndexOf(' '));
                }

                detectionHelper = detectionHelper.Trim(' ', '\t');

                if (!string.IsNullOrWhiteSpace(detectionMode))
                {
                    if (!detectionHelper.Contains(detectionMode))
                        continue;
                }

                var ready = true;

                // Make sure the adb permissions are okay
                if (detectionHelper == ADB_DETECT_UNAUTHORIZED)
                {
                    ready = false;
                    Aux("Unauthorized device detected, skipping...");
                }
                else if (detectionHelper == ADB_DETECT_OFFLINE)
                {
                    ready = false;
                    Aux("Offline device detected, skipping...");
                }

                var newDevice = new DetectedAdb(line, Str2ADBModes(detectionHelper), ready);
                devices.Add(newDevice);
            }

            return devices;
        }

        // Method overload to avoid passing parameters
        public static List<DetectedAdb> DetectDevices()
        {
            return DetectDevices(ADBModes.Any);
        }

        public static void StopServer()
        {
            // Stop ADB server
            ADBCmd("kill-server", checkError: false);
        }

        public static void StartServer()
        {
            // Start ADB server
            ADBCmd("devices", checkError: false);
        }

        // Connect to emulators
        public static bool ConnectToIP(string address)
        {
            // connected to 127.0.0.1:54333
            var request = ADBCmd($"connect {address}");

            // Now the device will appear at 'DetectDevices' list
            return request.Contains($"connected to {address}");
        }

        public static bool DisconnectFromIP(string address)
        {
            // connected to 127.0.0.1:54333
            var request = ADBCmd($"disconnect {address}");

            // Now the device will appear at 'DetectDevices' list
            return request.Contains($"disconnected {address}");
        }
        #endregion
    }
}

// ADBueno Library by Starlyn1232
