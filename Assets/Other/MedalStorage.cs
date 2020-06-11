using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalStorage
{
    // First number is max time for bronze, second for silver and third for gold
    private static int[] level1 = new int[] { 25, 18, 13 };
    private static int[] level2 = new int[] { 55, 40, 25 };
    private static int[] level3 = new int[] { 55, 48, 43 };

    public static int[] GetLevelInfo(int level)
    {
        switch (level)
        {
            case 1:
                return level1;
            case 2: 
                return level2;
            case 3: 
                return level3;
            default:
                break;
        }
        return null;
    }
}
