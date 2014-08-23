using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class UKStringExtension
{
    /// <summary>
    /// "blub", "b" => 2
    /// "blaublau", "au" => 2
    /// "blaublau", "xx" => 0
    /// "lalalala", "lala" => 3 (LALAlala, laLALAla, lalaLALA)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="sub"></param>
    /// <returns>number of occurrences</returns>
    public static int CountSubstring(this string s, string sub)
    {
        if (s == null) return 0;

        int count = 0;
        int i = 0;

        while (true)
        {
            i = s.IndexOf(sub, i);
            if (i >= 0)
            {
                ++count;
                i += 1;
            }
            else
            {
                break;
            }
        }

        return count;
    }
}
