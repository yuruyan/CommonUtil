using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        ParseQRCodeImage(openFileDialog.FileName);
    }

    /// <summary>
    /// 解析 QRCode
    /// </summary>
    /// <param name="filepath">图片路径</param>
    private void ParseQRCodeImage(string filepath) {
        try {
            QRCodeImage.Source = UIUtils.CopyImageSource(filepath);
            try {
                string? data = QRCodeTool.DecodeQRCode(filepath);
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
                ParseQRCodeImage(file);
            }
        }
    }
}
