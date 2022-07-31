using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.AspNetCore.Components.Forms;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonUtil.View;

public partial class CollectionToolView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty InputText1Property = DependencyProperty.Register("InputText1", typeof(string), typeof(CollectionToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputText2Property = DependencyProperty.Register("InputText2", typeof(string), typeof(CollectionToolView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(CollectionToolView), new PropertyMetadata(""));

    /// <summary>
    /// 输入
    /// </summary>
    public string InputText2 {
        get { return (string)GetValue(InputText2Property); }
        set { SetValue(InputText2Property, value); }
    }
    /// <summary>
    /// 输入
    /// </summary>
    public string InputText1 {
        get { return (string)GetValue(InputText1Property); }
        set { SetValue(InputText1Property, value); }
    }
    /// <summary>
    /// 输出结果
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }

    public CollectionToolView() {
        InitializeComponent();
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText2 = InputText1 = string.Empty;
        OutputText = string.Empty;
    }

    /// <summary>
    /// 并集
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UnionClickHandler(object sender, RoutedEventArgs e) {
        OutputText = string.Join(
            '\n',
            CollectionTool.Union(
                CommonUtils.NormalizeMultipleLineText(InputText1).Split('\n'),
                CommonUtils.NormalizeMultipleLineText(InputText2).Split('\n')
            )
        );
    }

    /// <summary>
    /// 交集
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IntersectClickHandler(object sender, RoutedEventArgs e) {
        OutputText = string.Join(
            '\n',
            CollectionTool.Intersect(
                CommonUtils.NormalizeMultipleLineText(InputText1).Split('\n'),
                CommonUtils.NormalizeMultipleLineText(InputText2).Split('\n')
            )
        );
    }

    /// <summary>
    /// 差集
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExceptClickHandler(object sender, RoutedEventArgs e) {
        OutputText = string.Join(
            '\n',
            CollectionTool.Except(
                CommonUtils.NormalizeMultipleLineText(InputText1).Split('\n'),
                CommonUtils.NormalizeMultipleLineText(InputText2).Split('\n')
            )
        );
    }
}

