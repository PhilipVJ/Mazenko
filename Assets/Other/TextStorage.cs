using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextStorage : MonoBehaviour
{
    private static string[] slow = new string[] { "That was too slow!", "You should try again", "A medal is within reach!" };
    private static string[] bronze = new string[] { "You can do better for sure!", "You should try again", "A silver medal is close"};
    private static string[] silver = new string[] { "Almost gold! You should try again!", "Not bad!", "Pretty fast indeed" };
    private static string[] gold = new string[] { "You rock!", "That was awesome!", "You should turn pro!" };

    public static string GetSilverText()
    {
        int random = Random.Range(0, silver.Length - 1);
        return silver[random];
    }


    public static string GetGoldText()
    {
        int random = Random.Range(0, gold.Length - 1);
        return gold[random];
    }

    public static string GetBronzeText()
    {
        int random = Random.Range(0, bronze.Length - 1);
        return bronze[random];
    }

    public static string GetSlowText()
    {
        int random = Random.Range(0, slow.Length - 1);
        return slow[random];
    }

}
