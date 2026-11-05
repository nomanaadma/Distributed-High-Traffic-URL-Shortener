using System.Diagnostics.Metrics;

namespace URL_Shortener.RedirectApi;

public static class ApplicationDiagnostics
{
    private const string ServiceName = "RedirectApi";
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> RedirectExecutedCounter 
        = Meter.CreateCounter<long>("redirect.executed");
}