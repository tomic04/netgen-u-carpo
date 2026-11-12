using System;
using System.IO;

namespace Carpo.Core
{
    public static class SpecialFolders
    {
        private static string userApplicationDataFolder = "";
        private static string spinAppDataFolder = "";
        private static string jupiterAppDataFolder = "";
        private static string jupiterGridSettingsFolder = "";
        private static string jupiterRSSFolder = "";
        private static string jupiterPatchesLayoutFolder = "";
        private static string jupiterFiltersFolder = "";
        private static string allUsersApplicationDataFolder = "";
        private static string allUsersSpinAppDataFolder = "";
        private static string allUsersJupiterAppDataFolder = "";
        private static string allUsersJupiterGridSettingsFolder = "";
        private static string allUsersJupiterFiltersFolder = "";

        static public string UserApplicationDataFolder
        {
            get
            {
                if (userApplicationDataFolder == "")
                    userApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return userApplicationDataFolder;
            }
        }

        static public string SpinAppDataFolder
        {
            get
            {
                if (spinAppDataFolder == "")
                    spinAppDataFolder = UserApplicationDataFolder + "\\SPIN Informatica\\";
                if (!Directory.Exists(spinAppDataFolder))
                    Directory.CreateDirectory(spinAppDataFolder);
                return spinAppDataFolder;
            }
        }

        static public string JupiterAppDataFolder
        {
            get
            {
                if (jupiterAppDataFolder == "")
                    jupiterAppDataFolder = SpinAppDataFolder + "Jupiter Software\\";
                if (!Directory.Exists(jupiterAppDataFolder))
                    Directory.CreateDirectory(jupiterAppDataFolder);
                return jupiterAppDataFolder;
            }
        }

        static public string JupiterGridSettingsFolder
        {
            get
            {
                if (jupiterGridSettingsFolder == "")
                    jupiterGridSettingsFolder = JupiterAppDataFolder + "Grid Settings\\";
                if (!Directory.Exists(jupiterGridSettingsFolder))
                    Directory.CreateDirectory(jupiterGridSettingsFolder);
                return jupiterGridSettingsFolder;
            }
        }

        static public string RSS
        {
            get
            {
                if (jupiterRSSFolder == "")
                    jupiterRSSFolder = JupiterAppDataFolder + "RSS\\";
                if (!Directory.Exists(jupiterRSSFolder))
                    Directory.CreateDirectory(jupiterRSSFolder);
                return jupiterRSSFolder;
            }
        }

        static public string PatchesLayout
        {
            get
            {
                if (jupiterPatchesLayoutFolder == "")
                    jupiterPatchesLayoutFolder = JupiterAppDataFolder + "PatchesLayout\\";
                if (!Directory.Exists(jupiterPatchesLayoutFolder))
                    Directory.CreateDirectory(jupiterPatchesLayoutFolder);
                return jupiterPatchesLayoutFolder;
            }
        }

        static public string JupiterFiltersFolder
        {
            get
            {
                if (jupiterFiltersFolder == "")
                    jupiterFiltersFolder = JupiterAppDataFolder + "Filters\\";
                if (!Directory.Exists(jupiterFiltersFolder))
                    Directory.CreateDirectory(jupiterFiltersFolder);
                return jupiterFiltersFolder;
            }
        }

        static public string AllUsersApplicationDataFolder
        {
            get
            {
                if (allUsersApplicationDataFolder == "")
                    allUsersApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return allUsersApplicationDataFolder;
            }
        }

        static public string AllUsersSpinAppDataFolder
        {
            get
            {
                if (allUsersSpinAppDataFolder == "")
                    allUsersSpinAppDataFolder = AllUsersApplicationDataFolder + "\\SPIN Informatica\\";
                if (!Directory.Exists(allUsersSpinAppDataFolder))
                    Directory.CreateDirectory(allUsersSpinAppDataFolder);
                return allUsersSpinAppDataFolder;
            }
        }

        static public string AllUsersJupiterAppDataFolder
        {
            get
            {
                if (allUsersJupiterAppDataFolder == "")
                    allUsersJupiterAppDataFolder = AllUsersSpinAppDataFolder + "Jupiter Software\\";
                if (!Directory.Exists(allUsersJupiterAppDataFolder))
                    Directory.CreateDirectory(allUsersJupiterAppDataFolder);
                return allUsersJupiterAppDataFolder;
            }
        }

        static public string AllUsersJupiterGridSettingsFolder
        {
            get
            {
                if (allUsersJupiterGridSettingsFolder == "")
                    allUsersJupiterGridSettingsFolder = AllUsersJupiterAppDataFolder + "Grid Settings\\";
                if (!Directory.Exists(allUsersJupiterGridSettingsFolder))
                    Directory.CreateDirectory(allUsersJupiterGridSettingsFolder);
                return allUsersJupiterGridSettingsFolder;
            }
        }

        static public string AllUsersJupiterFiltersFolder
        {
            get
            {
                if (allUsersJupiterFiltersFolder == "")
                    allUsersJupiterFiltersFolder = AllUsersJupiterAppDataFolder + "Filters\\";
                if (!Directory.Exists(allUsersJupiterFiltersFolder))
                    Directory.CreateDirectory(allUsersJupiterFiltersFolder);
                return allUsersJupiterFiltersFolder;
            }
        }
    }
}