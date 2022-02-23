using ModernWpf.Controls;
using System.Threading.Tasks;
using System.Windows;

namespace CommonUtil.View.Dialog {
    public partial class SimpleInputDialog : BaseDialog {

        /// <summary>
        /// Input Header
        /// </summary>
        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SimpleInputDialog), new PropertyMetadata(""));

        public SimpleInputDialog() : this("") {
        }

        public SimpleInputDialog(string title = "", string header = "", string text = "") {
            InitializeComponent();
            Title = title;
            Header = header;
            TargetTextBlock.Text = text;
        }

        /// <summary>
        /// Show Dialog
        /// </summary>
        /// <returns></returns>
        public async Task<DialogResult<string>> Show() {
            ContentDialogResult result = await ShowAsync();
            return new DialogResult<string>() {
                // 放在第一位
                Result = result,
                Data = TargetTextBlock.Text
            };
        }
    }
}
