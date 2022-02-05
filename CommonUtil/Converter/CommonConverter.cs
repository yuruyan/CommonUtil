using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CommonUtil.Converter {
    public class SafeDoubleConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                return System.Convert.ToDouble(value);
            } catch (Exception) {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 除以 1.5 Converter
    /// </summary>
    public class DivideHalfThreeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) / 1.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 除以 2 Converter
    /// </summary>
    public class DivideTwoConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 除以 3 Converter
    /// </summary>
    public class DivideThreeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) / 3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 为 0 则返回 ""
    /// </summary>
    public class HideZeroConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int v = System.Convert.ToInt32(value);
            return v <= 0 ? "" : v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 为 0 则隐藏
    /// </summary>
    public class HideIfZeroConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                return System.Convert.ToUInt64(value) == 0 ? Visibility.Collapsed : Visibility.Visible;
            } catch {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 不为 0 则隐藏
    /// </summary>
    public class HideIfNotZeroConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                return System.Convert.ToUInt64(value) != 0 ? Visibility.Collapsed : Visibility.Visible;
            } catch {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 为空则隐藏
    /// </summary>
    public class HideIfEmptyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.ToString().Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 不为空则隐藏
    /// </summary>
    public class HideIfNotEmptyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.ToString().Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 相等则返回 True
    /// </summary>
    public class EqualConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 不相等则返回 True
    /// </summary>
    public class NotEqualConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 加法 Convert，value + parameter
    /// </summary>
    public class AddConvert : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) + (parameter == null ? 0 : System.Convert.ToDouble(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 减法 Converter，value - parameter
    /// </summary>
    public class SubtractConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Math.Abs(System.Convert.ToDouble(value) - (parameter == null ? 0 : System.Convert.ToDouble(parameter)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 减法 Converter，parameter - value
    /// </summary>
    public class SubtractInvertConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (parameter == null) {
                return System.Convert.ToDouble(parameter);
            }
            return System.Convert.ToDouble(parameter) - System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 乘法 Converter，value * parameter
    /// </summary>
    public class MultiplyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) * (parameter == null ? 1 : System.Convert.ToDouble(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 除法 Converter，value / parameter
    /// </summary>
    public class DivideConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value) / (parameter == null ? 1 : System.Convert.ToDouble(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 除法 Converter，parameter / value
    /// </summary>
    public class DivideInvertConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (parameter == null) {
                return System.Convert.ToDouble(parameter);
            }
            return System.Convert.ToDouble(parameter) / System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// StringAppend Converter，前后用 '||' 分开
    /// </summary
    public class StringAppendConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (parameter == null) {
                return value;
            }
            string para = (string)parameter;
            string[] vs = para.Split("||");
            if (vs.Length == 0) {
                return value;
            }
            return $"{vs[0]}{value}{vs[1]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// DateTime 转 DateTime String Converter
    /// </summary>
    public class DateTimeToDateTimeStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string delimiter = (string)(parameter ?? "-");
            return ((DateTime)value).ToString($"yyyy{delimiter}MM{delimiter}dd HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// DateTime 转 Date String Converter
    /// </summary>
    public class DateTimeToDateStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string delimiter = (string)(parameter ?? "-");
            return ((DateTime)value).ToString($"yyyy{delimiter}MM{delimiter}dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// value 与 parameter 内容相同，返回 parameter 指定的 Visibility
    /// 不满足则返回 Visible
    /// parameter 格式：value|Visibility(大小写均可)
    /// </summary>
    public class VisibilityEqualConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (parameter == null) {
                throw new ArgumentNullException($"parameter is null");
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string[] ls = parameter.ToString().Split("|");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (ls.Length != 2) {
                throw new ArgumentException("content of parameter is invalid");
            }
            if (value.ToString() == ls[0]) {
                // 遍历 Visibility，找到相同的
                foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                    if (item.ToString().ToLower() == ls[1].ToLower()) {
                        return item;
                    }
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// value 与 parameter 包括的内容匹配，返回 parameter 指定的 Visibility
    /// 不满足则返回 Visible
    /// parameter 格式：[a b c]|Visibility(大小写均可)
    /// </summary>
    public class VisibilityIncludesConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                throw new ArgumentNullException($"value is null");
            }
            if (parameter == null) {
                throw new ArgumentNullException($"parameter is null");
            }
            string[] ls = parameter.ToString().Split("|");
            if (ls.Length != 2) {
                throw new ArgumentException("content of parameter is invalid");
            }
            var values = ls[0][1..^1].Split(" ").Select(v => v.ToLower()).ToHashSet();
            if (values.Contains(value.ToString().ToLower())) {
                // 遍历 Visibility，找到相同的
                foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                    if (item.ToString().ToLower() == ls[1].ToLower()) {
                        return item;
                    }
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// value 与 parameter 包括的内容不匹配，返回 parameter 指定的 Visibility
    /// 不满足则返回 Visible
    /// parameter 格式：[a b c]|Visibility(大小写均可)
    /// </summary>
    public class VisibilityNotIncludesConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                throw new ArgumentNullException($"value is null");
            }
            if (parameter == null) {
                throw new ArgumentNullException($"parameter is null");
            }
            string[] ls = parameter.ToString().Split("|");
            if (ls.Length != 2) {
                throw new ArgumentException("content of parameter is invalid");
            }
            var values = ls[0][1..^1].Split(" ").Select(v => v.ToLower()).ToHashSet();
            if (!values.Contains(value.ToString().ToLower())) {
                // 遍历 Visibility，找到相同的
                foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                    if (item.ToString().ToLower() == ls[1].ToLower()) {
                        return item;
                    }
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// value 与 parameter 内容不相同，返回 parameter 指定的 Visibility
    /// 不满足则返回 Visible
    /// parameter 格式：value|Visibility(大小写均可)
    /// </summary>
    public class VisibilityNotEqualConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (parameter == null) {
                throw new ArgumentNullException($"parameter is null");
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string[] ls = parameter.ToString().Split("|");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (ls.Length != 2) {
                throw new ArgumentException("content of parameter is invalid");
            }
            if (value.ToString() != ls[0]) {
                // 遍历 Visibility，找到相同的
                foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                    if (item.ToString().ToLower() == ls[1].ToLower()) {
                        return item;
                    }
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 显示文件名 Converter
    /// </summary>
    public class FileNameConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!File.Exists((string?)value)) {
                return string.Empty;
            }
            return new FileInfo((string)value).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 文件大小 Converter，自动显示相应大小
    /// </summary>
    public class FileSizeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var size = System.Convert.ToUInt64(value);
            if (size == 0) {
                return 0;
            }
            if (size < 1024) {
                return string.Format("{0} B", size);
            } else if (size < 0x100000) {
                return string.Format("{0:F2} KB", size / (double)1024);
            } else if (size < 0x40000000) {
                return string.Format("{0:F2} MB", size / (double)0x100000);
            } else {
                return string.Format("{0:F2} GB", size / (double)0x40000000);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}
