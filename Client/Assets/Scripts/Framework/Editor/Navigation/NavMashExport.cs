/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/11 23:17:43
** desc:  导航网格导出;
*********************************************************************************/

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace FrameworkEditor
{
    public static class NavMashExport
    {
        private static string _rootPath = Application.dataPath + "/ArtRes/NavMesh/";

        [MenuItem("myGameFramework/Navigation/Export NavMash", false, 0)]
        public static void Export()
        {
            /*
            string path = EditorUtility.OpenFilePanel("选择Scene文件", Application.dataPath, "unity");
            if (string.IsNullOrEmpty(path))
                return;
            */
            if (EditorUtility.DisplayDialog("提示", "确认导出当前场景的NavMesh数据！", "开始导出"))
            {
                var triangNavMesh = NavMesh.CalculateTriangulation();
                if (triangNavMesh.areas.Length == 0 && triangNavMesh.indices.Length == 0)
                {
                    return;
                }
                var mesh = new Mesh
                {
                    name = "_NavMesh",
                    vertices = triangNavMesh.vertices,
                    triangles = triangNavMesh.indices
                };
                var sceneName = SceneManager.GetActiveScene().name;
                var parentName = _rootPath + sceneName;
                var fileName = parentName + "/" + sceneName + "_NavMesh.obj";
                if (Directory.Exists(parentName))
                {
                    Directory.Delete(parentName, true);
                }
                Directory.CreateDirectory(parentName);
                AssetDatabase.Refresh();
                using (var sw = new StreamWriter(fileName))
                {
                    sw.Write(GenerateMesh(mesh));
                }
                AssetDatabase.Refresh();
                var assetName = fileName.Replace(Application.dataPath, "Assets");
                var navMesh = UnityObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(assetName));
                navMesh.name = "_NavMesh";
                ExportNavData(navMesh);
                LogHelper.Print("导出NavMesh完成:" + sceneName + "_NavMesh.obj");
                AssetDatabase.Refresh();
            }
        }

        private static string GenerateMesh(Mesh mesh)
        {
            var sb = new StringBuilder();
            sb.Append("g ").Append(mesh.name).Append("\n");
            foreach (var v in mesh.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (var v in mesh.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (var v in mesh.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (var m = 0; m < mesh.subMeshCount; m++)
            {
                sb.Append("\n");
                var triangles = mesh.GetTriangles(m);
                for (var i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }

        private static void ExportNavData(GameObject go)
        {
            var localVectors = go.transform.Find("_NavMesh").GetComponent<MeshFilter>().sharedMesh.vertices;
            var triangles = go.transform.Find("_NavMesh").GetComponent<MeshFilter>().sharedMesh.triangles;
            var worldVectors = new Vector3[localVectors.Length];//把mesh的本地坐标转成世界坐标;
            for (var i = 0; i < localVectors.Length; i++)
            {
                var pos = go.transform.TransformPoint(localVectors[i]);
                worldVectors[i] = pos;
            }
            var sb = new StringBuilder();
            sb.Append("local nav = {\n");
            for (var i = 0; i < triangles.Length; i += 3)
            {
                sb.AppendFormat("\t{{{0},{1},{2}}},\n", vector2lua(worldVectors[triangles[i]]), vector2lua(worldVectors[triangles[i + 1]]), vector2lua(worldVectors[triangles[i + 2]]));
            }
            sb.Append("}\n");
            sb.Append("return nav");
            var sceneName = SceneManager.GetActiveScene().name;
            using (var sw = new StreamWriter(_rootPath + sceneName + "/" + sceneName + "_NavMesh" + ".lua"))
            {
                sw.Write(sb.ToString());
            }
            UnityObject.DestroyImmediate(go);
        }

        private static string vector2lua(Vector3 vec)
        {
            return string.Format("{{{0},{1},{2}}}", vec.x, vec.y, vec.z);
        }
    }
}
