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

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Red)  .Name("r", "red")    .Description("Red value.").Default(0);
            parser.Configure(opt => opt.Green).Name("g", "green")  .Description("Green value.").Default(0);
            parser.Configure(opt => opt.Blue) .Name("b", "blue")   .Description("Blue value.").Default(0);
            parser.Configure(opt => opt.Reset).Name("d", "default").Description("Reset tritiums to default.").Default(false);

            var result = parser.Parse(args);

            if (result.HasErrors || args.Length <= 0)
            {
                foreach (var exception in result.Errors)
                    Console.WriteLine(exception.Message);

                Console.ReadKey();

                return;
            }
            else
            {
                Funcs func = new Funcs();

                var directory = "";
                var binDirectory = Directory.GetCurrentDirectory() + "/bin";
                var pathFile = binDirectory + "/path.txt";

                if (!Directory.Exists(binDirectory))
                {
                    Directory.CreateDirectory(binDirectory);
                }

                if (!File.Exists(pathFile))
                {
                    Console.Write("Please input your root redacted directory: ");
                    directory = Console.ReadLine() + @"\data\images";

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

                var programOptions = result.Result;

                if (programOptions.Reset)
                {
                    try
                    {
                        func.DeleteExistingTritiums(directory);
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

                        byte[] mtl_t6_attach_tritium_red_glo = func.ProcessImage(func.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_red_glo), col);
                        func.WriteBytesToFile(mtl_t6_attach_tritium_red_glo, "mtl_t6_attach_tritium_red_glo.png");
                        func.WriteBytesToFile(mtl_t6_attach_tritium_red_glo, "mtl_t6_attach_tritium_grn_glo.png");

                        byte[] gmtl_t6_attach_tritium_grn_col = func.ProcessImage(func.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_grn_col), col);
                        func.WriteBytesToFile(gmtl_t6_attach_tritium_grn_col, "~-gmtl_t6_attach_tritium_grn_col.png");

                        byte[] gmtl_t6_attach_tritium_red_col = func.ProcessImage(func.ImageToByteArray(Properties.Resources.__gmtl_t6_attach_tritium_red_col), col);
                        func.WriteBytesToFile(gmtl_t6_attach_tritium_red_col, "~-gmtl_t6_attach_tritium_red_col.png");
                        func.WriteBytesToFile(gmtl_t6_attach_tritium_red_col, "~-gmtl_t6_attach_tritium_wht_col.png");

                        byte[] gmtl_t6_attach_tritium_red_74df00e5 = func.ProcessImage(func.ImageToByteArray(Properties.Resources.___gmtl_t6_attach_tritium_red_74df00e5), col);
                        func.WriteBytesToFile(gmtl_t6_attach_tritium_red_74df00e5, "~~-gmtl_t6_attach_tritium_red~74df00e5.png");
                        func.WriteBytesToFile(gmtl_t6_attach_tritium_red_74df00e5, "~~-gmtl_t6_attach_tritium_wht~74df00e5.png");

                        byte[] mtl_t6_attach_tritium_red_text = func.ProcessImage(func.ImageToByteArray(Properties.Resources.mtl_t6_attach_tritium_red_text), col);
                        func.WriteBytesToFile(mtl_t6_attach_tritium_red_text, "mtl_t6_attach_tritium_red_text.png");

                        func.MoveToRedactedFolder(directory, binDirectory);

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        Console.ReadKey();
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
