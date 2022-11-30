using Newtonsoft.Json;

namespace CommonUtil.Core.Model;

public enum Environment {
    [JsonProperty("production")]
    Production,
    [JsonProperty("development")]
    Development
}
