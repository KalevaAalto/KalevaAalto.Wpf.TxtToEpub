using KalevaAalto.Models;
using KalevaAalto.Wpf.TxtToEpub.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                    ChapterInfos.Add(new ChapterInfo(novelChapter, result.Length) { Ser = ChapterInfos.Count});
                    result.Append(novelChapter.ToString());
                    result.Append(_novel.LineBreak + _novel.LineBreak);
                });
                result.Remove(result.Length - _novel.LineBreak.Length * 2, _novel.LineBreak.Length * 2);

                string rs = result.ToString();


                OnPropertyChanged(nameof(ChapterInfos));
                OnPropertyChanged(nameof(Length));
                OnPropertyChanged(nameof(PrologueLength));


                return rs; 
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


        public override async Task DividChapterByWordCount(string content)
        {
            _novel.Clear();

            int wordCount = 5000;

            string[] lines = NovelChapter.Split(content);

            int length = 0;
            int chapterCount = 0;
            ChapterInfo chapterInfo = new ChapterInfo(new NovelChapter($"第{(chapterCount +1).ToString(@"0000")}章-"),chapterCount);
            lines.ToList().ForEach(line =>
            {
                chapterInfo.NovelChapter.Append(line);
                length += line.Length;
                if(length >= wordCount)
                {
                    _novel.ChapterList.Add(chapterInfo.NovelChapter);
                    ChapterInfos.Add(chapterInfo);
                    length = 0;
                    chapterCount++;
                    chapterInfo = new ChapterInfo(new NovelChapter($"第{(chapterCount + 1).ToString(@"0000")}章-"), chapterCount);
                }
            });
            _novel.ChapterList.Add(chapterInfo.NovelChapter);
            ChapterInfos.Add(chapterInfo);
            OnPropertyChanged(nameof(Content));
        }


        public override string Title { get => _novel.Title; set { _novel.Title = value;OnPropertyChanged(nameof(this.Title)); } }
        public override void Refresh()
        {
            throw new NotImplementedException();
        }
        public override async Task RefreshAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task DeleteChapterAsync(int[] sers, CancellationToken token = default)
        {
            sers.ToList().ForEach(ser =>
            {
                ChapterInfo? chapterInfo = ChapterInfos.FirstOrDefault(it => it.Ser == ser) as ChapterInfo;
                if(chapterInfo is not null)
                {
                    _novel.ChapterList.Remove(chapterInfo.NovelChapter);
                }
            });


            OnPropertyChanged(nameof(Content));
        }

        protected override async Task LoadNovelFromTxtAsync(string fileName, CancellationToken token = default) => Content = await GetStringFromFileAsync(fileName, token);



        protected override async Task LoadNovelFromXmlAsync(string fileName, CancellationToken token = default) =>Content =(await Novel.LoadNovelFromXmlAsync(fileName, token)).Content;

        protected override async Task LoadNovelFromEpubAsync(string fileName, CancellationToken token = default) => Content = (await Novel.LoadNovelFromEpubAsync(fileName, token)).Content;
        

        protected override async Task SaveAsTxtAsync(string fileName, CancellationToken token = default) =>await System.IO.File.WriteAllTextAsync(fileName, Content, token);
        
        protected override async Task SaveAsXmlAsync(string fileName, CancellationToken token = default) =>await _novel.SaveAsXmlAsync(fileName, token);
        protected override async Task SaveAsEpubAsync(string fileName, CancellationToken token = default) =>await _novel.SaveAsEpubAsync(fileName, token);
        
    }
}
