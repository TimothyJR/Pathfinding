using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
	private PriorityNode<T> root;
	public int Count { get; set; }

	public void Enqueue(T item, float priority)
	{
		// Add item to queue
		if(root == null)
		{
			root = new PriorityNode<T>(item, priority);
		}
		else
		{
			FindNodePosition(item, priority, root);
		}

		Count++;
	}

	public void FindNodePosition(T item, float priority, PriorityNode<T> node)
	{
		if(node.PriorityValue > priority)
		{
			// New node is of higher priority
			if(node.Left != null)
			{
				FindNodePosition(item, priority, node.Left);
			}
			else
			{
				node.Left = new PriorityNode<T>(item, priority);
			}
		}
		else
		{
			// New node is of lower or same priority
			if(node.Right != null)
			{
				FindNodePosition(item, priority, node.Right);
			}
			else
			{
				node.Right = new PriorityNode<T>(item, priority);
			}
		}
	}

	public T Dequeue()
	{
		if(root == null)
		{
			return default(T);
		}

		// Find the left most node (Highest priority)
		PriorityNode<T> current = root;
		PriorityNode<T> previous = null;
		while(current.Left != null)
		{
			previous = current;
			current = current.Left;
		}

		// Remove the node
		if(previous == null)
		{
			// The root is being dequeued
			// Switch root to right if it exists
			if(current.Right != null)
			{
				root = current.Right;
			}
			else
			{
				root = null;
			}
		}
		else
		{
			// If there exists a right node to the one being removed
			// Move it to connect to the upper part of the tree
			if(current.Right != null)
			{
				previous.Left = current.Right;
			}
			else
			{
				previous.Left = null;
			}
		}

		Count--;

		return current.Value;
	}

}

public class PriorityNode<T>
{
	private PriorityNode<T> left;
	public PriorityNode<T> Left { get => left; set => left = value; }

	private PriorityNode<T> right;
	public PriorityNode<T> Right { get => right; set => right = value; }



	private float priorityValue;
	public float PriorityValue { get => priorityValue; }

	private T value;
	public T Value { get => value; }

	public PriorityNode(T item, float priority)
	{
		value = item;
		priorityValue = priority;
	}
}