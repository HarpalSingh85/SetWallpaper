using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SetWallpaper
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = null;
            string id = null;

            if (args.Length == 0)
            {
                Console.WriteLine("Provide path of image, user id");
                Console.WriteLine("Usuage is SetWallpaper.exe {filename} {id}");
                Console.WriteLine("\t\t filename : Specify filename to be set as wallpaper");
                Console.WriteLine("\t\t id [Optional] : Specify user ID to which wallpaper will be added");
            }
            else
            {

                if (args.Length > 0)
                {
                    uri = args[0];
                    SetCurrentWallpaper(uri);
                }

                else
                {
                    uri = args[0];
                    id = args[1];
                    SetCurrentWallpaper(uri, id);
                }

            }

        }



        private static void SetCurrentWallpaper(string uri, string id = null)
        {
            string sidString = null;
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    sidString = "default";
                }
                else
                {
                    NTAccount account = new NTAccount(id);
                    SecurityIdentifier s = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                    sidString = s.ToString();
                    Console.WriteLine(sidString);
                }

                Wallpaper.Set(uri, sidString, Wallpaper.Style.Stretched);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }




    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static void Set(string uri, string SID, Style style)
        {
            FileStream s = new FileStream(uri.ToString(), FileMode.Open);

            System.Drawing.Image Img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            Img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            string Regpath = $"{SID}\\Control Panel\\Desktop";
            if (SID.Equals("default"))
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (style == Style.Stretched)
                {
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                if (style == Style.Centered)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                if (style == Style.Tiled)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                }
            }
            else
            {
                RegistryKey key = Registry.Users.OpenSubKey(Regpath, true);
                if (style == Style.Stretched)
                {
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                if (style == Style.Centered)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                if (style == Style.Tiled)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                }
            }



            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }


    }
}
 

