using Newtonsoft.Json.Linq;

namespace CommonUtil.Store;

public class Config {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly string ConfigPath = "config/AppConfig.json";
    /// <summary>
    /// 环境模式
    /// </summary>
    public static readonly CommonUtil.Core.Model.ApplicationEnvironment Environment = Core.Model.ApplicationEnvironment.Development;

    static Config() {
        try {
            // 加载配置文件
            var jObject = JObject.Parse(File.ReadAllText(ConfigPath));
            JToken? environment = jObject.GetValue("environment");
            if (environment != null) {
                if (environment.ToObject<Core.Model.ApplicationEnvironment>() is Core.Model.ApplicationEnvironment env) {
                    Environment = env;
                }
            }
        } catch (Exception e) {
            // 配置文件加载失败
            Logger.Fatal(e);
            App.Current.Shutdown(-1);
        }
    }
}
