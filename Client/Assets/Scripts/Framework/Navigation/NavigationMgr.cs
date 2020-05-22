/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/11 23:20:34
** desc:  寻路管理;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class NavigationMgr : Singleton<NavigationMgr>
    {

        private static void NavigationTest()
        {
            var go = GameObject.Find("_NavMesh");
            if (go == null)
            {
                return;
            }
            var localVectors = go.GetComponent<MeshFilter>().sharedMesh.vertices;
            var triangles = go.GetComponent<MeshFilter>().sharedMesh.triangles;
            //把mesh的本地坐标转成世界坐标;
            var worldVectors = new Vector3[localVectors.Length];
            for (var i = 0; i < localVectors.Length; ++i)
            {
                Vector3 pos = go.transform.TransformPoint(localVectors[i]);
                worldVectors[i] = pos;
            }
            //检测点;
            var target = GameObject.Find("TestGameObject").transform.position;
            var result = false;
            for (var i = 0; i < triangles.Length; i += 3)
            {
                LogHelper.Print(string.Format("{0},{1},{2}", triangles[i], triangles[i + 1], triangles[i + 2]));
                if (IsInside(worldVectors[triangles[i]], worldVectors[triangles[i + 1]], worldVectors[triangles[i + 2]], target))
                {
                    result = true;
                    break;
                }
            }
            if (result)
            {
                LogHelper.Print("success!");
            }
            else
            {
                LogHelper.Print("error!");
            }
        }

        /// <summary>
        /// 判断点是否在三角形内;
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        public static bool IsInside(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            var v0 = C - A;
            var v1 = B - A;
            var v2 = P - A;
            var dot00 = Vector3.Dot(v0, v0);
            var dot01 = Vector3.Dot(v0, v1);
            var dot02 = Vector3.Dot(v0, v2);
            var dot11 = Vector3.Dot(v1, v1);
            var dot12 = Vector3.Dot(v1, v2);
            var inverDeno = 1 / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * inverDeno;
            // if u out of range, return directly;
            if (u < 0 || u > 1)
            {
                return false;
            }
            var v = (dot00 * dot12 - dot01 * dot02) * inverDeno;
            // if v out of range, return directly;
            if (v < 0 || v > 1)
            {
                return false;
            }
            return u + v <= 1;
        }
    }
}