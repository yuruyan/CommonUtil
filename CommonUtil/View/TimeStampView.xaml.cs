using CommonUtil.Core;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class TimeStampView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty CurrentTimeStampProperty = DependencyProperty.Register("CurrentTimeStamp", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty TimeStampToStringInputProperty = DependencyProperty.Register("TimeStampToStringInput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty TimeStampToStringOptionProperty = DependencyProperty.Register("TimeStampToStringOption", typeof(string), typeof(TimeStampView), new PropertyMetadata("毫秒(ms)"));
    public static readonly DependencyProperty TimeStampToStringOutputProperty = DependencyProperty.Register("TimeStampToStringOutput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty StringToTimeStampInputProperty = DependencyProperty.Register("StringToTimeStampInput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty StringToTimeStampOutputProperty = DependencyProperty.Register("StringToTimeStampOutput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty StringToTimeStampChoiceProperty = DependencyProperty.Register("StringToTimeStampChoice", typeof(string), typeof(TimeStampView), new PropertyMetadata("毫秒(ms)"));

    /// <summary>
    /// 当前时间戳
    /// </summary>
    public string CurrentTimeStamp {
        get { return (string)GetValue(CurrentTimeStampProperty); }
        set { SetValue(CurrentTimeStampProperty, value); }
    }
    /// <summary>
    /// 时间戳转日期字符串输入
    /// </summary>
    public string TimeStampToStringInput {
        get { return (string)GetValue(TimeStampToStringInputProperty); }
        set { SetValue(TimeStampToStringInputProperty, value); }
    }
    /// <summary>
    /// 时间戳转日期字符串选择
    /// </summary>
    public string TimeStampToStringOption {
        get { return (string)GetValue(TimeStampToStringOptionProperty); }
        set { SetValue(TimeStampToStringOptionProperty, value); }
    }
    /// <summary>
    /// 时间戳转日期字符串输出
    /// </summary>
    public string TimeStampToStringOutput {
        get { return (string)GetValue(TimeStampToStringOutputProperty); }
        set { SetValue(TimeStampToStringOutputProperty, value); }
    }
    /// <summary>
    /// 字符串日期转时间戳输入
    /// </summary>
    public string StringToTimeStampInput {
        get { return (string)GetValue(StringToTimeStampInputProperty); }
        set { SetValue(StringToTimeStampInputProperty, value); }
    }
    /// <summary>
    /// 字符串日期转时间戳输出
    /// </summary>
    public string StringToTimeStampOutput {
        get { return (string)GetValue(StringToTimeStampOutputProperty); }
        set { SetValue(StringToTimeStampOutputProperty, value); }
    }
    /// <summary>
    /// 字符串日期转时间戳选择
    /// </summary>
    public string StringToTimeStampChoice {
        get { return (string)GetValue(StringToTimeStampChoiceProperty); }
        set { SetValue(StringToTimeStampChoiceProperty, value); }
    }
    /// <summary>
    /// 更新时间 Timer
    /// </summary>
    private readonly System.Timers.Timer Timer;

    public TimeStampView() {
        InitializeComponent();
        CurrentTimeStamp = TimeStamp.GetCurrentMilliSeconds().ToString();
        TimeStampToStringInput = CurrentTimeStamp;
        TimeStampToStringOutput = TimeStamp.TimeStampToDateTimeString(TimeStamp.GetCurrentMilliSeconds());
        StringToTimeStampInput = TimeStampToStringOutput;
        StringToTimeStampOutput = TimeStampToStringInput;
        #region 更新时间戳
        Timer = new System.Timers.Timer(1000);
        Timer.Elapsed += UpdateTimeStamp;
        Timer.Start();
        #endregion
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void UpdateTimeStamp(object? sender, ElapsedEventArgs e) {
        Dispatcher.Invoke(() => {
            CurrentTimeStamp = TimeStamp.GetCurrentMilliSeconds().ToString();
        });
    }

    /// <summary>
    /// 复制当前时间戳
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyTimeStampClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(CurrentTimeStamp);
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 时间戳转字符串时间 Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimeStampToStringClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        try {
            long t = 0;
            if (TimeStampToStringOption.Contains("ms")) {
                t = (long)Convert.ToUInt64(TimeStampToStringInput);
            } else {
                t = (long)Convert.ToUInt64(TimeStampToStringInput + "000");
            }
            TimeStampToStringOutput = TimeStamp.TimeStampToDateTimeString(t);
        } catch (Exception error) {
            Logger.Info(error);
            CommonUITools.Widget.MessageBox.Error("转换失败！");
        }
    }

    /// <summary>
    /// 日期字符串转时间戳
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StringToTimeStampClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        try {
            long t = TimeStamp.StringToMilliSeconds(StringToTimeStampInput);
            if (!StringToTimeStampChoice.Contains("ms")) {
                t /= 1000;
            }
            StringToTimeStampOutput = t.ToString();
        } catch (Exception error) {
            Logger.Info(error);
            CommonUITools.Widget.MessageBox.Error("格式有误！");
        }
    }
}
