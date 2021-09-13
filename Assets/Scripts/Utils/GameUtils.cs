using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static string PlayerName
    {
        set{
            PlayerPrefs.SetString("PlayerName",value);
        }
        get{
            return PlayerPrefs.GetString("PlayerName");
        }
    }

    public static List<T> Shuffle<T>(ref List<T> list)
    {
        List<T> tempList = new List<T>();
        tempList.AddRange(list);
        int count = tempList.Count;

        for (int i = count - 1; i >= 0; --i)
        {
            int rand = UnityEngine.Random.Range(0, i);
            T tmp = tempList[i];
            tempList[i] = tempList[rand];
            tempList[rand] = tmp;
        }

        list = tempList;

        return tempList;
    }
}
