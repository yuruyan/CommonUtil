namespace CommonUtil.View.Dialog {
    public partial class WarningDialog : BaseDialog {
        public WarningDialog(string title = "警告", string detailText = "") : base(title, detailText) {
            InitializeComponent();
        }
    }
}
