using CommonUtil.Core;
using NLog;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class CSharpDependencyView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(CSharpDependencyView), new PropertyMetadata(""));
    public static readonly DependencyProperty TypeInfosProperty = DependencyProperty.Register("TypeInfos", typeof(ObservableCollection<TypeInfo>), typeof(CSharpDependencyView), new PropertyMetadata());

    /// <summary>
    /// 输出文本
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    public ObservableCollection<TypeInfo> TypeInfos {
        get { return (ObservableCollection<TypeInfo>)GetValue(TypeInfosProperty); }
        set { SetValue(TypeInfosProperty, value); }
    }

    public CSharpDependencyView() {
        InitializeComponent();
        TypeInfos = new();
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
        OutputText = string.Empty;
    }

    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddPropertyClick(object sender, RoutedEventArgs e) {
        TypeInfos.Add(new());
    }

    /// <summary>
    /// 生成代码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateCodeClick(object sender, RoutedEventArgs e) {
        OutputText = CSharpDependencyGenerator.CreateTemplate(TypeInfos);
    }

    private void removePropertyClick(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement elem && elem.DataContext is TypeInfo typeInfo) {
            TypeInfos.Remove(typeInfo);
        }
    }
}

