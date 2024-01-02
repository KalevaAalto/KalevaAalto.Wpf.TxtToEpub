using KalevaAalto.Wpf.TxtToEpub.Models.Enums;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KalevaAalto.Wpf.TxtToEpub.Models
{
    public abstract class INovelInfo : INotifyPropertyChanged
    {
        private ObservableCollection<IChapterInfo> _chapterInfos = new ObservableCollection<IChapterInfo>();


        public abstract string Title { get; set; }
        public abstract string Prologue { get; }
        public abstract int PrologueLength { get; }
        public abstract string Content { get; set; }
        public ObservableCollection<IChapterInfo> ChapterInfos => _chapterInfos;
        public abstract void Refresh();
        public abstract Task RefreshAsync();
        public abstract int Length { get; }

        public const string DefaultTitleRegexString = @"第[〇零一两二三四五六七八九十百千万亿\d]+[卷章回节集][\-\:\s]*(?<name>.*)";
        public string _titleRegexString = DefaultTitleRegexString;
        public string TitleRegexString { get => _titleRegexString; set  { _titleRegexString = value;OnPropertyChanged(nameof(TitleRegexString)); } }
        protected Regex TitleRegex => new Regex($"^{_titleRegexString}$");
        public async Task LoadAsync(string fileName, CancellationToken token)
        {
            Title = System.IO.Path.GetFileNameWithoutExtension(fileName);
            NovelFileFormat novelFileFormat = GetNovelFileFormat(Path.GetExtension(fileName));
            switch (novelFileFormat)
            {
                case NovelFileFormat.Epub:await LoadNovelFromEpubAsync(fileName, token);return;
                case NovelFileFormat.Xml:await LoadNovelFromXmlAsync(fileName, token);return;
                default:await LoadNovelFromTxtAsync(fileName, token);return;
            }
        }

        public abstract Task DeleteChapterAsync(string chapterTitle, CancellationToken token = default);


        protected abstract Task LoadNovelFromTxtAsync(string fileName, CancellationToken token = default);
        protected abstract Task LoadNovelFromXmlAsync(string fileName, CancellationToken token = default);
        protected abstract Task LoadNovelFromEpubAsync(string fileName, CancellationToken token = default);

        public async Task SaveAsync(string fileName, CancellationToken token)
        {
            NovelFileFormat novelFileFormat = GetNovelFileFormat(Path.GetExtension(fileName));
            switch (novelFileFormat)
            {
                case NovelFileFormat.Epub: await this.SaveAsEpubAsync(fileName, token); return;
                case NovelFileFormat.Xml: await this.SaveAsXmlAsync(fileName, token); return;
                case NovelFileFormat.Txt: await this.SaveAsTxtAsync(fileName, token); return;
                default: throw new NotFiniteNumberException();
            }
        }

        protected abstract Task SaveAsTxtAsync(string fileName, CancellationToken token = default);
        protected abstract Task SaveAsXmlAsync(string fileName, CancellationToken token = default);
        protected abstract Task SaveAsEpubAsync(string fileName, CancellationToken token = default);









        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
