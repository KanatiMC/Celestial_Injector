import os
import requests
import vdf
import pystyle
import psutil
import time
import winreg

from colorama import Fore, Style
from pathlib import Path

title = r"""
   ____              ___                                               ___ 
  6MMMMb/            `MM                               68b             `MM 
 8P    YM             MM                        /      Y89              MM 
6M      Y    ____     MM    ____      ____     /M              ___      MM 
MM          6MMMMb    MM   6MMMMb    6MMMMb\  /MMMMM   `MM    6MMMMb    MM 
MM         6M'  `Mb   MM  6M'  `Mb  MM'    `   MM       MM  8M'  `Mb    MM 
MM         MM    MM   MM  MM    MM  YM.        MM       MM      ,oMM    MM 
MM         MMMMMMMM   MM  MMMMMMMM   YMMMMb    MM       MM  ,6MM9'MM    MM 
YM      6  MM         MM  MM             `Mb   MM       MM  MM'   MM    MM 
 8b    d9  YM    d9   MM  YM    d9  L    ,MM   YM.  ,   MM  MM.  ,MM    MM 
  YMMMM9    YMMMM9   _MM_  YMMMM9   MYMMMM9     YMMM9  _MM_ `YMMM9'Yb. _MM_
---------------------------------------------------------------------------"""
path = ""

def main():
    global path
    print(pystyle.Center.XCenter(f"{Fore.CYAN}{Style.BRIGHT}{title}{Style.NORMAL}{Fore.RESET}"))
    if smi() == False:
        with open(os.path.join(os.getenv('TEMP'), "smi.exe"), 'wb') as file:
            file.write(requests.get("https://github.com/KanatiMC/Celestial/raw/main/smi.exe").content)
    if smidll() == False:
        with open(os.path.join(os.getenv('TEMP'), "SharpMonoInjector.dll"), 'wb') as file:
            file.write(requests.get("https://github.com/KanatiMC/Celestial/raw/main/SharpMonoInjector.dll").content)
    with open(os.path.join(os.getenv('TEMP'), "c.dll"), 'wb') as file:
        file.write(requests.get("https://github.com/TJGTA3/CelstialOptimizer/raw/master/Celstial_Optimizer.dll").content)
    if ovo():
        exec("taskkill /f /im 1v1_LOL.exe")
        time.sleep(2.5)
    else:
        patch()
        with open(fr"{path}\1v1_LOL_Data\Managed\1v1.dll", 'wb') as file:
            file.write(requests.get("https://github.com/TJGTA3/CelstialOptimizer/raw/master/1v1.dll").content)
        exec("start steam://rungameid/2305790")
        time.sleep(7.5)
        exec(r"%temp%\smi.exe inject -p 1v1_LOL -a %temp%\c.dll -n Celstial -c Loader -m Init")

def get_registry_value(path, key):
    try:
        with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, path) as registry_key:
            value, _ = winreg.QueryValueEx(registry_key, key)
            return value
    except FileNotFoundError:
        return None


def patch():
    global path
    if os.environ.get('ProgramFiles(x86)'):
        registry_key = r"SOFTWARE\WOW6432Node\Valve\Steam"
    else:
        registry_key = r"SOFTWARE\Valve\Steam"

    steam_path = get_registry_value(registry_key, "InstallPath")
    if not steam_path:
        return

    library_folders_path = os.path.join(steam_path, "steamapps", "libraryfolders.vdf")
    if not Path(library_folders_path).exists():
        return

    with open(library_folders_path, 'r') as file:
        library_folders = vdf.load(file)

    pathe = None
    for index, library_folder in library_folders.get('libraryfolders', {}).items():
        if 'apps' in library_folder:
            apps = library_folder['apps']
            if "2305790" in apps:
                possible_directory = os.path.join(library_folder.get('path', ''), "steamapps", "common", "1v1.LOL")
                pathe = possible_directory

    if pathe and Path(pathe).exists():
        path = pathe
        return

    celest_path = get_registry_value(r"Software\Celestial", "path")
    if celest_path and Path(celest_path).exists():
        path = celest_path
        return
    else:
        print("Enter In Your 1v1.LOL Path:")
        path = input()
        with winreg.CreateKey(winreg.HKEY_CURRENT_USER, r"Software\Celestial") as key:
            winreg.SetValueEx(key, "path", 0, winreg.REG_SZ, path)

def exec(command):
    os.system(command)

def ovo():
    return any(proc.name() == "1v1_LOL.exe" for proc in psutil.process_iter())

def smi():
    return any(file == "smi.exe" for file in os.listdir(os.getenv('TEMP')))

def smidll():
    return any(file == "SharpMonoInjector.dll" for file in os.listdir(os.getenv('TEMP')))

if __name__ == '__main__':
    main()

