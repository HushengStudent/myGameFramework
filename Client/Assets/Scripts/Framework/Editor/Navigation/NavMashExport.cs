/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/11 23:17:43
** desc:  导航网格导出;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    public static class NavMashExport
    {
        private static string _rootPath = Application.dataPath + "/ArtRes/NavMesh/";

        [MenuItem("MGame/Navigation/Export NavMash", false, 0)]
        public static void Export()
        {
            /*
            string path = EditorUtility.OpenFilePanel("选择Scene文件", Application.dataPath, "unity");
            if (string.IsNullOrEmpty(path))
                return;
            */
            bool export = EditorUtility.DisplayDialog("提示", "确认导出当前场景的NavMesh数据！", "开始导出");
            if (export)
            {
                NavMeshTriangulation triangNavMesh = NavMesh.CalculateTriangulation();
                if (triangNavMesh.areas.Length == 0 && triangNavMesh.indices.Length == 0)
                    return;
                Mesh mesh = new Mesh();
                mesh.name = "_NavMesh";
                mesh.vertices = triangNavMesh.vertices;
                mesh.triangles = triangNavMesh.indices;
                string sceneName = SceneManager.GetActiveScene().name;
                string parentName = _rootPath + sceneName;
                string fileName = parentName + "/" + sceneName + "_NavMesh.obj";
                if (Directory.Exists(parentName))
                    Directory.Delete(parentName,true);
                Directory.CreateDirectory(parentName);
                AssetDatabase.Refresh();
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write(GenerateMesh(mesh));
                }
                AssetDatabase.Refresh();
                string assetName = fileName.Replace(Application.dataPath, "Assets");
                GameObject navMesh = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(assetName));
                navMesh.name = "_NavMesh";
                ExportNavData(navMesh);
                LogUtil.LogUtility.Print("导出NavMesh完成:" + sceneName + "_NavMesh.obj");
                AssetDatabase.Refresh();
            }
        }

        private static string GenerateMesh(Mesh mesh)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("g ").Append(mesh.name).Append("\n");
            foreach (Vector3 v in mesh.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in mesh.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in mesh.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                sb.Append("\n");
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }

        private static void ExportNavData(GameObject go)
        {
            Vector3[] localVectors = go.transform.Find("_NavMesh").GetComponent<MeshFilter>().sharedMesh.vertices;
            int[] triangles = go.transform.Find("_NavMesh").GetComponent<MeshFilter>().sharedMesh.triangles;
            Vector3[] worldVectors = new Vector3[localVectors.Length];//把mesh的本地坐标转成世界坐标;
            for (int i = 0; i < localVectors.Length; i++)
            {
                Vector3 pos = go.transform.TransformPoint(localVectors[i]);
                worldVectors[i] = pos;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("local nav = {\n");
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.AppendFormat("\t{{{0},{1},{2}}},\n", vector2lua(worldVectors[triangles[i]]), vector2lua(worldVectors[triangles[i + 1]]), vector2lua(worldVectors[triangles[i + 2]]));
            }
            sb.Append("}\n");
            sb.Append("return nav");
            string sceneName = SceneManager.GetActiveScene().name;
            using (StreamWriter sw = new StreamWriter(_rootPath + sceneName + "/" + sceneName + "_NavMesh" + ".lua"))
            {
                sw.Write(sb.ToString());
            }
            Object.DestroyImmediate(go);
        }

        private static string vector2lua(Vector3 vec)
        {
            return string.Format("{{{0},{1},{2}}}", vec.x, vec.y, vec.z);
        }
    }
}
