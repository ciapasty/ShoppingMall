using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeController : MonoBehaviour {

	WorldController wc;

	public bool isBuildModeEnabled { get; protected set; }
	public TileType buildType { get; protected set; }

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startBuildModeWith(string type) {
		isBuildModeEnabled = true;
		switch (type) {
		case "wall":
			buildType = TileType.wall;
			break;
		case "demolish":
			buildType = TileType.floor;
			break;
		default:
			buildType = TileType.empty;
			isBuildModeEnabled = false;
			break;
		}
	}

	public void cancelBuildMode() {
		isBuildModeEnabled = false;
	}

}
