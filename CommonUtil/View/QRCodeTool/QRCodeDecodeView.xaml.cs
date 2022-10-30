using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
        var imageFile = openFileDialog.FileName;
        try {
            QRCodeImage.Source = new BitmapImage(new Uri(imageFile));
            try {
                string? data = QRCodeTool.DecodeQRCode(imageFile);
                // 解析成功
                if (data != null) {
                    DecodeText = data;
                    return;
                }
                // 清空
                DecodeText = string.Empty;
                throw new Exception();
            } catch {
                CommonUITools.Widget.MessageBox.Error("解析失败");
            }
        } catch (Exception error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("加载图片失败");
        }
    }
}
