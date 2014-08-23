using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class UKListExtension
{
    public static string ContentToString(this IEnumerable l)
    {
        if (l == null)
        {
            return "NULL";
        }
        else
        {
            StringBuilder b = new StringBuilder();

            b.Append("[");
        
            foreach(var it in l)
            {
                b.Append(it.ToString());
                b.Append(", ");
            }

            b.Append("]");
        
            return b.ToString();
        }
    }
}
