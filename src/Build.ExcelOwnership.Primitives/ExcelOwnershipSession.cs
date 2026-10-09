using Microsoft.Win32;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace Vmf.Build.ExcelOwnership.Primitives;

internal sealed class ExcelOwnershipSession : IDisposable
{
    private dynamic? excel;
    private dynamic? book;
    private Process? activationProcess;
    private readonly LifecycleJournal journal;
    private readonly Guid invocationId;
    private bool cleanupAttempted;
    private bool ownershipEvidencePersisted;
    private bool cleanupTopologyEvidencePersisted;

    public Ownership? Identity { get; private set; }
    public ActivationProcessIdentity? ActivationProcessIdentity { get; private set; }
    public ApplicationProcessIdentity? ApplicationProcessIdentity { get; private set; }
    public ApplicationIdentityValidation? ApplicationIdentityValidation { get; private set; }
    public CleanupTopologyValidation? CleanupTopologyValidation { get; private set; }
    public ProcessIdentityObservation? ActivationProcessReobservation { get; private set; }
    public ProcessIdentityObservation? ApplicationProcessReobservation { get; private set; }
    public bool CleanupTopologyValidated => CleanupTopologyValidation?.AggregateMatch == true && cleanupTopologyEvidencePersisted;
    public bool AttachmentValidated => ApplicationProcessIdentity?.ValidationState == ApplicationIdentityValidationState.Validated;
    public bool CleanupEligible => ApplicationProcessIdentity?.CleanupEligible == true;
    public ResidualState ActivationProcessResidualState { get; private set; } = ResidualState.NotApplicable;
    public ResidualState ApplicationProcessResidualState { get; private set; } = ResidualState.NotApplicable;
    public bool Clean { get; private set; }
    public int? RotExactMatchCount { get; private set; }
    public ResidualObservationTelemetry? ResidualObservationTelemetry { get; private set; }
    public CleanupTimingTelemetry CleanupTimingTelemetry { get; private set; } = new(null, null, null, null);
    internal dynamic? Workbook => book;

    internal ExcelOwnershipSession(LifecycleJournal journal, Guid invocationId)
    {
        this.journal = journal;
        this.invocationId = invocationId;
    }

