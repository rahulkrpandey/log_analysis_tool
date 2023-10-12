using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GroupBy : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject itemList;
    [SerializeField] private GameObject content;
    private string[] HEADERS;
    private bool[] selectedOptions;
    private bool showItemList;
    private Queue<GameObject> optionList;
    
    private void Start()
    {
        showItemList = false;
        optionList = new();
        //HEADERS = new string[10];
        //for (int i = 0; i < HEADERS.Length; i++)
        //{
        //    HEADERS[i] = $"item {i}";
        //}
        //PopulateOptions(HEADERS);
    }

    public void OnClickDropDown()
    {
        itemList.SetActive(showItemList = !showItemList);
    }


    public void PopulateOptions(bool disableActionCall, string[] _headers)
    {
        Debug.Log("Populating...");
        ClearOptionList();
        HEADERS = _headers;        
        foreach (string val in HEADERS)
        {
            Debug.Log(val);
            var obj = InstantiateItem(val);
            if (disableActionCall)
            {
                obj.GetComponent<Item>().ActionDisable = true;
            }
        }

        Debug.Log("Populating done...");
    }

    private GameObject InstantiateItem(string val)
    {
        var obj = Instantiate(item, content.transform);
        var _toggle = obj.GetComponent<Toggle>();
        _toggle.isOn = false;
        _toggle.GetComponentInChildren<Text>().text = val;
        optionList.Enqueue(obj);

        return obj;
    }

    private void ClearOptionList()
    {
        while (optionList.Count > 0)
        {
            var obj = optionList.Dequeue();
            Destroy(obj);
        }
    }

    public List<int> GetItems()
    {
        List<int> list = new();
        int idx = 0;
        foreach (var item in optionList)
        {
            if (item.GetComponent<Toggle>().isOn)
            {
                list.Add(idx);
            }

            idx++;
        }

        return list;
    }
}
