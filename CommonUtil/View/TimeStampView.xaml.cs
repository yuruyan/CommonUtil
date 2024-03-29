﻿namespace CommonUtil.View;

public partial class TimeStampView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty CurrentTimeStampProperty = DependencyProperty.Register("CurrentTimeStamp", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty TimeStampToStringInputProperty = DependencyProperty.Register("TimeStampToStringInput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty TimeStampToStringOptionProperty = DependencyProperty.Register("TimeStampToStringOption", typeof(string), typeof(TimeStampView), new PropertyMetadata("毫秒(ms)"));
    public static readonly DependencyProperty TimeStampToStringOutputProperty = DependencyProperty.Register("TimeStampToStringOutput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty StringToTimeStampInputProperty = DependencyProperty.Register("StringToTimeStampInput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty StringToTimeStampOutputProperty = DependencyProperty.Register("StringToTimeStampOutput", typeof(string), typeof(TimeStampView), new PropertyMetadata(""));
    public static readonly DependencyProperty StringToTimeStampChoiceProperty = DependencyProperty.Register("StringToTimeStampChoice", typeof(string), typeof(TimeStampView), new PropertyMetadata("毫秒(ms)"));
    public static readonly DependencyPropertyKey TimeStampOptionsPropertyKey = DependencyProperty.RegisterReadOnly("TimeStampOptions", typeof(IList<string>), typeof(TimeStampView), new PropertyMetadata());
    public static readonly DependencyProperty TimeStampOptionsProperty = TimeStampOptionsPropertyKey.DependencyProperty;

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
    /// 时间戳选项
    /// </summary>
    public IList<string> TimeStampOptions => (IList<string>)GetValue(TimeStampOptionsProperty);
    /// <summary>
    /// 更新时间 Timer
    /// </summary>
    private readonly DispatcherTimer UpdateTimeStampTimer = new();
    /// <summary>
    /// 毫秒选项值
    /// </summary>
    private readonly string MillisecondValue;

    public TimeStampView() : base(ResponsiveMode.Variable) {
        SetValue(TimeStampOptionsPropertyKey, DataSet.TimeStampOptions.ToArray());
        InitializeComponent();
        #region 初始化 TimeStamp
        MillisecondValue = TimeStampOptions[0];
        CurrentTimeStamp = TimeStamp.GetCurrentMilliSeconds().ToString();
        StringToTimeStampOutput = TimeStampToStringInput = CurrentTimeStamp;
        StringToTimeStampInput = TimeStampToStringOutput = TimeStamp.TimeStampToDateTimeString(TimeStamp.GetCurrentMilliSeconds());
        #endregion
        #region 更新时间戳
        UpdateTimeStampTimer.Interval = TimeSpan.FromSeconds(1);
        UpdateTimeStampTimer.Tick += (_, _) => {
            CurrentTimeStamp = TimeStamp.GetCurrentMilliSeconds().ToString();
        };
        #endregion
    }

    protected override void IsExpandedPropertyChangedHandler(ResponsiveLayout self, DependencyPropertyChangedEventArgs e) {
        // 反转顺序
        StringToTimeStampPanel.ReverseChildrenOrder();
        TimeStampToStringPanel.ReverseChildrenOrder();
    }

    /// <summary>
    /// 复制当前时间戳
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyTimeStampClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(CurrentTimeStamp);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 时间戳转字符串时间 Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimeStampToStringClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        try {
            TimeStampToStringOutput = TimeStamp.TimeStampToDateTimeString(Convert.ToInt64(
                TimeStampToStringOption == MillisecondValue ? TimeStampToStringInput : TimeStampToStringInput + "000"
            ));
        } catch (Exception error) {
            Logger.Info(error);
            MessageBoxUtils.Error("转换失败！");
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
            if (StringToTimeStampChoice != MillisecondValue) {
                t /= 1000;
            }
            StringToTimeStampOutput = t.ToString();
        } catch (Exception error) {
            Logger.Info(error);
            MessageBoxUtils.Error("转换失败！");
        }
    }

    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        UpdateTimeStampTimer.Start();
    }

    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        UpdateTimeStampTimer.Stop();
    }
}
