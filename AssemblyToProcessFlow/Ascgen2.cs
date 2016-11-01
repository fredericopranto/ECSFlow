using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ECSFlow
{
    class Ascgen2
    {
        public string Filename { get; private set; }
        public Color BackgroundColor { get; private set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Init Main");
            LoadImage("C://");
            Console.WriteLine("End Main");
        }


        /// <summary>
        /// Load the specified image into the picturebox, and setup the form etc.
        /// </summary>
        /// <param name="imagePath">Path to the image</param>
        /// <returns>Did the image load correctly?</returns>
        public static bool LoadImage(string imagePath)
        {
            //ECSFlow
            //try
            //{
            Image image;

            using (Image loadedImage = Image.FromFile(imagePath))
            {
                Size size;

                if (loadedImage is Metafile)
                {
                    size = new Size(1000, (int)((1000f * ((float)loadedImage.Height / (float)loadedImage.Width)) + 0.5f));
                }
                else
                {
                    size = new Size(loadedImage.Width, loadedImage.Height);
                }

                image = new Bitmap(size.Width, size.Height);

                using (Graphics g = Graphics.FromImage(image))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(loadedImage, 0, 0, size.Width, size.Height);
                }
            }


            return LoadImage(image);
            //}
            //catch (OutOfMemoryException)
            //{ // Catch any bad image files
            //    MessageBox.Show(
            //                Resource.GetString("Unknown or Unsupported File"),
            //                Resource.GetString("Error"),
            //                MessageBoxButtons.OK,
            //                MessageBoxIcon.Error);

            //    return false;
            //}
            //catch (FileNotFoundException)
            //{
            //    MessageBox.Show(
            //                Resource.GetString("File Not Found"),
            //                Resource.GetString("Error"),
            //                MessageBoxButtons.OK,
            //                MessageBoxIcon.Error);

            //    return false;
            //}
        }

        private static bool LoadImage(Image image)
        {
            throw new OutOfMemoryException();
        }
    }
}
