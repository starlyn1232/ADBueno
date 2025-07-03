using ADBueno.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ADBueno.Utils.CMDUtils;
using static ADBueno.Utils.Constants;
using static ADBueno.Utils.Debugging;
using static ADBueno.Utils.Util;

// ADBueno Library by Starlyn1232

namespace ADBueno
{
    // Avoid ambiguity using class name 'ADB'
    public class ADB
    {
        // Enumerators
        public enum ADBModes
        {
            Any,
            Device,
            Recovery,
            Sideload
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

        // Attributes
        private string serialNumber = string.Empty;
        private bool rootMode = false;
        public ADBModes mode = ADBModes.Device;

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

        // Properties
        public string GetSerial
        {
            get { return serialNumber; }
        }

        // Check if device is fully booted to the system
        public bool IsBootCompleted
        {
            get
            {
                var checkProp = GetProp(ADB_PROP_BOOTCOMPLETED);
                Aux($"Result: [{checkProp}]");

                return checkProp == "1";
            }
        }

        // Functions
        // Calling class method from static method :)
        public string Cmd(string cmd, bool checkError = true, int timer = 0)
        {
            return ADBCmd(cmd, serialNumber, checkError);
        }

        public string Shell(string cmd, bool checkError = true, int timer = 0)
        {
            if (rootMode)
            {
                cmd = $"su -c {cmd}";
            }

            var result = ADBShell(cmd, serialNumber, checkError, timer: timer);

            // Root mode must grant global access to FS (At least ReadOnly)
            if (rootMode && result.Contains(ADB_ERROR_PERMISSION_MISSING))
                throw new Exception("Root mode is NOT working properly");

            return result;
        }

        public string RootShell(string cmd, bool checkError = true, int timer = 0)
        {
            cmd = $"su -c {cmd}";

            return ADBShell(cmd, serialNumber, checkError, timer: timer);
        }

        public bool RootCheck()
        {
            var checking = Shell($"su -v");

            return !checking.ToLower()
                .Contains(ADB_ERROR_NO_ROOT);
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

        // Connect to emulators
        public bool ConnectToIP(string address)
        {
            // connected to 127.0.0.1:54333
            var request = Cmd($"connect {address}");

            // Now the device will appear at 'DetectDevices' list
            return request.Contains($"connected to {address}");
        }

        // Get / Set properties
        public string GetProp(string property)
        {
            return Shell($"getprop {property}").Trim();
        }

        public string SetProp<T>(string property, T value)
        {
            return Shell($"setprop {property} {value.ToString()}");
        }

        // Run Logcat
        public string Logcat(string filter = "", int timer = 3000)
        {
            var result = Shell($"logcat | grep '{filter}'", timer: timer);
            return result;
        }

        // List files
        // FS = FileSystem
        public List<string> FSList(string path, FSMode mode,
            string containsItem = "", int deepSearch = 0)
        {
            var list = new List<string>();

            if (!path.EndsWith("/"))
                path += "/";

            var queryToFS = string.Empty;

            // Search files using '^-' regexp
            if (mode == FSMode.Files)
            {
                queryToFS = $"shell \"ls -l {path} | grep \'^-\'\"";
            }
            // Search folders using '^-' regexp
            else if (mode == FSMode.Folders)
            {
                queryToFS = $"shell \"ls -l {path} | grep \'^d\'\"";
            }
            // Search files without filter
            else
            {
                queryToFS = $"shell \"ls -l {path}\"";
            }

            var result = Cmd(queryToFS);
            var reader = new StringReader(result);
            var line = string.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                // Parse file/folder name
                if (!line.Contains(':') ||
                    line.ToLower().Contains(ADB_ERROR_NOT_FOUND) ||
                    line == "." || line == "..")
                    continue;

                // Example 2025-05-30 11:58 build.prop = (1):(2)5(3)8(4) build.prop
                list.Add(line.Substring(line.LastIndexOf(':') + 4));
            }

            if (!string.IsNullOrEmpty(containsItem) && list.Count > 0)
            {
                if (list.Contains(containsItem))
                    return FSList($"{path}{list[0]}", mode);

                if (deepSearch == 0)
                {
                    return new List<string>();
                }

                return FSList($"{path}{list[0]}", mode, containsItem, deepSearch - 1);
            }

            return list;
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
            return Cmd($"push \"{filename}\" {destination}", checkError: false)
                .Contains(ADB_PUSH_SUCCESS);
        }

