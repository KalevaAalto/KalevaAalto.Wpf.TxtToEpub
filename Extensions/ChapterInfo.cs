using KalevaAalto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalevaAalto.Wpf.TxtToEpub.Extensions
{
    internal class ChapterInfo : Models.IChapterInfo
    {
        private NovelChapter _novelChapter;
        private int _titlePos;
        public ChapterInfo(NovelChapter novelChapter,int pos)
        {
            _novelChapter = novelChapter;
            _titlePos = pos;
        }

        public override int TitlePos => _titlePos;
        public override int Length => _novelChapter.Length;
        public override string Content { get => _novelChapter.Content; set => _novelChapter.Content= value; }
        public override string Title { get => _novelChapter.Title; set => _novelChapter.Title=value; }
    }
}
