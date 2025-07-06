# ADBueno: Your New Favorite ADB Library for C#

ADBueno is a powerful and comprehensive C# library designed to simplify your interactions with Android Debug Bridge (ADB). Whether you're a developer, tester, or automation engineer, ADBueno provides a robust and easy-to-use set of functionalities to streamline your Android device management and testing workflows.

📺 **Development Journey**  
Want to see how ADBueno came to life? Check out the development process on my YouTube channel:  
[Watch the development series here!](https://www.youtube.com/watch?v=Fawe9lNzrJ4)

---

## ✨ Features

### 🔌 Device Management
- Detect single or multiple connected ADB devices (including unauthorized/offline).
- Connect to devices via specific serials or universally.
- Detect specific device modes and boot completion.
- Reboot into normal, bootloader, recovery, or custom modes.
- Reboot and wait for device readiness.
- Connect/disconnect to emulators or hidden devices via ADB CONNECT.
- Start/Stop ADB server.
- Change ADB executable location dynamically.

### 🧠 Device Info & Properties
- Read all Android properties (`UpdateProps`).
- Get main device information (`GetDeviceInfo`).
- Use predefined constants for common `getprop` keys (`ADB_PROPS`).
- New `Str` utility class for string operations.

### 🗂️ File System Operations
- Push, pull, copy, move, and remove files or folders.
- Create files and directories (`FSCreate`).
- Check file/folder existence and size (`FSGetSize`).
- Change permissions recursively (`FSChmod`).
- Clear folder contents (`FSClearFolder`).
- Read file contents (`FSCat`), zerout files (`FSZerout`), and run scripts (`RunScript`).
- Improved `FSRemove`, `FSPush`, `FSPull`, and `FSList` with additional path outputs.

### 📦 APK & Package Management
- Install, uninstall, and list APKs (`APKList`).
- Get APK installation path (`APKPath`).
- Check APK-related data files (`APKData`).
- Enable/disable packages.
- New `PackageManager`:
  - Clear app data, dump info, hide/unhide apps.
  - Grant/revoke permissions.
  - Manage users.

### 🧰 Advanced Tools
- Capture and filter logcat output.
- Execute `dd` commands (root).
- List GPT partitions (`GPTList`) and manage them with `GPTManager`:
  - Find, mount, zerout, dump, and write partitions.

### 📺 Screen
- Get screen resolution (`GetScreenSize`, `ScreenSize`).
- Capture screenshots (`ScreenCapture`).
- Record screen with custom resolution and bitrate (`ScreenRecording`).

### ⚙️ System Settings
- `SettingsManager` for system, global, and secure settings:
  - List, read, write, and delete settings.

### ⌨️ Control your device
- `InputManager`:
  - Screen Tap.
  - Send word or whole sentence.
  - Simulate keyboard key stroke. (Enter, Arrow, Back, Home, Numbers, Letters, etc)
  - Simulate touch swipe.
  - Drag and drop.

### 🧠 Activity Management
- `ActivityManager`:
  - List, start, stop, and kill activities.
  - Start/stop services and send broadcasts.

### 🧱 Partition Management
- New `Partition` class for GPT handling.
- `RemoveFRP` function for factory reset protection removal.

### 🧪 Utilities & Debugging
- Custom debugging event system (`UpdateAux`).
- Improved root detection and shell command execution.
- New `ADBException` class for structured error handling.
- IDisposable support for `ADB` object cleanup.
- Organized codebase using C# regions for better readability.

---

## 🚀 Getting Started

### Installation

Install via NuGet:

```bash
dotnet add package ADBueno
```

### Basic Usage

```csharp
using ADBueno;
using System;

internal class Program
{
    static void Main(string[] args)
    {
        var devices = ADB.DetectDevices();
        int count = devices.Count;

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"Current device: {devices[i].SerialNumber}");
        }

        if (count == 0)
            throw new Exception("ADB DEVICES NOT FOUND");

        ADB adb = new ADB(devices[0].SerialNumber);

        PauseMsg("\n\nPress Enter to exit");
    }

    static void PauseMsg(string msg)
    {
        Console.WriteLine(msg);
        Console.ReadKey();
    }
}
```

More examples coming soon in the Wiki and Samples directory!

---

## 🤝 Contributing

We welcome contributions! If you have suggestions, bug reports, or feature ideas, feel free to open an issue or submit a pull request.

---

## 📄 License

This project is licensed under the MIT License – see the LICENSE file for details.

---

## 🙏 Acknowledgments

- Inspired by the need for a robust and user-friendly ADB library in C#.
- Thanks to the open-source community for their invaluable tools and documentation.
```
