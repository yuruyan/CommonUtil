namespace CommonUtil.Model;

public interface IGenerable<out T> {
    public T Generate();
}

public interface IGenerable<In, out Out> {
    public Out Generate(In arg);
}
