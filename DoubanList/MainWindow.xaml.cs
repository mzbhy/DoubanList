using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using MahApps.Metro;
using System.Windows.Threading;

namespace DoubanList
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            btnSkin.Click += (s, e) => skinUI.IsOpen = true;
            skinPanel.AddHandler(Button.ClickEvent, new RoutedEventHandler(ChangeSkin));
            InitSkins();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //string ColorSetting = UISettings.MyWindowColor[int.Parse(ConfigurationManager.AppSettings["Background"].ToString())].ColorID;
            //SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorSetting));
            //this.Background = brush;
            //UISettings.EnableBlur(this);
            string accent = ConfigurationManager.AppSettings["Background"].ToString();
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accent), ThemeManager.GetAppTheme("BaseLight"));
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
                //Image MovieCover = new Image();
                //test.Images.Add(JsonDouban.images.large);
                //MovieCover.Source = new BitmapImage(new Uri(JsonDouban.images.large));
                //MovieCover.Margin = new System.Windows.Thickness { Left = 43, Top = 120, Right = 0, Bottom = 0 };
                //MovieCover.Width = 300;
                //MovieCover.Height = 435;
                //MovieCover.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                //MovieCover.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //grid1.Children.Add(MovieCover);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    Image MovieCover = new Image();
                    MovieCover.Source = new BitmapImage(new Uri(JsonDouban.images.large));
                    MovieCover.Width = 300;
                    MovieCover.Height = 435;
                    this.listBox.Items.Add(MovieCover);
                }));
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
            List<string> ImageUrls = new List<string>();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "视频文件|*.mkv; *.rmvb; *.rm; *.wmv; *.avi; *.mpg; *.mpeg; *.mp4|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                new Thread(new ThreadStart(() => {
                    foreach (string FileName in openFileDialog.FileNames)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                        {
                            WaitingProgress Loading = new WaitingProgress();
                            this.listBox.Items.Add(Loading);
                        }));
                        MovieFileName = MovieTitleGet(FileName);
                        HttpWebRequest request;
                        request = (HttpWebRequest)WebRequest.Create(SearchUrl + MovieFileName + '}');
                        HttpWebResponse response;
                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                            string result = myreader.ReadToEnd();
                            myreader.Close();
                            SearchResult JsonDouban = JsonConvert.DeserializeObject<SearchResult>(result);
                            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                            {
                                int listBoxLength = this.listBox.Items.Count;
                                this.listBox.Items.RemoveAt(listBoxLength - 1);
                                if (JsonDouban.subjects.Count > 0)
                                {
                                    textBox.Text = MovieFileName;
                                    Image MovieCover = new Image();
                                    MovieCover.Source = new BitmapImage(new Uri(JsonDouban.subjects[0].images.large));
                                    MovieCover.Width = 300;
                                    MovieCover.Height = 435;
                                    this.listBox.Items.Add(MovieCover);
                                }
                                else
                                {
                                    this.ShowMessageAsync("没有搜索到该电影！", "请确认所选文件为电影文件");
                                }
                            }));
                        }
                        catch
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                            {
                                this.ShowMessageAsync("网络连接失败！", e.ToString());
                            }));
                        }  
                    }
                })).Start();
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            UISettings myUISettings = new UISettings();
            myUISettings.Show();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            CancellationToken token;
            TaskScheduler uiSched = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(DialogsBeforeExit, token, TaskCreationOptions.None, uiSched);
        }

        private async void DialogsBeforeExit()
        {
            MessageDialogResult result = await this.ShowMessageAsync(this.Title, "您真的要离开吗?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative)
            {
                return;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 初始化所有皮肤控件
        /// </summary>
        private void InitSkins()
        {
            var accents = ThemeManager.Accents;
            Style btnStyle = App.Current.FindResource("btnSkinStyle") as Style;
            foreach (var accent in accents)
            {
                //新建换肤按钮
                Button btnskin = new Button();
                btnskin.Style = btnStyle;
                btnskin.Name = accent.Name;
                SolidColorBrush scb = accent.Resources["AccentColorBrush"] as SolidColorBrush;
                btnskin.Background = scb;
                skinPanel.Children.Add(btnskin);
            }
        }
        /// <summary>
        /// 实现换肤
        /// </summary>
        private void ChangeSkin(object obj, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                String accent = (e.OriginalSource as Button).Name;
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accent), ThemeManager.GetAppTheme("BaseLight"));
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Background"].Value = accent;
                config.Save(ConfigurationSaveMode.Modified);  //保存修改
                ConfigurationManager.RefreshSection("appSettings"); //必须刷新才能读取修改后的值
            }
        }
    }
}
