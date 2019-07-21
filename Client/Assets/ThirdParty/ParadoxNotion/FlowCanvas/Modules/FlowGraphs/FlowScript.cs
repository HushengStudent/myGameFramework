namespace FlowCanvas
{

    ///FlowScripts are assigned or bound to FlowScriptControllers
    [UnityEngine.CreateAssetMenu(menuName = "ParadoxNotion/FlowCanvas/FlowScript Asset")]
    public class FlowScript : FlowScriptBase
    {

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/ParadoxNotion/FlowCanvas/Create/FlowScript Asset", false, 1)]
        public static void CreateFlowScript() {
            var fs = ParadoxNotion.Design.EditorUtils.CreateAsset<FlowScript>();
            UnityEditor.Selection.activeObject = fs;
        }

#endif
        ///----------------------------------------------------------------------------------------------
    }
}
