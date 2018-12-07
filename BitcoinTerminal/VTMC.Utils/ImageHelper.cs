/*************************************************
*Author:zhangdanhong
*Date:2017/04/27 18:29:02
*Des:图片工具类
************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace VTMC.Utils
{
    /// <summary>
    /// 图片处理工具类
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// 图片添加水印 
        /// 
        /// </summary>
        /// <param name="sourcePath">图片路径</param>
        /// <param name="waterMarkPath">水印图片路径</param>
        public static bool AddImageWaterMark(string sourcePath, string waterMarkPath, int axisx, int axisy)
        {
            LogHelper.WriteMethodLog(true);
            Image image = null;
            Image copyImage = null;
            Graphics g = null;
            Bitmap bmp = null;
            try
            {
                //加图片水印 
                image = Image.FromFile(sourcePath);
                copyImage = Image.FromFile(waterMarkPath);
                g = Graphics.FromImage(image);
                g.DrawImage(copyImage, new Rectangle(axisx, axisy, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                g.Dispose();
                //保存加水印过后的图片 
                bmp = new Bitmap(image);
                image.Dispose();
                bmp.Save(sourcePath, ImageFormat.Jpeg);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
                bmp?.Dispose();
                copyImage?.Dispose();
            }
        }

        /// <summary>
        /// 图片添加文字
        /// </summary>
        /// <param name="sourcePath">图片地址</param>
        /// <param name="waterText">添加文字</param>
        /// <param name="axisx">X坐标</param>
        /// <param name="axisy">Y坐标</param>
        /// <param name="font">添加文字字体</param>
        /// <param name="whiteBrush">添加文字画笔</param>
        /// <param name="p">添加文字画框笔（为null时不画框）</param>
        /// <returns></returns>
        public static bool AddImageWaterMarkText(string sourcePath, string waterText, int axisx, int axisy, Font font = null, SolidBrush whiteBrush = null, Pen p = null)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                if (!File.Exists(sourcePath))
                {
                    throw new FileNotFoundException(string.Format("The file:{0} don't exist!:", sourcePath));
                }

                if (string.IsNullOrWhiteSpace(waterText))
                {
                    return true;
                }

                Image image = Image.FromFile(sourcePath);
                Bitmap bitmap = new Bitmap(image, image.Width, image.Height);
                image.Dispose();
                Graphics g = Graphics.FromImage(bitmap);
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (font == null)
                {
                    float fontSize = 30.0f; //字体大小 
                    font = new Font("微软雅黑", fontSize, FontStyle.Bold);//定义字体 
                }

                SizeF size = g.MeasureString(waterText, font);  //文本的size

                float rectWidth = size.Width + 5;
                float rectHeight = size.Height;
                //声明矩形域  
                RectangleF textArea = new RectangleF(axisx, axisy, rectWidth, rectHeight);

                if (whiteBrush == null)
                {
                    whiteBrush = new SolidBrush(Color.Gray);   //画文字用 
                }

                if (p != null)
                {
                    g.DrawRectangle(p, textArea.X, textArea.Y, textArea.Width, textArea.Height);
                }

                g.DrawString(waterText, font, whiteBrush, textArea);

                bitmap.Save(sourcePath, ImageFormat.Jpeg);
                bitmap.Dispose();
                g.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }

        }

        /// 无损压缩图片    
        /// <param name="sFile">原图片</param>    
        /// <param name="dFile">压缩后保存位置</param>    
        /// <param name="dHeight">高度</param>    
        /// <param name="dWidth"></param>    
        /// <param name="flag">压缩质量(数字越小压缩率越高) 1-100</param>    
        /// <returns></returns>    

        public static Image GetPicThumbnail(Image iSource, int dHeight, int dWidth, int flag)
        {
            LogHelper.WriteMethodLog(true);

            ImageFormat tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;

            try
            {
                //按比例缩放  
                Size tem_size = new Size(iSource.Width, iSource.Height);

                if (tem_size.Width > dHeight || tem_size.Width > dWidth)
                {
                    if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                    {
                        sW = dWidth;
                        sH = (dWidth * tem_size.Height) / tem_size.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = (tem_size.Width * dHeight) / tem_size.Height;
                    }
                }
                else
                {
                    sW = tem_size.Width;
                    sH = tem_size.Height;
                }

                Bitmap ob = new Bitmap(dWidth, dHeight);
                Graphics g = Graphics.FromImage(ob);

                g.Clear(Color.WhiteSmoke);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

                g.Dispose();
                //以下代码为保存图片时，设置压缩质量    
                EncoderParameters ep = new EncoderParameters();
                long[] qy = new long[1];
                qy[0] = flag;//设置压缩的比例1-100    
                EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
                ep.Param[0] = eParam;

                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                return ob;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
                iSource.Dispose();
            }
        }

        /// <summary>
        /// 图片转Base64字符
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string ImgToBase64String(Bitmap bitmap)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    return Convert.ToBase64String(arr);
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        /// <summary>
        /// 图片转Base64字符
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ImgToBase64String(string path)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Image img = Image.FromFile(path))
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] arr = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(arr, 0, (int)ms.Length);
                        return Convert.ToBase64String(arr);
                    }
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 图片转Base64字符
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ImgToBase64String(Image img)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    return Convert.ToBase64String(arr);
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 图片转Base64字符
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ImgToBase64String(Image img, ImageFormat imageFormat)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, imageFormat);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    return Convert.ToBase64String(arr);
                }
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// Base64字符转图片
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public static Bitmap Base64ToImg(string base64Str)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                Bitmap bitmap;
                byte[] b = Convert.FromBase64String(base64Str);
                using (MemoryStream ms = new MemoryStream(b))
                {
                    bitmap = new Bitmap(ms);
                }

                Bitmap refbmp = new Bitmap(bitmap);
                bitmap.Dispose();

                return refbmp;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="img"></param>
        /// <param name="path"></param>
        public static void SaveImage(Image img, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(path)) Directory.CreateDirectory(dir);
            img.Save(path, ImageFormat.Jpeg);
        }

        /// <summary>
        /// 读取本地图片到BitmapImage
        /// </summary>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapImage ReadImagePathToBitmap(string imgPath)
        {
            using (BinaryReader binReader = new BinaryReader(File.Open(imgPath, FileMode.Open)))
            {
                FileInfo fileInfo = new FileInfo(imgPath);
                byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);

                // 将图片字节赋值给BitmapImage 
                System.Windows.Media.Imaging.BitmapImage bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(bytes);
                bitmap.EndInit();

                // 最后给Image控件赋值
                return bitmap;
            }
        }


        /// <summary>
        /// 转换为指定DIP的Bitmap
        /// </summary>
        /// <param name="soure"></param>
        /// <param name="dpi"></param>
        /// <returns></returns>
        public static Bitmap ConvertImageDPI(Bitmap soure, int dpi)
        {
            Bitmap bmpTarget = new Bitmap(soure.Width, soure.Height);
            Graphics g = Graphics.FromImage(bmpTarget);
            g.DrawImage(soure, 0, 0, soure.Width, soure.Height);
            bmpTarget.SetResolution(dpi, dpi);

            return bmpTarget;
        }

        public static BitmapSource BitmapToBitmapSource(Bitmap bitmapIcon)
        {
            IntPtr ipIcon = bitmapIcon.GetHbitmap();//从GDI+ Bitmap创建GDI位图对象

            BitmapSource bitmapSourceIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ipIcon, IntPtr.Zero, System.Windows.Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            return bitmapSourceIcon;
        }

        /// <summary>
        /// Bitmap 转 BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        /// <summary>
        /// BitmapImage 转 Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

    }
}