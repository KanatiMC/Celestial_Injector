using System.Diagnostics;
using System.Net;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SharpMonoInjector;


namespace Celestial;

class Program
{
    private static string path;
    public static void ExecuteCommand(string command)
    {
        Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c " + command)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = false,
            RedirectStandardOutput = false
        });

        process.WaitForExit();
        process.Close();
    }
    public static void Main(string[] args)
    {
        Console.Title = "Celsital Injector by Kanti <3";
        string title =
        "   ____              ___                                               ___ \n" +
            "  6MMMMb/            `MM                               68b             `MM \n" +
            " 8P    YM             MM                        /      Y89              MM \n" +
            "6M      Y    ____     MM    ____      ____     /M              ___      MM \n" +
            "MM          6MMMMb    MM   6MMMMb    6MMMMb\\  /MMMMM   `MM    6MMMMb    MM \n" +
            "MM         6M'  `Mb   MM  6M'  `Mb  MM'    `   MM       MM  8M'  `Mb    MM \n" +
            "MM         MM    MM   MM  MM    MM  YM.        MM       MM      ,oMM    MM \n" +
            "MM         MMMMMMMM   MM  MMMMMMMM   YMMMMb    MM       MM  ,6MM9'MM    MM \n" +
            "YM      6  MM         MM  MM             `Mb   MM       MM  MM'   MM    MM \n" +
            " 8b    d9  YM    d9   MM  YM    d9  L    ,MM   YM.  ,   MM  MM.  ,MM    MM \n" +
            "  YMMMM9    YMMMM9   _MM_  YMMMM9   MYMMMM9     YMMM9  _MM_ `YMMM9'Yb. _MM_\n"+
            "---------------------------------------------------------------------------";

        
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(title);
        Console.ForegroundColor = ConsoleColor.White;
        if (Process.GetProcessesByName("1v1_LOL").Length != 0)
        {
            Console.WriteLine("1v1.LOL Found, Closing Game.");
            ExecuteCommand("taskkill /f /im 1v1_LOL.exe");
            Thread.Sleep(2500);
        }
        Console.WriteLine("Patching 1v1.dll");
        Patch();
        Console.WriteLine("Patched, Starting 1v1.LOL");
        ExecuteCommand("start steam://rungameid/2305790");
        Thread.Sleep(7500);
        new Injector("1v1_LOL").Inject(new WebClient().DownloadData("https://github.com/TJGTA3/CelstialOptimizer/raw/master/Celstial_Optimizer.dll"), 
            "Celstial", "Loader", "Init");
        Console.WriteLine("Injected Sucessfully Into 1v1.LOL");
    }

    static void Patch()
    {
        string registryKey = Environment.Is64BitOperatingSystem
            ? "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam"
            : "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam";

        string steamPath = Registry.GetValue(registryKey, "InstallPath", null) as string;
        if (string.IsNullOrEmpty(steamPath))
            return;

        VProperty libraryFolders =
            VdfConvert.Deserialize(File.ReadAllText(Path.Combine(steamPath, "steamapps\\libraryfolders.vdf")));
        for (int index = 0; index < libraryFolders.Value.Count(); index++)
        {
            VToken? libraryFolder = libraryFolders.Value[index.ToString()];
            if (libraryFolder == null)
                continue;
            JObject steamApps = JObject.Parse(libraryFolder["apps"].ToJson().ToString());
            if (!steamApps.ContainsKey("2305790"))
                continue;
            string possibleDirectory = Path.Combine(libraryFolder["path"].ToString(), "steamapps\\common\\1v1.LOL");
            
            path = possibleDirectory;
        }

        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
        {
            File.WriteAllBytes(Path.Combine(path, "1v1_LOL_Data\\Managed\\1v1.dll"),
                new WebClient().DownloadData("https://github.com/TJGTA3/CelstialOptimizer/raw/master/1v1.dll"));
            return;
        }

        if (!string.IsNullOrEmpty(new Regedit("Software\\Celestial").Read("path")) &&
            Directory.Exists(new Regedit("Software\\Celestial").Read("path")))
        {
            File.WriteAllBytes(Path.Combine(path, "1v1_LOL_Data\\Managed\\1v1.dll"),
                new WebClient().DownloadData("https://github.com/TJGTA3/CelstialOptimizer/raw/master/1v1.dll"));
        }
        else
        {
            Console.WriteLine("Enter In Your 1v1.LOL Path:");
            path = Console.ReadLine();
            new Regedit("Software\\Celestial").SetValue("path", path);
            File.WriteAllBytes(Path.Combine(path, "1v1_LOL_Data\\Managed\\1v1.dll"),
                new WebClient().DownloadData("https://github.com/TJGTA3/CelstialOptimizer/raw/master/1v1.dll"));
        }
    }



}