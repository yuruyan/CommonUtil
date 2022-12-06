namespace CommonUtil.Model;

public delegate void CommonFileProcess(
    string inputFile,
    string outputFile,
    CancellationToken? token,
    Action<double>? callback
);

public delegate T CommonFileProcess<T>(
    string inputFile,
    string outputFile,
    CancellationToken? token,
    Action<double>? callback
);
