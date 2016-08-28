using System.Collections.Generic;

namespace LD36.Structures
{
	internal class GraphNode<T>
	{
		public GraphNode(T data)
		{
			Data = data;
			Neighbors = new List<GraphNode<T>>();
		}

		public T Data { get; }

		public List<GraphNode<T>> Neighbors { get; }

		public bool Visited { get; set; }
	}
}
