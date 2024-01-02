using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Linq;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Data;
using System.Net.Http.Headers;
using KalevaAalto.Wpf.Base;
using System.IO.Compression;
using Google.Protobuf.WellKnownTypes;
using KalevaAalto.Models;
using static KalevaAalto.Main;
using KalevaAalto.Wpf.TxtToEpub.Services;
using KalevaAalto.Wpf.TxtToEpub.Models;
using Microsoft.Extensions.DependencyInjection;
using KalevaAalto.Wpf.TxtToEpub.Models.Enums;
using KalevaAalto.TxtToEpub;

namespace KalevaAalto.Wpf.TxtToEpub.Forms
{
    /// <summary>
    /// novel_form.xaml 的交互逻辑
    /// </summary>
    public partial class Form_Novel : Window
    {
        /// <summary>
        /// 小说文档文件夹路径，默认为程序当下文件夹的路径
        /// </summary>
        private string _path = System.IO.Directory.GetCurrentDirectory();


        private NovelFileFormat _suffix = NovelFileFormat.Txt;
        



        private INovelInfo _novelInfo = Static.ServiceProvider.GetService<INovelInfo>()!;
        private DispatcherTimer _timer = new DispatcherTimer();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        public Form_Novel()
        {
            //初始化窗口
            InitializeComponent();
            DataContext = _novelInfo;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            #region 添加间歇执行的任务
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) => {
                NovelSizeStatusbar.Text = $"字数：{_novelInfo.Length.ToString(@"#,##0")}字；";
                PrologueSizeStatusbar.Text = $"序章：{_novelInfo.PrologueLength.ToString(@"#,##0")}字；";
                ChapterCountStatusbar.Text = $"章节：{_novelInfo.ChapterInfos.Count.ToString(@"#,##0")}章；";
            };
            _timer.Start();
            #endregion

#if DEBUG
            TestButton.Visibility = Visibility.Visible;
#endif


        }


        private void Window_Closed(object sender, EventArgs e)
        {
            _tokenSource.Cancel();
            _timer.Stop();
        }

        private string FindString { get => FindTextbox.Text; set => FindTextbox.Text = value; }
        private string ReplaceString { get => ReplaceTextbox.Text; set => ReplaceTextbox.Text = value; }
        private bool IsUsingRegex => IsUsingRegexCheckbox.IsChecked ?? false;
        private string TitleRegex { get => TitleRegexTextbox.Text; set => TitleRegexTextbox.Text = value; }
        private bool ListviewChaptersIsMulti => Checkbox_ListviewChaptersIsMulti.IsChecked ?? false;





