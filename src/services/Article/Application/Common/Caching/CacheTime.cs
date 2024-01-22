namespace Application.Common.Caching;

public enum CacheTime : byte
{
    None,
    OneMinute,
    FifteenMinutes,
    ThirtyMinutes,
    OneHour,
    ThreeHours,
    SixHours,
    TwelveHours,
    OneDay,
    OneWeek,
    OneMonth,
    OneYear
}
