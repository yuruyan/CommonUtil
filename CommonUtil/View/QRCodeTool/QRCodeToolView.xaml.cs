using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUITools.Widget;
using CommonUtil.Core;
using CommonUtil.Model;
using Microsoft.Win32;
using ModernWpf.Controls;
using NLog;
using QRCoder.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class QRCodeToolView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty QRCodeImageSourceProperty = DependencyProperty.Register("QRCodeImageSource", typeof(ImageSource), typeof(QRCodeToolView), new PropertyMetadata());

    /// <summary>
    /// 二维码 ImageSource
    /// </summary>
    public ImageSource QRCodeImageSource {
        get { return (ImageSource)GetValue(QRCodeImageSourceProperty); }
        set { SetValue(QRCodeImageSourceProperty, value); }
    }
    private readonly Type[] Routers = {
        typeof(URLQRCodeView),
        typeof(SMSQRCodeView),
    };
    private readonly RouterService RouterService;
    private byte[] QRCodeImage = Array.Empty<byte>();

    public QRCodeToolView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
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
        var input = "InputText";
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
            data = QRCodeTool.GenerateQRCodeForText(input, new(), format);
            try {
                File.WriteAllBytes(path, data);
                Dispatcher.Invoke(() => {
                    NotificationBox.Success("保存成功", "点击打开", () => {
                        UIUtils.OpenFileInDirectoryAsync(path);
                    });
                });
            } catch (Exception e) {
                Dispatcher.Invoke(() => MessageBox.Error("保存失败！"));
                Logger.Error(e);
            }
        } catch (DataTooLongException e) {
            Dispatcher.Invoke(() => MessageBox.Error("文本过长！"));
            Logger.Error(e);
        } catch (Exception e) {
            Dispatcher.Invoke(() => MessageBox.Error("生成失败"));
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
        if (RouterService.GetInstance(ContentFrame.CurrentSourcePageType) is IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>> generator) {
            ThrottleUtils.ThrottleAsync(GenerateImageClick, async () => {
                byte[] data = Array.Empty<byte>();
                // 生成二维码
                try {
                    data = await generator.Generate(new(QRCodeFormat.PNG, new()));
                } catch (DataTooLongException) {
                    MessageBox.Error("文本过长！");
                    return;
                } catch (Exception e) {
                    MessageBox.Error($"生成失败：{e.Message}");
                    return;
                }
                QRCodeImage = data;
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(QRCodeImage);
                image.EndInit();
                QRCodeImageSource = image;
            });
        }
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        //InputText = string.Empty;
    }

    /// <summary>
    /// 导航变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is FrameworkElement element) {
            RouterService.Navigate(Routers.First(r => r.Name == element.Name));
        }
    }
}

