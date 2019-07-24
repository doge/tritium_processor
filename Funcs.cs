using ImageProcessor;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace tritium_processor
{
    class Funcs
    {
        public byte[] ImageToByteArray(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
        }

        public void WriteBytesToFile(byte[] bytes, string fileName)
        {
            File.WriteAllBytes(Directory.GetCurrentDirectory() + $"/bin/{ fileName }", bytes);
            Console.WriteLine($"wrote { fileName } to { Directory.GetCurrentDirectory() }\\bin");
        }

        public byte[] ProcessImage(byte[] photoBytes, Color color)
        {
            if (photoBytes != null)
            {
                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            imageFactory.Load(inStream).Hue(System.Convert.ToInt32(color.GetHue())).Save(outStream);
                        }

                        return outStream.ToArray();
                    }
                }
            }

            return null;
        }

        public void DeleteExistingTritiums(string directory)
        {
            var files = Directory.GetFiles(directory);
            string[] tritiums = { "mtl_t6_attach_tritium_red_glo.png", "mtl_t6_attach_tritium_grn_glo.png", "~-gmtl_t6_attach_tritium_grn_col.png", "~-gmtl_t6_attach_tritium_red_col.png",
                  "~-gmtl_t6_attach_tritium_wht_col.png", "~~-gmtl_t6_attach_tritium_red~74df00e5.png", "~~-gmtl_t6_attach_tritium_wht~74df00e5.png", "mtl_t6_attach_tritium_red_text.png" };

            foreach (var file in files)
            {
                foreach(var tritium in tritiums)
                {
                    if (Path.GetFileName(file) == tritium)
                    {
                        File.Delete(file);

                        Console.WriteLine($"deleted { Path.GetFileName(file) } from { directory }");
                    }
                }
            }

            Console.WriteLine("All existing tritiums deleted.");
            Console.ReadKey();
        }

        public void MoveToRedactedFolder(string directory, string binDirectory)
        {
            Console.Write("Would you like the tritiums to be moved to your redacted folder? [y/n]: ");
            var input = Console.ReadLine().ToLower();
            if (input == "y" || input == "yes")
            {
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

                Console.WriteLine("All tritiums moved successfully.");
                Console.ReadKey();
            }
        }

        public string[] CheckAndReturnDirectories()
        {
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

            return new string[] { directory, binDirectory };
        }
    }
}
