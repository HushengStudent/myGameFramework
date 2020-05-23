/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 23:40:04
** desc:  测试;
*********************************************************************************/

using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameworkTest : MonoBehaviour
{
    private GameObject _go;
    private Queue<GameObject> _goQueue = new Queue<GameObject>();

    private void Awake()
    {
        _go = new GameObject();
        _goQueue.Enqueue(_go);
        ModelInit();
    }

    public void NetWorkTest()
    {
        Packet_LoginRequest req = new Packet_LoginRequest();
        req.Data.id = 1001;
        req.Data.name = "HushengStudent";
        NetMgr.Singleton.Send<Packet_LoginRequest>(req);
    }

    public void GetObjectFromPool()
    {
        if (_go)
        {
            GameObject temp = PoolMgr.Singleton.GetUnityObject(_go) as GameObject;
            _goQueue.Enqueue(temp);
        }
    }

    public void ReleaseObject2Pool()
    {
        if (_goQueue.Count > 0)
        {
            PoolMgr.Singleton.ReleaseUnityObject(_goQueue.Dequeue());
        }
    }

    public void ClearPool()
    {
        CoroutineMgr.Singleton.RunCoroutine(PoolMgr.Singleton.ClearPool());
    }

    public void BehaviorTreeTest()
    {
        var entity = EntityMgr.Singleton.GetEntity<RoleEntity>(ulong.MaxValue);
        if (entity == null)
        {
            entity = EntityMgr.Singleton.CreateEntity<RoleEntity>(1, ulong.MaxValue, "BehaviorTree");
        }
        BehaviorTreeMgr.Singleton.CreateBehaviorTree(entity, "Bin/Bt/BehaviourTree.BT", true);
    }

    private void ModelInit()
    {
        var role = EntityMgr.Singleton.CreateEntity<RoleEntity>(1, 1, "_entity_test");
        role.GameObjectEx.SetLocalPosition(0.5f, 17f, -20f);
        role.GameObjectEx.SetLocalRotation(0f, 180f, 0f);
        role.GameObjectEx.SetLocalScale(1f, 1f, 1f);
    }

    public void ChangeHead()
    {
        var role = EntityMgr.Singleton.GetEntity<RoleEntity>(1);
        var modelComponent = role.GameObjectEx.ModelComponent as CombineModelComponent;
        if (modelComponent != null)
        {
            modelComponent.SetHead("Prefab/Models/Avatar/ch_pc_hou_008_tou.prefab");
        }
    }

    public void ChangeBody()
    {

    }

    public void ChangeHand()
    {

    }

    public void ChangeFeet()
    {

    }

    public void ChangeWeapon()
    {

    }

    public void ScreenShot()
    {
        UIScreenShotHelper.ExecuteScreenShot(null);
    }

    public void CameraShot()
    {
        var camera = transform.Find("Camera").GetComponent<Camera>();
        UIScreenShotHelper.ExecuteCameraShot(camera, null);
    }
}
