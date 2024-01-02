using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KalevaAalto.Wpf.TxtToEpub.Models.Enums
{
    public enum NovelFileFormat : byte
    {
        Txt,Xml,Epub
    }
}


namespace KalevaAalto.Wpf.TxtToEpub
{
    public static partial class Static
    {
        public static Models.Enums.NovelFileFormat GetNovelFileFormat(string suffix)
        {
            switch (suffix.ToLower())
            {
                case @".epub":return Models.Enums.NovelFileFormat.Epub;
                case @".xml":return Models.Enums.NovelFileFormat.Xml;
                case @".txt":return Models.Enums.NovelFileFormat.Txt;
                default:throw new NotFiniteNumberException();
            }
        }

        public static string GetSuffix(this Models.Enums.NovelFileFormat novelFileFormat)
        {
            switch (novelFileFormat)
            {
                case Models.Enums.NovelFileFormat.Txt:return @".txt";
                case Models.Enums.NovelFileFormat.Xml:return @".xml";
                case Models.Enums.NovelFileFormat.Epub:return @".epub";
            }
            throw new NotImplementedException();
        }
        



    }
}