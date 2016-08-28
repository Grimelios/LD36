using System;
using System.Collections.Generic;
using LD36.Entities;
using LD36.Structures;
using Microsoft.Xna.Framework;

namespace LD36.Generation
{
	internal class ArtifactGenerator
	{
		private const int TotalArtifacts = 3;
		private const int ArtifactOffset = 20;

		private int scaleMultiplier;

		private GraphNode<Rectangle> roomGraph;

		public ArtifactGenerator(GraphNode<Rectangle> roomGraph, int scaleMultiplier)
		{
			this.roomGraph = roomGraph;
			this.scaleMultiplier = scaleMultiplier;
		}

		public void Generate()
		{
			List<Rectangle> leafRooms = GetLeafRooms();
			Random random = new Random();

			for (int i = 0; i < TotalArtifacts; i++)
			{
				int index = random.Next(leafRooms.Count);

				Rectangle room = leafRooms[index];
				leafRooms.RemoveAt(index);

				Vector2 artifactPosition = new Vector2(room.Left + room.Width / 2, room.Bottom) * Constants.TileSize * scaleMultiplier;
				artifactPosition.Y -= ArtifactOffset;
				EntityUtilities.AddEntity(new Artifact(artifactPosition));
			}
		}

		private List<Rectangle> GetLeafRooms()
		{
			// I'm assuming all nodes start unvisited.
			List<Rectangle> leafRooms = new List<Rectangle>();
			Queue<GraphNode<Rectangle>> nodeQueue = new Queue<GraphNode<Rectangle>>();
			nodeQueue.Enqueue(roomGraph);

			while (nodeQueue.Count > 0)
			{
				GraphNode<Rectangle> node = nodeQueue.Dequeue();

				if (!node.Visited)
				{
					List<GraphNode<Rectangle>> neighbors = node.Neighbors;

					if (neighbors.Count == 1 && node.Data != Rectangle.Empty)
					{
						leafRooms.Add(node.Data);
					}

					neighbors.ForEach(n => nodeQueue.Enqueue(n));
					node.Visited = true;
				}
			}

			return leafRooms;
		}
	}
}
