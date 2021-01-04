using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class NodeGrid : MonoBehaviour
{
	[SerializeField]
	private List<Node> grid = new List<Node>();
	public List<Node> Grid { set => grid = value; }

	[SerializeField]
	private int height = -1;
	public int Height { get => height; }

	[SerializeField]
	private int width = -1;
	public int Width { get => width; }

	[SerializeField]
	private GameObject nodePrefab = null;
	public GameObject NodePrefab { get { return nodePrefab; } set { nodePrefab = value; } }

	[SerializeField]
	private Material pathBaseMaterial;
	public Material PathBaseMaterial { get { return pathBaseMaterial; } set { pathBaseMaterial = value; } }

	[SerializeField]
	private Material pathNotTraversableMaterial;
	public Material PathNotTraversableMaterial { get { return pathNotTraversableMaterial; } set { pathNotTraversableMaterial = value; } }

	[SerializeField]
	private Material pathCheckedMaterial;
	public Material PathCheckedMaterial { get { return pathCheckedMaterial; } set { pathCheckedMaterial = value; } }

	[SerializeField]
	private Material pathGoalMaterial;
	public Material PathGoalMaterial { get { return pathGoalMaterial; } set { pathGoalMaterial = value; } }

	[SerializeField]
	private Material pathFinalMaterial;
	public Material PathFinalMaterial { get { return pathFinalMaterial; } set { pathFinalMaterial = value; } }

	[SerializeField]
	private Material pathStartMaterial;
	public Material PathStartMaterial { get { return pathStartMaterial; } set { pathStartMaterial = value; } }

	private bool searching = false;

	private float checkTime = 0.1f;

	private IEnumerator search;

	// Start is called before the first frame update
	private void Start()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Node n = GetNode(i, j);
				n.SpawnedTile = GameObject.Instantiate(nodePrefab, new Vector3(i, 0.0f, -j), Quaternion.identity);
				n.SpawnedTile.transform.name = $"Tile ({i}, {j})";

				if(n.Traversable)
				{
					n.SpawnedTile.GetComponent<MeshRenderer>().material = pathBaseMaterial;
					n.SpawnedTile.transform.GetChild(0).GetComponent<TextMeshPro>().text = n.TravelCost.ToString();
				}
				else
				{
					n.SpawnedTile.GetComponent<MeshRenderer>().material = pathNotTraversableMaterial;
					n.SpawnedTile.transform.GetChild(0).gameObject.SetActive(false);
				}
			}
		}
	}

	private void Update()
	{
		// Only start a search if there are no searches currently
		if(!searching)
		{
			if (Input.GetKey(KeyCode.Q))
			{
				// Breadth First
				search = BreadthFirstSearch(grid[0], grid[grid.Count - 1]);
				StartCoroutine(search);
			}
			else if (Input.GetKey(KeyCode.W))
			{
				// Depth First
				search = DepthFirstSearch(grid[0], grid[grid.Count - 1]);
				StartCoroutine(search);
			}
			else if(Input.GetKey(KeyCode.E))
			{
				// Dijkstra's
				search = DijkstraSearch(grid[0], grid[grid.Count - 1]);
				StartCoroutine(search);
			}
			else if (Input.GetKey(KeyCode.R))
			{
				// A*
				search = AStar(grid[0], grid[grid.Count - 1]);
				StartCoroutine(search);
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.T))
			{
				// Reset Graph
				searching = false;
				for(int i = 0; i < grid.Count; i++)
				{
					grid[i].PathfindingNode = null;
					grid[i].CostToStart = int.MaxValue;
					if(grid[i].Traversable)
					{
						grid[i].SpawnedTile.GetComponent<MeshRenderer>().material = pathBaseMaterial;
					}
				}
			}
		}
	}

	IEnumerator BreadthFirstSearch(Node start, Node end)
	{
		searching = true;
		start.SpawnedTile.GetComponent<MeshRenderer>().material = pathStartMaterial;
		end.SpawnedTile.GetComponent<MeshRenderer>().material = pathGoalMaterial;

		// BFS uses Queues
		Queue<Node> nodesToCheck = new Queue<Node>();
		nodesToCheck.Enqueue(start);

		Node n = null;

		while (nodesToCheck.Count > 0)
		{
			n = nodesToCheck.Dequeue();

			if(n == end)
			{
				// Found the target
				// Color the path leading to the target and end
				Debug.Log("Made it to end");
				n = n.PathfindingNode;
				while (n != start)
				{
					n.SpawnedTile.GetComponent<MeshRenderer>().material = pathFinalMaterial;
					n = n.PathfindingNode;
				}

				break;
			}
			else
			{

				if(n != start)
				{
					n.SpawnedTile.GetComponent<MeshRenderer>().material = pathCheckedMaterial;
				}

				// Add all nodes to the queue
				if (n.Right.connected && GetNode(n.Right).Traversable && GetNode(n.Right).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Right)))
				{
					nodesToCheck.Enqueue(GetNode(n.Right));
					GetNode(n.Right).PathfindingNode = n;
				}
				if (n.Down.connected && GetNode(n.Down).Traversable && GetNode(n.Down).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Down)))
				{
					nodesToCheck.Enqueue(GetNode(n.Down));
					GetNode(n.Down).PathfindingNode = n;
				}
				if (n.Left.connected && GetNode(n.Left).Traversable && GetNode(n.Left).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Left)))
				{
					nodesToCheck.Enqueue(GetNode(n.Left));
					GetNode(n.Left).PathfindingNode = n;
				}
				if (n.Up.connected && GetNode(n.Up).Traversable && GetNode(n.Up).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Up)))
				{
					nodesToCheck.Enqueue(GetNode(n.Up));
					GetNode(n.Up).PathfindingNode = n;
				}
			}

			yield return new WaitForSeconds(checkTime);
		}
	}

	IEnumerator DepthFirstSearch(Node start, Node end)
	{
		start.SpawnedTile.GetComponent<MeshRenderer>().material = pathStartMaterial;
		end.SpawnedTile.GetComponent<MeshRenderer>().material = pathGoalMaterial;
		searching = true;
		int[] directionOrder = GetDirection(start, end);

		Stack<Node> nodesToCheck = new Stack<Node>();
		nodesToCheck.Push(start);

		while (nodesToCheck.Count > 0)
		{
			Node n = nodesToCheck.Pop();

			if (n == end)
			{
				// Found the target
				// Color the path leading to the target and end
				Debug.Log("Made it to end");
				n = n.PathfindingNode;
				nodesToCheck.Clear();
				while (n != start)
				{
					n.SpawnedTile.GetComponent<MeshRenderer>().material = pathFinalMaterial;
					n = n.PathfindingNode;
				}

				break;
			}
			else
			{
				// Mark tile as checked
				if (n != start)
				{
					n.SpawnedTile.GetComponent<MeshRenderer>().material = pathCheckedMaterial;
				}
				yield return new WaitForSeconds(checkTime);
				// 0 - Left, 1 - Right, 2 - Up, 3 - Down
				// Go in reverse order to allow our most desired direction to go first
				for (int i = directionOrder.Length - 1; i >= 0; i--)
				{
					switch (directionOrder[i])
					{
						case 0:
							if (n.Left.connected && GetNode(n.Left).Traversable && GetNode(n.Left).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Left)))
							{
								nodesToCheck.Push(GetNode(n.Left));
								GetNode(n.Left).PathfindingNode = n;
							}
							break;
						case 1:
							if (n.Right.connected && GetNode(n.Right).Traversable && GetNode(n.Right).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Right)))
							{
								nodesToCheck.Push(GetNode(n.Right));
								GetNode(n.Right).PathfindingNode = n;
							}
							break;
						case 2:
							if (n.Up.connected && GetNode(n.Up).Traversable && GetNode(n.Up).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Up)))
							{
								nodesToCheck.Push(GetNode(n.Up));
								GetNode(n.Up).PathfindingNode = n;
							}
							break;
						case 3:
							if (n.Down.connected && GetNode(n.Down).Traversable && GetNode(n.Down).PathfindingNode == null && !nodesToCheck.Contains(GetNode(n.Down)))
							{
								nodesToCheck.Push(GetNode(n.Down));
								GetNode(n.Down).PathfindingNode = n;
							}
							break;
					}
				}
			}
		}
	}

	IEnumerator DijkstraSearch(Node start, Node end)
	{
		start.SpawnedTile.GetComponent<MeshRenderer>().material = pathStartMaterial;
		end.SpawnedTile.GetComponent<MeshRenderer>().material = pathGoalMaterial;
		searching = true;
		start.CostToStart = 0;

		int distanceToEnd = int.MaxValue;
		PriorityQueue<Node> nodesToCheck = new PriorityQueue<Node>();
		Node currentNode = null;

		nodesToCheck.Enqueue(start, 0);

		while(nodesToCheck.Count > 0)
		{
			// Get node with lowest cost
			currentNode = nodesToCheck.Dequeue();

			if (currentNode == end)
			{
				// Found the target
				distanceToEnd = currentNode.CostToStart;
			}

			// Mark tile as checked
			if (currentNode != start && currentNode != end)
			{
				currentNode.SpawnedTile.GetComponent<MeshRenderer>().material = pathCheckedMaterial;
			}

			DijkstraNodeCheck(currentNode, currentNode.Right, ref nodesToCheck, distanceToEnd);
			DijkstraNodeCheck(currentNode, currentNode.Left, ref nodesToCheck, distanceToEnd);
			DijkstraNodeCheck(currentNode, currentNode.Up, ref nodesToCheck, distanceToEnd);
			DijkstraNodeCheck(currentNode, currentNode.Down, ref nodesToCheck, distanceToEnd);

			yield return new WaitForSeconds(checkTime);
		}

		while (end != start)
		{
			end = end.PathfindingNode;
			end.SpawnedTile.GetComponent<MeshRenderer>().material = pathFinalMaterial;
		}
	}

	private void DijkstraNodeCheck(Node current, NodePoint check, ref PriorityQueue<Node> nodesToCheck, int distanceToEnd)
	{
		if(check.connected)
		{
			Node checking = GetNode(check);
			if (checking.Traversable &&
				checking.TravelCost + current.CostToStart < checking.CostToStart &&
				checking.TravelCost + current.CostToStart < distanceToEnd)
			{
				checking.CostToStart = current.CostToStart + checking.TravelCost;
				checking.PathfindingNode = current;
				nodesToCheck.Enqueue(checking, checking.CostToStart);
			}
		}
	}

	IEnumerator AStar(Node start, Node end)
	{
		start.SpawnedTile.GetComponent<MeshRenderer>().material = pathStartMaterial;
		end.SpawnedTile.GetComponent<MeshRenderer>().material = pathGoalMaterial;
		searching = true;
		start.CostToStart = 0;

		PriorityQueue<Node> nodesToCheck = new PriorityQueue<Node>();
		Node currentNode = null;

		nodesToCheck.Enqueue(start, 0);

		while (nodesToCheck.Count > 0)
		{
			// Get node with lowest cost
			currentNode = nodesToCheck.Dequeue();

			if (currentNode == end)
			{
				// Found the target
				while (end != start)
				{
					end = end.PathfindingNode;
					end.SpawnedTile.GetComponent<MeshRenderer>().material = pathFinalMaterial;
				}

				break;
			}

			// Mark tile as checked
			if (currentNode != start && currentNode != end)
			{
				currentNode.SpawnedTile.GetComponent<MeshRenderer>().material = pathCheckedMaterial;
			}

			AStarNodeCheck(currentNode, currentNode.Right, ref nodesToCheck, end.ThisNode);
			AStarNodeCheck(currentNode, currentNode.Left, ref nodesToCheck, end.ThisNode);
			AStarNodeCheck(currentNode, currentNode.Up, ref nodesToCheck, end.ThisNode);
			AStarNodeCheck(currentNode, currentNode.Down, ref nodesToCheck, end.ThisNode);

			yield return new WaitForSeconds(checkTime);
		}
	}

	private void AStarNodeCheck(Node current, NodePoint check, ref PriorityQueue<Node> nodesToCheck, NodePoint end)
	{
		if (check.connected)
		{

			Node checking = GetNode(check);
			if (checking.Traversable &&
				checking.TravelCost + current.CostToStart < checking.CostToStart)
			{
				checking.CostToStart = current.CostToStart + checking.TravelCost;
				checking.PathfindingNode = current;
				nodesToCheck.Enqueue(checking, checking.CostToStart + Heuristic(check, end));
			}
		}
	}

	private float Heuristic(NodePoint a, NodePoint b)
	{
		return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
	}

	private int[] GetDirection(Node start, Node end)
	{
		// 0 - Left, 1 - Right, 2 - Up, 3 - Down
		int[] directionOrder = new int[4];

		if (Mathf.Abs(end.ThisNode.x - start.ThisNode.x) < Mathf.Abs(end.ThisNode.y - start.ThisNode.y))
		{
			// X direction is closer than Y
			if (end.ThisNode.x - start.ThisNode.x < 0)
			{
				// Negative X direction
				directionOrder[0] = 0;
				if (end.ThisNode.y - start.ThisNode.y < 0)
				{
					// Negative Y direction
					directionOrder[1] = 2;
					directionOrder[2] = 1;
					directionOrder[3] = 3;
				}
				else
				{
					directionOrder[1] = 3;
					directionOrder[2] = 1;
					directionOrder[3] = 2;
				}
			}
			else
			{
				// Positive X direction
				directionOrder[0] = 1;
				if (end.ThisNode.y - start.ThisNode.y < 0)
				{
					// Negative Y direction
					directionOrder[1] = 2;
					directionOrder[2] = 0;
					directionOrder[3] = 3;
				}
				else
				{
					directionOrder[1] = 3;
					directionOrder[2] = 0;
					directionOrder[3] = 2;
				}
			}
		}
		else
		{
			// Y direction is closer than X
			if (end.ThisNode.y - start.ThisNode.y < 0)
			{
				// Negative Y direction
				directionOrder[0] = 2;
				if (end.ThisNode.x - start.ThisNode.x < 0)
				{
					// Negative X direction
					directionOrder[1] = 0;
					directionOrder[2] = 3;
					directionOrder[3] = 1;
				}
				else
				{
					directionOrder[1] = 1;
					directionOrder[2] = 3;
					directionOrder[3] = 0;
				}
			}
			else
			{
				// Positive Y direction
				directionOrder[0] = 3;
				if (end.ThisNode.x - start.ThisNode.x < 0)
				{
					// Negative X direction
					directionOrder[1] = 0;
					directionOrder[2] = 2;
					directionOrder[3] = 1;
				}
				else
				{
					directionOrder[1] = 1;
					directionOrder[2] = 2;
					directionOrder[3] = 0;
				}
			}
		}

		return directionOrder;
	}


	#region NodeSetUp

	public Node GetNode(int x, int y)
	{
		return (grid[x + y * width]);
	}

	public Node GetNode(NodePoint point)
	{
		return (grid[point.x + point.y * width]);
	}

	public void Clear()
	{
		grid.Clear();
	}

	public void AlterSize(int newWidth, int newHeight)
	{
		List<Node> newGrid = new List<Node>();

		for (int i = 0; i < newHeight; i++)
		{
			for (int j = 0; j < newWidth; j++)
			{
				newGrid.Add(new Node());
			}
		}

		if(grid.Count > 0)
		{
			for (int i = 0; i < Mathf.Min(width, newWidth); i++)
			{
				for (int j = 0; j < Mathf.Min(height, newHeight); j++)
				{
					newGrid[i + newWidth * j] = GetNode(i, j);
				}
			}
		}

		grid = newGrid;

		width = newWidth;
		height = newHeight;

		// Create new connections
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				GetNode(i, j).ThisNode = new NodePoint(i, j);

				// Set left
				GetNode(i, j).Left = new NodePoint(i - 1, j);
				if (!(i > 0))
				{
					GetNode(i, j).Left.connected = false;
				}

				// Set right
				GetNode(i, j).Right = new NodePoint(i + 1, j);
				if (!(i < width - 1))
				{
					GetNode(i, j).Right.connected = false;
				}

				// Set up
				GetNode(i, j).Up = new NodePoint(i, j - 1);
				if (!(j > 0))
				{
					GetNode(i, j).Up.connected = false;
				}

				// Set down
				GetNode(i, j).Down = new NodePoint(i, j + 1);
				if (!(j < height - 1))
				{
					GetNode(i, j).Down.connected = false;
				}
			}
		}

	}

	public int Count()
	{
		return grid.Count;
	}
	#endregion
}

[System.Serializable]
public class NodeRow
{
	[SerializeField]
	private List<Node> nodes = new List<Node>();

	public int Count
	{ get { return nodes.Count; } }

	public Node this[int key]
	{
		get { return nodes[key]; }
		set { nodes[key] = value; }
	}

	public void Add(Node n)
	{
		nodes.Add(n);
	}

	public void RemoveRange(int index, int count)
	{
		nodes.RemoveRange(index, count);
	}
}