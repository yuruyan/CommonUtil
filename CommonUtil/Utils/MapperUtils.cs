using AutoMapper;

namespace CommonUtil.Utils;

public class MapperUtils {
    public static IMapper Instance => CommonUtils.GetSingletonInstance<IMapper>(() => {
        return new MapperConfiguration(cfg => {
            cfg.ClearPrefixes();
            cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
            cfg.AddMaps(typeof(ToolMenuItemProfile));
        }).CreateMapper();
    });

    /// <summary>
    /// 包装 Dispatcher.Invoke Map 方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T Map<T>(object obj) => UIUtils.RunOnUIThread(() => Instance.Map<T>(obj));
}
