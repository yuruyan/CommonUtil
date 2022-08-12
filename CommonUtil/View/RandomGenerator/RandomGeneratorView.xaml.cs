using CommonUITools.Route;
using CommonUtil.Model;
using ModernWpf.Controls;
using NLog;
using System;
using System.Windows;

namespace CommonUtil.View;

public partial class RandomGeneratorView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RandomGeneratorView), new PropertyMetadata(""));
    /// <summary>
    /// 输出
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }

    private readonly Type[] Routers = {
        typeof(RandomStringGeneratorView),
        typeof(RandomNumberGeneratorView),
        typeof(RandomGeneratorWithRegexView),
        typeof(RandomGeneratorWithDataSourceView),
    };
    private readonly RouterService RouterService;

    public RandomGeneratorView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
    }

    /// <summary>
    /// 复制
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (RouterService.GetInstance(ContentFrame.CurrentSourcePageType) is IGenerable<string> generator) {
            OutputText = string.Join('\n', generator.Generate());
        }
    }

    /// <summary>
    /// 导航变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is FrameworkElement element) {
            foreach (var item in Routers) {
                if (item.Name == element.Name) {
                    RouterService.Navigate(item);
                    break;
                }
            }
        }
    }
}

