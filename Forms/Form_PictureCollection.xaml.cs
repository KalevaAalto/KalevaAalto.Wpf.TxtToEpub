using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Drawing;
using System.Drawing.Imaging;
using SkiaSharp;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using KalevaAalto.Wpf;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using Google.Protobuf.WellKnownTypes;
using MimeKit.Cryptography;
using KalevaAalto.Models;
using SqlSugar;
using KalevaAalto.Wpf.TxtToEpub.Extensions;
using System.Windows.Threading;
#pragma warning disable 1998

namespace KalevaAalto.Wpf.TxtToEpub.Forms
{
    /// <summary>
    /// PictureCollection.xaml 的交互逻辑
    /// </summary>
    public partial class Form_PictureCollection : Window
    {

        /// <summary>
        /// 图像精度
        /// </summary>
        private int Accuracy => 80;
        /// <summary>
        /// 限制图像最大宽度
        /// </summary>
        private new int MaxWidth => 2048;
        /// <summary>
        /// 限制图像最大高度
        /// </summary>
        private new int MaxHeight => 2048;



        /// <summary>
        /// 图片信息列表
        /// </summary>
        private ObservableCollection<Models.IImageInfo> imageInfos = new ObservableCollection<Models.IImageInfo>();
        private DispatcherTimer timer = new DispatcherTimer();

        public Form_PictureCollection()
        {
            this.InitializeComponent();


        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.ItemsSource = this.imageInfos;
            this.ComboBox_Format.ItemsSource = ImageFormatStrings;
            this.ImageFormat = Models.Enums.ImageFormat.Jpg;

            
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += (sender, e) =>
            {
                this.StatusBarItem_First.Text = this.FirstStatus;
            };
            this.timer.Start();

#if DEBUG
            this.TestButton.Visibility = Visibility.Visible;
#endif

            await this.BrushPictureInfos();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            this.timer.Stop();
        }




        private bool IsChangeFileName=> this.CheckBox_IsRename.IsChecked == null ? false : this.CheckBox_IsRename.IsChecked.Value;






        private string FirstStatus { get; set; } = string.Empty;
        

        private int StartNumber { get => System.Math.Max(0, KalevaAalto.Main.GetInt32(this.TextBox_StartNumber.Text)); set => this.TextBox_StartNumber.Text = value.ToString(); }
        
        private Models.Enums.ImageFormat ImageFormat
        {
            get {  return GetImageFormat(this.ComboBox_Format.Text);}
            set {  this.ComboBox_Format.Text = value.GetSuffix(); }
        }

        /// <summary>
        /// 刷新图像信息列表
        /// </summary>
        private async Task BrushPictureInfos()
        {
            //检查是否为空
            this.FirstStatus = $"当前有{this.imageInfos.Count}张图片！";
            if (this.imageInfos.Count <= 0)
            {
                return;
            }

            Models.Enums.ImageFormat imageFormat = this.ImageFormat;

            if (this.IsChangeFileName)
            {
                int number = this.StartNumber;
                string name = this.TextBox_Name.Text;
                //获取要描述的数据
                foreach (Models.IImageInfo imageInfo in this.imageInfos)
                {
                    imageInfo.Status = string.Empty;
                    imageInfo.AfterName = $"{name}_{number.ToString(@"0000")}";
                    imageInfo.AfterFormat = imageFormat;
                    number++;
                }
            }
            else
            {
                await Parallel.ForEachAsync(this.imageInfos, (imageInfo, _) =>{
                    imageInfo.Status = string.Empty;
                    imageInfo.AfterFormat = imageFormat;
                    imageInfo.AfterName = imageInfo.BeforeName;
                    return ValueTask.CompletedTask;
                });
            }


            

        }


