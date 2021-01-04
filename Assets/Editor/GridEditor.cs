using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeGrid))]
public class GridEditor : Editor
{
	private bool init = false;
	private int width = 1;
	private int height = 1;
	private Vector2 traversabilityPosition;
	private Vector2 travelCostPosition;

	public override void OnInspectorGUI()
	{
		NodeGrid grid = (NodeGrid)target;

		if(!init)
		{
			width = grid.Width;
			height = grid.Height;
		}


		grid.NodePrefab = EditorGUILayout.ObjectField("Node Prefab", grid.NodePrefab, typeof(GameObject), false) as GameObject;
		grid.PathBaseMaterial = EditorGUILayout.ObjectField("Base Material", grid.PathBaseMaterial, typeof(Material), false) as Material;
		grid.PathNotTraversableMaterial = EditorGUILayout.ObjectField("Not Traversable Material", grid.PathNotTraversableMaterial, typeof(Material), false) as Material;
		grid.PathCheckedMaterial = EditorGUILayout.ObjectField("Checked Material", grid.PathCheckedMaterial, typeof(Material), false) as Material;
		grid.PathGoalMaterial = EditorGUILayout.ObjectField("Goal Material", grid.PathGoalMaterial, typeof(Material), false) as Material;
		grid.PathFinalMaterial = EditorGUILayout.ObjectField("Final Path Material", grid.PathFinalMaterial, typeof(Material), false) as Material;
		grid.PathStartMaterial = EditorGUILayout.ObjectField("Start Material", grid.PathStartMaterial, typeof(Material), false) as Material;
		width = Mathf.Max(EditorGUILayout.DelayedIntField("X Dimension:", width), 1);
		height = Mathf.Max(EditorGUILayout.DelayedIntField("Y Dimension:", height), 1);

		if(width != grid.Width || height != grid.Height)
		{
			grid.AlterSize(width, height);

			EditorUtility.SetDirty(target);
		}


		traversabilityPosition = EditorGUILayout.BeginScrollView(traversabilityPosition);
		EditorGUILayout.LabelField("Traversability");
		EditorGUILayout.BeginVertical();
		for (int i = 0; i < grid.Height; i++)
		{
			EditorGUILayout.BeginHorizontal();
			for (int j = 0; j < grid.Width; j++)
			{
				grid.GetNode(j, i).Traversable = EditorGUILayout.Toggle(grid.GetNode(j, i).Traversable);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();


		travelCostPosition = EditorGUILayout.BeginScrollView(travelCostPosition);
		EditorGUILayout.LabelField("Travel Cost");
		EditorGUILayout.BeginVertical();
		for (int i = 0; i < grid.Height; i++)
		{
			EditorGUILayout.BeginHorizontal();
			for (int j = 0; j < grid.Width; j++)
			{
				if(grid.GetNode(j, i).Traversable)
				{
					grid.GetNode(j, i).TravelCost = EditorGUILayout.IntField(grid.GetNode(j, i).TravelCost);
				}
				else
				{
					GUI.enabled = false;
					grid.GetNode(j, i).TravelCost = EditorGUILayout.IntField(grid.GetNode(j, i).TravelCost);
					GUI.enabled = true;
				}

			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}