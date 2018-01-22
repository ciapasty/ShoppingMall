using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area {

	public string type { get; protected set; }
	public Color color { get; protected set; }

	public Area(string type, Color color) {
		this.type = type;
		this.color = color;
	}

}
