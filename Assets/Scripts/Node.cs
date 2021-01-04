using UnityEngine;

[System.Serializable]
public class Node
{
	/// <summary>
	/// Information for this node
	/// </summary>
	[SerializeField]
	private NodePoint thisNode = null;
	public NodePoint ThisNode { get => thisNode; set => thisNode = value; }

	/// <summary>
	/// Information for node to the left
	/// </summary>
	[SerializeField]
	private NodePoint left = null;
	public NodePoint Left { get => left; set => left = value; }

	/// <summary>
	/// Information for node to the right
	/// </summary>
	[SerializeField]
	private NodePoint right = null;
	public NodePoint Right { get => right; set => right = value; }

	/// <summary>
	/// Information for node above
	/// </summary>
	[SerializeField]
	private NodePoint up = null;
	public NodePoint Up { get => up; set => up = value; }

	/// <summary>
	/// Information for node below
	/// </summary>
	[SerializeField]
	private NodePoint down = null;
	public NodePoint Down { get => down; set => down = value; }

	/// <summary>
	/// Whether this node can be walked on
	/// </summary>
	[SerializeField]
	private bool traversable = true;
	public bool Traversable { get => traversable; set => traversable = value; }

	/// <summary>
	/// How much it costs to go onto this node
	/// </summary>
	[SerializeField]
	private int travelCost = 1;
	public int TravelCost { get => travelCost; set => travelCost = value; }

	/// <summary>
	/// The game object that is displayed with this node
	/// </summary>
	[SerializeField]
	private GameObject spawnedTile;
	public GameObject SpawnedTile { get => spawnedTile; set => spawnedTile = value; }

	/// <summary>
	/// The node that leads back to the starting node for pathfinding
	/// This could be moved into the actual pathfinding algorithm
	/// </summary>
	private Node pathfindingNode = null;
	public Node PathfindingNode { get => pathfindingNode; set => pathfindingNode = value; }

	/// <summary>
	/// The cost to go from this node to the start
	/// This could be moved into the actual pathfinding algorithm
	/// </summary>
	private int costToStart = int.MaxValue;
	public int CostToStart { get => costToStart; set => costToStart = value; }
}

/// <summary>
/// Helper class to contain some information about nodes
/// </summary>
[System.Serializable]
public class NodePoint
{
	/// <summary>
	/// X position of a node
	/// </summary>
	[SerializeField]
	public int x;

	/// <summary>
	/// Y position of a node
	/// </summary>
	[SerializeField]
	public int y;

	/// <summary>
	/// Since the editor will initialize all NodePoints,
	/// this will be used to tell if there is actually a node connected
	/// This could be used to make one way travel
	/// </summary>
	[SerializeField]
	public bool connected = true;

	/// <summary>
	/// Initialize
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public NodePoint(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}