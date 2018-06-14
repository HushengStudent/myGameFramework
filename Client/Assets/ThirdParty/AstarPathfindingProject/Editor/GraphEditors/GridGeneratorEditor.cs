using UnityEngine;
using UnityEditor;
using Pathfinding.Serialization;

namespace Pathfinding {
	using Pathfinding.Util;

	[CustomGraphEditor(typeof(GridGraph), "Grid Graph")]
	public class GridGraphEditor : GraphEditor {
		[JsonMember]
		public bool locked = true;

		[JsonMember]
		public bool showExtra;

		GraphTransform savedTransform;
		Vector2 savedDimensions;
		float savedNodeSize;

		public bool isMouseDown;

		[JsonMember]
		public GridPivot pivot;

		/** Cached gui style */
		static GUIStyle lockStyle;

		/** Cached gui style */
		static GUIStyle gridPivotSelectBackground;

		/** Cached gui style */
		static GUIStyle gridPivotSelectButton;

		static readonly float standardIsometric = 90-Mathf.Atan(1/Mathf.Sqrt(2))*Mathf.Rad2Deg;
		static readonly float standardDimetric = Mathf.Acos(1/2f)*Mathf.Rad2Deg;

		/** Rounds a vector's components to multiples of 0.5 (i.e 0.5, 1.0, 1.5, etc.) if very close to them */
		public static Vector3 RoundVector3 (Vector3 v) {
			const int Multiplier = 2;

			if (Mathf.Abs(Multiplier*v.x - Mathf.Round(Multiplier*v.x)) < 0.001f) v.x = Mathf.Round(Multiplier*v.x)/Multiplier;
			if (Mathf.Abs(Multiplier*v.y - Mathf.Round(Multiplier*v.y)) < 0.001f) v.y = Mathf.Round(Multiplier*v.y)/Multiplier;
			if (Mathf.Abs(Multiplier*v.z - Mathf.Round(Multiplier*v.z)) < 0.001f) v.z = Mathf.Round(Multiplier*v.z)/Multiplier;
			return v;
		}

		public override void OnInspectorGUI (NavGraph target) {
			var graph = target as GridGraph;

			DrawFirstSection(graph);
			Separator();
			DrawMiddleSection(graph);
			Separator();
			DrawCollisionEditor(graph.collision);

			if (graph.collision.use2D) {
				if (Mathf.Abs(Vector3.Dot(Vector3.forward, Quaternion.Euler(graph.rotation) * Vector3.up)) < 0.9f) {
					EditorGUILayout.HelpBox("When using 2D physics it is recommended to rotate the graph so that it aligns with the 2D plane.", MessageType.Warning);
				}
			}

			Separator();
			DrawLastSection(graph);
		}

		void DrawFirstSection (GridGraph graph) {
			var normalizedPivotPoint = NormalizedPivotPoint(graph, pivot);
			var worldPoint = graph.CalculateTransform().Transform(normalizedPivotPoint);
			int newWidth, newDepth;

			DrawWidthDepthFields(graph, out newWidth, out newDepth);

			var newNodeSize = EditorGUILayout.FloatField(new GUIContent("Node size", "The size of a single node. The size is the side of the node square in world units"), graph.nodeSize);

			newNodeSize = newNodeSize <= 0.01F ? 0.01F : newNodeSize;

			float prevRatio = graph.aspectRatio;
			graph.aspectRatio = EditorGUILayout.FloatField(new GUIContent("Aspect Ratio", "Scaling of the nodes width/depth ratio. Good for isometric games"), graph.aspectRatio);

			DrawIsometricField(graph);

			if ((graph.nodeSize != newNodeSize && locked) || (newWidth != graph.width || newDepth != graph.depth) || prevRatio != graph.aspectRatio) {
				graph.nodeSize = newNodeSize;
				graph.SetDimensions(newWidth, newDepth, newNodeSize);

				normalizedPivotPoint = NormalizedPivotPoint(graph, pivot);
				var newWorldPoint = graph.CalculateTransform().Transform(normalizedPivotPoint);
				// Move the center so that the pivot point stays at the same point in the world
				graph.center += worldPoint - newWorldPoint;
				graph.center = RoundVector3(graph.center);
				graph.UpdateTransform();
				AutoScan();
			}

			if ((graph.nodeSize != newNodeSize && !locked)) {
				graph.nodeSize = newNodeSize;
				graph.UpdateTransform();
			}

			DrawPositionField(graph);

			graph.rotation = RoundVector3(EditorGUILayout.Vector3Field("Rotation", graph.rotation));
		}

