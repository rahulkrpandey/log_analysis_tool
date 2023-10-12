using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelStepItem : MonoBehaviour
{
   public void DeleteItem()
   {
        Actions.RemoveItemFromList(gameObject);
   }
}
