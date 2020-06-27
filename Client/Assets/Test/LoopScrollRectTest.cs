/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/08 21:55:26
** desc:  #####
*********************************************************************************/

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LoopScrollRect))]
[DisallowMultipleComponent]
public class LoopScrollRectTest : MonoBehaviour
{
    public int totalCount = -1;

    void Start()
    {
        var ls = GetComponent<LoopScrollRect>();
        ls.onItemUpdate += OnItemUpdate;
        ls.onItemRecycle += OnItemRecycle;
        ls.totalCount = totalCount;
        ls.RefillCells();
    }

    private void OnItemUpdate(int index, GameObject go)
    {
        go.name = index.ToString();
        LogHelper.Print("--->>>onItemUpdate:" + index.ToString());
    }

    private void OnItemRecycle(int index, GameObject go)
    {
        LogHelper.Print("--->>>onItemRecycle:" + index.ToString());
    }
}

