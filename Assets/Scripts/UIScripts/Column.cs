using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Column : MonoBehaviour
{
    public void PopulateColumnOptions(string[] options)
    {
        var dropdown = GetComponent<TMP_Dropdown>();
        dropdown.interactable = false;
        try
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(options.ToList());
        } catch(Exception e)
        {
            Debug.Log(e.Message);
        } finally
        {
            dropdown.interactable = true;
        }
    }
}
