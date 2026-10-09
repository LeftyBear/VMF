using System.Diagnostics;

internal sealed record ExcelProcessBaseline(int Count, IReadOnlyList<int> ProcessIds);

internal static class ExcelProcessBaselineObserver
{
    internal static ExcelProcessBaseline Observe()
    {
        var processes = Process.GetProcessesByName("EXCEL");
        try
        {
            var processIds = processes.Select(process => process.Id).OrderBy(processId => processId).ToArray();
            return new ExcelProcessBaseline(processIds.Length, processIds);
        }
        finally
        {
            foreach (var process in processes)
                process.Dispose();
        }
    }
}
