using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	public int width = 20;
	public int height = 20;

	List<GameObject> characters;

	void OnEnable() {
		characters = new List<GameObject>();

		if(Instance != null) {
			Debug.LogError("There should never be two world controllers.");
		}
		Instance = this;

		world = new World(width, height);
		world.registerTileChangedCallback(onTileChanged);
		world.registerCharacterSpawnedCallback(onCharacterSpawned);
	}

	public World world { get; protected set; }

	// Use this for initialization
	void Start () {
		Camera.main.transform.position = new Vector3(world.width/2, world.height/2, Camera.main.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void onTileChanged(Tile tile) {
		FindObjectOfType<MapSpriteController>().updateTileGraphics(tile);
	}

	void onCharacterSpawned(Character c) {
		string prefab = "prefabs/worker_"+Random.Range(1,3);
//		if (c.weight > 90) {
//			prefab = "prefabs/character_3";
//		}

		GameObject charGO = (GameObject)Instantiate(Resources.Load(prefab));
		charGO.transform.position = new Vector3(c.currTile.x+0.5f, c.currTile.y+0.5f, 0f);
		charGO.GetComponent<CharacterMove>().setCharacter(c); 

		characters.Add(charGO);
	}
}
