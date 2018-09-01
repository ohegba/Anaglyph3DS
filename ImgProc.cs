using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Anaglyph3DS
{
    class ImgProc
    {

        public static Bitmap MultiplyStereo(Image a, Image b)
        {
            Bitmap ra = new Bitmap(a);
            Bitmap rb = new Bitmap(b);

            Color left = Form1.leftColor;//Color.FromArgb(255, 0, 0);
            Color right = Form1.rightColor;//Color.FromArgb(0, 255, 255);

            Desat(ra);
            Desat(rb);

            Bitmap cyanotype = new Bitmap(ra.Width, ra.Height);
            Bitmap seeingred = new Bitmap(ra.Width, ra.Height);

            using (Graphics gfx = Graphics.FromImage(cyanotype))
            using (SolidBrush brush = new SolidBrush(left))
            {
                gfx.FillRectangle(brush, 0, 0, ra.Width, ra.Height);
            }

            using (Graphics gfx = Graphics.FromImage(seeingred))
            using (SolidBrush brush = new SolidBrush(right))
            {
                gfx.FillRectangle(brush, 0, 0, ra.Width, ra.Height);
            }

            ra = Multiply(ra, cyanotype);
            rb = Multiply(rb, seeingred);

           // MakeMoreChannel(ra,0);
            //MakeMoreChannel(rb,2);

            Console.WriteLine("rseddy to multiply");

            return Screen(ra,rb);



        }

        public static byte[] desaturate(byte r, byte g, byte b) 
        {
        var intensity = 0.3 * r + 0.59 * g + 0.11 * b;
        var k = 1;
        r = (byte)Math.Floor(intensity * k + r * (1 - k));
        g = (byte)Math.Floor(intensity * k + g * (1 - k));
        b = (byte)Math.Floor(intensity * k + b * (1 - k));
        return new byte[]{r,g,b};
        }

        public static void Desat(Bitmap bmp)
        {
            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
            bmp.LockBits(rect, ImageLockMode.ReadWrite,
                         pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int numBytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // blue value for every other pixel in the the bitmap.
            // for (int counter = 0; counter < rgbValues.Length; counter += 3)
            //   rgbValues[counter] = 255;

            for (int counter = 0; counter < rgbValues.Length; counter += 3)
            {
                byte[] rgb = desaturate(rgbValues[counter + 2], rgbValues[counter + 1], rgbValues[counter]);
                rgbValues[counter] = rgb[0];
                rgbValues[counter+1] = rgb[1];
                rgbValues[counter+2] = rgb[2];
            }
                    

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
        }


        public static Bitmap Multiply(Bitmap image, Bitmap mask)
        {

            Bitmap canvas = new Bitmap(image);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, canvas.Width, canvas.Height);
            BitmapData bmpData =
            canvas.LockBits(rect, ImageLockMode.ReadWrite,
                         pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int numBytes = canvas.Width * canvas.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

          
            Console.WriteLine("getten teh beits!");
            byte[] rgbValuesMask = getBitmapData(mask);
            Console.WriteLine("got em!");
            // Manipulate the bitmap, such as changing the
            // blue value for every other pixel in the the bitmap.
           for (int counter = 0; counter < rgbValues.Length; counter += 1)
                rgbValues[counter] = (byte) Math.Min(Math.Max(0,(rgbValuesMask[counter] * rgbValues[counter])/255), 255);

            Console.WriteLine("made manips");

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Unlock the bits.
            canvas.UnlockBits(bmpData);

            return canvas;
        }


        public static Bitmap Screen(Bitmap image, Bitmap mask)
        {

            Bitmap canvas = new Bitmap(image);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, canvas.Width, canvas.Height);
            BitmapData bmpData =
            canvas.LockBits(rect, ImageLockMode.ReadWrite,
                         pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int numBytes = canvas.Width * canvas.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);


            Console.WriteLine("getten teh beits!");
            byte[] rgbValuesMask = getBitmapData(mask);
            Console.WriteLine("got em!");
            // Manipulate the bitmap, such as changing the
            // blue value for every other pixel in the the bitmap.
            for (int counter = 0; counter < rgbValues.Length; counter += 1)
                //rgbValues[counter] = ((byte) Math.Min(Math.Max(0,(255-((255 - rgbValues[counter]) * (255 - rgbValuesMask[counter]))/255)), 255));
                rgbValues[counter] = (byte) Math.Max(0,(255-(((255 - rgbValuesMask[counter]) * (255 - rgbValues[counter]))/255)));
            Console.WriteLine("maide manips");

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Unlock the bits.
            canvas.UnlockBits(bmpData);

            return canvas;
        }


        public static byte[] getBitmapData(Bitmap bmp)
        {
            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
            bmp.LockBits(rect, ImageLockMode.ReadWrite,
                         pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int numBytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return (rgbValues);
        }


        public static void MakeMoreChannel(Bitmap bmp, int mod)
        {
            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
            bmp.LockBits(rect, ImageLockMode.ReadWrite,
                         pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int numBytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // blue value for every other pixel in the the bitmap.
           // for (int counter = 0; counter < rgbValues.Length; counter += 3)
             //   rgbValues[counter] = 255;

            for (int counter = 0; counter < rgbValues.Length; counter += 1)
                if (counter % 3 == mod)
                rgbValues[counter] = 255;

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
        }
    }
}
