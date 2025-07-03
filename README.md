ADBueno: Your New Favorite ADB Library for C#

ADBueno is a powerful and comprehensive C# library designed to simplify your interactions with Android Debug Bridge (ADB). Whether you're a developer, tester, or automation engineer, ADBueno provides a robust and easy-to-use set of functionalities to streamline your Android device management and testing workflows.

✨ Features
ADBueno empowers you with a wide array of ADB functionalities, including:

    Device Management:
    
        Detect single or multiple connected ADB devices.
        Connect to devices via specific serials or universally.
        Quickly detect device presence.
        Detect specific device modes.
        Check for boot completion.
        Reboot devices into various modes (e.g., normal, bootloader, recovery).
        Reboot and wait for device to come online.
        Detect and wait for device to be ready.
        Connect to emulators or hidden devices via ADB CONNECT.
    
    Property & Shell Interactions:
    
        Get and set device properties (GETPROP / SETPROP).    
        Run arbitrary shell commands on the device.
        Check root information and run root shell commands.
        Set root mode.
    
    File & Package Operations:
    
        Pull and push files to/from the device.
        List internal files and directories.
        Check if a file or folder exists on the device.
        Remove files.
        Change file permissions.
        Install and uninstall APKs.
        List installed APKs.
        Disable and enable packages.
        
    Logging & Advanced Tools:
    
        Capture device logcat output.
        Filter logcat output for specific tags or keywords.
        Execute DD commands (requires root).
        List GPT partitions (requires root).

🚀 Getting Started

Installation

ADBueno is available as a NuGet package. You can install it via the NuGet Package Manager in Visual Studio or using the .NET CLI:

Bash

    dotnet add package ADBueno
    
Basic Usage

Here's a quick example to get you started:

    C#
    
    using ADBueno;
    using System;
    
    internal class Program
    {
        static void Main(string[] args)
        {
            // Detect devices
            var devices = ADB.DetectDevices();
            int count = devices.Count;
    
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"Current device: {devices[i].SerialNumber}");
            }
    
            if (count == 0)
                throw new Exception("ADB DEVICES NOT FOUND");
    
            // Test serial detection
            ADB adb = new ADB(devices[0].SerialNumber);
    
            // Exit msg
            PauseMsg("\n\nPress Enter to exit");
        }
    
        static void PauseMsg(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadKey();
        }
    }

For more detailed examples and advanced usage, please refer to the Wiki (coming soon!) or the Samples directory in the repository.

🤝 Contributing
We welcome contributions! If you have suggestions for improvements, new features, or find any bugs, please open an issue or submit a pull request.

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

udos Acknowledgments

    Inspired by the need for a robust and user-friendly ADB library in C#.
    Thanks to the open-source community for their invaluable resources and tools.

Enjoy building amazing Android tools with ADBueno!

Remember to replace:

    YourUsername/ADBueno with your actual GitHub repository path for the license badge and potential wiki link.
    Create a CONTRIBUTING.md and a LICENSE file in your repository.
    Consider creating a Samples directory with more elaborate code examples.
    Eventually, create a Wiki for more in-depth documentation.
