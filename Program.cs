using CommandLine;
using CommandLine.Text;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MakeQR
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                try
                {
                    if (o.CenterImage != null & !File.Exists(o.CenterImage))
                        throw new FileNotFoundException("The center image could not be found!");
                    if (o.OutputFileName != null & Path.IsPathRooted(o.OutputFileName))
                        throw new DirectoryNotFoundException("The output file path is not routed!");
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(o.InputURL, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage;
                    if (o.CenterImage != null)
                        qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(o.CenterImage));
                    else
                        qrCodeImage = qrCode.GetGraphic(20);
                    if (o.OutputFileName != null)
                    {
                        qrCodeImage.Save(o.OutputFileName);
                        Console.WriteLine("The QR code was successfully saved as {0}", o.OutputFileName);
                    }
                    else
                    {
                        Clipboard.SetImage(qrCodeImage);
                        Console.WriteLine("The QR code was successfully copied to clipboard");
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An unexpected exception occured!");
                    Console.WriteLine(e);
                }
            });
            if (Debugger.IsAttached)
                Console.ReadKey();
        }
    }
    class Options
    {
        [Option('o', "output", Required = false, HelpText = "Output filename")]
        public string OutputFileName { get; set; }
        [Option('c', "centerimage", Required = false, HelpText = "The image displayed in the center")]
        public string CenterImage { get; set; }
        [Option('r', "resolution", Required = false, HelpText = "Sets height and width per block in the QR code", Default = 20)]
        public int Resolution { get; set; }
        
        [Value(0, MetaName = "URL", HelpText = "The URL that should be encoded in the QR code", Required = true)]
        public string InputURL { get; set; }

        [Usage(ApplicationAlias = "MakeQR.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("Creates and copies a rickroll QR code", new Options { InputURL = "https://youtu.be/dQw4w9WgXcQ" }),
                    new Example("Creates and copies a rickroll QR code with a resolution of 30px per block, a center image and saves it to disk", new Options { InputURL = "https://youtu.be/dQw4w9WgXcQ", Resolution = 30, CenterImage = "image.png", OutputFileName = "output.png" })
                };
            }
        }
    }
}