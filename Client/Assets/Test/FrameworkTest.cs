/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 23:40:04
** desc:  测试;
*********************************************************************************/

using Framework;
using Framework.BehaviorTreeModule;
using Framework.ECSModule;
using Framework.NetModule;
using Framework.ObjectPoolModule;
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
        NetMgr.singleton.Send(req);
    }

    public void GetObjectFromPool()
    {
        
    }

    public void ReleaseObject2Pool()
    {
        
    }

    public void ClearPool()
    {
        CoroutineMgr.singleton.RunCoroutine(PoolMgr.singleton.ClearPool());
    }

    public void BehaviorTreeTest()
    {
        var entity = EntityMgr.singleton.GetEntity<RoleEntity>(ulong.MaxValue);
        if (entity == null)
        {
            entity = EntityMgr.singleton.CreateEntity<RoleEntity>(1, ulong.MaxValue, "BehaviorTree");
        }
        BehaviorTreeMgr.singleton.CreateBehaviorTree(entity, "Bin/Bt/BehaviourTree.BT", true);
    }

    private void ModelInit()
    {
        var role = EntityMgr.singleton.CreateEntity<RoleEntity>(1, 1, "_entity_test");
        role.GameObject.SetLocalPosition(0.5f, 17f, -20f);
        role.GameObject.SetLocalRotation(0f, 180f, 0f);
        role.GameObject.SetLocalScale(1f, 1f, 1f);
    }

    public void ChangeHead()
    {
        var role = EntityMgr.singleton.GetEntity<RoleEntity>(1);
        var modelComponent = role.GameObject.ModelComponent as CombineModelComponent;
        if (modelComponent != null)
        {
            modelComponent.SetHead("Assets/Bundles/Prefab/Models/Avatar/ch_pc_hou_008_tou.prefab");
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
