namespace CommonUtil.Core.Core;

public static class DesktopAutomation {
    public static async Task InputTextAsync(string text) {
        await WindowsInput.Simulate
            .Events()
            .Click(text)
            .Invoke();
    }
}
