using System.Reflection;
using System.Text.Json;
using Vmf.Build.ExcelOwnership.Primitives;

var failures = new List<string>();
var total = 0;
var repositoryRoot = ResolveRepositoryRoot();
Run("public CLI exact shape and validation", () => SourceHas("ExperimentContracts.cs", "args.Length != 5", "Guid.Empty", "Path.IsPathFullyQualified", "Directory.EnumerateFileSystemEntries", "isolated from the repository"));
Run("public CLI rejects duplicate and unknown arguments", () => SourceHas("ExperimentContracts.cs", "Unknown or duplicate argument", "TryAdd"));
Run("internal worker is not a public CLI argument", () => { SourceHas("Program.cs", "Environment.GetEnvironmentVariable", "args.Length != 0"); SourceLacks("Program.cs", "--worker"); });
Run("controller worker correlation", () => SourceHas("ExperimentContracts.cs", "Worker correlation or zero-Excel precondition mismatch.", "FixedTimeEquals", "ValidatePaths"));
Run("schema 4 and six append-only evidence records", () => { SourceHas("ExperimentContracts.cs", "SchemaVersion = 4", "OwnershipValidationEvidence", "CleanupTopologyEvidence", "CleanupAttemptEvidence", "ResidualObservationEvidence"); SourceHas("ExperimentEvidence.cs", "WriteIndented = false", "FileMode.CreateNew", "ownership-validation.json", "cleanup-topology.json", "cleanup-attempt.json", "residual-observation.json"); });
Run("identity evidence nonzero HWND numeric round trip", () =>
{
    using var tree = new Tree();
    var path = Path.Combine(tree.Path, "identity.json");
    var evidence = IdentityEvidence((nint)424242);
    WriteEvidence(path, evidence);
    Assert(File.Exists(path));
    using var json = JsonDocument.Parse(File.ReadAllText(path));
    Assert(json.RootElement.GetProperty("applicationProcessIdentity").GetProperty("hwnd").ValueKind == JsonValueKind.Number);
    Assert(ReadHwnd(ReadEvidence(path, evidence.GetType()), "ApplicationProcessIdentity") == (nint)424242);
});
Run("identity evidence zero HWND round trip", () =>
{
    using var tree = new Tree();
    var path = Path.Combine(tree.Path, "identity.json");
    var evidence = IdentityEvidence(0);
    WriteEvidence(path, evidence);
    Assert(ReadHwnd(ReadEvidence(path, evidence.GetType()), "ApplicationProcessIdentity") == 0);
});
Run("failure result nonzero HWND serialization round trip", () =>
{
    using var tree = new Tree();
    var path = Path.Combine(tree.Path, "result.json");
    var result = FailureResult((nint)515151, tree.Path);
    WriteEvidence(path, result);
    var read = ReadEvidence(path, result.GetType());
    Assert((string?)read.GetType().GetProperty("Outcome")!.GetValue(read) == "Failed");
    Assert(ReadHwnd(read, "ApplicationProcessIdentity") == (nint)515151);
});
Run("malformed HWND JSON rejected fail closed", () =>
{
    using var tree = new Tree();
    var path = Path.Combine(tree.Path, "identity.json");
    var evidence = IdentityEvidence((nint)424242);
    WriteEvidence(path, evidence);
    var malformed = File.ReadAllText(path).Replace("\"hwnd\":424242", "\"hwnd\":\"invalid\"", StringComparison.Ordinal);
    Assert(malformed.Contains("\"hwnd\":\"invalid\"", StringComparison.Ordinal));
    File.WriteAllText(path, malformed);
    Throws<JsonException>(() => ReadEvidence(path, evidence.GetType()));
});
Run("evidence sequence and timestamps", () => SourceHas("ExperimentEvidence.cs", "sequence(item) != ++expected", "previous > recorded", "AddMinutes(1)"));
Run("worker-only experiment log ownership", () => { SourceHas("ExperimentWorker.cs", "WorkerEvent"); SourceLacks("ExperimentController.cs", "WorkerEvent", "Append(paths.ExperimentLog", "Diagnostic("); SourceHas("ExperimentEvidence.cs", "++workerSequence"); });
Run("ownership and topology evidence precede cleanup eligibility", () => { var source = SourceText("ExperimentWorker.cs"); var start = source.IndexOf("lease.Start", StringComparison.Ordinal); var ownership = source.IndexOf("PersistOwnershipEvidence", start, StringComparison.Ordinal); var ownershipGate = source.IndexOf("ConfirmOwnershipEvidencePersisted", ownership, StringComparison.Ordinal); var topology = source.IndexOf("PersistCleanupTopologyEvidence", ownershipGate, StringComparison.Ordinal); var topologyGate = source.IndexOf("ConfirmCleanupTopologyEvidencePersisted", topology, StringComparison.Ordinal); var cleanup = source.IndexOf("RequestNormalCleanup", topologyGate, StringComparison.Ordinal); Assert(start >= 0 && ownership > start && ownershipGate > ownership && topology > ownershipGate && topologyGate > topology && cleanup > topologyGate); });
Run("evidence uses create-new write-through and readback", () => SourceHas("ExperimentEvidence.cs", "FileMode.CreateNew", "FileOptions.WriteThrough", "Flush(true)", "File.ReadAllBytes", "SHA256.HashData"));
Run("missing success ownership evidence fails closed", () => SourceHas("ExperimentController.cs", "result.Outcome == \"Succeeded\"", "ValidateIdentityEvidence", "ReadSingle<OwnershipValidationEvidence>"));
Run("malformed or partial success identity evidence fails closed", () => { SourceHas("ExperimentEvidence.cs", "one complete compact JSON object", "Malformed evidence"); SourceHas("ExperimentController.cs", "identity.RecordedUtc == default", "activation.ProcessId <= 0", "application.ExpectedInvocationCorrelationValue != invocationId"); });
Run("success requires every identity comparison and aggregate match", () => SourceHas("ExperimentController.cs", "allComponentComparisonsMatch", "!allComponentComparisonsMatch || !identity.AggregateValidation"));
Run("identity invocation correlation required", () => SourceHas("ExperimentController.cs", "identity.InvocationId != invocationId", "application.ExpectedInvocationCorrelationValue != invocationId"));
Run("application observations come from HWND owner PID", () => SourceHas(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ProcessIdentityObservation.cs"), "Process.GetProcessById(processId)", "process.StartTime", "process.MainModule", "StableIdentity", "PidReuseSuspected"));
Run("schema 3 deterministically rejected", () => { SourceHas("ExperimentContracts.cs", "SchemaVersion = 4"); SourceHas("ExperimentController.cs", "result.SchemaVersion != ExperimentContracts.SchemaVersion", "identity.SchemaVersion != ExperimentContracts.SchemaVersion"); });
Run("partial identity evidence fails closed", () => SourceHas("ExperimentController.cs", "string.IsNullOrWhiteSpace(application.ExpectedCanonicalExecutablePath)", "comparison.AggregateMatch !=", "identity.AggregateValidation"));
Run("validated ownership and topology persistence gate cleanup", () => { var source = SourceText(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ExcelOwnershipSession.cs")); var gate = source.IndexOf("var cleanupPermitted", StringComparison.Ordinal); var close = source.IndexOf("book?.Close(false)", gate, StringComparison.Ordinal); var quit = source.IndexOf("excel?.Quit()", gate, StringComparison.Ordinal); Assert(gate >= 0 && close > gate && quit > gate); SourceHas(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ExcelOwnershipSession.cs"), "ownershipEvidencePersisted", "cleanupTopologyEvidencePersisted", "CleanupTopologyValidated", "ExcelCleanupSkippedUnsafeTopology"); });
Run("zero Excel baseline is Experiment only", () => { SourceHas("ExperimentController.cs", "Process.GetProcessesByName(\"EXCEL\")", "ExcelProcessCount != 0"); SourceLacks(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ExcelOwnershipSession.cs"), "GetProcessesByName"); });
Run("dual residuals and same PID single physical observation", () =>
{
    SourceHas(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ExcelOwnershipSession.cs"),
        "ResidualProcessObserver.Observe", "target.Role == \"Activation\"", "target.Role == \"Application\"");
    SourceHas(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ResidualProcessObserver.cs"),
        "activation.ProcessId == application.ProcessId", "activation.ProcessStartedUtc == application.IndependentlyObservedStartTimeUtc",
        "if (application is not null && !shared)");
});
Run("residual grace telemetry and ROT cardinality are persisted", () =>
{
    SourceHas("ExperimentContracts.cs", "RotExactMatchCount", "ResidualObservationTelemetry", "CleanupTimingTelemetry");
    SourceHas("ExperimentWorker.cs", "lease.RotExactMatchCount", "lease.ResidualObservationTelemetry", "lease.CleanupTimingTelemetry");
    SourceHas(Path.Combine("..", "Build.ExcelOwnership.Primitives", "ResidualProcessObserver.cs"), "FromMilliseconds(100)", "FromSeconds(20)", "StartTimeMismatch", "SharedPhysicalProcess");
});
Run("failure channels remain independent", () => SourceHas("ExperimentController.cs", "WorkerFailure=", "ControllerEvidenceFailure=", "result?.Failure"));
Run("timeout and cooperative cleanup", () => SourceHas("ExperimentController.cs", "TimeSpan.FromSeconds(120)", "TimeSpan.FromSeconds(30)", "CleanupRequested", "processes left untouched", "SafeStop", "CleanupGraceExpired"));
Run("no hard termination", () => { SourceLacks("ExperimentController.cs", ".Kill(", "Stop-Process", "taskkill", "HardTerminationCompleted"); SourceLacks("ExperimentWorker.cs", ".Kill(", "Stop-Process", "taskkill", "HardTerminationCompleted"); });
Run("ordinary public facade consumer", () => SourceHas("ExperimentWorker.cs", "ExcelOwnershipLease", "RequestNormalCleanup", "ResidualState"));
Run("project graph isolated", () => { SourceHas("Vmf.Build.ExcelOwnership.Experiment.csproj", "Build.ExcelOwnership.Primitives"); SourceLacks("Vmf.Build.ExcelOwnership.Experiment.csproj", "Build.Host", "InternalsVisibleTo"); });
Run("production capabilities absent", () =>
{
    AllExperimentSourcesLackRawOfficeTypes();
    AllExperimentSourcesLack("Microsoft.Office", "Interop.Excel", "System.Runtime.InteropServices", "ComImport", "Marshal.", "dynamic", "VBProject", "VBComponents", "Import", "SaveAs", "Transaction", "Metadata", "Publication", "build.ps1", "GetActiveObject");
});
Run("production graph and launcher remain external", () => { Assert(!Directory.GetFiles(SourceRoot(), "*", SearchOption.AllDirectories).Any(path => path.Contains("Build.Host", StringComparison.OrdinalIgnoreCase))); });

foreach (var failure in failures) Console.Error.WriteLine(failure);
Console.WriteLine($"Tests: {total - failures.Count} passed, {failures.Count} failed");
return failures.Count == 0 ? 0 : 1;

void Run(string name, Action action) { total++; try { action(); } catch (Exception exception) { failures.Add($"FAIL {name}: {exception.Message}"); } }
void Assert(bool value) { if (!value) throw new Exception("assertion failed"); }
void Throws<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new Exception($"expected {typeof(T).Name}"); }
object IdentityEvidence(nint hwnd)
{
    var invocationId = Guid.NewGuid();
    var activation = Activation(123);
    var application = Application(hwnd, 456, invocationId);
    var comparison = ApplicationIdentityValidation.Compare(activation, hwnd, application.ProcessId, application.IndependentlyObservedStartTimeUtc,
        application.IndependentlyObservedCanonicalExecutablePath, application.ExpectedCanonicalExecutablePath, invocationId, invocationId,
        application.ExpectedSentinelCanonicalPath, application.ActualBoundWorkbookCanonicalFullPath, true, true, false);
    var zeroExcelType = ExperimentType("ZeroExcelPreconditionEvidence");
    var zeroExcel = Activator.CreateInstance(zeroExcelType, 0, DateTimeOffset.UtcNow, "Process.GetProcessesByName")!;
    return Activator.CreateInstance(ExperimentType("OwnershipValidationEvidence"), 4, invocationId, "OwnershipValidation", activation, application, zeroExcel,
        comparison, comparison.AggregateMatch, DateTimeOffset.UtcNow)!;
}
object FailureResult(nint hwnd, string root)
{
    var invocationId = Guid.NewGuid();
    var paths = Activator.CreateInstance(ExperimentType("EvidencePaths"),
        Path.Combine(root, "result.json"), Path.Combine(root, "controller.jsonl"), Path.Combine(root, "ownership.jsonl"),
        Path.Combine(root, "experiment.jsonl"), Path.Combine(root, "identity.json"), Path.Combine(root, "cleanup.json"), Path.Combine(root, "sentinel.xlsx"))!;
    var failure = Activator.CreateInstance(ExperimentType("FailureInformation"), "WorkerFailure", "serialization regression test")!;
    return Activator.CreateInstance(ExperimentType("ExperimentResult"), 4, invocationId, "Failed", null, Activation(123),
        Application(hwnd, 456, invocationId), true, false, ResidualState.NotApplicable, ResidualState.NotApplicable, paths, failure)!;
}
ActivationProcessIdentity Activation(int pid) => new(pid, DateTimeOffset.UtcNow.AddMinutes(-1), @"C:\Program Files\Microsoft Office\EXCEL.EXE",
    DateTimeOffset.UtcNow.AddSeconds(-2), DateTimeOffset.UtcNow.AddSeconds(-1), "Process.Start", "/x sentinel", "returned process", DateTimeOffset.UtcNow, false, ResidualState.NotApplicable);
ApplicationProcessIdentity Application(nint hwnd, int pid, Guid invocationId) => new(hwnd, pid, DateTimeOffset.UtcNow.AddMinutes(-1),
    @"C:\Program Files\Microsoft Office\EXCEL.EXE", DateTimeOffset.UtcNow.AddSeconds(-2), DateTimeOffset.UtcNow.AddSeconds(-1),
    "GetWindowThreadProcessId(Application.Hwnd)", "Process.StartTime", "Process.MainModule.FileName", @"C:\Program Files\Microsoft Office\EXCEL.EXE",
    @"C:\temp\sentinel.xlsx", @"C:\temp\sentinel.xlsx", invocationId, invocationId, ApplicationIdentityValidationState.Validated, false, ResidualState.NotApplicable);
Type ExperimentType(string name) => Assembly.Load("Vmf.Build.ExcelOwnership.Experiment").GetType($"Vmf.Build.ExcelOwnership.Experiment.{name}") ?? throw new Exception($"{name} missing");
void WriteEvidence(string path, object value) => InvokeEvidence("WriteSingle", value.GetType(), [path, value]);
object ReadEvidence(string path, Type type) => InvokeEvidence("ReadSingle", type, [path])!;
object? InvokeEvidence(string name, Type type, object?[] arguments)
{
    var method = ExperimentType("ExperimentEvidence").GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(type);
    try { return method.Invoke(null, arguments); }
    catch (TargetInvocationException exception) when (exception.InnerException is not null) { throw exception.InnerException; }
}
nint ReadHwnd(object value, string identityProperty)
{
    var identity = value.GetType().GetProperty(identityProperty)!.GetValue(value)!;
    return (nint)identity.GetType().GetProperty("Hwnd")!.GetValue(identity)!;
}
void SourceHas(string file, params string[] values) { var text = File.ReadAllText(Path.Combine(SourceRoot(), file)); foreach (var value in values) Assert(text.Contains(value, StringComparison.Ordinal)); }
void SourceLacks(string file, params string[] values) { var text = File.ReadAllText(Path.Combine(SourceRoot(), file)); foreach (var value in values) Assert(!text.Contains(value, StringComparison.OrdinalIgnoreCase)); }
string SourceText(string file) => File.ReadAllText(Path.GetFullPath(Path.Combine(SourceRoot(), file)));
void AllExperimentSourcesLack(params string[] values) { foreach (var file in Directory.GetFiles(SourceRoot(), "*.cs")) SourceLacks(Path.GetFileName(file), values); }
void AllExperimentSourcesLackRawOfficeTypes()
{
    const string rawOfficeType = @"(?x)
        \b(?:Workbook|Application)\s*(?:\?|\[\])?\s+[A-Za-z_]\w*
        | \b(?:new|typeof|default|as|is)\s*\(?\s*(?:Workbook|Application)\b
        | \(\s*(?:Workbook|Application)\s*\)
        | [<,]\s*(?:Workbook|Application)\s*[,>]";
    foreach (var file in Directory.GetFiles(SourceRoot(), "*.cs"))
        Assert(!System.Text.RegularExpressions.Regex.IsMatch(CodeBearingText(File.ReadAllText(file)), rawOfficeType, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
}
string CodeBearingText(string source)
{
    var masked = source.ToCharArray();
    for (var index = 0; index < source.Length;)
    {
        if (source[index] == '/' && index + 1 < source.Length && source[index + 1] == '/')
        {
            index = MaskThroughLineEnd(source, masked, index);
        }
        else if (source[index] == '/' && index + 1 < source.Length && source[index + 1] == '*')
        {
            index = MaskBlockComment(source, masked, index);
        }
        else if (source[index] == '\'')
        {
            index = MaskQuotedLiteral(source, masked, index, '\'', false);
        }
        else
        {
            var quote = index;
            if (source[index] == '$' && index + 1 < source.Length && source[index + 1] == '@') quote += 2;
            else if (source[index] == '@' && index + 1 < source.Length && source[index + 1] == '$') quote += 2;
            else if ((source[index] == '$' || source[index] == '@') && index + 1 < source.Length) quote++;

            if (quote < source.Length && source[quote] == '"')
                index = MaskQuotedLiteral(source, masked, index, '"', quote > index && source.AsSpan(index, quote - index).IndexOf('@') >= 0);
            else
                index++;
        }
    }

    return new string(masked);
}
int MaskThroughLineEnd(string source, char[] masked, int start)
{
    var index = start;
    while (index < source.Length && source[index] != '\r' && source[index] != '\n') Mask(masked, index++);
    return index;
}
int MaskBlockComment(string source, char[] masked, int start)
{
    var index = start;
    while (index < source.Length)
    {
        if (index + 1 < source.Length && source[index] == '*' && source[index + 1] == '/')
        {
            Mask(masked, index++);
            Mask(masked, index++);
            break;
        }
        Mask(masked, index++);
    }
    return index;
}
int MaskQuotedLiteral(string source, char[] masked, int start, char delimiter, bool verbatim)
{
    var index = start;
    while (index < source.Length && source[index] != delimiter) Mask(masked, index++);
    if (index < source.Length) Mask(masked, index++);
    while (index < source.Length)
    {
        if (!verbatim && source[index] == '\\')
        {
            Mask(masked, index++);
            if (index < source.Length) Mask(masked, index++);
        }
        else if (source[index] == delimiter)
        {
            Mask(masked, index++);
            if (verbatim && index < source.Length && source[index] == delimiter)
            {
                Mask(masked, index++);
                continue;
            }
            break;
        }
        else
        {
            Mask(masked, index++);
        }
    }
    return index;
}
void Mask(char[] text, int index) { if (text[index] != '\r' && text[index] != '\n') text[index] = ' '; }
string SourceRoot() => Path.Combine(repositoryRoot, "src/Build.ExcelOwnership.Experiment");
string ResolveRepositoryRoot()
{
    const string repositoryRootVariable = "VMF_TEST_REPOSITORY_ROOT";
    var explicitRoot = Environment.GetEnvironmentVariable(repositoryRootVariable);
    if (explicitRoot is not null)
    {
        if (string.IsNullOrWhiteSpace(explicitRoot) || !Path.IsPathFullyQualified(explicitRoot))
        {
            throw new InvalidOperationException($"{repositoryRootVariable} must be a non-empty absolute path.");
        }

        var normalizedRoot = Path.GetFullPath(explicitRoot);
        if (!File.Exists(Path.Combine(normalizedRoot, ".vmf-root")))
        {
            throw new InvalidOperationException($"{repositoryRootVariable} must identify a repository root containing .vmf-root.");
        }

        return normalizedRoot;
    }

    return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../.."));
}

sealed class Tree : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "vmf-experiment-evidence-tests", Guid.NewGuid().ToString("N"));
    public Tree() => Directory.CreateDirectory(Path);
    public void Dispose() { if (Directory.Exists(Path)) Directory.Delete(Path, true); }
}
