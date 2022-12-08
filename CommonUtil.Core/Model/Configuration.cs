using Newtonsoft.Json;

namespace CommonUtil.Core.Model;

public enum ApplicationEnvironment {
    [JsonProperty("production")]
    Production,
    [JsonProperty("development")]
    Development
}
