using UnityEngine;
using System.Collections.Generic;

public class Path_TileGraph {

	public Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph(World world) {

		nodes = new Dictionary<Tile, Path_Node<Tile>>();

		for (int x = 0; x < world.width; x++) {
			for (int y = 0; y < world.height; y++) {

				Tile t = world.getTileAt(x, y);

				//if (t.movementCost > 0) { // Tiles with cost 0 are UNWALKABLE
					Path_Node<Tile> n = new Path_Node<Tile>();
					n.data = t;
					nodes.Add(t, n);
				//}
			}
		}

		foreach (Tile t in nodes.Keys) {
			Path_Node<Tile> n = nodes[t];
			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

			Tile[] neighbours = t.getNeighbours(true); // NOTE: some of the nieghbour could be null!

			for (int i = 0; i < neighbours.Length; i++) {
				if (neighbours[i] != null && neighbours[i].movementCost > 0) {
					if (IsClippingCorner(t, neighbours[i])) {
						continue;
					}

					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.cost = neighbours[i].movementCost;
					e.node = nodes[neighbours[i]];
					edges.Add(e);
				}
			}

			n.edges = edges.ToArray();
		}
	}

	bool IsClippingCorner(Tile curr, Tile neigh) {
		int dX = curr.x - neigh.x;
		int dY = curr.y - neigh.y;

		if (Mathf.Abs(dX)+Mathf.Abs(dY) == 2) {
			if (curr.world.getTileAt(curr.x-dX, curr.y).movementCost < 0) {
				return true;
			}
			if (curr.world.getTileAt(curr.x, curr.y-dY).movementCost < 0) {
				return true;
			}
		}

		return false;
	}
}
