using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }

	public int width = 100;
	public int height = 100;

	Dictionary<Character, GameObject> characters;
	Dictionary<Supply, GameObject> supplyGameObjects;

	float deliveryTimer = 3f;

	void OnEnable() {
		characters = new Dictionary<Character, GameObject>();
		supplyGameObjects = new Dictionary<Supply, GameObject>();

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

		// TEMP -> move to SupplyManager
		if (deliveryTimer > 0) {
			deliveryTimer -= Time.deltaTime;
		} else {
			deliverSupplies();
			deliveryTimer = 3f;
		}
	}

	void onTileChanged(Tile tile) {
		FindObjectOfType<MapSpriteController>().updateTileGraphics(tile);
	}

	void onCharacterSpawned(Character c) {
		// TEMP
		string prefab = "prefabs/worker_"+Random.Range(1,3);
//		if (c.weight > 90) {
//			prefab = "prefabs/character_3";
//		}

		GameObject charGO = (GameObject)Instantiate(Resources.Load(prefab));
		charGO.transform.position = new Vector3(c.currTile.x+0.5f, c.currTile.y+0.5f, 0f);
		charGO.GetComponent<WorkerController>().setCharacter(c); 

		c.registerPickupSupplyCallback(pickupSupply);
		c.registerSupplyUsedCallback(supplyUsed);
		c.registerDropSupplyCallback(dropSupply);

		characters.Add(c, charGO);
	}

	// TEMP box spawn
	void deliverSupplies() {
		if (world.jobQueue.pendingJobs.Count > 0) {
			foreach (var job in world.jobQueue.pendingJobs.ToArray()) {
				if (job.supply != null) {
					Tile tile = world.getEmptyTile();

					GameObject supGO= new GameObject();
					supGO.transform.SetParent(this.transform);
					supGO.transform.position = new Vector3(tile.x+0.5f, tile.y+0.5f, 0);
					SpriteRenderer supSR = supGO.AddComponent<SpriteRenderer>();
					supSR.sprite = Resources.Load<Sprite>("sprites/supply/box");

					supplyGameObjects.Add(job.supply, supGO);

					job.supply.setTile(tile);
				}
			}
		}
	}

	void pickupSupply(Character c) {
		GameObject supGO = supplyGameObjects[c.job.supply];
		supGO.transform.SetParent(characters[c].transform);
		supGO.transform.localPosition = new Vector3(0, 0.45f, 0);
		supGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	void supplyUsed(Character c) {
		GameObject supGO = supplyGameObjects[c.job.supply];
		supplyGameObjects.Remove(c.job.supply);
		Destroy(supGO);
	}

	void dropSupply(Character c) {
		GameObject supGO = supplyGameObjects[c.job.supply];
		supGO.transform.SetParent(this.transform);
		supGO.transform.position = new Vector3(c.currTile.x+0.5f, c.currTile.y+0.5f, 0);
		c.job.supply.setTile(c.currTile);
	}
}
