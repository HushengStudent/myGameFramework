#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
[InitializeOnLoad]
public class FSPStaticMeshManagerEditor
{
	public static List<ShadowReceiver> currentStaticReceivers;

	static FSPStaticMeshManagerEditor() {
		EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
	}

	static void OnPlaymodeStateChanged()
	{
		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
			TraverseReceivers();
			RecreateFSPStaticHolder();
		}
	}

	static void TraverseReceivers() {
		currentStaticReceivers = new List<ShadowReceiver>();

		foreach (ShadowReceiver receiver in Object.FindObjectsOfType(typeof(ShadowReceiver)))
		{
			if (receiver.gameObject.isStatic) {
				currentStaticReceivers.Add(receiver);
			}
		}
	}

	static void RecreateFSPStaticHolder() {
		bool staticsExist = false;

		foreach (ShadowReceiver receiver in currentStaticReceivers) {
			if (receiver.gameObject.isStatic) {
				staticsExist = true;
				break;
			}
		}

		GameObject staticHolder = (GameObject)GameObject.Find("_FSPStaticHolder");

		if (staticHolder != null) {
			GameObject.DestroyImmediate(staticHolder);
		}

		if (!staticsExist) {
			return;
		}
			
		staticHolder = new GameObject("_FSPStaticHolder");
		staticHolder.AddComponent<FSPStaticMeshHolder>();
		staticHolder.isStatic = false;

		MeshFilter meshFilter;
		Mesh mesh;
		Mesh meshCopy;
		int id = 0;

		foreach (ShadowReceiver receiver in currentStaticReceivers) {
			meshFilter = receiver.GetComponent<MeshFilter>();
			receiver._id = id++;

			EditorUtility.SetDirty(receiver);

			if (meshFilter != null) {
				mesh = meshFilter.sharedMesh;
				meshCopy = new Mesh();
				meshCopy.vertices = mesh.vertices;
				meshCopy.triangles = mesh.triangles;
				meshCopy.normals = mesh.normals;
				meshCopy.name = "_copy";
		
			//	meshCopy.MarkDynamic();

				GameObject meshLinker = new GameObject(receiver._id.ToString());
				meshLinker.isStatic = false;
				meshLinker.transform.parent = staticHolder.transform;
				meshLinker.transform.position = receiver.transform.position;
				meshLinker.AddComponent<MeshFilter>().mesh = meshCopy;

				MeshRenderer meshRenderer = meshLinker.AddComponent<MeshRenderer>();
				meshRenderer.enabled = false;
			}
		}
	}
}
#endif