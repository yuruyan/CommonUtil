using System.Runtime.InteropServices;

namespace CommonUtilBootstrapper;

public partial class MainForm : Form {
    public MainForm() {
        InitializeComponent();
        Init();
    }

    /// <summary>
    /// 初始化，添加控件
    /// </summary>
    private void Init() {
        CenterToScreen();
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        var pb = new PictureBox {
            Image = Resource.Resource.SplashWindow,
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.CenterImage
        };
        pb.MouseDown += PictureBoxMouseDownHandler;
        Width = pb.Image.Width;
        Height = pb.Image.Height;
        Controls.Add(pb);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool ReleaseCapture();
    private const int WM_NCLBUTTONDOWN = 0x00A1;
    private const int HTCAPTION = 2;

    /// <summary>
    /// 拖拽
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PictureBoxMouseDownHandler(object? sender, MouseEventArgs e) {
        // 按下的是鼠标左键 
        if (e.Button == MouseButtons.Left) {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero); // 拖动窗体 
        }
    }
}