        /// <summary>
        /// 小说章节区域行点击事件
        /// </summary>
        private void ChaptersListview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                IChapterInfo selectedPerson = (((ListViewItem)sender!).DataContext as IChapterInfo)!;
                NovelContentTextbox.Select(selectedPerson.TitlePos, selectedPerson.Title.Length);
                NovelContentTextbox.JumpToPoint(selectedPerson.TitlePos);
            }
            catch
            {
                return;
            }

        }


        /// <summary>
        /// 小说章节列表区域行双击事件
        /// </summary>
        private void ListViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Listview_Chapters.SelectedIndex = -1;
        }

        /// <summary>
        /// 小说章节列表区域是否可多选的单选框的选择事件
        /// </summary>
        private void ListviewChaptersIsMultiCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Listview_Chapters.SelectedIndex = -1;
            Listview_Chapters.SelectionMode = SelectionMode.Multiple;
        }

        /// <summary>
        /// 小说章节列表区域是否可多选的单选框的取消选择事件
        /// </summary>
        private void ListviewChaptersIsMultiCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Listview_Chapters.SelectionMode = SelectionMode.Single;
        }


        /// <summary>
        /// 删除章节
        /// </summary>
        private async void DeleteChapterButton_Click(object sender, RoutedEventArgs e)
        {


            throw new NotImplementedException();


        }






        private string fileFilter
        {
            get
            {
                switch (_suffix)
                {
                    case NovelFileFormat.Epub:
                        return @"Epub Documents(*.epub)|*.epub|Text Documents(*.txt)|*.txt|Xml Documents(*.xml)|*.xml";
                    case NovelFileFormat.Xml:
                        return @"Xml Documents(*.xml)|*.xml|Text Documents(*.txt)|*.txt|Epub Documents(*.epub)|*.epub";
                    case NovelFileFormat.Txt:
                        return @"Text Documents(*.txt)|*.txt|Xml Documents(*.xml)|*.xml|Epub Documents(*.epub)|*.epub";
                    default:throw new NotFiniteNumberException();
                }
            }
        }
        /// <summary>
        /// 打开按钮
        /// </summary>
        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {

            string[]? fileNames = OpenFiles(fileFilter);

            if(fileNames is null || fileNames.Length==0)
            {
                return;
            }

            try
            {
                if (fileNames.Length == 1)
                {
                    string fileName = fileNames[0];
                    _path = System.IO.Path.GetDirectoryName(fileName)!;
                    _suffix = GetNovelFileFormat(System.IO.Path.GetExtension(fileName));
                    await _novelInfo.LoadAsync(fileName,_tokenSource.Token);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            

        }

        /// <summary>
        /// 保存为txt的按钮
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string? fileName = SaveFile(fileFilter, Title);
            if (fileName == null)
            {
                return;
            }

            try
            {
                _novelInfo.Content = NovelContentTextbox.Text;
                _path = System.IO.Path.GetDirectoryName(fileName)!;
                _suffix = GetNovelFileFormat(System.IO.Path.GetExtension(fileName));
                await _novelInfo.SaveAsync(fileName, _tokenSource.Token);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            

            
        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// 字符串查找按钮
        /// </summary>
        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            //如果查找字符串为空，则不执行该函数
            if (this.FindString.Length == 0)
            {
                return;
            }


            throw new NotImplementedException();
        }

        /// <summary>
        /// 字符串替换按钮
        /// </summary>
        private async void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //如果查找字符串为空，则不执行该函数
            if (this.FindString.Length == 0)
            {
                return;
            }


            if (this.IsUsingRegex)
            {
                try
                {
                    Regex regex = new Regex(this.FindString);
                    MatchCollection matchs = regex.Matches(this.NovelContentTextbox.Text);
                    DataTable table = new DataTable();
                    table.Columns.Add(@"替换前", typeof(string));
                    table.Columns.Add(@"替换后", typeof(string));
                    foreach(Match match in matchs)
                    {
                        DataRow row = table.NewRow();
                        row[@"替换前"] = match.Groups[0];
                        row[@"替换后"] = RegexFormat(this.ReplaceString,match.Groups);
                        table.Rows.Add(row);
                    }
                    if(!datatable_messagebox.DataTable_Comfrimd(table,$"替换{matchs.Count.ToString("#,##0")}项！！！"))return;
                    this.NovelContentTextbox.Text = regex.Replace(this.NovelContentTextbox.Text, this.ReplaceString);
                    MessageBox.Show(@"替换成功！！！", @"提示");
                }
                catch
                {
                    MessageBox.Show(@"替换失败......", @"提示");
                }
                    
            }
            else
            {

                //显示提示框
                MessageBoxResult result = MessageBox.Show($"共有{this.NovelContentTextbox.Text.SubStringCount( this.FindString)}可替换！！！" ,
                    @"请确认！！！",  MessageBoxButton.OKCancel);

                //判断点击了哪个按钮
                if (result != MessageBoxResult.OK)
                {
                    return;
                }


                this.NovelContentTextbox.Text = this.NovelContentTextbox.Text.Replace(this.FindString,this.ReplaceString);
                MessageBox.Show(@"替换成功！！！", @"提示");
            }
        
        }


        /// <summary>
        /// 按字数分章节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DivideChaptersByWordCountButton_Click(object sender, RoutedEventArgs e)
        {

            
            MessageBoxResult result = MessageBox.Show(@"是否按字数分章节？",@"请确认！！！", MessageBoxButton.OKCancel);

            
            if (result != MessageBoxResult.OK)
            {
                return;
            }

            await this.Dispatcher.Invoke(async () =>
            {
                throw new NotImplementedException();
            });
        }



        /// <summary>
        /// 更新标题
        /// </summary>
        private void ReplaceTitleButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 窗口的键盘动作函数
        /// </summary>
        private async void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _novelInfo.Content = NovelContentTextbox.Text;
                await _novelInfo.SaveAsync(System.IO.Path.Combine(_path,_novelInfo.Title + _suffix.GetSuffix()), _tokenSource.Token);
            }
        }




        #region 菜单栏

        /// <summary>
        /// 打开图片管理工具
        /// </summary>
        private void PictureCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            Form_PictureCollection pictureCollection = new Form_PictureCollection();
            pictureCollection.Show();
            this.Close();
        }


        #endregion

        /// <summary>
        /// 测试用按钮
        /// </summary>
        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            await TestService.Test();
            
#endif

        }



        
    }
}