    internal void Start(string sentinelPath)
    {
        journal.Record("ExcelActivationAboutToBegin");
        var executablePath = ProcessIdentity.Canonical(ResolveExcel());
        var canonicalSentinelPath = Path.GetFullPath(sentinelPath);
        CreateSentinel(canonicalSentinelPath);
        var arguments = $"/x \"{canonicalSentinelPath}\"";
        var launchRequestedUtc = DateTimeOffset.UtcNow;
        activationProcess = Process.Start(new ProcessStartInfo(executablePath, arguments) { UseShellExecute = false }) ?? throw new InvalidOperationException("Excel launch failed.");
        var launchReturnedUtc = DateTimeOffset.UtcNow;
        activationProcess.Refresh();
        var activationStartedUtc = new DateTimeOffset(activationProcess.StartTime.ToUniversalTime(), TimeSpan.Zero);
        ActivationProcessIdentity = new ActivationProcessIdentity(activationProcess.Id, activationStartedUtc, executablePath,
            launchRequestedUtc, launchReturnedUtc, "Process.Start(ProcessStartInfo)", arguments,
            "PID/start/executable observed from Process returned by Process.Start", launchReturnedUtc, false, ResidualState.NotApplicable);
        Identity = new Ownership(activationProcess.Id, activationStartedUtc, executablePath, invocationId, canonicalSentinelPath);
        journal.Record("ActivationProcessIdentityObserved", Identity);

        try
        {
            journal.Record("RotAttachmentWaiting", Identity);
            try
            {
                var attachment = RotSentinelAttachment.Attach(canonicalSentinelPath);
                book = attachment.RunningObject;
                RotExactMatchCount = attachment.ExactMatchCount;
                journal.Record("RotAttachmentObserved", Identity);
            }
            catch (RotAttachmentException exception)
            {
                journal.Record(exception.Failure == RotAttachmentFailure.Ambiguous ? "RotAttachmentAmbiguous" : "RotAttachmentTimedOut", Identity);
                throw;
            }

            // Before validation, these are the only permitted bootstrap COM observations.
            var actualSentinelPath = (string)book.FullName;
            var actualInvocationId = ReadInvocationCorrelation(book);
            excel = book.Application;
            var hwnd = (nint)excel.Hwnd;
            GetWindowThreadProcessId(hwnd, out uint hwndOwnerPid);

            // Start time and executable are independently acquired from the HWND-owner PID.
            var observed = ProcessIdentityObserver.Observe(checked((int)hwndOwnerPid));
            ApplicationIdentityValidation = global::Vmf.Build.ExcelOwnership.Primitives.ApplicationIdentityValidation.Compare(
                ActivationProcessIdentity, hwnd, observed.ProcessId, observed.ProcessStartedUtc, observed.CanonicalExecutablePath,
                executablePath, invocationId, actualInvocationId, canonicalSentinelPath, actualSentinelPath,
                true, observed.StableIdentity, observed.PidReuseSuspected);
            var state = ApplicationIdentityValidation.AggregateMatch ? ApplicationIdentityValidationState.Validated : ApplicationIdentityValidationState.Rejected;
            ApplicationProcessIdentity = new ApplicationProcessIdentity(
                hwnd, observed.ProcessId, observed.ProcessStartedUtc, observed.CanonicalExecutablePath,
                observed.ObservationStartedUtc, observed.ObservationCompletedUtc,
                "GetWindowThreadProcessId(Application.Hwnd)", observed.StartTimeAcquisitionProvenance, observed.ExecutableAcquisitionProvenance,
                executablePath, ProcessIdentity.Canonical(canonicalSentinelPath), ProcessIdentity.Canonical(actualSentinelPath), invocationId, actualInvocationId,
                state, false, ResidualState.NotApplicable);
            journal.Record("ApplicationProcessIdentityObserved", Identity);
            if (state == ApplicationIdentityValidationState.Rejected)
                journal.Record("ApplicationProcessIdentityRejected", Identity);
            if (state != ApplicationIdentityValidationState.Validated)
                throw new InvalidOperationException("Excel application identity mismatch.");

            excel.Visible = false;
            excel.DisplayAlerts = false;
        }
        catch
        {
            Release(ref book);
            Release(ref excel);
            throw;
        }
    }

    internal void ConfirmOwnershipEvidencePersisted(EvidencePersistenceConfirmation confirmation)
    {
        if (ApplicationProcessIdentity?.ValidationState != ApplicationIdentityValidationState.Validated || ApplicationIdentityValidation?.AggregateMatch != true)
            throw new InvalidOperationException("Validated Application process identity is required before evidence persistence can be confirmed.");
        ValidateConfirmation(confirmation, "OwnershipValidation");
        journal.Record("OwnershipEvidencePersisted", Identity);
        journal.Record("OwnershipPersistenceConfirmed", Identity);
        journal.Record("ApplicationProcessIdentityValidated", Identity);
        ownershipEvidencePersisted = true;
    }

