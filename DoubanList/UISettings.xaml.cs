using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using System.Configuration;

namespace DoubanList
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    public class WindowColor
    {
        public string Name { set; get; }
        public string ColorID { set; get; }
    }

    /// <summary>
    /// UISettings.xaml 的交互逻辑
    /// </summary>
    public partial class UISettings : Window
    {
        public static List<WindowColor> MyWindowColor = new List<WindowColor>
        {
            new WindowColor { Name = "高冷黑", ColorID = "#B41E282C" },
            new WindowColor { Name = "墨水蓝", ColorID = "#B400284B" },
            new WindowColor { Name = "竹林绿", ColorID = "#AF00320F" },
            new WindowColor { Name = "桃花粉", ColorID = "#9DFF96AF" },
            new WindowColor { Name = "滑稽黄", ColorID = "#96F5C819" },
            new WindowColor { Name = "春节红", ColorID = "#9DFF3246" },
            new WindowColor { Name = "葡萄紫", ColorID = "#B0551264" },
            new WindowColor { Name = "橙子橙", ColorID = "#96FF6E0A" },
            new WindowColor { Name = "草儿绿", ColorID = "#96649105" }
        };

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public UISettings()
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
            comboBox.ItemsSource = MyWindowColor;
            int ColorIndex = int.Parse(ConfigurationManager.AppSettings["Background"].ToString());
            comboBox.Text = MyWindowColor[ColorIndex].Name;
            //EnableBlur(this);
        }

        /// <summary>
        /// 使能窗口Aero效果
        /// </summary>
        public static void EnableBlur(Window thisWindow)
        {
            var windowHelper = new WindowInteropHelper(thisWindow);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBox.SelectedIndex != -1)
            {
                //MessageBox.Show("你选择的颜色编号为：\n" +
                //    comboBox.SelectedValue);
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBox.SelectedValue.ToString()));
                this.Background = brush;
                Application.Current.MainWindow.Background = brush;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Background"].Value = comboBox.SelectedIndex.ToString();
                config.Save(ConfigurationSaveMode.Modified);  //保存修改
                ConfigurationManager.RefreshSection("appSettings"); //必须刷新才能读取修改后的值
            }
        }


    }
}
