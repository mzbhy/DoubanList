using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace DoubanList
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string ColorSetting = UISettings.MyWindowColor[int.Parse(ConfigurationManager.AppSettings["Background"].ToString())].ColorID;
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorSetting));
            this.Background = brush;
            UISettings.EnableBlur(this);
        }

        /// <summary>
        /// 窗口鼠标左键按下的事件处理函数，用于实现无边框window的拖拽
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void CloseWindow(object sender, RoutedEventArgs args)
        {
            this.Close();
        }



        private int test()
        {
            string url = "http://movie.douban.com/api/v2/subject/1764796";
            string url1 = "http://api.douban.com/v2/movie/subject/1764796";
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(url1);
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = myreader.ReadToEnd();
            myreader.Close();
            if (result == null)
            {
                MessageBox.Show("网络异常", "提示");
                return 0;
            }
            else
            {
                DoubanInfo JsonDouban = JsonConvert.DeserializeObject<DoubanInfo>(result);
                Image MovieCover = new Image();
                MovieCover.Source = new BitmapImage(new Uri(JsonDouban.images.large));
                MovieCover.Margin = new System.Windows.Thickness { Left = 43, Top = 120, Right = 0, Bottom = 0 };
                MovieCover.Width = 300;
                MovieCover.Height = 435;
                MovieCover.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                MovieCover.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                grid1.Children.Add(MovieCover);
            }
            return 1;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            test();            
        }

        private string MovieTitleGet(string FileName)
        {
            string MovieTitle = null;
            MovieTitle = System.IO.Path.GetFileNameWithoutExtension(FileName);
            List<string> sArray = MovieTitle.Split('.').ToList();
            MovieTitle = sArray[0] + ' ';
            sArray.RemoveAt(0);
            if (sArray.Count != 0)
            {
                foreach (string s in sArray)
                {
                    if (!Char.IsNumber(s[0]))
                    {
                        MovieTitle += (s + ' ');
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (MovieTitle.Substring(MovieTitle.Length - 1, 1) == " ")
                MovieTitle = MovieTitle.Substring(0, MovieTitle.Length - 1);
            return MovieTitle;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string MovieFileName;
            string SearchUrl = "https://api.douban.com/v2/movie/search?q={";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "视频文件|*.mkv; *.rmvb; *.rm; *.wmv; *.avi; *.mpg; *.mpeg; *.mp4|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                MovieFileName = MovieTitleGet(openFileDialog.FileName);
                textBox.Text = MovieFileName;
                HttpWebRequest request;
                request = (HttpWebRequest)WebRequest.Create(SearchUrl + MovieFileName + '}');
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string result = myreader.ReadToEnd();
                myreader.Close();
                if (result == null)
                {
                    MessageBox.Show("网络异常", "提示");
                }
                else
                {
                    SearchResult JsonDouban = JsonConvert.DeserializeObject<SearchResult>(result);
                    if(JsonDouban.subjects.Count > 0)
                    {
                        Image MovieCover = new Image();
                        MovieCover.Source = new BitmapImage(new Uri(JsonDouban.subjects[0].images.large));
                        MovieCover.Width = 300;
                        MovieCover.Height = 435;
                        listBox.Items.Add(MovieCover);
                    }
                    else
                    {
                        MessageBox.Show("没有搜索到该电影", "提示");
                    }
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            UISettings myUISettings = new UISettings();
            myUISettings.Show();
        }
    }
}