        /// <summary>
        /// 批量修改图片文件名
        /// </summary>
        /// <param name="root">文件夹路径</param>
        /// <param name="fileNames">文件名数组</param>
        private void ChangeGreatPictureName(string root,string[] fileNames)
        {
            //生成一个长度为16的随机字符串
            string randomString = KalevaAalto.Main.GetRandomString(@"abcdefghijklmnopqrstuvwxyz0123456789",16);


            //转换成文件信息
            KalevaAalto.Models.FileSystem.FileNameInfo[] fileInfos = fileNames.GetFileNameInfos();

            //排序
            fileInfos=fileInfos.Where(it=>!it.Path.Contains(@"pixiv_未分类")).OrderBy(it=>it.FileName).ToArray();

            //添加计数器
            int number = 0;
            int sum = fileInfos.Length;



            //将文件名改为随机字符
            number = 0;
            foreach (KalevaAalto.Models.FileSystem.FileNameInfo fileNameInfo in fileInfos)
            {
                this.FirstStatus = $"将文件名改为随机字符运行中：{number}/{sum}；";
                string beforeFileName = fileNameInfo.FileName ?? string.Empty;
                fileNameInfo.Name = randomString + '_' + number.ToString(@"00000000");
                string afterFileName = fileNameInfo.FileName ?? string.Empty;
                File.Move(beforeFileName, afterFileName);
                number++;
            }

            //记录文件数量
            Dictionary<string, int> dic = new Dictionary<string, int>();

            //批量重命名
            number = 0;
            foreach(KalevaAalto.Models.FileSystem.FileNameInfo fileNameInfo in fileInfos)
            {
                this.FirstStatus = $"批量重命名运行中：{number}/{sum}；";
                string beforeFileName = fileNameInfo.FileName ?? string.Empty;
                string shortPath = @"二次元美少女";
                if (root != fileNameInfo.Path) {
                    shortPath = fileNameInfo.Path.Substring(root.Length + 1, fileNameInfo.Path.Length - root.Length - 1);
                }
                if (!dic.ContainsKey(shortPath)) dic[shortPath] = 0;
                fileNameInfo.Name = shortPath.Replace('\\', '_') + '_' + dic[shortPath].ToString(@"000000");
                string afterFileName = fileNameInfo.FileName ?? string.Empty;
                File.Move(beforeFileName, afterFileName);
                dic[shortPath]++;
                number++;
            }


        }


        #region 操作栏
        /// <summary>
        /// 添加图片信息列表
        /// </summary>
        private async void Add_Click(object sender, RoutedEventArgs e)
        {

            #region 获取文件列表
            StringBuilder filterString = new StringBuilder();
            foreach(string format in ImageFormatStrings)
            {
                filterString.Append(@"*.");
                filterString.Append(format);
                filterString.Append(';');
            }
            string[]? fileNames = OpenFiles($"图片({filterString})|{filterString}");
            if(fileNames is null)
            {
                return;
            }
            #endregion




            
            

            foreach (string fileName in fileNames)
            {
                this.imageInfos.Add(GreateImageInfo(fileName));
            }
            


            await this.BrushPictureInfos();
        }


        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            string? selectedFolderPath = OpenFileFolder();
            if (selectedFolderPath == null) return;

            //转换成文件信息
            KalevaAalto.Models.FileSystem.FileNameInfo[] fileInfos = KalevaAalto.Main.GetFilesRecursively(selectedFolderPath).GetFileNameInfos();
            foreach(KalevaAalto.Models.FileSystem.FileNameInfo fileNameInfo in fileInfos)
            {
                if (!ImageFormatStrings.Contains(fileNameInfo.Suffix.ToLower())) continue;
                this.imageInfos.Add(GreateImageInfo(fileNameInfo.FileName));
            }


            await this.BrushPictureInfos();

        }


