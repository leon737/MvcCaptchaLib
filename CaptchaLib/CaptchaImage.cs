/*
ASP.NET MVC Web Application Captcha Library Copyright (C) 2009-2012 Leonid Medyantsev, Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CaptchaLib
{
    public class CaptchaImage : ICaptchaImage
    {
        readonly Random r;

        public int EllipseCount { get; set; }
        public bool IsColoured { get; set; }
        public Point BgColorRange { get; set; }
        public Point FgColorRange { get; set; }
        public Point NoiseColorRange { get; set; }
        public Color BgColor { get; set; }
        public Point XShift { get; set; }
        public Point YShift { get; set; }
        public Point XDistortion { get; set; }
        public Point YDistortion { get; set; }
        public Point Angle { get; set; }
        public int NoiseCount { get; set; }

        Point fgAlpha;
        public Point FgAlpha
        {
            get
            {
                return fgAlpha;
            }
            set
            {
                if (value.X < 0 || value.X > 100 || value.Y < 0 || value.Y > 100)
                    throw new ArgumentException("FgAlpha between 0 and 100");
                fgAlpha = new Point(value.X, value.Y);
            }
        }

        public CaptchaImage()
        {
            r = GenerateRandomInit();
            EllipseCount = 50;
            IsColoured = true;
            BgColorRange = new Point(180, 256);
            FgColorRange = new Point(0, 150);
            NoiseColorRange = new Point(100, 256);
            BgColor = Color.White;
            XShift = new Point(-10, 10);
            YShift = new Point(-8, 8);
            XDistortion = new Point(5, 10);
            YDistortion = new Point(0, 7);
            FgAlpha = new Point(50, 70);
            Angle = new Point(-45, 45);
            NoiseCount = 1000;
        }


        private string captchaValue;
        public string CaptchaValue
        {
            get
            {
                captchaValue = GenerateNewCaptchaValue();
                return captchaValue;
            }
        }

        private string GenerateNewCaptchaValue()
        {
            var digits = new string(Enumerable.Repeat(Enumerable.Range(0, 10), 4).SelectMany(v => v)
                 .OrderBy(v => Guid.NewGuid()).Take(4).Select(v => v.ToString(CultureInfo.InvariantCulture)[0]).ToArray());
            return digits;
        }


        public void SaveImageToStream(Stream outputStream, int quality, int width, int height)
        {
            SaveImageToStream(outputStream, quality, width, height, captchaValue);
        }


        public void SaveImageToStream(Stream outputStream, int quality, int width, int height, string s)
        {
            if (quality < 0 || quality > 100) throw new ArgumentException("quality must be between 0 and 100");
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            Encoder myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var b = CreateCaptchaImage(width, height, s);
            b.Save(outputStream, myImageCodecInfo, myEncoderParameters);
            b.Dispose();
        }

        Bitmap CreateCaptchaImage(int width, int height, string s)
        {
            var b = new Bitmap(width, height);
            var g = Graphics.FromImage(b);

            CreateBackground(g);
            PrintChars(g, s);
            CreateNoise(g);

            g.Dispose();
            return b;
        }

        void CreateBackground(Graphics g)
        {
            Size imgSize = g.VisibleClipBounds.Size.ToSize();
            g.FillRectangle(new SolidBrush(BgColor), g.VisibleClipBounds);
            for (int i = 0; i < EllipseCount; i++)
                g.FillEllipse(
                    new SolidBrush(GetRandomColor(BgColorRange)),
                    r.Next(-imgSize.Width / 4, imgSize.Width), r.Next(-imgSize.Height / 4, imgSize.Height), r.Next(imgSize.Width / 4, imgSize.Width / 2), r.Next(imgSize.Height / 4, imgSize.Height / 2)
                    );
        }

        void PrintChars(Graphics g, string s)
        {
            if (string.IsNullOrEmpty(s)) return;

            Size imgSize = g.VisibleClipBounds.Size.ToSize();
            int lW = imgSize.Width / s.Length;
            int lH = imgSize.Height;
            for (int i = 0; i < s.Length; i++)
            {
                float[][] colorMatrixElements = { 
                new float[] {1,  0,  0,  0, 0},
                new float[] {0,  1,  0,  0, 0},
                new float[] {0,  0,  1,  0, 0},
                new float[] {0,  0,  0, GetRandom(FgAlpha)/100f, 0},
                new float[] {0,  0,  0,  0, 1}};
                var colorMatrix = new ColorMatrix(colorMatrixElements);
                var imageAttr = new ImageAttributes();
                imageAttr.SetColorMatrix(colorMatrix);

                Bitmap b = PrintChar(s.Substring(i, 1));
                int x = GetRandom(XShift, i == 0);
                int y = GetRandom(YShift, i == 0);

                var p = new[] 
                {
                    new Point(lW * i + x + GetRandom(XDistortion, i==0),  y + GetRandom(YDistortion, i==0)),
                    new Point(lW * (i+1) + x + GetRandom(XDistortion, i==0), y + GetRandom(YDistortion, i==0)),
                    new Point(lW * i + x + GetRandom(XDistortion, i==0), b.Height + y + GetRandom(YDistortion, i==0))
                };
                g.DrawImage(b, p, new Rectangle(0, 0, b.Width, b.Height), GraphicsUnit.Pixel, imageAttr);
                b.Dispose();
            }
        }

        Bitmap PrintChar(string s)
        {
            var b = new Bitmap(56, 56);
            var g = Graphics.FromImage(b);
            var f = new Font(new Font("Times", 30), FontStyle.Bold);
            Brush br = new SolidBrush(GetRandomColor(FgColorRange));
            g.TranslateTransform(-20, -20, MatrixOrder.Append);
            g.RotateTransform(GetRandom(Angle), MatrixOrder.Append);
            g.TranslateTransform(20, 20, MatrixOrder.Append);
            g.DrawString(s, f, br, new PointF(0, 0));
            return b;
        }

        void CreateNoise(Graphics g)
        {
            Size imgSize = g.VisibleClipBounds.Size.ToSize();
            for (int i = 0; i < NoiseCount; i++)
            {
                var p = new Pen(GetRandomColor(NoiseColorRange));
                g.DrawEllipse(p, new Rectangle(r.Next(imgSize.Width), r.Next(imgSize.Height), 1, 1));
            }
        }

        int GetRandom(Point range)
        {
            return r.Next(range.X, range.Y);
        }

        int GetRandom(Point range, bool fromZero)
        {
            return r.Next(fromZero?0:range.X, range.Y);
        }
        
        Color GetRandomColor(Point range)
        {
            if (IsColoured)
                return Color.FromArgb(GetRandom(range), GetRandom(range), GetRandom(range));
            int c = GetRandom(range);
            return Color.FromArgb(c, c, c);
        }

        static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        static Random GenerateRandomInit()
        {
            var b = new byte[4];
            RandomNumberGenerator.Create().GetBytes(b);
            return new Random(b[0] + (b[1] << 8) + (b[2] << 16) + ((b[3] << 24)));
        }
    }
}
