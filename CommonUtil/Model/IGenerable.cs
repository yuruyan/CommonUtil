using System.Collections.Generic;

namespace CommonUtil.Model;

internal interface IGenerable<out T> {
    public IEnumerable<T> Generate();
}
