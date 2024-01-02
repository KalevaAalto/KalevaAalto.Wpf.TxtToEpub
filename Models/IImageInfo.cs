using Org.BouncyCastle.Asn1.Tsp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalevaAalto.Wpf.TxtToEpub.Models
{
    /// <summary>
    /// 定义图像信息类
    /// </summary>
    public abstract class IImageInfo : INotifyPropertyChanged
    {


        /// <summary>
        /// 文件所在文件夹的路径
        /// </summary>
        public abstract string Path { get; }


        /// <summary>
        /// 原文件格式
        /// </summary>
        public abstract Enums.ImageFormat BeforeFormat { get; }

        /// <summary>
        /// 原文件名
        /// </summary>
        public abstract string BeforeName { get; }

        private string _aftername = string.Empty;
        /// <summary>
        /// 修改后的文件路径
        /// </summary>
        public string AfterName { get => this._aftername; set { this._aftername = value; this.OnPropertyChanged(nameof(this.AfterName)); } }


        private Enums.ImageFormat _afterFormat = Enums.ImageFormat.Jpg;
        /// <summary>
        /// 修改后的文件格式
        /// </summary>
        public Enums.ImageFormat AfterFormat { get => this._afterFormat; set { this._afterFormat = value; this.OnPropertyChanged(nameof(this.AfterFormatName)); } }

        public string BeforeFormatName => this.BeforeFormat.GetSuffix();
        public string AfterFormatName => this.AfterFormat.GetSuffix();

        /// <summary>
        /// 文件名
        /// </summary>
        public abstract string BeforeFileName { get; }



        
        /// <summary>
        /// 文件大小
        /// </summary>
        public abstract int Size { get; }


        private string _error = string.Empty;
        public string Error { get => this._error; set { this._error = value;this.OnPropertyChanged(nameof(this.Error)); } }

        private string _status = string.Empty;
        public string Status { get => this._status; set { this._status = value; this.OnPropertyChanged(nameof(this.Status)); } }


        /// <summary>
        /// 通过图片信息来修改图片信息
        /// </summary>
        public abstract void ChangePicture(int maxWidth, int maxHeight,int accuracy);


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
