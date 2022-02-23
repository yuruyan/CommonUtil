namespace CommonUtil.View.Dialog {
    public partial class InfoDialog : BaseDialog {
        public InfoDialog(string title = "提示", string detailText = "") : base(title, detailText) {
            InitializeComponent();
        }
    }
}
