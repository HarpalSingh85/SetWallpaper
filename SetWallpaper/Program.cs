using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static SetWallpaper.Wallpaper;

namespace SetWallpaper
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = null;
            string Wallpaperstyle = null;
            string id = null;

            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("Provide path of image file and style");
                    Console.WriteLine("\r\n");
                    Console.WriteLine("Usuage is SETWALLPAPER {FILENAME} {STYLE : Fill | Tiled | Centered | Stretched}");
                    Console.WriteLine("\r\n");
                    Console.WriteLine("\t\tFILENAME : Specify filename to be set as wallpaper.");
                    Console.WriteLine("\t\tID [Optional] : Specify user ID to which wallpaper will be set.");
                    break;

                case 1:
                    uri = args[0];
                    SetCurrentWallpaper(uri);
                    break;

                case 2:
                    uri = args[0];
                    Wallpaperstyle = args[1];
                    switch (Wallpaperstyle)
                    {
                        case "Fill":
                            SetCurrentWallpaper(uri, Style.Fill);
                            break;

                        case "Tiled":
                            SetCurrentWallpaper(uri, Style.Tiled);
                            break;

                        case "Centered":
                            SetCurrentWallpaper(uri, Style.Centered);
                            break;

                        case "Stretched":
                            SetCurrentWallpaper(uri, Style.Stretched);
                            break;

                        default:
                            SetCurrentWallpaper(uri, Style.Fill);
                            break;

                    }
                    break;

                case 3:
                    uri = args[0];
                    Wallpaperstyle = args[1];
                    id = args[2];
                    switch (Wallpaperstyle)
                    {
                        case "Fill":
                            SetCurrentWallpaper(uri, Style.Fill, id);
                            break;

                        case "Tiled":
                            SetCurrentWallpaper(uri, Style.Tiled, id);
                            break;

                        case "Centered":
                            SetCurrentWallpaper(uri, Style.Centered, id);
                            break;

                        case "Stretched":
                            SetCurrentWallpaper(uri, Style.Stretched, id);
                            break;

                        default:
                            SetCurrentWallpaper(uri, Style.Fill, id);
                            break;

                    }
                    break;

                default:
                    Console.WriteLine("Provide path of image file, style and/or user id");
                    Console.WriteLine("\r\n");
                    Console.WriteLine("Usuage is SETWALLPAPER {FILENAME} {STYLE : Fill | Tiled | Centered | Stretched} {UserID}");
                    Console.WriteLine("\r\n");
                    Console.WriteLine("\t\tFILENAME : Specify filename to be set as wallpaper.");
                    Console.WriteLine("\t\tID [Optional] : Specify user ID to which wallpaper will be set.");
                    break;

            }

        }



        private static void SetCurrentWallpaper(string uri, Style style = Style.Fill, string id = null)
        {
            string sidString = null;
            try
            {
                if (String.IsNullOrEmpty(id))
                {   
                    if(File.Exists(@"C:\Help&Support\user.txt"))
                    {
                        id = File.ReadAllText(@"C:\Help&Support\user.txt");
                        id = id.Replace("\r\n", string.Empty);
                        NTAccount account = new NTAccount(id);
                        SecurityIdentifier s = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                        sidString = s.ToString();
                       // Console.WriteLine(sidString);
                    }
                    else
                    {
                        sidString = "default";
                    }
                    
                }
                else
                {
                    NTAccount account = new NTAccount(id);
                    SecurityIdentifier s = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                    sidString = s.ToString();
                   // Console.WriteLine(sidString);
                }

                Wallpaper.Set(uri, sidString, style);
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
            Stretched,
            Fill
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
                if (style == Style.Fill)
                {
                    key.SetValue(@"WallpaperStyle", 10.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
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
                if (style == Style.Fill)
                {
                    key.SetValue(@"WallpaperStyle", 10.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
            }



            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }


    }
}
 

