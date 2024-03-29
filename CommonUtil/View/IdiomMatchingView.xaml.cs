﻿using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class IdiomMatchingView : System.Windows.Controls.Page {
    //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputIdiomProperty = DependencyProperty.Register("InputIdiom", typeof(string), typeof(IdiomMatchingView), new PropertyMetadata(""));
    public static readonly DependencyProperty MatchListProperty = DependencyProperty.Register("MatchList", typeof(List<string>), typeof(IdiomMatchingView), new PropertyMetadata());

    /// <summary>
    /// 输入 idiom
    /// </summary>
    public string InputIdiom {
        get { return (string)GetValue(InputIdiomProperty); }
        set { SetValue(InputIdiomProperty, value); }
    }
    /// <summary>
    /// 匹配列表
    /// </summary>
    public List<string> MatchList {
        get { return (List<string>)GetValue(MatchListProperty); }
        set { SetValue(MatchListProperty, value); }
    }

    public IdiomMatchingView() {
        InitializeComponent();
        Task.Run(IdiomMatching.InitializeExplicitly);
    }

    /// <summary>
    /// 生成点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void GenerateClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        await GenerateMatchList();
    }

    /// <summary>
    /// 生成成语接龙
    /// </summary>
    private async Task GenerateMatchList() {
        if (InputIdiom.Trim() == string.Empty) {
            MessageBoxUtils.Info("请输入文本");
            return;
        }
        string idiom = InputIdiom.Trim();
        MatchList = await Task.Run(() => {
            var matches = IdiomMatching.GetMatchList(idiom);
            var list = new List<string>(matches.Count);
            foreach (var item in matches) {
                list.Add(string.Join(" => ", item));
            }
            return list;
        });
    }

    /// <summary>
    /// 输入改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void InputIdiomChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
        if (sender.Text.Trim() != string.Empty) {
            sender.ItemsSource = IdiomMatching.IdiomList
                .Where(s => s.Contains(sender.Text))
                .Take(8);
        }
    }

    /// <summary>
    /// 键盘按下 Enter 触发查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void IdiomInputBoxKeyUp(object sender, KeyEventArgs e) {
        e.Handled = true;
        if (e.Key == Key.Enter) {
            await GenerateMatchList();
        }
    }

    /// <summary>
    /// 复制
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(string.Join(
            '\n',
            ResultListBox.SelectedItems.Cast<string>()
        ));
        MessageBoxUtils.Success("已复制");
    }
}
