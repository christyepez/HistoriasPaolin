namespace HistoriasPaolin.Application.Common;

public sealed class SystemCurrentUser : ICurrentUser
{
    public string UserName => Environment.UserName is { Length: > 0 } userName ? userName : "system";
}
