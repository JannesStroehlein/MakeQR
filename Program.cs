using QRCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeQR
{
    class Program
    {
        public static OutputType OutputType;
        public static string OutputPath;
        public static string DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public static bool FileWriteSuccessfull = false;
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    Console.WriteLine("Verwendung: makeqr {link}/help [Argumente]");
                    Console.WriteLine("Mögliche Argumente:");
                    Console.WriteLine("-o {Pfad} - Gibt an, wo das Foto gespeichert werden soll.");
                    Console.WriteLine("help - Zeigt diese Informationen an.");
                    return;
                }
                else if (args.Length == 3)
                {
                    OutputType = OutputType.File;
                    if (args[1].Contains("-o"))
                    {
                        OutputPath = args[2];
                        if (!Path.IsPathRooted(OutputPath))
                            OutputPath = DefaultDirectory + "\\" + OutputPath;
                        if (!Path.IsPathRooted(OutputPath))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Der angegebene Pfad ist ungültig.");
                            Console.ResetColor();
                            if (Debugger.IsAttached)
                                Console.ReadKey();
                        }
                    }
                }
                else
                    OutputType = OutputType.Clipboard;

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(args[0], QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(Assembly.GetEntryAssembly().Location.Replace("MakeQR.exe", "") + @"\centerimage.png"));

                switch (OutputType)
                {
                    case OutputType.Clipboard:
                        try
                        {
                            Clipboard.SetImage(qrCodeImage);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Der QR-Code wurde erfolgreich in die Zwischenablage kopiert.");
                            Console.ResetColor();
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Der QR-Code konnte nicht in die Zwischenablage kopiert werden.");
                            Console.Write("Error: ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(e.Message);
                            Console.ResetColor();
                            if (Debugger.IsAttached)
                                Console.ReadKey();
                        }
                        break;
                    case OutputType.File:
                        try
                        {
                            qrCodeImage.Save(OutputPath);
                            FileWriteSuccessfull = true;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Der QR-Code wurde als ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(OutputPath);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(" erfolgreich gespeichert.");
                            Console.ResetColor();
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Der QR-Code konnte nicht gespeichert werden.");
                            Console.Write("Error: ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(e.Message);
                            Console.ResetColor();
                            if (Debugger.IsAttached)
                                Console.ReadKey();
                        }
                        break;
                }
                //if (!FileWriteSuccessfull)
                //    File.Delete(OutputPath);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Es wurden keine Argumente genannt.");
                Console.ResetColor();
            }
            if (Debugger.IsAttached)
                Console.ReadKey();
        }
    }
    public enum OutputType { Clipboard, File }
}