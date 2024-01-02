using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KalevaAalto.Wpf.TxtToEpub.Models
{
    public abstract class IChapterInfo
    {
        public abstract int TitlePos { get; }
        public int Ser { get; set; }

        public abstract int Length { get; }
        public abstract string Title { get; set; }

        public abstract string Content { get; set; }

        



    }
}
