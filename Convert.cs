using ImageProcessor;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace tritium_processor
{
    class Convert
    {
        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
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

        public void WriteBytesToFile(byte[] bytes, string fileName)
        {
            File.WriteAllBytes(Directory.GetCurrentDirectory() + $"/bin/{ fileName }.png", bytes);
            Console.WriteLine($"wrote { fileName }.png to { Directory.GetCurrentDirectory() }\\bin");

            Thread.Sleep(100);
        }
    }
}
