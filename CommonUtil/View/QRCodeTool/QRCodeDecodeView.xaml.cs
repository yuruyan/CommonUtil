using CommonUITools.Model;
using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace CommonUtil.View;

public partial class QRCodeDecodeView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty DecodeTextProperty = DependencyProperty.Register("DecodeText", typeof(string), typeof(QRCodeDecodeView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 解析后的文本
    /// </summary>
    public string DecodeText {
        get { return (string)GetValue(DecodeTextProperty); }
        set { SetValue(DecodeTextProperty, value); }
    }

    public QRCodeDecodeView() {
        InitializeComponent();
        Loaded += (_, _) => {
            this.Focus();
        };
    }

    /// <summary>
    /// 选择图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectImageClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var openFileDialog = new OpenFileDialog() {
            Title = "选择图片",
            Filter = "PNG File|*.png|JPG File|*.jpg|JPEG File|*.jpeg|BMP File|*.bmp|All Files|*.*"
        };
        if (openFileDialog.ShowDialog() != true) {
            return;
        }
        // 释放资源
        if (QRCodeImage.Source is IDisposable image) {
            QRCodeImage.ClearValue(Image.SourceProperty);
            image.Dispose();
        }
        DoParseQRCodeImage(openFileDialog.FileName);
    }

    /// <summary>
    /// 解析 QRCode 错误处理
    /// </summary>
    /// <param name="method"></param>
    [NoException]
    private async void HandleParseQRCodeImageExceptionAsync(Func<Task<string?>> method) {
        try {
            var data = await method();
            // 解析失败
            if (data is null) {
                // 清空
                DecodeText = string.Empty;
                throw new ParseException();
            }
            DecodeText = data;
        } catch (LoadException error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("加载图片失败");
        } catch (ParseException error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("解析失败");
        } catch (Exception error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("解析失败");
        }
    }

    /// <summary>
    /// 解析 QRCode
    /// </summary>
    /// <param name="filepath">图片路径</param>
    [NoException]
    private void DoParseQRCodeImage(string filepath)
        => HandleParseQRCodeImageExceptionAsync(async () => {
            QRCodeImage.Source = UIUtils.CopyImageSource(filepath);
            return await Task.Run(() => QRCodeTool.DecodeQRCode(filepath));
        });

    /// <summary>
    /// 解析 QRCode
    /// </summary>
    /// <param name="bitmapSource"></param>
    [NoException]
    private void DoParseQRCodeImage(BitmapSource bitmapSource)
        => HandleParseQRCodeImageExceptionAsync(async () => {
            using var bitmap = UIUtils.BitmapSourceToBitmap(bitmapSource);
            // 加载图片失败
            if (bitmap is null) {
                throw new FormatException();
            }
            return await Task.Run(() => {
                using var stream = UIUtils.BitmapToStream(bitmap);
                return QRCodeTool.DecodeQRCode(stream);
            });
        });

    /// <summary>
    /// 拖拽图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void QRCodeImageDropHandler(object sender, DragEventArgs e) {
        e.Handled = true;
        if (e.Data.GetData(DataFormats.FileDrop) is IEnumerable<string> array) {
            // 判断是否为文件
            if (array.FirstOrDefault() is var file && file != null && File.Exists(file)) {
                DoParseQRCodeImage(file);
            }
        }
    }

    /// <summary>
    /// 在浏览器中打开
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenInBrowserHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (string.IsNullOrEmpty(DecodeText)) {
            return;
        }
        TaskUtils.Try(() => UIUtils.OpenInBrowser(DecodeText));
    }

    /// <summary>
    /// PasteImageCanExecute
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PasteImageCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = e.Handled = true;
    }

    /// <summary>
    /// PasteImageExecuted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PasteImageExecutedHandler(object sender, ExecutedRoutedEventArgs e) {
        e.Handled = true;
        ThrottleUtils.Throttle(PasteImageExecutedHandler, () => {
            var source = Clipboard.GetImage();
            if (source is null) {
                return;
            }
            // 释放资源
            if (QRCodeImage.Source is IDisposable image) {
                image.Dispose();
            }
            QRCodeImage.Source = source;
            DoParseQRCodeImage(source);
        }, 1000);
    }

}
