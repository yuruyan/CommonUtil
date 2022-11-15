using CommonUtil.Route;
using NLog;
using System;

namespace CommonUtil.View;

public partial class CommonEncodingView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(UnicodeEncodingView),
        typeof(UTF8EncodingView),
        typeof(URLEncodingView),
        typeof(HexEncodingView),
    };

    public CommonEncodingView() {
        InitializeComponent();
        NavigationUtils.EnableNavigation(
            NavigationView,
            new(ContentFrame, Routers),
            ContentFrame
        );
    }
}