        /// <summary>
        /// 清空图片信息列表
        /// </summary>
        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.imageInfos.Clear();
            await this.BrushPictureInfos();
        }


        /// <summary>
        /// 刷新图片信息列表
        /// </summary>
        private async void Brush_Click(object sender, RoutedEventArgs e)
        {
            await this.BrushPictureInfos();
        }

        /// <summary>
        /// 根据图片信息列表来修改图片
        /// </summary>
        private async void Change_Click(object sender, RoutedEventArgs e)
        {
            #region 检查是否有错误
            bool isError = false;
            foreach(Models.IImageInfo imageInfo in this.imageInfos)
            {
                if (!string.IsNullOrEmpty(imageInfo.Error))
                {
                    isError = true;
                    break;
                }
            }
            if (isError)
            {
                MessageBoxResult result = MessageBox.Show("有错误提示是否继续操作？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            #endregion



            // 创建计时器对象
            Stopwatch stopwatch = Stopwatch.StartNew();


            this.Start(true);

            int number = 0; //进行计数
            int count = this.imageInfos.Count;

            this.FirstStatus = $"初始化中；";
            foreach (Models.IImageInfo imageInfo in this.imageInfos)
            {
                imageInfo.Status = @"F";
            }
            await Task.Run(() =>
            {
                this.imageInfos.AsParallel().ForAll(imageInfo =>{
                    this.FirstStatus = $"运行中：{number}/{count}；";
                    imageInfo.ChangePicture(this.MaxWidth, this.MaxHeight, this.Accuracy);
                    Interlocked.Increment(ref number);
                    imageInfo.Status = @"T";
                });
            });


            this.FirstStatus = @"已停止！！！";

            this.Start(false);
            await this.BrushPictureInfos();

            // 获取经过的时间
            MessageBox.Show(@"运行已完成：" + stopwatch.ClockString());
        }

        #endregion




        #region 菜单栏
        /// <summary>
        /// 返回
        /// </summary>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Form_Novel novel_Form = new Form_Novel();
            novel_Form.Show();
            this.Close();
            
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void WindowClosing_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 批量修改二次元美少女图片文件名
        /// </summary>
        private async void ChangeGreatPictureName_Click(object sender, RoutedEventArgs e)
        {
            //获取文件夹路径
            string? selectedFolderPath = OpenFileFolder();
            if(selectedFolderPath is null)
            {
                return;
            }

            //获取文件夹列表
            string[] subfolders = Directory.GetDirectories(selectedFolderPath);

            //检查
            if ((!subfolders.Contains($"{selectedFolderPath}\\pixiv_其它"))|| (!subfolders.Contains($"{selectedFolderPath}\\pixiv_R18G")))
            {
                MessageBox.Show(@"不是合法的二次元美少女图片文件夹！！！");
                return;
            }

            this.FirstStatus = @"运行中！！！";
            //获取文件列表
            string[] subfiles = KalevaAalto.Main.GetFilesRecursively(selectedFolderPath);
            this.FirstStatus = @"已获取文件列表！！！";

            await Task.Run(() => { ChangeGreatPictureName(selectedFolderPath, subfiles); });


            this.FirstStatus = @"已停止！！！";
        }


        #endregion


        #region 全局控制


        /// <summary>
        /// 文件名文本框被修改后的动作
        /// </summary>
        private async void TextBox_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            await this.BrushPictureInfos();
        }

        /// <summary>
        /// 图片格式选项框被修改后的动作
        /// </summary>
        private async void ComboBox_Format_DropDownClosed(object sender, EventArgs e)
        {
            await this.BrushPictureInfos();
        }

        /// <summary>
        /// 限制序号输入框内只能输入数字
        /// </summary>
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 使用正则表达式来验证输入是否为数字
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^[0-9]+$"))
            {
                e.Handled = true; // 阻止非数字字符输入
            }
        }

        private async void TextBox_StartNumber_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox numericTextBox = this.TextBox_StartNumber;


            while (numericTextBox.Text.Length>1 && numericTextBox.Text[0] == '0')
            {
                numericTextBox.Text = numericTextBox.Text.Remove(0,1);
                numericTextBox.CaretIndex += 1;
            }
            

            // 限制文本框的最大长度为6
            if (numericTextBox.Text.Length > 6)
            {
                numericTextBox.Text = numericTextBox.Text.Substring(0, 6);
                numericTextBox.CaretIndex = numericTextBox.Text.Length;
            }

            // 确保文本框中至少保留一个数字
            if (string.IsNullOrEmpty(numericTextBox.Text))
            {
                numericTextBox.Text = @"0";
                numericTextBox.CaretIndex = 1;
            }
            await this.BrushPictureInfos();
        }

        /// <summary>
        /// 是否修改文件名单选框被修改后的动作
        /// </summary>
        private async void CheckBox_IsRename_Checked(object sender, RoutedEventArgs e)
        {
            await this.BrushPictureInfos();
        }

        /// <summary>
        /// 进程开始或结束
        /// </summary>
        /// <param name="status">进程是开始还是结束</param>
        private void Start(bool status)
        {
            if (status)
            {
                this.TextBox_Name.IsEnabled = false;
            }
            else
            {
                this.TextBox_Name.IsEnabled = true;
            }
        }


        #endregion



        

        /// <summary>
        /// 测试专用函数
        /// </summary>
        private async void Test_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show(@"测试成功！！！");
#if DEBUG

            await this.BrushPictureInfos();
#endif
        }

        
    }
}
