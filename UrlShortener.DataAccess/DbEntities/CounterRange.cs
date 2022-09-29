namespace UrlShortener.DataAccess.DbEntities;

public class CounterRange
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong RangeStart { get; init; }
    public ulong LastUsedValue { get; set; }
}