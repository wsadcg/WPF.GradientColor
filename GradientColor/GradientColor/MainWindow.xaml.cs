using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GradientColor
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

        private int _DivNum = 0;
        private int _Pointer = 0;

        private int[] _GradientColor = null;
        private double[] _GradientStep = null;
        private Color _InputColor = Colors.Transparent;

        private void inputDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(inputDec.Text, @"^((25[0-5]|2[0-4]\d|[1]{1}\d{1}\d{1}|[1-9]{1}\d{1}|\d{1})($|(?!\,$)\,)){3}$"))
            {
                string[] arrDEC = inputDec.Text.Split(',');
                string[] arrHEX = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    arrHEX[i] = Convert.ToString(Convert.ToInt32(arrDEC[i]), 16).ToUpper().PadLeft(2, '0');
                }
                if (string.Join("", arrHEX) != inputHex.Text)
                {
                    inputHex.Text = string.Join("", arrHEX);
                }
                _InputColor = Color.FromArgb(255, Convert.ToByte(arrDEC[0]), Convert.ToByte(arrDEC[1]), Convert.ToByte(arrDEC[2]));
                rect.Fill = new SolidColorBrush(_InputColor);
            }
            else if (Regex.IsMatch(inputDec.Text, @"^((25[0-5]|2[0-4]\d|[1]{1}\d{1}\d{1}|[1-9]{1}\d{1}|\d{1})($|(?!\,$)\,)){4}$"))
            {
                string[] arrDEC = inputDec.Text.Split(',');
                string[] arrHEX = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    arrHEX[i] = Convert.ToString(Convert.ToInt32(arrDEC[i]), 16).ToUpper().PadLeft(2, '0');
                }
                if (string.Join("", arrHEX) != inputHex.Text)
                {
                    inputHex.Text = string.Join("", arrHEX);
                }
                _InputColor = Color.FromArgb(Convert.ToByte(arrDEC[0]), Convert.ToByte(arrDEC[1]), Convert.ToByte(arrDEC[2]), Convert.ToByte(arrDEC[3]));
                rect.Fill = new SolidColorBrush(_InputColor);
            }
        }
        private void inputHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputHex.Text.Length == 6)
            {
                string[] arrHEX = new string[3];
                string[] arrDEC = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    arrHEX[i] = inputHex.Text.Substring(i * 2, 2);
                    arrDEC[i] = Convert.ToInt32(arrHEX[i], 16).ToString();
                }
                if (string.Join(",", arrDEC) != inputDec.Text)
                {
                    inputDec.Text = string.Join(",", arrDEC);
                }
                _InputColor = Color.FromArgb(255, Convert.ToByte(arrDEC[0]), Convert.ToByte(arrDEC[1]), Convert.ToByte(arrDEC[2]));
                rect.Fill = new SolidColorBrush(_InputColor);
            }
            else if (inputHex.Text.Length == 8)
            {
                string[] arrHEX = new string[4];
                string[] arrDEC = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    arrHEX[i] = inputHex.Text.Substring(i * 2, 2);
                    arrDEC[i] = Convert.ToInt32(arrHEX[i], 16).ToString();
                }
                if (string.Join(",", arrDEC) != inputDec.Text)
                {
                    inputDec.Text = string.Join(",", arrDEC);
                }
                _InputColor = Color.FromArgb(Convert.ToByte(arrDEC[0]), Convert.ToByte(arrDEC[1]), Convert.ToByte(arrDEC[2]), Convert.ToByte(arrDEC[3]));
                rect.Fill = new SolidColorBrush(_InputColor);
            }
        }
        private void divide_Click(object sender, RoutedEventArgs e)
        {
            _DivNum = 0;
            _Pointer = 0;

            if (_InputColor != Colors.Transparent && divideNum.Text != "" && int.TryParse(divideNum.Text, out _DivNum))
            {
                if (_DivNum <= 0) return;
                result.Children.RemoveRange(0, result.Children.Count - 1);

                // 梯度计算
                _GradientStep = new double[3];
                _GradientStep[0] = (255 - _InputColor.R) / (double)_DivNum;
                _GradientStep[1] = (255 - _InputColor.G) / (double)_DivNum;
                _GradientStep[2] = (255 - _InputColor.B) / (double)_DivNum;

                // 当前颜色赋值
                _GradientColor = new int[3];
                _GradientColor[0] = _InputColor.R;
                _GradientColor[1] = _InputColor.G;
                _GradientColor[2] = _InputColor.B;

                if (divideUp.IsChecked == true)
                {
                    addBtn.Visibility = Visibility.Collapsed;
                    GetColorGroup(_DivNum, _DivNum);
                }
                else
                {
                    // 防止梯度变化过小导致死循环
                    _GradientStep[0] = Math.Max(0.1, _GradientStep[0]);
                    _GradientStep[1] = Math.Max(0.1, _GradientStep[1]);
                    _GradientStep[2] = Math.Max(0.1, _GradientStep[2]);
                    addBtn.Visibility = Visibility.Visible;

                    GetColorGroup(_DivNum, _Pointer + 100);
                }
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            GetColorGroup(_DivNum, _Pointer + 100);
        }
        private void GetColorGroup(int divNum, int endPtr = int.MaxValue)
        {
            while (_Pointer < endPtr)
            {
                if (divideUp.IsChecked == true)
                {
                    _GradientColor[0] = Math.Min(255, _InputColor.R + (int)(_Pointer * _GradientStep[0]));
                    _GradientColor[1] = Math.Min(255, _InputColor.G + (int)(_Pointer * _GradientStep[1]));
                    _GradientColor[2] = Math.Min(255, _InputColor.B + (int)(_Pointer * _GradientStep[2]));

                }
                else
                {
                    _GradientColor[0] = Math.Max(0, _InputColor.R - (int)(_Pointer * _GradientStep[0]));
                    _GradientColor[1] = Math.Max(0, _InputColor.G - (int)(_Pointer * _GradientStep[1]));
                    _GradientColor[2] = Math.Max(0, _InputColor.B - (int)(_Pointer * _GradientStep[2]));
                    if (_GradientColor[0] + _GradientColor[1] + _GradientColor[2] == 0)
                    {
                        _Pointer = endPtr;
                        addBtn.Visibility = Visibility.Collapsed;
                    }
                }

                Color backColor = Color.FromArgb(_InputColor.A, (byte)_GradientColor[0], (byte)_GradientColor[1], (byte)_GradientColor[2]);
                TextBox textBox = new TextBox
                {
                    Width = 80,
                    Height = 30,
                    IsReadOnly = true,
                    BorderBrush = new SolidColorBrush(Colors.Transparent),
                    Background = new SolidColorBrush(backColor),
                    Foreground = new SolidColorBrush(GetReverseForegroundColor(GetGrayLevel(backColor))),
                    Text = "#" + Convert.ToString(_InputColor.A, 16).ToUpper().PadLeft(2, '0') + Convert.ToString(_GradientColor[0], 16).ToUpper().PadLeft(2, '0') + Convert.ToString(_GradientColor[1], 16).ToUpper().PadLeft(2, '0') + Convert.ToString(_GradientColor[2], 16).ToUpper().PadLeft(2, '0'),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                result.Children.Insert(result.Children.Count - 1, textBox);

                _Pointer++;
            }
        }

        /// <summary>
        /// 获取一个颜色的人眼感知亮度，并以 0~1 之间的小数表示。
        /// 考虑到可能有透明度，当Color.A很小时，返回-1
        /// </summary>
        private double GetGrayLevel(Color color)
        {
            if (color.A < 100) return -1;
            return (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
        }

        /// <summary>
        /// 返回对比明显的前景色
        /// </summary>
        /// <param name="grayLevel"></param>
        /// <returns></returns>
        private static Color GetReverseForegroundColor(double grayLevel) => grayLevel > 0.5 ? Colors.Black : (grayLevel == -1 ? Colors.Black : Colors.White);

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = (bool)setTopmost.IsChecked;
        }
    }
}
