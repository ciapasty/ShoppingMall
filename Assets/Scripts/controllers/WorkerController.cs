using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerController : MonoBehaviour {

	WorldController wc;

	Rigidbody2D body;
	float neighboursRadius = 1.5f;
	float minCharacterDistance = 0.4f;
	float forceRate = 1000;
	float randomForceFactor = 3;
	public float speed = 1f;

	Animator anim;

	public Character character;

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;

		body = GetComponent<Rigidbody2D>();
		body.mass = character.weight;

		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		doMovement();
	}

	void doMovement() {
		Vector3 v = Vector3.zero;
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(GetComponent<Renderer>().bounds.center, neighboursRadius);
		foreach (var collider in hitColliders) {
			if (collider.gameObject.tag == "Character") {
				if (collider.gameObject != gameObject) {
					int count = 0;
					float d = Vector2.Distance(collider.transform.position, transform.position);
					if (d > 0f && d < minCharacterDistance) {
						v += (collider.transform.position-transform.position).normalized/d;
						count++;
					}
					if (count > 0) {
						v /= count;
					}
				}
			}
		}

		Vector2 force = -(new Vector2(v.x, v.y)).normalized;

		if (character.nextTile != null) {
			force += getMoveDirection(gameObject.transform.position, character.nextTile);
		}
		Tile currTile = wc.world.getTileAtPosition(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+0.1f, 0));
		character.updatePosition(currTile, Time.deltaTime);

		if (body.velocity.magnitude > 0.5f) {
			anim.SetFloat("speed", body.velocity.magnitude/3);
		}

		body.AddForce( force*forceRate * (speed/currTile.movementCost) );
		anim.SetBool("isWalking", (body.velocity.x != 0 || body.velocity.y != 0));
		GetComponent<SpriteRenderer>().flipX = (body.velocity.x < -0.2f);
	}

	Vector2 getMoveDirection(Vector3 position, Tile tile2) {
		Vector2 dir = new Vector2(tile2.x+0.5f-position.x, tile2.y+0.5f-position.y);
		dir.Normalize();

		// Randomize movement
		dir.x += Random.Range(-randomForceFactor, randomForceFactor)/10f;
		dir.y += Random.Range(-randomForceFactor, randomForceFactor)/10f;

		return dir;
	}

	public void setCharacter(Character c) {
		character = c;
	}
}
