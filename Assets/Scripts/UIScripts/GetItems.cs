using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItems : MonoBehaviour
{
    public List<int> GetItemKeys()
    {
        List<int> list = new();
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            if (transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                list.Add(i);
            }
        }
        Debug.Log("Get Item is called...");
        return list;
    }
}
