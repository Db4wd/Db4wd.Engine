namespace DbForward.Postgres;

internal static class DbVersion
{
    internal static string Create(int sequenceId)
    {
        return $"{DateTime.Now:yyyy-MM-dd}.{sequenceId:D5}";
    }
}