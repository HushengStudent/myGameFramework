/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/07 16:39:13
** desc:  单例模板
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
	    protected static T instance = null;
	
	    public static T Instance
	    {
	        get
	        {
	            if (null == instance)
	            {
	                GameObject go = GameObject.Find("~!@#$%^&*()_MonoSingleton_");
	                if(null == go)
	                {
	                    go = new GameObject("~!@#$%^&*()_MonoSingleton_");
	                    DontDestroyOnLoad(go);
	                }
	                instance = go.AddComponent<T>();
	            }
	            return instance;
	        }
	    }
	
	    /// <summary>
	    /// 构造函数;
	    /// </summary>
	    protected MonoSingleton()
	    {
	        if (null != instance)
                LogUtil.LogUtility.Print("This " + (typeof(T)).ToString() + " Singleton Instance is not null!");
	        Init();
	    }
	
	    public virtual void Init() { }
	    public virtual void StartEx() { }
	    public virtual void AwakeEx() { }
	    public virtual void OnEnableEx() { }
	    public virtual void FixedUpdateEx() { }
	    public virtual void UpdateEx() { }
	    public virtual void LateUpdateEx() { }
	    public virtual void OnDisableEx() { }
	    public virtual void OnDestroyEx() { }
	
	    void Start(){ StartEx(); }
	    void Awake() { AwakeEx(); }
	    void OnEnable() { OnEnableEx(); }
	    void FixedUpdate() { FixedUpdateEx(); }
	    void Update() { UpdateEx(); }
	    void LateUpdate() { LateUpdateEx(); }
	    void OnDisable() { OnDisableEx(); }
	    void OnDestroy()
	    {
	        instance = null;
	        OnDestroyEx();
	    }
	}
}
