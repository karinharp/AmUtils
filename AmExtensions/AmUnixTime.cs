using System;

namespace am
{
    
public static class AmUnixTime
{
    
    private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    ///   現在時刻からUnixTimeを計算する.
    /// </summary>
    /// <returns>UnixTime</returns>
    public static long Now()
    {
	return ( FromDateTime( DateTime.UtcNow ) );
    }

    /// <summary>
    ///   UnixTimeからDateTimeに変換.
    /// </summary>
    /// <param name="unixTime">unixTime 変換したいUnixTime</param>  
    /// <example>
    /// <returns>UnixTime</returns>
    public static DateTime FromUnixTime( long unixTime )
    {
	return UNIX_EPOCH.AddSeconds( unixTime ).ToLocalTime();
    }
    
    /// <param name="dateTime">DateTimeオブジェクト</param>
    /// <returns>UnixTime</returns>
    public static long FromDateTime( DateTime dateTime )
    {
	double nowTicks = ( dateTime.ToUniversalTime() - UNIX_EPOCH ).TotalSeconds;
	return (long)nowTicks;
    }
}
}
