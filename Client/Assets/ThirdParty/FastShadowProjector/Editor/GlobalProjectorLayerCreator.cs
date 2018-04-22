using UnityEngine;
using UnityEditor;
 
[InitializeOnLoad]
public class GlobalProjetorLayerCreator
{
    static GlobalProjetorLayerCreator()
    {
       	ShadowProjectorEditor.CheckGlobalProjectorLayer();
    }
}