		void DrawWidthDepthFields (GridGraph graph, out int newWidth, out int newDepth) {
			lockStyle = lockStyle ?? AstarPathEditor.astarSkin.FindStyle("GridSizeLock") ?? new GUIStyle();

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			newWidth = EditorGUILayout.IntField(new GUIContent("Width (nodes)", "Width of the graph in nodes"), graph.width);
			newDepth = EditorGUILayout.IntField(new GUIContent("Depth (nodes)", "Depth (or height you might also call it) of the graph in nodes"), graph.depth);

			// Clamping will be done elsewhere as well
			// but this prevents negative widths from being converted to positive ones (since an absolute value will be taken)
			newWidth = Mathf.Max(newWidth, 1);
			newDepth = Mathf.Max(newDepth, 1);

			GUILayout.EndVertical();

			Rect lockRect = GUILayoutUtility.GetRect(lockStyle.fixedWidth, lockStyle.fixedHeight);

			GUILayout.EndHorizontal();

			// All the layouts mess up the margin to the next control, so add it manually
			GUILayout.Space(2);

			// Add a small offset to make it better centred around the controls
			lockRect.y += 3;
			lockRect.width = lockStyle.fixedWidth;
			lockRect.height = lockStyle.fixedHeight;
			lockRect.x += lockStyle.margin.left;
			lockRect.y += lockStyle.margin.top;

			locked = GUI.Toggle(lockRect, locked,
				new GUIContent("", "If the width and depth values are locked, " +
					"changing the node size will scale the grid while keeping the number of nodes consistent " +
					"instead of keeping the size the same and changing the number of nodes in the graph"), lockStyle);
		}

		void DrawIsometricField (GridGraph graph) {
			var isometricGUIContent = new GUIContent("Isometric Angle", "For an isometric 2D game, you can use this parameter to scale the graph correctly.\nIt can also be used to create a hexagonal grid.\nYou may want to rotate the graph 45 degrees around the Y axis to make it line up better.");
			var isometricOptions = new [] { new GUIContent("None (0°)"), new GUIContent("Isometric (≈54.74°)"), new GUIContent("Dimetric (60°)"), new GUIContent("Custom") };
			var isometricValues = new [] { 0f, standardIsometric, standardDimetric };
			var isometricOption = isometricValues.Length;

			for (int i = 0; i < isometricValues.Length; i++) {
				if (Mathf.Approximately(graph.isometricAngle, isometricValues[i])) {
					isometricOption = i;
				}
			}

			var prevIsometricOption = isometricOption;
			isometricOption = EditorGUILayout.IntPopup(isometricGUIContent, isometricOption, isometricOptions, new [] { 0, 1, 2, 3 });
			if (prevIsometricOption != isometricOption) {
				// Change to something that will not match the predefined values above
				graph.isometricAngle = 45;
			}

			if (isometricOption < isometricValues.Length) {
				graph.isometricAngle = isometricValues[isometricOption];
			} else {
				EditorGUI.indentLevel++;
				// Custom
				graph.isometricAngle = EditorGUILayout.FloatField(isometricGUIContent, graph.isometricAngle);
				EditorGUI.indentLevel--;
			}
		}

		static Vector3 NormalizedPivotPoint (GridGraph graph, GridPivot pivot) {
			switch (pivot) {
			case GridPivot.Center:
			default:
				return new Vector3(graph.width/2f, 0, graph.depth/2f);
			case GridPivot.TopLeft:
				return new Vector3(0, 0, graph.depth);
			case GridPivot.TopRight:
				return new Vector3(graph.width, 0, graph.depth);
			case GridPivot.BottomLeft:
				return new Vector3(0, 0, 0);
			case GridPivot.BottomRight:
				return new Vector3(graph.width, 0, 0);
			}
		}

