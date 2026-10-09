namespace Vmf.Build.ExcelOwnership.Experiment;

internal static class Program
{
    internal const string WorkerRequestEnvironmentVariable = "VMF_EXCEL_OWNERSHIP_EXPERIMENT_WORKER_REQUEST";
    internal const string WorkerTokenEnvironmentVariable = "VMF_EXCEL_OWNERSHIP_EXPERIMENT_WORKER_TOKEN";

    public static int Main(string[] args)
    {
        try
        {
            var workerRequest = Environment.GetEnvironmentVariable(WorkerRequestEnvironmentVariable);
            var workerToken = Environment.GetEnvironmentVariable(WorkerTokenEnvironmentVariable);
            if (workerRequest is not null || workerToken is not null)
            {
                if (args.Length != 0 || string.IsNullOrWhiteSpace(workerRequest) || string.IsNullOrWhiteSpace(workerToken))
                    throw new InvalidDataException("Malformed internal worker activation.");
                return ExperimentWorker.Run(workerRequest, workerToken);
            }

            var request = ExperimentContracts.ParsePublicCommand(args);
            return new ExperimentController().Run(request);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 2;
        }
    }
}
