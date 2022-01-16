/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2022/01/16 15:40:13
** desc:  #####
*********************************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class FbxProcessorHelper
    {
        private static readonly string _meshPath = "Assets/Bundles/Mesh/Models";

        [MenuItem("Assets/myGameFramework/Helper/FbxProcessor", false, 0)]
        public static void CopyLoadPath()
        {
            var targets = Selection.objects;
            if (targets.Length > 1 || targets.Length < 1)
            {
                return;
            }

            var asset = targets[0] as GameObject;
            if (!asset)
            {
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(asset);
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            importer.isReadable = false;
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;
            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var target = Object.Instantiate(asset);
            var skinnedMeshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
            if (!skinnedMeshRenderer)
            {
                return;
            }

            var name = asset.name;
            var path = $"{_meshPath}/{name}";

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);

            var mesh = Object.Instantiate(skinnedMeshRenderer.sharedMesh);
            mesh.colors = null;
            mesh.tangents = null;
            //mesh.normals = null;
            mesh.uv3 = null;
            mesh.uv4 = null;
            AssetDatabase.CreateAsset(mesh, $"{path}/{name}.asset");
            AssetDatabase.SaveAssets();
            Object.DestroyImmediate(target);
            AssetDatabase.Refresh();
        }
    }
}