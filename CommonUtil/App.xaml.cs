using System.Windows;

namespace CommonUtil {
    public partial class App : Application {
        public App() {
            new SplashScreen("/Resource/SplashWindow.png").Show(true);
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            new MainWindow().Show();
        }
    }
}