		void DrawPositionField (GridGraph graph) {
			GUILayout.BeginHorizontal();
			var normalizedPivotPoint = NormalizedPivotPoint(graph, pivot);
			var worldPoint = RoundVector3(graph.CalculateTransform().Transform(normalizedPivotPoint));
			var newWorldPoint = EditorGUILayout.Vector3Field(ObjectNames.NicifyVariableName(pivot.ToString()), worldPoint);
			var delta = newWorldPoint - worldPoint;
			if (delta.magnitude > 0.001f) {
				graph.center += delta;
			}

			pivot = PivotPointSelector(pivot);
			GUILayout.EndHorizontal();
		}

		protected virtual void DrawMiddleSection (GridGraph graph) {
			DrawNeighbours(graph);
			DrawMaxClimb(graph);
			DrawMaxSlope(graph);
			DrawErosion(graph);
		}

		protected virtual void DrawCutCorners (GridGraph graph) {
			graph.cutCorners = EditorGUILayout.Toggle(new GUIContent("Cut Corners", "Enables or disables cutting corners. See docs for image example"), graph.cutCorners);
			if (!graph.cutCorners && graph.useJumpPointSearch) {
				EditorGUILayout.HelpBox("Jump Point Search only works if 'Cut Corners' is enabled.", MessageType.Error);
			}
		}

		protected virtual void DrawNeighbours (GridGraph graph) {
			graph.neighbours = (NumNeighbours)EditorGUILayout.EnumPopup(new GUIContent("Connections", "Sets how many connections a node should have to it's neighbour nodes."), graph.neighbours);

			EditorGUI.indentLevel++;

			if (graph.neighbours == NumNeighbours.Eight) {
				DrawCutCorners(graph);
			}

			if (graph.neighbours == NumNeighbours.Six) {
				graph.uniformEdgeCosts = EditorGUILayout.Toggle(new GUIContent("Hexagon connection costs", "Tweak the edge costs in the graph to be more suitable for hexagon graphs"), graph.uniformEdgeCosts);
				if ((!Mathf.Approximately(graph.isometricAngle, standardIsometric) || !graph.uniformEdgeCosts) && GUILayout.Button("Configure as hexagon graph")) {
					graph.isometricAngle = standardIsometric;
					graph.uniformEdgeCosts = true;
				}
			} else {
				graph.uniformEdgeCosts = false;
			}

			EditorGUI.indentLevel--;

			if (graph.neighbours != NumNeighbours.Eight && graph.useJumpPointSearch) {
				EditorGUILayout.HelpBox("Jump Point Search only works for 8 neighbours.", MessageType.Error);
			}
		}

		protected virtual void DrawMaxClimb (GridGraph graph) {
			graph.maxClimb = EditorGUILayout.FloatField(new GUIContent("Max Climb", "How high in world units, relative to the graph, should a climbable level be. A zero (0) indicates infinity"), graph.maxClimb);
			if (graph.maxClimb < 0) graph.maxClimb = 0;
		}

		protected void DrawMaxSlope (GridGraph graph) {
			graph.maxSlope = EditorGUILayout.Slider(new GUIContent("Max Slope", "Sets the max slope in degrees for a point to be walkable. Only enabled if Height Testing is enabled."), graph.maxSlope, 0, 90F);
		}

