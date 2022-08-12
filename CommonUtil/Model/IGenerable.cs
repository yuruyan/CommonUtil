using System.Collections.Generic;

namespace CommonUtil.Model;

internal interface IGenerable<out T> {
    public IEnumerable<T> Generate();
}

internal interface IGenerable<In, out Out> {
    public IEnumerable<Out> Generate(In arg);
}
