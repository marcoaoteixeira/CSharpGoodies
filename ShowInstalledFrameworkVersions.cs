using System;
using Microsoft.Win32;

public static void Main(params string[] args) {
	// Opens the registry key for the .NET Framework entry.
    using (var ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, string.Empty).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\")) {
        // As an alternative, if you know the computers you will query are running .NET Framework 4.5 
        // or later, you can use:
        // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
        // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
        foreach (var versionKeyName in ndpKey.GetSubKeyNames()) {
            if (versionKeyName.StartsWith("v")) {

                var versionKey = ndpKey.OpenSubKey(versionKeyName);
                var version = (string)versionKey.GetValue("Version", string.Empty);
                var servicePack = versionKey.GetValue("SP", string.Empty).ToString();
                var install = versionKey.GetValue("Install", string.Empty).ToString();
                
				if (install == string.Empty) //no install info, must be later.
                    Console.WriteLine($"{versionKeyName}  {version}");
                else {
                    if (servicePack != string.Empty && install == "1") {
                        Console.WriteLine($"{versionKeyName}  {version} SP{servicePack}");
                    }
                }
                
				if (version != string.Empty) { continue; }
				
                foreach (string subKeyName in versionKey.GetSubKeyNames()) {
                    var subKey = versionKey.OpenSubKey(subKeyName);
                    version = (string)subKey.GetValue("Version", string.Empty);
                    if (version != string.Empty) {
						servicePack = subKey.GetValue("SP", string.Empty).ToString();
					}
                    install = subKey.GetValue("Install", string.Empty).ToString();
                    if (install == string.Empty) { //no install info, must be later.
						Console.WriteLine($"{versionKeyName} {version}");
					}
                    else {
                        if (servicePack != string.Empty && install == "1") {
                            Console.WriteLine($"  {subKeyName} {version} SP {servicePack}");
                        } else if (install == "1") {
                            Console.WriteLine($"  {subKeyName} {version}");
                        }
                    }
                }
            }
        }
    }
}