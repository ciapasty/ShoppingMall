using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf{ 

	public string type { get; protected set; }
	public Area area { get; protected set; }

	public Shelf(string type) {
		this.type = type;
	}

	public void setArea(Area area) {
		this.area = area;
	}

	public void postJob(Tile tile) {
		
	}
}
