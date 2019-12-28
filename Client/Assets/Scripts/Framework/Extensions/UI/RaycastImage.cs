/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/28 17:53:41
** desc:  µã»÷Image,½µµÍÏñËØÌî³äÂÊ;
*********************************************************************************/

namespace UnityEngine.UI
{
    public class RaycastImage : MaskableGraphic
    {
        protected override void Awake()
        {
            base.Awake();
            material = null;
            raycastTarget = true;
            color = new Color32(0, 0, 0, 0);
        }

        protected RaycastImage()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}