		protected void DrawErosion (GridGraph graph) {
			graph.erodeIterations = EditorGUILayout.IntField(new GUIContent("Erosion iterations", "Sets how many times the graph should be eroded. This adds extra margin to objects."), graph.erodeIterations);
			graph.erodeIterations = graph.erodeIterations < 0 ? 0 : (graph.erodeIterations > 16 ? 16 : graph.erodeIterations); //Clamp iterations to [0,16]

			if (graph.erodeIterations > 0) {
				EditorGUI.indentLevel++;
				graph.erosionUseTags = EditorGUILayout.Toggle(new GUIContent("Erosion Uses Tags", "Instead of making nodes unwalkable, " +
						"nodes will have their tag set to a value corresponding to their erosion level, " +
						"which is a quite good measurement of their distance to the closest wall.\nSee online documentation for more info."),
					graph.erosionUseTags);
				if (graph.erosionUseTags) {
					EditorGUI.indentLevel++;
					graph.erosionFirstTag = EditorGUILayoutx.TagField("First Tag", graph.erosionFirstTag);
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
		}

		void DrawLastSection (GridGraph graph) {
			GUILayout.BeginHorizontal();
			GUILayout.Space(18);
			graph.showMeshSurface = GUILayout.Toggle(graph.showMeshSurface, new GUIContent("Show surface", "Toggles gizmos for drawing the surface of the mesh"), EditorStyles.miniButtonLeft);
			graph.showMeshOutline = GUILayout.Toggle(graph.showMeshOutline, new GUIContent("Show outline", "Toggles gizmos for drawing an outline of the nodes"), EditorStyles.miniButtonMid);
			graph.showNodeConnections = GUILayout.Toggle(graph.showNodeConnections, new GUIContent("Show connections", "Toggles gizmos for drawing node connections"), EditorStyles.miniButtonRight);
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Advanced"), EditorStyles.boldLabel);

			DrawPenaltyModifications(graph);
			DrawJPS(graph);
		}

		void DrawPenaltyModifications (GridGraph graph) {
			showExtra = EditorGUILayout.Foldout(showExtra, "Penalty Modifications");

			if (showExtra) {
				EditorGUI.indentLevel += 2;

				graph.penaltyAngle = ToggleGroup(new GUIContent("Angle Penalty", "Adds a penalty based on the slope of the node"), graph.penaltyAngle);
				if (graph.penaltyAngle) {
					EditorGUI.indentLevel++;
					graph.penaltyAngleFactor = EditorGUILayout.FloatField(new GUIContent("Factor", "Scale of the penalty. A negative value should not be used"), graph.penaltyAngleFactor);
					graph.penaltyAnglePower = EditorGUILayout.Slider("Power", graph.penaltyAnglePower, 0.1f, 10f);
					EditorGUILayout.HelpBox("Applies penalty to nodes based on the angle of the hit surface during the Height Testing\nPenalty applied is: P=(1-cos(angle)^power)*factor.", MessageType.None);

					EditorGUI.indentLevel--;
				}

				graph.penaltyPosition = ToggleGroup("Position Penalty", graph.penaltyPosition);
				if (graph.penaltyPosition) {
					EditorGUI.indentLevel++;
					graph.penaltyPositionOffset = EditorGUILayout.FloatField("Offset", graph.penaltyPositionOffset);
					graph.penaltyPositionFactor = EditorGUILayout.FloatField("Factor", graph.penaltyPositionFactor);
					EditorGUILayout.HelpBox("Applies penalty to nodes based on their Y coordinate\nSampled in Int3 space, i.e it is multiplied with Int3.Precision first ("+Int3.Precision+")\n" +
						"Be very careful when using negative values since a negative penalty will underflow and instead get really high", MessageType.None);
					EditorGUI.indentLevel--;
				}

				DrawTextureData(graph.textureData, graph);
				EditorGUI.indentLevel -= 2;
			}
		}

		protected virtual void DrawJPS (GridGraph graph) {
			graph.useJumpPointSearch = EditorGUILayout.Toggle(new GUIContent("Use Jump Point Search", "Jump Point Search can significantly speed up pathfinding. But only works on uniformly weighted graphs"), graph.useJumpPointSearch);
			if (graph.useJumpPointSearch) {
				EditorGUILayout.HelpBox("Jump Point Search assumes that there are no penalties applied to the graph. Tag penalties cannot be used either.", MessageType.Warning);

#if !ASTAR_JPS
				EditorGUILayout.HelpBox("JPS needs to be enabled using a compiler directive before it can be used.\n" +
					"Enabling this will add ASTAR_JPS to the Scriping Define Symbols field in the Unity Player Settings", MessageType.Warning);
				if (GUILayout.Button("Enable Jump Point Search support")) {
					OptimizationHandler.EnableDefine("ASTAR_JPS");
				}
#endif
			} else {
#if ASTAR_JPS
				EditorGUILayout.HelpBox("If you are not using JPS in any scene, you can disable it to save memory", MessageType.Info);
				if (GUILayout.Button("Disable Jump Point Search support")) {
					OptimizationHandler.DisableDefine("ASTAR_JPS");
				}
#endif
			}
		}

		/** Draws the inspector for a \link Pathfinding.GraphCollision GraphCollision class \endlink */
		protected virtual void DrawCollisionEditor (GraphCollision collision) {
			collision = collision ?? new GraphCollision();

			DrawUse2DPhysics(collision);

			collision.collisionCheck = ToggleGroup("Collision testing", collision.collisionCheck);
			if (collision.collisionCheck) {
				collision.type = (ColliderType)EditorGUILayout.EnumPopup("Collider type", collision.type);
				if (collision.use2D && collision.type == ColliderType.Capsule) {
					EditorGUILayout.HelpBox("Capsules cannot be used with 2D physics. Pick some other collider type.", MessageType.Error);
				}

				EditorGUI.BeginDisabledGroup(collision.type != ColliderType.Capsule && collision.type != ColliderType.Sphere);
				collision.diameter = EditorGUILayout.FloatField(new GUIContent("Diameter", "Diameter of the capsule or sphere. 1 equals one node width"), collision.diameter);
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(collision.type != ColliderType.Capsule && collision.type != ColliderType.Ray);
				collision.height = EditorGUILayout.FloatField(new GUIContent("Height/Length", "Height of cylinder or length of ray in world units"), collision.height);
				EditorGUI.EndDisabledGroup();

				collision.collisionOffset = EditorGUILayout.FloatField(new GUIContent("Offset", "Offset upwards from the node. Can be used so that obstacles can be used as ground and at the same time as obstacles for lower positioned nodes"), collision.collisionOffset);

				collision.mask = EditorGUILayoutx.LayerMaskField("Mask", collision.mask);
			}

			GUILayout.Space(2);

			if (collision.use2D) {
				EditorGUI.BeginDisabledGroup(collision.use2D);
				ToggleGroup("Height testing", false);
				EditorGUI.EndDisabledGroup();
			} else {
				collision.heightCheck = ToggleGroup("Height testing", collision.heightCheck);
				if (collision.heightCheck) {
					collision.fromHeight = EditorGUILayout.FloatField(new GUIContent("Ray length", "The height from which to check for ground"), collision.fromHeight);

					collision.heightMask = EditorGUILayoutx.LayerMaskField("Mask", collision.heightMask);

					collision.thickRaycast = EditorGUILayout.Toggle(new GUIContent("Thick Raycast", "Use a thick line instead of a thin line"), collision.thickRaycast);

					if (collision.thickRaycast) {
						EditorGUI.indentLevel++;
						collision.thickRaycastDiameter = EditorGUILayout.FloatField(new GUIContent("Diameter", "Diameter of the thick raycast"), collision.thickRaycastDiameter);
						EditorGUI.indentLevel--;
					}

					collision.unwalkableWhenNoGround = EditorGUILayout.Toggle(new GUIContent("Unwalkable when no ground", "Make nodes unwalkable when no ground was found with the height raycast. If height raycast is turned off, this doesn't affect anything"), collision.unwalkableWhenNoGround);
				}
			}
		}

		protected virtual void DrawUse2DPhysics (GraphCollision collision) {
			collision.use2D = EditorGUILayout.Toggle(new GUIContent("Use 2D Physics", "Use the Physics2D API for collision checking"), collision.use2D);
		}

		static void SaveReferenceTexture (GridGraph graph) {
			if (graph.nodes == null || graph.nodes.Length != graph.width * graph.depth) {
				AstarPath.active.Scan();
			}

			if (graph.nodes.Length != graph.width * graph.depth) {
				Debug.LogError("Couldn't create reference image since width*depth != nodes.Length");
				return;
			}

			if (graph.nodes.Length == 0) {
				Debug.LogError("Couldn't create reference image since the graph is too small (0*0)");
				return;
			}

			var tex = new Texture2D(graph.width, graph.depth);

			float maxY = float.NegativeInfinity;
			for (int i = 0; i < graph.nodes.Length; i++) {
				Vector3 p = graph.transform.InverseTransform((Vector3)graph.nodes[i].position);
				maxY = p.y > maxY ? p.y : maxY;
			}

			var cols = new Color[graph.width*graph.depth];

			for (int z = 0; z < graph.depth; z++) {
				for (int x = 0; x < graph.width; x++) {
					GraphNode node = graph.nodes[z*graph.width+x];
					float v = node.Walkable ? 1F : 0.0F;
					Vector3 p = graph.transform.InverseTransform((Vector3)node.position);
					float q = p.y / maxY;
					cols[z*graph.width+x] = new Color(v, q, 0);
				}
			}
			tex.SetPixels(cols);
			tex.Apply();

			string path = AssetDatabase.GenerateUniqueAssetPath("Assets/gridReference.png");

			using (var outstream = new System.IO.StreamWriter(path)) {
				using (var outfile = new System.IO.BinaryWriter(outstream.BaseStream)) {
					outfile.Write(tex.EncodeToPNG());
				}
			}
			AssetDatabase.Refresh();
			Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Texture));

			EditorGUIUtility.PingObject(obj);
		}