    internal CleanupTopologyValidation ObserveCleanupTopology()
    {
        if (!ownershipEvidencePersisted || ActivationProcessIdentity is null || ApplicationProcessIdentity is null || book is null)
            throw new InvalidOperationException("Durably confirmed ownership evidence is required before cleanup-topology observation.");
        ActivationProcessReobservation = ProcessIdentityObserver.Observe(ActivationProcessIdentity.ProcessId);
        ApplicationProcessReobservation = ProcessIdentityObserver.Observe(ApplicationProcessIdentity.ProcessId);
        var sentinelRevalidated = RotSentinelAttachment.TryNormalizeLocalDrivePath((string)book.FullName, out var actualSentinel) &&
            StringComparer.OrdinalIgnoreCase.Equals(actualSentinel, ApplicationProcessIdentity.ExpectedSentinelCanonicalPath);
        var invocationRevalidated = ReadInvocationCorrelation(book) == invocationId;
        CleanupTopologyValidation = global::Vmf.Build.ExcelOwnership.Primitives.CleanupTopologyValidation.Compare(
            ActivationProcessIdentity, ApplicationProcessIdentity, ActivationProcessReobservation, ApplicationProcessReobservation,
            sentinelRevalidated, invocationRevalidated);
        journal.Record("CleanupTopologyObserved", Identity);
        if (!CleanupTopologyValidation.AggregateMatch) journal.Record("CleanupTopologyRejected", Identity);
        return CleanupTopologyValidation;
    }

    internal void ConfirmCleanupTopologyEvidencePersisted(EvidencePersistenceConfirmation confirmation)
    {
        if (!ownershipEvidencePersisted || CleanupTopologyValidation?.AggregateMatch != true)
            throw new InvalidOperationException("A matching cleanup topology is required before evidence persistence can be confirmed.");
        ValidateConfirmation(confirmation, "CleanupTopology");
        journal.Record("CleanupTopologyEvidencePersisted", Identity);
        journal.Record("CleanupTopologyPersistenceConfirmed", Identity);
        cleanupTopologyEvidencePersisted = true;
        journal.Record("CleanupTopologyValidated", Identity);
        ApplicationProcessIdentity = ApplicationProcessIdentity! with { CleanupEligible = true };
    }

    public void Dispose()
    {
        if (cleanupAttempted) return;
        cleanupAttempted = true;
        Exception? failure = null;
        if (ActivationProcessIdentity is null)
        {
            Release(ref book);
            Release(ref excel);
            return;
        }
        var cleanupPermitted = ApplicationProcessIdentity?.ValidationState == ApplicationIdentityValidationState.Validated &&
            ownershipEvidencePersisted && CleanupTopologyValidated && cleanupTopologyEvidencePersisted && ApplicationProcessIdentity.CleanupEligible;
        if (!cleanupPermitted)
        {
            journal.Record("ExcelCleanupSkippedUnsafeTopology", Identity);
        }
        else
        {
            journal.Record("ExcelCleanupStarted", Identity);
            DateTimeOffset? closeStartedUtc = null;
            DateTimeOffset? closeCompletedUtc = null;
            DateTimeOffset? quitStartedUtc = null;
            DateTimeOffset? quitCompletedUtc = null;
            try
            {
                closeStartedUtc = DateTimeOffset.UtcNow;
                book?.Close(false);
            }
            catch (Exception exception) { failure = exception; }
            finally { closeCompletedUtc = DateTimeOffset.UtcNow; }
            try
            {
                quitStartedUtc = DateTimeOffset.UtcNow;
                excel?.Quit();
            }
            catch (Exception exception) { failure ??= exception; }
            finally { quitCompletedUtc = DateTimeOffset.UtcNow; }
            CleanupTimingTelemetry = new CleanupTimingTelemetry(closeStartedUtc, closeCompletedUtc, quitStartedUtc, quitCompletedUtc);
            journal.Record(failure is null ? "ExcelCleanupActionsCompleted" : "ExcelCleanupFailed", Identity);
        }

        Release(ref book);
        Release(ref excel);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        ResidualObservationTelemetry = ResidualProcessObserver.Observe(ActivationProcessIdentity, ApplicationProcessIdentity);
        ActivationProcessResidualState = ResidualObservationTelemetry.Targets.Single(target => target.Role == "Activation").FinalResidualState;
        ApplicationProcessResidualState = ResidualObservationTelemetry.Targets.SingleOrDefault(target => target.Role == "Application")?.FinalResidualState
            ?? ResidualState.NotApplicable;
        if (ActivationProcessIdentity is not null)
            ActivationProcessIdentity = ActivationProcessIdentity with { LifetimeObservedUtc = DateTimeOffset.UtcNow,
                HasExited = ActivationProcessResidualState == ResidualState.Clear ? true : ActivationProcessResidualState == ResidualState.Residual ? false : null,
                ResidualState = ActivationProcessResidualState };
        if (ApplicationProcessIdentity is not null)
            ApplicationProcessIdentity = ApplicationProcessIdentity with { ResidualState = ApplicationProcessResidualState };

        journal.Record("ExcelResidualChecked", Identity);
        Clean = cleanupPermitted && failure is null && ActivationProcessResidualState == ResidualState.Clear && ApplicationProcessResidualState == ResidualState.Clear;
        if (Clean)
        {
            journal.Record("ExcelCleanupCompleted", Identity);
            return;
        }
        if (cleanupPermitted && failure is null) journal.Record("ExcelCleanupFailed", Identity);
        if (cleanupPermitted)
            throw new InvalidOperationException($"Cleanup failed: activation={ActivationProcessResidualState}; application={ApplicationProcessResidualState}.", failure);
    }

