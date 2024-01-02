using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KalevaAalto.Wpf.TxtToEpub.Models.Enums
{
    public enum ImageFormat
    {
        Jpg,Jpeg, Png,Bmp,Gif
    }
}


namespace KalevaAalto.Wpf.TxtToEpub
{
    public static partial class Static
    {

        public static Models.Enums.ImageFormat[] ImageFormats = (System.Enum.GetValues(typeof(Models.Enums.ImageFormat)) as Models.Enums.ImageFormat[])!;
        public static string[] ImageFormatStrings = (System.Enum.GetValues(typeof(Models.Enums.ImageFormat)) as Models.Enums.ImageFormat[])!.Select(it => it.GetSuffix()).ToArray();
        public static string GetSuffix(this Models.Enums.ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case Models.Enums.ImageFormat.Jpg: return @"jpg";
                case Models.Enums.ImageFormat.Jpeg:return @"jpeg";
                case Models.Enums.ImageFormat.Png:return @"png";
                case Models.Enums.ImageFormat.Bmp:return @"bmp";
                case Models.Enums.ImageFormat.Gif:return @"gif";
            }
            throw new NotImplementedException();
        }


        public static Models.Enums.ImageFormat GetImageFormat(string suffix)
        {
            switch (suffix)
            {
                case @"jpg":return Models.Enums.ImageFormat.Jpg;
                case @"jpeg":return Models.Enums.ImageFormat.Jpeg;
                case @"png":return Models.Enums.ImageFormat.Png;
                case @"bmp":return Models.Enums.ImageFormat.Bmp;
                case @"gif":return Models.Enums.ImageFormat.Gif;
            }
            throw new ArgumentException(@"参数值不合法",nameof(suffix));
        }

    }
}