		protected static readonly string[] ChannelUseNames = { "None", "Penalty", "Height", "Walkability and Penalty" };

		/** Draws settings for using a texture as source for a grid.
		 * \astarpro
		 */
		protected virtual void DrawTextureData (GridGraph.TextureData data, GridGraph graph) {
			if (data == null) {
				return;
			}

			data.enabled = ToggleGroup("Use Texture", data.enabled);
			if (!data.enabled) {
				return;
			}

			bool preGUI = GUI.enabled;
			GUI.enabled = data.enabled && GUI.enabled;

			EditorGUI.indentLevel++;
			data.source = ObjectField("Source", data.source, typeof(Texture2D), false) as Texture2D;

			if (data.source != null) {
				string path = AssetDatabase.GetAssetPath(data.source);

				if (path != "") {
					var importer = AssetImporter.GetAtPath(path) as TextureImporter;
					if (!importer.isReadable) {
						if (FixLabel("Texture is not readable")) {
							importer.isReadable = true;
							EditorUtility.SetDirty(importer);
							AssetDatabase.ImportAsset(path);
						}
					}
				}
			}

			for (int i = 0; i < 3; i++) {
				string channelName = i == 0 ? "R" : (i == 1 ? "G" : "B");
				data.channels[i] = (GridGraph.TextureData.ChannelUse)EditorGUILayout.Popup(channelName, (int)data.channels[i], ChannelUseNames);

				if (data.channels[i] != GridGraph.TextureData.ChannelUse.None) {
					EditorGUI.indentLevel++;
					data.factors[i] = EditorGUILayout.FloatField("Factor", data.factors[i]);

					string help = "";
					switch (data.channels[i]) {
					case GridGraph.TextureData.ChannelUse.Penalty:
						help = "Nodes are applied penalty according to channel '"+channelName+"', multiplied with factor";
						break;
					case GridGraph.TextureData.ChannelUse.Position:
						help = "Nodes Y position is changed according to channel '"+channelName+"', multiplied with factor";

						if (graph.collision.heightCheck) {
							EditorGUILayout.HelpBox("Getting position both from raycast and from texture. You should disable one of them", MessageType.Error);
						}
						break;
					case GridGraph.TextureData.ChannelUse.WalkablePenalty:
						help = "If channel '"+channelName+"' is 0, the node is made unwalkable. Otherwise the node is applied penalty multiplied with factor";
						break;
					}

					EditorGUILayout.HelpBox(help, MessageType.None);

					EditorGUI.indentLevel--;
				}
			}

			if (GUILayout.Button("Generate Reference")) {
				SaveReferenceTexture(graph);
			}

			GUI.enabled = preGUI;
			EditorGUI.indentLevel--;
		}

