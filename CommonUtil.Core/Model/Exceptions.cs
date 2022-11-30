using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil.Core;

/// <summary>
/// 模式解析失败
/// </summary>
public class PatternParseException : ArgumentException {
    public PatternParseException() {
    }

    public PatternParseException(string? message) : base(message) {
    }

    public PatternParseException(string? message, Exception? innerException) : base(message, innerException) {
    }

    public PatternParseException(string? message, string? paramName) : base(message, paramName) {
    }

    public PatternParseException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException) {
    }

    protected PatternParseException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}
