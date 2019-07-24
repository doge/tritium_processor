using MatthiWare.CommandLine;
using System;
using System.IO;
using System.Drawing;

namespace tritium_processor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "tritium_processor";

            var binDirectory = Directory.GetCurrentDirectory() + "/bin";

            if (!Directory.Exists(binDirectory))
            {
                Directory.CreateDirectory(binDirectory);
            }

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Red)  .Name("r", "red")    .Description("Red value.")  .Required();
            parser.Configure(opt => opt.Green).Name("g", "green")  .Description("Green value.").Required();
            parser.Configure(opt => opt.Blue) .Name("b", "blue")   .Description("Blue value.") .Required();
            parser.Configure(opt => opt.Reset).Name("d", "default").Description("Reset tritiums to default.").Default(false);

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                foreach (var exception in result.Errors)
                    Console.WriteLine(exception.Message);

                Console.ReadKey();

                return;
            }
            else
            {
                Convert conv = new Convert();
                var programOptions = result.Result;

                if (programOptions.Reset)
                {
                    try
                    {
                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_red_glo), "mtl_t6_attach_tritium_red_glo");
                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_grn_glo), "mtl_t6_attach_tritium_grn_glo");

                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_grn_col), "~-gmtl_t6_attach_tritium_grn_col");
                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_red_col), "~-gmtl_t6_attach_tritium_red_col");
                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_wht_col), "~-gmtl_t6_attach_tritium_wht_col");

                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.___gmtl_t6_attach_tritium_red_74df00e5), "~-gmtl_t6_attach_tritium_red~74df00e5");
                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.___gmtl_t6_attach_tritium_wht_74df00e5), "~-gmtl_t6_attach_tritium_wht~74df00e5");
                        conv.WriteBytesToFile(conv.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_red_text), "mtl_t6_attach_tritium_red_text");

                        MoveToRedactedFolder();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        Console.ReadKey();
                    }
                }
                else
                {
                    try
                    {
                        var col = Color.FromArgb(programOptions.Red, programOptions.Green, programOptions.Blue);

                        byte[] mtl_t6_attach_tritium_red_glo = conv.ProcessImage(conv.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_red_glo), col);
                        conv.WriteBytesToFile(mtl_t6_attach_tritium_red_glo, "mtl_t6_attach_tritium_red_glo");
                        conv.WriteBytesToFile(mtl_t6_attach_tritium_red_glo, "mtl_t6_attach_tritium_grn_glo");

                        byte[] gmtl_t6_attach_tritium_grn_col = conv.ProcessImage(conv.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_grn_col), col);
                        conv.WriteBytesToFile(gmtl_t6_attach_tritium_grn_col, "~-gmtl_t6_attach_tritium_grn_col");

                        byte[] gmtl_t6_attach_tritium_red_col = conv.ProcessImage(conv.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_red_col), col);
                        conv.WriteBytesToFile(gmtl_t6_attach_tritium_red_col, "~-gmtl_t6_attach_tritium_red_col");
                        conv.WriteBytesToFile(gmtl_t6_attach_tritium_red_col, "~-gmtl_t6_attach_tritium_wht_col");

                        byte[] gmtl_t6_attach_tritium_red_74df00e5 = conv.ProcessImage(conv.ImageToByteArray(Properties.Resources.___gmtl_t6_attach_tritium_red_74df00e5), col);
                        conv.WriteBytesToFile(gmtl_t6_attach_tritium_red_74df00e5, "~~-gmtl_t6_attach_tritium_red~74df00e5");
                        conv.WriteBytesToFile(gmtl_t6_attach_tritium_red_74df00e5, "~~-gmtl_t6_attach_tritium_wht~74df00e5");

                        byte[] mtl_t6_attach_tritium_red_text = conv.ProcessImage(conv.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_red_text), col);
                        conv.WriteBytesToFile(mtl_t6_attach_tritium_red_text, "mtl_t6_attach_tritium_red_text");

                        MoveToRedactedFolder();

                        Console.ReadKey();

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        Console.ReadKey();
                    }
                }
            }
        }


        private static void MoveToRedactedFolder()
        {
            var binDirectory = Directory.GetCurrentDirectory() + "/bin";

            Console.Write("Would you like the tritiums to be moved to your redacted folder? [y/n]: ");
            var input = Console.ReadLine().ToLower();
            if (input == "y" || input == "yes")
            {
                var directory = "";
                var pathFile = binDirectory + "/path.txt";

                if (!File.Exists(pathFile))
                {
                    Console.Write("Please input your root redacted directory: ");
                    directory = Console.ReadLine() + "/data/images";

                    File.WriteAllText(pathFile, directory);
                }
                else
                {
                    directory = File.ReadAllText(pathFile);
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var files = Directory.GetFiles(binDirectory);
                foreach (var file in files)
                {
                    if (Path.GetFileName(file) != "path.txt")
                    {
                        if (File.Exists(directory + "/" + Path.GetFileName(file)))
                        {
                            File.Delete(directory + "/" + Path.GetFileName(file));
                        }

                        File.Copy(file, directory + "/" + Path.GetFileName(file));

                        Console.WriteLine($"moved {Path.GetFileName(file)} to {directory}");
                    }
                }
            }
        }

        private class Options
        {
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }
            public bool Reset { get; set; }
        }

    }
}
