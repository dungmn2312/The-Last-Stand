using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserverItem
{
    public void OnNotifyTakeItem(ItemController itemController);
}
