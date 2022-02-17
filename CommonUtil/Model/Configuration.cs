using Newtonsoft.Json;

namespace CommonUtil.Model;

public enum Environment {
    [JsonProperty("production")]
    Production,
    [JsonProperty("development")]
    Development
}
