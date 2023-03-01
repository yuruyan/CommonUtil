﻿using WindowsInput.Events;
using WindowsInput.Events.Sources;

namespace CommonUtil.Core;

public static class DesktopAutomation {
    private static readonly IKeyboardEventSource KeyboardEvent = WindowsInput.Capture.Global.KeyboardAsync();
    private static readonly IDictionary<EventBuilder, CancellationTokenSource> EventBuilderCancellationTokenDict = new Dictionary<EventBuilder, CancellationTokenSource>();

    static DesktopAutomation() {
        KeyboardEvent.KeyDown += (_, e) => {
            // 监听 Escape 按下事件
            if (e.Data.Key == KeyCode.Escape) {
                foreach (var (_, source) in EventBuilderCancellationTokenDict) {
                    source.Cancel();
                }
            }
        };
    }

    /// <summary>
    /// 设置按下 Escape 时自动取消
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static EventBuilder CancelOnEscape(EventBuilder builder) {
        EventBuilderCancellationTokenDict[builder] = new();
        return builder;
    }

    /// <summary>
    /// 输入文本
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static EventBuilder InputText(EventBuilder builder, string text) => builder.Click(text);

    /// <summary>
    /// 鼠标单击
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="button"></param>
    /// <returns></returns>
    public static EventBuilder MouseClick(EventBuilder builder, ButtonCode button) => builder.Click(button);

    /// <summary>
    /// 鼠标双击
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="button"></param>
    /// <returns></returns>
    public static EventBuilder MouseDoubleClick(EventBuilder builder, ButtonCode button) => builder.DoubleClick(button);

    /// <summary>
    /// 鼠标双击
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static EventBuilder MouseMove(EventBuilder builder, Point point) => builder.MoveTo((int)point.X, (int)point.Y);

    /// <summary>
    /// 鼠标滚动
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="button"></param>
    /// <returns></returns>
    public static EventBuilder MouseScroll(EventBuilder builder, ButtonCode button, int offset) => builder.Scroll(button, offset);

    /// <summary>
    /// 等待
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="millisecond"></param>
    /// <returns></returns>
    public static EventBuilder Wait(EventBuilder builder, uint millisecond) => builder.Wait(millisecond);

    /// <summary>
    /// 按下按键后松开
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static EventBuilder PressKey(EventBuilder builder, KeyCode code) => builder.Click(code);

    /// <summary>
    /// 按下快捷键后松开
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="shortcuts"></param>
    /// <returns></returns>
    public static EventBuilder PressKeyShortcut(EventBuilder builder, params KeyCode[] shortcuts) => builder.Click(shortcuts);

    /// <summary>
    /// 异步运行
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static async Task<bool> RunAsync(EventBuilder builder) {
        if (EventBuilderCancellationTokenDict.TryGetValue(builder, out var tokenSource)) {
            var options = new InvokeOptions();
            options.Cancellation.Token = tokenSource.Token;
            return await builder.Invoke(options);
        }
        return await builder.Invoke();
    }
}
