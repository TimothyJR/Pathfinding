using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
	[SerializeField]
	private NodePoint thisNode = null;
	public NodePoint ThisNode { get => thisNode; set => thisNode = value; }

	[SerializeField]
	private NodePoint left = null;
	public NodePoint Left { get => left; set => left = value; }

	[SerializeField]
	private NodePoint right = null;
	public NodePoint Right { get => right; set => right = value; }

	[SerializeField]
	private NodePoint up = null;
	public NodePoint Up { get => up; set => up = value; }

	[SerializeField]
	private NodePoint down = null;
	public NodePoint Down { get => down; set => down = value; }

	[SerializeField]
	private bool traversable = true;
	public bool Traversable { get => traversable; set => traversable = value; }

	[SerializeField]
	private int travelCost = 1;
	public int TravelCost { get => travelCost; set => travelCost = value; }

	[SerializeField]
	private GameObject spawnedTile;
	public GameObject SpawnedTile { get => spawnedTile; set => spawnedTile = value; }

	private Node pathfindingNode = null;
	public Node PathfindingNode { get => pathfindingNode; set => pathfindingNode = value; }

	private int costToStart = int.MaxValue;
	public int CostToStart { get => costToStart; set => costToStart = value; }
}

[System.Serializable]
public class NodePoint
{
	[SerializeField]
	public int x;

	[SerializeField]
	public int y;

	[SerializeField]
	public bool connected = true;

	public NodePoint(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}