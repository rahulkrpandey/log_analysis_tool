using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    private bool actionDisable;
    public bool ActionDisable
    {
        get
        {
            return actionDisable;
        }

        set
        {
            actionDisable = this;
        }
    }

    public void OnItemTap()
    {
        if (actionDisable)
        {
            return;
        }
        Actions.ChangeGroupItem(this.transform.GetComponentInChildren<Text>().text, this.gameObject);
    }
}
