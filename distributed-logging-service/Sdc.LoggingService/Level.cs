namespace Sdc.LoggingService
{
    public sealed record Level(string ShortForm, int Priority);

    public static class Levels
    {
        public static Level Critical => new("CRT", 4);
        public static Level Debug => new("DBG", 0);
        public static Level Error => new("ERR", 3);
        public static Level Information => new("INF", 1);
        public static Level Warning => new("WRN", 2);
    }
}
