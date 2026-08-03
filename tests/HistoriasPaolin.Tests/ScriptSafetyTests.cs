namespace HistoriasPaolin.Tests;

public sealed class ScriptSafetyTests
{
    [Fact]
    public void ResetScriptRequiresDevelopmentExactDatabaseAndConfirmation()
    {
        var scriptPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "scripts",
            "reset-local-database.ps1"));
        var script = File.ReadAllText(scriptPath);

        Assert.Contains("ASPNETCORE_ENVIRONMENT -ne \"Development\"", script);
        Assert.Contains("SQLSERVER_DATABASE -ne \"HistoriasPaolinDb\"", script);
        Assert.Contains("RESET HistoriasPaolinDb", script);
    }
}
