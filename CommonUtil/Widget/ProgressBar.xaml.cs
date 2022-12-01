using System.Windows.Media;

namespace CommonUtil.Widget;

public partial class ProgressBar : UserControl {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty ProcessProperty = DependencyProperty.Register("Process", typeof(double), typeof(ProgressBar), new PropertyMetadata(0.0));
    /// <summary>
    /// 进度
    /// </summary>
    public double Process {
        get { return (double)GetValue(ProcessProperty); }
        set { SetValue(ProcessProperty, value); }
    }
    /// <summary>
    /// 进度条前景色
    /// </summary>
    public SolidColorBrush ProgressBarForeground {
        get { return (SolidColorBrush)GetValue(ProgressBarForegroundProperty); }
        set { SetValue(ProgressBarForegroundProperty, value); }
    }
    public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(SolidColorBrush), typeof(ProgressBar), new PropertyMetadata());

    public ProgressBar() {
        ProgressBarForeground = (TryFindResource("SuccessBackground") as SolidColorBrush) ?? UIUtils.StringToBrush("#28A745");
        InitializeComponent();
    }
}
