using System;

public class Utils {

    public static long GetTimeinMilliseconds()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

}
