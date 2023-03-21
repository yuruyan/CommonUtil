using QRCoder.Exceptions;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;

namespace CommonUtil.View;

public partial class QRCodeGeneratorView : ResponsivePage, INavigationService {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty QRCodeImageSourceProperty = DependencyProperty.Register("QRCodeImageSource", typeof(ImageSource), typeof(QRCodeGeneratorView), new PropertyMetadata());
    public static readonly DependencyProperty QRCodeForegroundProperty = DependencyProperty.Register("QRCodeForeground", typeof(Color), typeof(QRCodeGeneratorView), new PropertyMetadata(Colors.Black, QRCodeForegroundPropertyChangedHandler));
    public static readonly DependencyProperty QRCodeForegroundTextProperty = DependencyProperty.Register("QRCodeForegroundText", typeof(string), typeof(QRCodeGeneratorView), new PropertyMetadata("#000000"));
    public static readonly DependencyProperty ECCLevelsProperty = DependencyProperty.Register("ECCLevels", typeof(IList<byte>), typeof(QRCodeGeneratorView), new PropertyMetadata());
    public static readonly DependencyProperty ECCLevelComboxSelectedIndexProperty = DependencyProperty.Register("ECCLevelComboxSelectedIndex", typeof(int), typeof(QRCodeGeneratorView), new PropertyMetadata(2));
    public static readonly DependencyProperty ImageQualityComboSelectedIndexProperty = DependencyProperty.Register("ImageQualityComboSelectedIndex", typeof(int), typeof(QRCodeGeneratorView), new PropertyMetadata(2));
    public static readonly DependencyProperty ImageQualityListProperty = DependencyProperty.Register("ImageQualityList", typeof(IList<string>), typeof(QRCodeGeneratorView), new PropertyMetadata());
    public static readonly DependencyProperty IsQRCodeDecodeViewSelectedProperty = DependencyProperty.Register("IsQRCodeDecodeViewSelected", typeof(bool), typeof(QRCodeGeneratorView), new PropertyMetadata(true));
    public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(QRCodeGeneratorView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 二维码 ImageSource
    /// </summary>
    public ImageSource QRCodeImageSource {
        get { return (ImageSource)GetValue(QRCodeImageSourceProperty); }
        set { SetValue(QRCodeImageSourceProperty, value); }
    }
    /// <summary>
    /// 二维码前景色
    /// </summary>
    public Color QRCodeForeground {
        get { return (Color)GetValue(QRCodeForegroundProperty); }
        set { SetValue(QRCodeForegroundProperty, value); }
    }
    /// <summary>
    /// 二维码前景色文本
    /// </summary>
    public string QRCodeForegroundText {
        get { return (string)GetValue(QRCodeForegroundTextProperty); }
        set { SetValue(QRCodeForegroundTextProperty, value); }
    }
    /// <summary>
    /// 容错率列表
    /// </summary>
    public IList<byte> ECCLevels {
        get { return (IList<byte>)GetValue(ECCLevelsProperty); }
        set { SetValue(ECCLevelsProperty, value); }
    }
    /// <summary>
    /// 选中 ECCLevel Index
    /// </summary>
    public int ECCLevelComboxSelectedIndex {
        get { return (int)GetValue(ECCLevelComboxSelectedIndexProperty); }
        set { SetValue(ECCLevelComboxSelectedIndexProperty, value); }
    }
    /// <summary>
    /// 图像质量选中 Index
    /// </summary>
    public int ImageQualityComboSelectedIndex {
        get { return (int)GetValue(ImageQualityComboSelectedIndexProperty); }
        set { SetValue(ImageQualityComboSelectedIndexProperty, value); }
    }
    /// <summary>
    /// 图片质量列表
    /// </summary>
    public IList<string> ImageQualityList {
        get { return (IList<string>)GetValue(ImageQualityListProperty); }
        set { SetValue(ImageQualityListProperty, value); }
    }
    /// <summary>
    /// 当前页面是否是二维码解析页面
    /// </summary>
    public bool IsQRCodeDecodeViewSelected {
        get { return (bool)GetValue(IsQRCodeDecodeViewSelectedProperty); }
        set { SetValue(IsQRCodeDecodeViewSelectedProperty, value); }
    }
    /// <summary>
    /// 图标路径
    /// </summary>
    public string IconPath {
        get { return (string)GetValue(IconPathProperty); }
        set { SetValue(IconPathProperty, value); }
    }

    private static readonly Type[] Routers = {
        typeof(URLQRCodeView),
        typeof(SMSQRCodeView),
        typeof(WIFIQRCodeView),
        typeof(MailQRCodeView),
        typeof(PhoneNumberQRCodeView),
        typeof(GeolocationQRCodeView),
    };
    private readonly SaveFileDialog SaveFileDialog = new() {
        Filter = "PNG File|*.png|BMP File|*.bmp|JPG File|*.jpg|SVG File|*.svg|PDF File|*.pdf",
    };
    private readonly OpenFileDialog OpenFileDialog = new() {
        Filter = "PNG File|*.png|BMP File|*.bmp|JPG File|*.jpg|All Files|*.*",
    };
    private readonly RouterService RouterService;
    /// <summary>
    /// 当前图片缓存
    /// </summary>
    private byte[] QRCodeImage = Array.Empty<byte>();
    private BitmapImage? IconBitmapImage;

    public QRCodeGeneratorView() {
        ECCLevels = DataSet.QRCodeECCLevelDict.Keys.ToList();
        ImageQualityList = DataSet.QRCodeImageQualityDict.Keys.ToList();
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
    }

    private static void QRCodeForegroundPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is QRCodeGeneratorView self) {
            self.QRCodeForegroundText = self.QRCodeForeground.ToString();
        }
    }

    protected override void IsExpandedPropertyChangedHandler(ResponsivePage self, DependencyPropertyChangedEventArgs e) {
        if (e.NewValue is true) {
            QRCodeGeneratorPanel.Rows = 1;
            QRCodeGeneratorPanel.Columns = 2;
        } else {
            QRCodeGeneratorPanel.Rows = 2;
            QRCodeGeneratorPanel.Columns = 1;
        }
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveImageClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 没有则先生成
        if (QRCodeImage == null || !QRCodeImage.Any()) {
            return;
        }
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }

        string extension = Path.GetExtension(SaveFileDialog.FileName).ToLowerInvariant();
        var format = extension switch {
            ".bmp" => QRCodeFormat.BMP,
            ".jpg" => QRCodeFormat.JPG,
            ".png" => QRCodeFormat.PNG,
            ".svg" => QRCodeFormat.SVG,
            ".pdf" => QRCodeFormat.PDF,
            _ => QRCodeFormat.PNG
        };
        SaveImageAsync(SaveFileDialog.FileName, format);
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="format"></param>
    private async void SaveImageAsync(string path, QRCodeFormat format) {
        var data = await GenerateImage(format);
        // 生成失败
        if (data is null) {
            return;
        }
        try {
            await File.WriteAllBytesAsync(path, data);
            MessageBoxUtils.NotifySuccess("保存成功", "点击打开", callback: () => {
                UIUtils.OpenFileInExplorerAsync(path);
            });
        } catch {
            MessageBoxUtils.Error("保存失败！");
        }
    }

    /// <summary>
    /// 生成图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateImageClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ThrottleUtils.ThrottleAsync(GenerateImageClick, async () => {
            byte[]? data = await GenerateImage();
            // 生成失败
            if (data is null) {
                return;
            }
            QRCodeImage = data;
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(QRCodeImage);
            image.EndInit();
            QRCodeImageSource = image;
        });
    }

    /// <summary>
    /// 生成图片，并自动显示失败消息
    /// </summary>
    /// <param name="format">生成格式</param>
    /// <returns>生成失败返回 null</returns>
    private async Task<byte[]?> GenerateImage(QRCodeFormat format = QRCodeFormat.PNG) {
        var generator = CommonUtils.NullCheck(
            RouterService.GetInstance(ContentFrame.CurrentSourcePageType)
            as IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>>
        );
        byte[] data;
        // 生成二维码
        Bitmap? icon = null;
        // 加载 icon
        if (IconBitmapImage != null) {
            icon = TaskUtils.Try(() => new Bitmap(IconBitmapImage.StreamSource));
            // 加载失败
            if (icon == null) {
                MessageBoxUtils.Error("图标加载失败");
                return null;
            }
        }
        try {
            data = await generator.Generate(new(format, new() {
                Foreground = System.Drawing.Color.FromArgb(
                    QRCodeForeground.R,
                    QRCodeForeground.G,
                    QRCodeForeground.B
                ),
                Image = icon,
                PixelPerModule = DataSet.QRCodeImageQualityDict[ImageQualityList[ImageQualityComboSelectedIndex]],
                ECCLevel = DataSet.QRCodeECCLevelDict[ECCLevels[ECCLevelComboxSelectedIndex]]
            }));
        } catch (DataTooLongException) {
            MessageBoxUtils.Error("文本过长！");
            return null;
        } catch (Exception e) {
            MessageBoxUtils.Error($"生成失败：{e.Message}");
            return null;
        }
        // 验证不通过
        if (!data.Any()) {
            return null;
        }
        return data;
    }

    /// <summary>
    /// 打开 ColorPicker
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void QRCodeForegroundRectangleMouseUp(object sender, MouseButtonEventArgs e) {
        if (sender is FrameworkElement element) {
            element.ContextMenu.IsOpen = true;
            element.ContextMenu.UpdateDefaultStyle();
        }
    }

    /// <summary>
    /// 防止 ContextMenu 关闭
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void QRCodeForegroundColorPickerPanelMouseUp(object sender, MouseButtonEventArgs e) => e.Handled = true;

    /// <summary>
    /// 设置 QRCodeForegroundColorPicker DataContext
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void QRCodeForegroundColorPickerLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element) {
            element.DataContext = this;
        }
    }

    /// <summary>
    /// 用户输入颜色值
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void QRCodeForegroundKeyUpHandler(object sender, KeyEventArgs e) {
        if (sender is TextBox textBox) {
            var color = TaskUtils.Try(() => UIUtils.StringToColor(textBox.Text) as Color?);
            // 转换失败
            if (color is null) {
                return;
            }
            QRCodeForeground = color.Value;
        }
    }

    /// <summary>
    /// 选择图标
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChooseIconMouseUpHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (OpenFileDialog.ShowDialog() != true) {
            return;
        }
        IconPath = OpenFileDialog.FileName;
        // 尝试加载图标
        try {
            var iconBi = UIUtils.CopyImageSource(IconPath);
            IconImage.Source = iconBi;
            // 释放资源
            IconBitmapImage?.StreamSource?.Close();
            IconBitmapImage = iconBi;
        } catch {
            MessageBoxUtils.Error("加载图标失败");
        }
    }

    /// <summary>
    /// 清除 Icon
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearIconImageClickHandler(object sender, RoutedEventArgs e) {
        IconPath = string.Empty;
        IconImage.Source = null;
        IconBitmapImage?.StreamSource?.Close();
        IconBitmapImage = null;
    }

    /// <summary>
    /// 导航
    /// </summary>
    /// <param name="data">为目标页面类型</param>
    public void Navigated(object? data) {
        if (data is Type type) {
            RouterService.Navigate(type);
        }
    }
}
