using KalevaAalto.Models.FileSystem;
using KalevaAalto.Wpf.TxtToEpub.Models.Enums;
using MimeKit.Cryptography;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.ComponentModel;
using SqlSugar;

namespace KalevaAalto.Wpf.TxtToEpub.Extensions
{
    public class ImageInfo : Models.IImageInfo
    {
        private FileNameInfo beforeFileNameInfo;

        private FileNameInfo afterFileNameInfo => new FileNameInfo(this.Path,this.AfterName,this.AfterFormat.GetSuffix());
        public override string Path => this.beforeFileNameInfo.Path;
        public override string BeforeName => this.beforeFileNameInfo.Name;
        public override ImageFormat BeforeFormat => GetImageFormat(this.beforeFileNameInfo.Suffix);
        public override string BeforeFileName => this.beforeFileNameInfo.FileName;

        private int _size;
        public override int Size { get => this._size; }
        public ImageInfo(string fileName) 
        { 
            
            this.beforeFileNameInfo = new FileNameInfo(fileName);
            this._size = (int)(this.beforeFileNameInfo.Size / 1024);
            if (!ImageFormats.Contains(this.BeforeFormat))
            {
                throw new ArgumentException(@"文件格式应为常见的图片格式",nameof(this.BeforeFormat));
            }
        }



        private readonly static Dictionary<Models.Enums.ImageFormat, SKEncodedImageFormat> sKEncodedImageFormats = new Dictionary<ImageFormat, SKEncodedImageFormat>
        {
            {ImageFormat.Jpg, SKEncodedImageFormat.Jpeg},
            {ImageFormat.Jpeg, SKEncodedImageFormat.Jpeg},
            {ImageFormat.Png, SKEncodedImageFormat.Png},
            {ImageFormat.Bmp, SKEncodedImageFormat.Bmp},
            {ImageFormat.Gif, SKEncodedImageFormat.Gif},
        };
        public override void ChangePicture(int maxWidth, int maxHeight, int accuracy)
        {

            SKBitmap skBitmap = SKBitmap.Decode(this.BeforeFileName);// 读取图片
            if (skBitmap == null) return;
            int originalWidth = skBitmap.Width;//获取宽度
            int originalHeight = skBitmap.Height;//获取高度
            //int colorDepth = skBitmap.Info.BitsPerPixel;// 获取位深


            if (originalWidth > maxWidth || originalHeight > maxHeight) //检查文件是否超出大小并调整尺寸
            {
                float widthScale = (float)maxWidth / originalWidth;
                float heightScale = (float)maxHeight / originalHeight;
                float scale = System.Math.Min(widthScale, heightScale);
                SKBitmap newSkBitmap = skBitmap.Resize(new SKImageInfo((int)(originalWidth * scale), (int)(originalHeight * scale)), SKFilterQuality.High);
                skBitmap.Dispose();
                skBitmap = newSkBitmap;
            }


            using (SKImage skImage = SKImage.FromBitmap(skBitmap))
            {
                using (SKData skData = skImage.Encode(sKEncodedImageFormats[this.AfterFormat], accuracy))
                {
                    System.IO.File.WriteAllBytes(this.afterFileNameInfo.FileName, skData.ToArray());  // 保存图片文件
                }
            }
            skBitmap.Dispose();


            //删除旧文件
            if (this.BeforeFileName != this.afterFileNameInfo.FileName && File.Exists(this.BeforeFileName))
            {
                File.Delete(this.BeforeFileName);
            }


            this.beforeFileNameInfo = this.afterFileNameInfo;
            this._size = (int)(this.beforeFileNameInfo.Size / 1024);



            this.OnPropertyChanged(nameof(this.BeforeName));
            this.OnPropertyChanged(nameof(this.BeforeFormatName));
            this.OnPropertyChanged(nameof(this.Size));


        }

        
    }
}
