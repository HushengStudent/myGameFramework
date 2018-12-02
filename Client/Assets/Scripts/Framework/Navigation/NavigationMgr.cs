/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/11 23:20:34
** desc:  寻路管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class NavigationMgr : Singleton<NavigationMgr>
    {

        private static void NavigationTest()
        {
            GameObject go = GameObject.Find("_NavMesh");
            if (go == null)
                return;
            Vector3[] localVectors = go.GetComponent<MeshFilter>().sharedMesh.vertices;
            int[] triangles = go.GetComponent<MeshFilter>().sharedMesh.triangles;
            //把mesh的本地坐标转成世界坐标;
            Vector3[] worldVectors = new Vector3[localVectors.Length];
            for (int i = 0; i < localVectors.Length; ++i)
            {
                Vector3 pos = go.transform.TransformPoint(localVectors[i]);
                worldVectors[i] = pos;
            }
            //检测点;
            Vector3 target = GameObject.Find("TestGameObject").transform.position;
            bool result = false;
            for (int i = 0; i < triangles.Length; i += 3)
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
            Vector3 v0 = C - A;
            Vector3 v1 = B - A;
            Vector3 v2 = P - A;
            float dot00 = Vector3.Dot(v0, v0);
            float dot01 = Vector3.Dot(v0, v1);
            float dot02 = Vector3.Dot(v0, v2);
            float dot11 = Vector3.Dot(v1, v1);
            float dot12 = Vector3.Dot(v1, v2);
            float inverDeno = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * inverDeno;
            // if u out of range, return directly;
            if (u < 0 || u > 1)
                return false;
            float v = (dot00 * dot12 - dot01 * dot02) * inverDeno;
            // if v out of range, return directly;
            if (v < 0 || v > 1)
                return false;
            return u + v <= 1;
        }
    }
}