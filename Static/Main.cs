using KalevaAalto.Wpf.TxtToEpub.Extensions;
using KalevaAalto.Wpf.TxtToEpub.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalevaAalto.Wpf.TxtToEpub
{
    public static partial class Static
    {
        public static IServiceProvider ServiceProvider = new ServiceCollection()
            .AddTransient<INovelInfo,NovelInfo>()
            .BuildServiceProvider();
        public static Models.IImageInfo GreateImageInfo(string fileName) => new Extensions.ImageInfo(fileName);

        public const string DefaultNovelTitle = @"空白文档";


    }
}