        public bool FSPull(string filename, string saveFile = "")
        {
            Aux($"CMD: [pull \"{filename}\" {saveFile}]");
            var result = Cmd($"pull \"{filename}\" {saveFile}", checkError: false);

            return result.Contains(ADB_PULL_SUCCESS) &&
                !result.ToLower().Contains(ADB_ERROR_NOT_FOUND);
        }

        // Remove files
        public bool FSRemove(string filepath, bool mustExist = true, FSMode mode = FSMode.Any)
        {
            if (mustExist)
            {
                if (!filepath.Contains('/'))
                    throw new ArgumentException("the filepath must contains '/' for correct location");

                if (!FSExist(filepath, mode))
                    return false;
            }

            Shell($"rm -rf {filepath}");
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

        public string FSCreatePermission(FSPermission owner, FSPermission group, FSPermission others)
        {
            int ownerPermission = (int)owner;
            int groupPermission = (int)group;
            int othersPermission = (int)others;

            return $"{ownerPermission}{groupPermission}{othersPermission}";
        }

        public bool FSChmod(string filepath, string permissions, bool mustExist = true)
        {
            if (mustExist)
            {
                if (!filepath.Contains('/'))
                    throw new ArgumentException("the filepath must contains '/' for correct location");

                if (!FSExist(filepath, FSMode.Any))
                    return false;
            }

            Shell($"chmod {permissions} {filepath}");
            return true;
        }

        public bool FSUseDD(string from, string to, bool useRoot = false)
        {
            if (useRoot && !RootCheck())
                return false;

            // adb shell su -c dd if=/data/local/tmp/adb.exe of=/data/local/tmp/adb.exe.ready
            var cmd = $"dd if={from} to={to}";

            return Shell(cmd)
                .Contains(ADB_DD_OKAY);
        }

        // Install / uninstall APK(s)
        public bool APKInstall(string apkfile)
        {
            return Cmd($"install \"{apkfile}\"").ToLower()
                .Contains(ADB_SUCCESS);
        }

        public bool APKUninstall(string packagename)
        {
            return Shell($"pm uninstall {packagename}").ToLower()
                .Contains(ADB_SUCCESS);
        }

        public List<string> APKInstalled(string filter = "")
        {
            var list = new List<string>();

            var cmd = "pm -l";

            // APK Lookup filter
            if (!string.IsNullOrEmpty(filter))
            {
                cmd = $"{cmd} | grep '{filter}'";
            }

            var request = Shell(cmd);
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
            var methodA = fullJob ? Shell($"pm uninstall {packagename}") : string.Empty;
            var methodB = Shell($"pm disable-user {packagename}");

            return methodA
                .ToLower()
                .Contains(ADB_SUCCESS) ||
                methodB
                .ToLower()
                .Contains(ADB_APK_DISABLED);
        }

        public bool APKEnabler(string packagename)
        {
            return Shell($"pm default-state {packagename}")
                .ToLower()
                .Contains(ADB_APK_ENABLED);
        }

        public List<string> ListGPT()
        {
            // Try generic path
            var list = FSList("/dev/block/bootdevice/by-name/", FSMode.Any);

            if (list.Count == 0)
            {
                Aux("Starting dynamic GPT lookup...");

                // We'll iterate deeper in 10 directories looking for 'by-name' folder
                list = FSList("/dev/block/platform/", FSMode.Any, "by-name", 10);

                // I know that this is not okay yet
                if (list.Contains("by-name"))
                {
                    return list;
                }
            }

            return list;
        }

        // Static Funtions

        #region STATIC FUNCTIONS
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
                throw new Exception("DEVICE UNPLUGGED");
            }

            // Check for errors
            if (checkError && result.Contains(ADB_ERROR))
                throw new Exception("ERROR FOUND");

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

                // Make sure the adb permissions are okay
                if (detectionHelper == ADB_DETECT_UNAUTHORIZED)
                {
                    Aux("Unauthorized device detected, skipping...");
                    continue;
                }
                else if (detectionHelper == ADB_DETECT_OFFLINE)
                {
                    Aux("Offline device detected, skipping...");
                    continue;
                }

                devices.Add(new DetectedAdb(line, detectionHelper));
            }

            return devices;
        }

        // Method overload to avoid passing parameters
        public static List<DetectedAdb> DetectDevices()
        {
            return DetectDevices(ADBModes.Any);
        }
        #endregion
    }
}
