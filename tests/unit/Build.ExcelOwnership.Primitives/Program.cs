using System.Reflection;
using System.Text.Json;
using Vmf.Build.ExcelOwnership.Primitives;

if (args.Length == 1 && args[0] == "--observe-excel-processes")
{
    Console.WriteLine(JsonSerializer.Serialize(ExcelProcessBaselineObserver.Observe()));
    return 0;
}

var failures = new List<string>();
var total = 0;
Run("ROT path acceptance is exact and local", () =>
{
    Assert(RotMatch(@"C:\temp\sentinel.xlsx", @"C:\temp\sentinel.xlsx"));
    Assert(RotMatch(@"c:\TEMP\SENTINEL.xlsx", @"C:\temp\sentinel.xlsx"));
    foreach (var value in new[] { "sentinel.xlsx", @"\\server\share\sentinel.xlsx", "file:///C:/temp/sentinel.xlsx", @"\\?\C:\temp\sentinel.xlsx", @"C:\temp\sentinel.xlsx!Workbook" })
        Assert(!RotMatch(value, @"C:\temp\sentinel.xlsx"));
});
Run("ROT source is non-activating and bounded", () =>
{
    var source = Source("src", "Build.ExcelOwnership.Primitives", "RotSentinelAttachment.cs");
    Has(source, "GetRunningObjectTable", "EnumRunning", "GetDisplayName", "rot.GetObject", "FromMilliseconds(100)", "FromSeconds(20)", "Stopwatch", "Task.Delay",
        "RotSentinelAttachmentResult", "ExactMatchCount", "runningObject ??", ", 1)", "RotAttachmentFailure.Ambiguous");
    Lacks(source, "BindToMoniker", "GetActiveObject", "Workbooks", "GetProcessesByName");
    Assert(source.IndexOf("if (match is not null)", StringComparison.Ordinal) < source.IndexOf("rot.GetObject", StringComparison.Ordinal));
});
Run("ownership does not require activation PID equality", () =>
{
    var activation = Activation(123);
    var comparison = ApplicationIdentityValidation.Compare(activation, 10, 456, activation.ProcessStartedUtc,
        activation.CanonicalExecutablePath, activation.CanonicalExecutablePath, Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Guid.Parse("11111111-1111-1111-1111-111111111111"), @"C:\temp\sentinel.xlsx", @"C:\temp\sentinel.xlsx", true, true, false);
    Assert(comparison.AggregateMatch && !comparison.ActivationPidMatchesApplicationPid);
});
Run("cleanup topology requires same stable identity and correlations", () =>
{
    var activation = Activation(123);
    var application = Application(123, activation.ProcessStartedUtc, activation.CanonicalExecutablePath);
    var activationObserved = Observation(123, activation.ProcessStartedUtc, activation.CanonicalExecutablePath);
    var applicationObserved = Observation(123, application.IndependentlyObservedStartTimeUtc, application.IndependentlyObservedCanonicalExecutablePath);
    Assert(CleanupTopologyValidation.Compare(activation, application, activationObserved, applicationObserved, true, true).AggregateMatch);
    Assert(!CleanupTopologyValidation.Compare(activation, application with { ProcessId = 456 }, activationObserved,
        applicationObserved with { ProcessId = 456 }, true, true).AggregateMatch);
    Assert(!CleanupTopologyValidation.Compare(activation, application, activationObserved with { ProcessStartedUtc = activation.ProcessStartedUtc.AddSeconds(1) }, applicationObserved, true, true).AggregateMatch);
    Assert(!CleanupTopologyValidation.Compare(activation, application, activationObserved with { CanonicalExecutablePath = @"C:\other\EXCEL.EXE" }, applicationObserved, true, true).AggregateMatch);
    Assert(!CleanupTopologyValidation.Compare(activation, application, activationObserved, applicationObserved, false, true).AggregateMatch);
    Assert(!CleanupTopologyValidation.Compare(activation, application, activationObserved, applicationObserved, true, false).AggregateMatch);
});
Run("cleanup topology DTO serializes immutable numeric identities", () =>
{
    var activation = Activation(123);
    var application = Application(123, activation.ProcessStartedUtc, activation.CanonicalExecutablePath);
    var validation = CleanupTopologyValidation.Compare(activation, application,
        Observation(123, activation.ProcessStartedUtc, activation.CanonicalExecutablePath),
        Observation(123, application.IndependentlyObservedStartTimeUtc, application.IndependentlyObservedCanonicalExecutablePath), true, true);
    using var json = JsonDocument.Parse(JsonSerializer.Serialize(validation));
    Assert(json.RootElement.GetProperty("ActivationProcessId").GetInt32() == 123);
    Assert(validation.AggregateMatch);
});
Run("lifecycle success requires both persistence gates", () =>
{
    using var tree = new Tree();
    var id = Guid.NewGuid();
    var journal = new LifecycleJournal(Path.Combine(tree.Path, "life.jsonl"), id);
    RecordThroughTopologyValidated(journal);
    journal.Record("ExcelOperationStarted"); journal.Record("ExcelCleanupStarted"); journal.Record("ExcelCleanupActionsCompleted");
    journal.Record("ExcelResidualChecked"); journal.Record("ExcelCleanupCompleted"); journal.Record("InvocationSucceeded");
    Assert(LifecycleJournal.Validate(Path.Combine(tree.Path, "life.jsonl"), id).Last().State == "InvocationSucceeded");
});
Run("topology rejection cannot reach cleanup", () =>
{
    using var tree = new Tree();
    var journal = new LifecycleJournal(Path.Combine(tree.Path, "life.jsonl"), Guid.NewGuid());
    RecordThroughOwnershipValidated(journal); journal.Record("CleanupTopologyObserved"); journal.Record("CleanupTopologyRejected");
    Throws<InvalidDataException>(() => journal.Record("ExcelCleanupStarted"));
    Throws<InvalidDataException>(() => journal.Record("ExcelOperationStarted"));
    journal.Record("ExcelCleanupSkippedUnsafeTopology"); journal.Record("ExcelResidualChecked"); journal.Record("InvocationFailed");
});
Run("cleanup cannot precede ownership or topology persistence", () =>
{
    foreach (var stop in new[] { "ApplicationProcessIdentityObserved", "OwnershipEvidencePersisted", "OwnershipPersistenceConfirmed", "CleanupTopologyObserved", "CleanupTopologyEvidencePersisted", "CleanupTopologyPersistenceConfirmed" })
    {
        using var tree = new Tree();
        var journal = new LifecycleJournal(Path.Combine(tree.Path, stop + ".jsonl"), Guid.NewGuid());
        RecordUntil(journal, stop);
        Throws<InvalidDataException>(() => journal.Record("ExcelCleanupStarted"));
    }
});
Run("facade changes are additive and obsolete assertion fails closed", () =>
{
    var lease = typeof(ExcelOwnershipLease);
    Assert(lease.GetProperty("CleanupTopologyValidation")?.PropertyType == typeof(CleanupTopologyValidation));
    Assert(lease.GetProperty("CleanupTopologyValidated")?.PropertyType == typeof(bool));
    Assert(lease.GetMethod("ConfirmOwnershipEvidencePersisted", [typeof(EvidencePersistenceConfirmation)]) is not null);
    Assert(lease.GetMethod("ObserveCleanupTopology", Type.EmptyTypes)?.ReturnType == typeof(CleanupTopologyValidation));
    Assert(lease.GetMethod("ConfirmCleanupTopologyEvidencePersisted", [typeof(EvidencePersistenceConfirmation)]) is not null);
    Assert(lease.GetMethod("ConfirmIdentityEvidencePersisted", Type.EmptyTypes)?.GetCustomAttribute<ObsoleteAttribute>() is not null);
});
Run("session cleanup gate contains both durable confirmations", () =>
{
    var source = Source("src", "Build.ExcelOwnership.Primitives", "ExcelOwnershipSession.cs");
    Has(source, "ownershipEvidencePersisted", "CleanupTopologyValidated", "cleanupTopologyEvidencePersisted", "book?.Close(false)", "excel?.Quit()", "ExcelCleanupSkippedUnsafeTopology");
    Assert(source.IndexOf("var cleanupPermitted", StringComparison.Ordinal) < source.IndexOf("book?.Close(false)", StringComparison.Ordinal));
    Lacks(source, "Process.Kill", ".Kill(", "Stop-Process", "taskkill");
});
Run("process exits on first residual observation", () =>
{
    var activation = Activation(123);
    var result = ObserveResiduals(activation, null, _ => new(false, null, true));
    Assert(result.AttemptCount == 1 && result.Targets.Single().FinalResidualState == ResidualState.Clear);
});
Run("process exits during residual grace", () =>
{
    var activation = Activation(123);
    var probes = 0;
    var result = ObserveResiduals(activation, null, _ => ++probes == 1
        ? new(true, activation.ProcessStartedUtc, true)
        : new(false, null, true));
    Assert(probes == 2 && result.Targets.Single().FinalResidualState == ResidualState.Clear);
});
Run("owned identity surviving full logical grace is residual", () =>
{
    var activation = Activation(123);
    var result = ObserveResiduals(activation, null, _ => new(true, activation.ProcessStartedUtc, true));
    Assert(result.ObservationCompletedUtc - result.ObservationStartedUtc == TimeSpan.FromSeconds(20));
    Assert(result.PollingIntervalMilliseconds == 100 && result.MaximumGraceMilliseconds == 20000);
    Assert(result.Targets.Single().FinalResidualState == ResidualState.Residual);
});
Run("residual observations revalidate start time", () =>
{
    var activation = Activation(123);
    var probes = 0;
    var result = ObserveResiduals(activation, null, _ => ++probes == 1
        ? new(true, activation.ProcessStartedUtc, true)
        : new(true, activation.ProcessStartedUtc.AddSeconds(1), true));
    Assert(probes == 2);
    Assert(result.Targets.Single().FinalIdentityValidationOutcome == ResidualIdentityValidationOutcome.StartTimeMismatch);
    Assert(result.Targets.Single().FinalResidualState == ResidualState.CheckFailed);
});
Run("same PID topology probes one physical identity per poll", () =>
{
    var activation = Activation(123);
    var application = Application(123, activation.ProcessStartedUtc, activation.CanonicalExecutablePath);
    var probes = 0;
    var result = ObserveResiduals(activation, application, _ => ++probes == 1
        ? new(true, activation.ProcessStartedUtc, true)
        : new(false, null, true));
    Assert(probes == 2 && result.SharedPhysicalProcess && result.Targets.Count == 2);
    Assert(result.Targets.All(target => target.FinalResidualState == ResidualState.Clear));
});
Run("residual observation and cleanup introduce no process manipulation", () =>
{
    var observer = Source("src", "Build.ExcelOwnership.Primitives", "ResidualProcessObserver.cs");
    var session = Source("src", "Build.ExcelOwnership.Primitives", "ExcelOwnershipSession.cs");
    var baseline = Source("tests", "unit", "Build.ExcelOwnership.Primitives", "ExcelProcessBaselineObserver.cs");
    Lacks(observer, ".Kill(", "CloseMainWindow", "Stop-Process", "taskkill");
    Lacks(session, ".Kill(", "CloseMainWindow", "Stop-Process", "taskkill");
    Lacks(baseline, ".Kill(", "CloseMainWindow", "Stop-Process", "taskkill");
    Has(baseline, "Process.GetProcessesByName(\"EXCEL\")");
    Has(session, "CleanupTimingTelemetry = new CleanupTimingTelemetry(closeStartedUtc, closeCompletedUtc, quitStartedUtc, quitCompletedUtc)",
        "ResidualObservationTelemetry");
});

