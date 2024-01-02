using KalevaAalto.Models;
using KalevaAalto.Wpf.TxtToEpub.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
#pragma warning disable 1998

namespace KalevaAalto.Wpf.TxtToEpub.Extensions
{
    public class NovelInfo : Models.INovelInfo
    {
        public override string Content {
            get
            {
                ChapterInfos.Clear();

                if (Length == 0) return string.Empty;

                StringBuilder result = new StringBuilder();
                result.Append(_novel.Prologue);
                result.Append(_novel.LineBreak + _novel.LineBreak);
                _novel.ChapterList.ForEach(novelChapter => {
                    ChapterInfos.Add(new ChapterInfo(novelChapter,result.Length));
                    result.Append(novelChapter.ToString());
                    result.Append(_novel.LineBreak + _novel.LineBreak);
                });
                result.Remove(result.Length - _novel.LineBreak.Length * 2, _novel.LineBreak.Length * 2);



                OnPropertyChanged(nameof(ChapterInfos));
                OnPropertyChanged(nameof(Length));
                OnPropertyChanged(nameof(PrologueLength));
                return result.ToString(); 
            }
            set
            {
                _novel.SetContent(value, TitleRegexString);
                OnPropertyChanged(nameof(Content));
            }
        }

        private Novel _novel = new Novel(DefaultNovelTitle);

        public override string Prologue { get => _novel.Prologue; }
        public override int PrologueLength { get => _novel.PrologueLength ; }

        public override int Length => _novel.Length;


        public override string Title { get => _novel.Title; set { _novel.Title = value;OnPropertyChanged(nameof(this.Title)); } }
        public override void Refresh()
        {
            throw new NotImplementedException();
        }
        public override async Task RefreshAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task DeleteChapterAsync(string chapterTitle, CancellationToken token = default)
        {
            IChapterInfo? chapterInfo = ChapterInfos.FirstOrDefault(it=>it.Title == chapterTitle);
            if(chapterInfo is not null)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception($"小说“{Title}”找不到章节“{chapterTitle}”；");
            }

            throw new NotImplementedException();
        }

        protected override async Task LoadNovelFromTxtAsync(string fileName, CancellationToken token = default) =>Content = await GetStringFromFileAsync(fileName);
        

        protected override async Task LoadNovelFromXmlAsync(string fileName, CancellationToken token = default) =>Content =(await Novel.LoadNovelFromXmlAsync(fileName, token)).Content;

        protected override async Task LoadNovelFromEpubAsync(string fileName, CancellationToken token = default) => Content = (await Novel.LoadNovelFromEpubAsync(fileName, token)).Content;
        

        protected override async Task SaveAsTxtAsync(string fileName, CancellationToken token = default) =>await System.IO.File.WriteAllTextAsync(fileName, Content, token);
        
        protected override async Task SaveAsXmlAsync(string fileName, CancellationToken token = default) =>await _novel.SaveAsXmlAsync(fileName, token);
        protected override async Task SaveAsEpubAsync(string fileName, CancellationToken token = default) =>await _novel.SaveAsEpubAsync(fileName, token);
        
    }
}
