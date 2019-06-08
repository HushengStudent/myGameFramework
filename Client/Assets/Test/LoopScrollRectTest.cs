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
        ls.onItemUpdate += onItemUpdate;
        ls.onItemRecycle += onItemRecycle;
        ls.totalCount = totalCount;
        ls.RefillCells();
    }

    private void onItemUpdate(int index, GameObject go)
    {
        go.name = index.ToString();
        LogHelper.PrintError("--->>>onItemUpdate:" + index.ToString());
    }

    private void onItemRecycle(int index, GameObject go)
    {
        LogHelper.PrintError("--->>>onItemRecycle:" + index.ToString());
    }
}