		public static GridPivot PivotPointSelector (GridPivot pivot) {
			// Find required styles
			gridPivotSelectBackground = gridPivotSelectBackground ?? AstarPathEditor.astarSkin.FindStyle("GridPivotSelectBackground");
			gridPivotSelectButton = gridPivotSelectButton ?? AstarPathEditor.astarSkin.FindStyle("GridPivotSelectButton");

			Rect r = GUILayoutUtility.GetRect(19, 19, gridPivotSelectBackground);

			// I have no idea why... but this is required for it to work well
			r.y -= 14;

			r.width = 19;
			r.height = 19;

			if (gridPivotSelectBackground == null) {
				return pivot;
			}

			if (Event.current.type == EventType.Repaint) {
				gridPivotSelectBackground.Draw(r, false, false, false, false);
			}

			if (GUI.Toggle(new Rect(r.x, r.y, 7, 7), pivot == GridPivot.TopLeft, "", gridPivotSelectButton))
				pivot = GridPivot.TopLeft;

			if (GUI.Toggle(new Rect(r.x+12, r.y, 7, 7), pivot == GridPivot.TopRight, "", gridPivotSelectButton))
				pivot = GridPivot.TopRight;

			if (GUI.Toggle(new Rect(r.x+12, r.y+12, 7, 7), pivot == GridPivot.BottomRight, "", gridPivotSelectButton))
				pivot = GridPivot.BottomRight;

			if (GUI.Toggle(new Rect(r.x, r.y+12, 7, 7), pivot == GridPivot.BottomLeft, "", gridPivotSelectButton))
				pivot = GridPivot.BottomLeft;

			if (GUI.Toggle(new Rect(r.x+6, r.y+6, 7, 7), pivot == GridPivot.Center, "", gridPivotSelectButton))
				pivot = GridPivot.Center;

			return pivot;
		}

