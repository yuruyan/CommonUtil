using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonUtil.Utils {
    public class Loading {
        private static ProgressBar ProgressBar;

        public Loading(ProgressBar progressBar) {
            if (ProgressBar == null) {
                ProgressBar = progressBar;
            }
        }

        public static void StartLoading() {
            if (ProgressBar != null) {
                ProgressBar.IsIndeterminate = true;
            }
        }

        public static void StopLoading() {
            if (ProgressBar != null) {
                ProgressBar.IsIndeterminate = false;
            }
        }
    }

    public class UIUtils {
        /// <summary>
        /// 左键是否单击
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsLeftButtonDown(MouseButtonEventArgs e) {
            return e.LeftButton == MouseButtonState.Pressed;
        }

        /// <summary>
        /// 右键是否单击
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsRightButtonDown(MouseButtonEventArgs e) {
            return e.RightButton == MouseButtonState.Pressed;
        }

        /// <summary>
        /// 对齐 ContextMenu 位置、调整宽度
        /// </summary>
        /// <param name="menuOf">ContextMenu 所属控件</param>
        /// <param name="e"></param>
        public static void AlignContextMenu(FrameworkElement menuOf, MouseButtonEventArgs e) {
            ContextMenu contextMenu = menuOf.ContextMenu;
            Point point = e.GetPosition(menuOf);
            contextMenu.HorizontalOffset = -point.X;
            contextMenu.VerticalOffset = -point.Y;
            contextMenu.VerticalOffset += menuOf.ActualHeight;
            contextMenu.Width = menuOf.ActualWidth;
        }

        /// <summary>
        /// string 转 Brush
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Brush StringToBrush(string color) {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        /// <summary>
        /// string 转 Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color StringToColor(string color) {
            return ((SolidColorBrush)StringToBrush(color)).Color;
        }
    }
}
