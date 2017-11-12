using UnityEngine;
using System.Collections;

public class Path_Edge<T> {
	public float cost; // Cost to raverse this edge, i.e. cost to enter the tile
	public Path_Node<T> node;
}