		static readonly Vector3[] handlePoints = new [] { new Vector3(0.0f, 0, 0.5f), new Vector3(1.0f, 0, 0.5f), new Vector3(0.5f, 0, 0.0f), new Vector3(0.5f, 0, 1.0f) };

		public override void OnSceneGUI (NavGraph target) {
			Event e = Event.current;

			var graph = target as GridGraph;

			graph.UpdateTransform();
			var currentTransform = graph.transform * Matrix4x4.Scale(new Vector3(graph.width, 1, graph.depth));

			if (e.type == EventType.MouseDown) {
				isMouseDown = true;
			} else if (e.type == EventType.MouseUp) {
				isMouseDown = false;
			}

			if (!isMouseDown) {
				savedTransform = currentTransform;
				savedDimensions = new Vector2(graph.width, graph.depth);
				savedNodeSize = graph.nodeSize;
			}

			Handles.matrix = Matrix4x4.identity;
			Handles.color = AstarColor.BoundsHandles;
#if UNITY_5_5_OR_NEWER
			Handles.CapFunction cap = Handles.CylinderHandleCap;
#else
			Handles.DrawCapFunction cap = Handles.CylinderCap;
#endif

			var center = currentTransform.Transform(new Vector3(0.5f, 0, 0.5f));
			if (Tools.current == Tool.Scale) {
				const float HandleScale = 0.1f;

				Vector3 mn = Vector3.zero;
				Vector3 mx = Vector3.zero;
				EditorGUI.BeginChangeCheck();
				for (int i = 0; i < handlePoints.Length; i++) {
					var ps = currentTransform.Transform(handlePoints[i]);
					Vector3 p = savedTransform.InverseTransform(Handles.Slider(ps, ps - center, HandleScale*HandleUtility.GetHandleSize(ps), cap, 0));

					// Snap to increments of whole nodes
					p.x = Mathf.Round(p.x * savedDimensions.x) / savedDimensions.x;
					p.z = Mathf.Round(p.z * savedDimensions.y) / savedDimensions.y;

					if (i == 0) {
						mn = mx = p;
					} else {
						mn = Vector3.Min(mn, p);
						mx = Vector3.Max(mx, p);
					}
				}

				if (EditorGUI.EndChangeCheck()) {
					graph.center = savedTransform.Transform((mn + mx) * 0.5f);
					graph.unclampedSize = Vector2.Scale(new Vector2(mx.x - mn.x, mx.z - mn.z), savedDimensions) * savedNodeSize;
				}
			} else if (Tools.current == Tool.Move) {
				EditorGUI.BeginChangeCheck();
				center = Handles.PositionHandle(graph.center, Quaternion.identity);

				if (EditorGUI.EndChangeCheck() && Tools.viewTool != ViewTool.Orbit) {
					graph.center = center;
				}
			} else if (Tools.current == Tool.Rotate) {
				EditorGUI.BeginChangeCheck();
				var rot = Handles.RotationHandle(Quaternion.Euler(graph.rotation), graph.center);

				if (EditorGUI.EndChangeCheck() && Tools.viewTool != ViewTool.Orbit) {
					graph.rotation = rot.eulerAngles;
				}
			}

			Handles.matrix = Matrix4x4.identity;
		}

		public enum GridPivot {
			Center,
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}
	}
}
