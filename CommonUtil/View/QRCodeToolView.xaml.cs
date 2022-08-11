using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using QRCoder.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonUtil.View;

public partial class QRCodeToolView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(QRCodeToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty QRCodeImageSourceProperty = DependencyProperty.Register("QRCodeImageSource", typeof(ImageSource), typeof(QRCodeToolView), new PropertyMetadata());

    /// <summary>
    /// 输入文本
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 二维码 ImageSource
    /// </summary>
    public ImageSource QRCodeImageSource {
        get { return (ImageSource)GetValue(QRCodeImageSourceProperty); }
        set { SetValue(QRCodeImageSourceProperty, value); }
    }
    private byte[] QRCodeImage = Array.Empty<byte>();

    public QRCodeToolView() {
        InitializeComponent();
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveImageClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var dialog = new SaveFileDialog {
            Filter = "PNG File|*.png|BMP File|*.bmp|JPG File|*.jpg|SVG File|*.svg|PDF File|*.pdf",
        };
        if (dialog.ShowDialog() != true) {
            return;
        }
        string ext = new FileInfo(dialog.FileName).Extension.TrimStart('.').ToLower();
        var input = InputText;
        bool found = false;
        foreach (var name in Enum.GetNames(typeof(QRCodeFormat))) {
            if (name.ToLower() == ext) {
                found = true;
                Task.Run(() => {
                    SaveImage(input, dialog.FileName, (QRCodeFormat)Enum.Parse(typeof(QRCodeFormat), name));
                });
                break;
            }
        }
        // 未提供的格式，生成 png
        if (!found) {
            Task.Run(() => SaveImage(input + ".png", dialog.FileName, QRCodeFormat.PNG));
        }
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="input"></param>
    /// <param name="path"></param>
    /// <param name="format"></param>
    private void SaveImage(string input, string path, QRCodeFormat format) {
        byte[] data = Array.Empty<byte>();
        try {
            data = QRCodeTool.GenerateQRCode(input, format);
            try {
                File.WriteAllBytes(path, data);
                Dispatcher.Invoke(() => {
                    CommonUITools.Widget.NotificationBox.Success("保存成功", "点击打开", () => {
                        UIUtils.OpenFileInDirectoryAsync(path);
                    });
                });
            } catch (Exception e) {
                Dispatcher.Invoke(() => CommonUITools.Widget.MessageBox.Error("保存失败！"));
                Logger.Error(e);
            }
        } catch (DataTooLongException e) {
            Dispatcher.Invoke(() => CommonUITools.Widget.MessageBox.Error("文本过长！"));
            Logger.Error(e);
        } catch (Exception e) {
            Dispatcher.Invoke(() => CommonUITools.Widget.MessageBox.Error("生成失败"));
            Logger.Error(e);
        }
    }

    /// <summary>
    /// 生成图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateImageClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        string text = InputText;
        Task.Run(() => {
            try {
                byte[] vs = QRCodeTool.GenerateQRCode(text);
                Dispatcher.Invoke(() => {
                    QRCodeImage = vs;
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(QRCodeImage);
                    image.EndInit();
                    QRCodeImageSource = image;
                });
            } catch (DataTooLongException error) {
                Dispatcher.Invoke(() => CommonUITools.Widget.MessageBox.Error("文本过长！"));
                Logger.Error(error);
            } catch (Exception error) {
                Dispatcher.Invoke(() => CommonUITools.Widget.MessageBox.Error("生成失败"));
                Logger.Error(error);
            }
        });
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText = string.Empty;
    }
}