foreach (var failure in failures) Console.Error.WriteLine(failure);
Console.WriteLine($"Tests: {total - failures.Count} passed, {failures.Count} failed");
return failures.Count == 0 ? 0 : 1;

void Run(string name, Action action) { total++; try { action(); } catch (Exception exception) { failures.Add($"FAIL {name}: {exception.GetType().Name}: {exception.Message}"); } }
void Assert(bool value) { if (!value) throw new Exception("assertion failed"); }
void Throws<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new Exception($"expected {typeof(T).Name}"); }
void Has(string source, params string[] values) { foreach (var value in values) Assert(source.Contains(value, StringComparison.Ordinal)); }
void Lacks(string source, params string[] values) { foreach (var value in values) Assert(!source.Contains(value, StringComparison.OrdinalIgnoreCase)); }
string Source(params string[] parts) => File.ReadAllText(Path.Combine(Repo(), Path.Combine(parts)));
string Repo() => Path.GetFullPath(Environment.GetEnvironmentVariable("VMF_TEST_REPOSITORY_ROOT") ?? Path.Combine(AppContext.BaseDirectory, "../../../../../.."));
bool RotMatch(string candidate, string expected)
{
    var type = typeof(Ownership).Assembly.GetType("Vmf.Build.ExcelOwnership.Primitives.RotSentinelAttachment")!;
    return (bool)type.GetMethod("IsExactSentinelDisplayName", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [candidate, expected])!;
}
void RecordUntil(LifecycleJournal journal, string stop)
{
    var states = new[] { "InvocationStarted", "ExcelActivationAboutToBegin", "ActivationProcessIdentityObserved", "RotAttachmentWaiting", "RotAttachmentObserved", "ApplicationProcessIdentityObserved", "OwnershipEvidencePersisted", "OwnershipPersistenceConfirmed", "ApplicationProcessIdentityValidated", "CleanupTopologyObserved", "CleanupTopologyEvidencePersisted", "CleanupTopologyPersistenceConfirmed" };
    foreach (var state in states) { journal.Record(state); if (state == stop) return; }
}
void RecordThroughOwnershipValidated(LifecycleJournal journal) => RecordUntil(journal, "ApplicationProcessIdentityValidated");
void RecordThroughTopologyValidated(LifecycleJournal journal) { RecordUntil(journal, "CleanupTopologyPersistenceConfirmed"); journal.Record("CleanupTopologyValidated"); }
ActivationProcessIdentity Activation(int pid) => new(pid, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), @"C:\Program Files\Microsoft Office\EXCEL.EXE", DateTimeOffset.UtcNow.AddSeconds(-2), DateTimeOffset.UtcNow.AddSeconds(-1), "Process.Start", "/x sentinel", "returned process", DateTimeOffset.UtcNow, false, ResidualState.NotApplicable);
ApplicationProcessIdentity Application(int pid, DateTimeOffset started, string executable) => new((nint)12345, pid, started, executable, DateTimeOffset.UtcNow.AddSeconds(-2), DateTimeOffset.UtcNow.AddSeconds(-1), "GetWindowThreadProcessId(Application.Hwnd)", "Process.StartTime", "Process.MainModule.FileName", executable, @"C:\temp\sentinel.xlsx", @"C:\temp\sentinel.xlsx", Guid.Parse("11111111-1111-1111-1111-111111111111"), Guid.Parse("11111111-1111-1111-1111-111111111111"), ApplicationIdentityValidationState.Validated, false, ResidualState.NotApplicable);
ProcessIdentityObservation Observation(int pid, DateTimeOffset started, string executable) => new(pid, started, executable, DateTimeOffset.UtcNow.AddSeconds(-2), DateTimeOffset.UtcNow.AddSeconds(-1), "StartTime", "Executable", true, false);
ResidualObservationTelemetry ObserveResiduals(ActivationProcessIdentity activation, ApplicationProcessIdentity? application,
    Func<int, ResidualProcessProbe> probe)
{
    var elapsed = TimeSpan.Zero;
    var utc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    return ResidualProcessObserver.Observe(activation, application, probe, () => elapsed,
        delay => { elapsed += delay; utc += delay; }, () => utc);
}
sealed class Tree : IDisposable { public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "vmf-ownership-tests", Guid.NewGuid().ToString("N")); public Tree() => Directory.CreateDirectory(Path); public void Dispose() { if (Directory.Exists(Path)) Directory.Delete(Path, true); } }
