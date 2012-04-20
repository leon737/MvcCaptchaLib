/*
ASP.NET MVC Web Application Captcha Library Copyright (C) 2009-2012 Leonid Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Web;
using System.Web.Mvc;
using System.IO;

namespace CaptchaLib
{
    public class CaptchaResult : ActionResult
    {

        // default buffer size as defined in BufferedStream type 
        private const int BufferSize = 0x1000;
 
        private readonly ICaptchaImage image;
        private readonly int quality;
        private readonly int width;
        private readonly int height;

        internal CaptchaResult(ICaptchaImage image, int quality, int width, int height)
        {
            this.image = image;
            this.quality = quality;
            this.width = width;
            this.height = height;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            var ms = new MemoryStream();
            image.SaveImageToStream(ms, quality, width, height);
            ms.Position = 0;
            WriteStream(response, ms);
        }

        protected void WriteStream(HttpResponseBase response, Stream stream)
        {
            // grab chunks of data and write to the output stream 
            var outputStream = response.OutputStream;
            using (stream)
            {
                var buffer = new byte[BufferSize];
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, BufferSize);
                    if (bytesRead == 0)
                    {
                        // no more data 
                        break;
                    }
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }
        } 
    }
}