    private void ValidateConfirmation(EvidencePersistenceConfirmation confirmation, string artifactKind)
    {
        if (confirmation.InvocationId != invocationId || !string.Equals(confirmation.ArtifactKind, artifactKind, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(confirmation.ArtifactPath) || !Path.IsPathFullyQualified(confirmation.ArtifactPath) ||
            confirmation.ByteLength <= 0 || confirmation.Sha256.Length != 64 || confirmation.Sha256.Any(character => !Uri.IsHexDigit(character)) ||
            confirmation.TargetRecordedUtc == default || confirmation.ConfirmedUtc < confirmation.TargetRecordedUtc)
            throw new InvalidDataException("Evidence persistence confirmation is invalid or mismatched.");
    }

    private static string ResolveExcel()
    {
        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
        using (var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\excel.exe"))
            if (key?.GetValue(null) is string path && File.Exists(path)) return Path.GetFullPath(path);
        throw new FileNotFoundException("Installed EXCEL.EXE not found.");
    }

    private void CreateSentinel(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        Write(archive, "[Content_Types].xml", "<?xml version=\"1.0\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/><Override PartName=\"/docProps/custom.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.custom-properties+xml\"/></Types>");
        Write(archive, "_rels/.rels", "<?xml version=\"1.0\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/><Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties\" Target=\"docProps/custom.xml\"/></Relationships>");
        Write(archive, "xl/workbook.xml", "<?xml version=\"1.0\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Sheet1\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
        Write(archive, "xl/_rels/workbook.xml.rels", "<?xml version=\"1.0\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");
        Write(archive, "xl/worksheets/sheet1.xml", "<?xml version=\"1.0\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData/></worksheet>");
        Write(archive, "docProps/custom.xml", $"<?xml version=\"1.0\"?><Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/custom-properties\" xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\"><property fmtid=\"{{D5CDD505-2E9C-101B-9397-08002B2CF9AE}}\" pid=\"2\" name=\"VmfInvocationId\"><vt:lpwstr>{invocationId:D}</vt:lpwstr></property></Properties>");
    }

    private static Guid ReadInvocationCorrelation(dynamic workbook)
    {
        dynamic? properties = null; dynamic? property = null;
        try { properties = workbook.CustomDocumentProperties; property = properties.Item("VmfInvocationId"); var value = (string)property.Value; return Guid.TryParse(value, out var parsed) ? parsed : Guid.Empty; }
        finally { Release(ref property); Release(ref properties); }
    }

    private static void Write(ZipArchive archive, string name, string value) { var entry = archive.CreateEntry(name); using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)); writer.Write(value); }
    private static void Release(ref dynamic? value) { if (value is not null && Marshal.IsComObject(value)) Marshal.FinalReleaseComObject(value); value = null; }

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint windowHandle, out uint processId);
}
