using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour {

	WorldController wc;

	Rigidbody2D body;
	public float forceRate = 1000;
	public float _randomForceFactor = 8;
	float randomForceFactor;
	//Vector2 movementDir;

	Animator anim;

	public Character cr;

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;

		body = GetComponent<Rigidbody2D>();
		body.mass = cr.weight;

		anim = GetComponent<Animator>();

		randomForceFactor = _randomForceFactor;
	}
	
	// Update is called once per frame
	void Update () {
		if (cr.nextTile != null) {
			body.AddForce(getMoveDirection(gameObject.transform.position, cr.nextTile)*forceRate*cr.speed);
		}
		cr.updateCurrentTile(wc.world.getTileAtPosition(gameObject.transform.position), Time.deltaTime);

		if (body.velocity.magnitude > 0.5f) {
			//Debug.Log(body.velocity.magnitude);
			anim.SetFloat("speed", body.velocity.magnitude/3);
		}

		anim.SetBool("isWalking", (body.velocity.x != 0 || body.velocity.y != 0));
		GetComponent<SpriteRenderer>().flipX = (body.velocity.x < -0.5f);
	}

	void OnCollisionEnter2D(Collision2D col) {
		//Vector2 towardsCollision = new Vector2(col.transform.position.x-gameObject.transform.position.x, col.transform.position.y-gameObject.transform.position.y);

		//towardsCollision.x += Random.Range(-randomForceFactor, randomForceFactor)/10f;
		//towardsCollision.y += Random.Range(-randomForceFactor, randomForceFactor)/10f;

		Vector2 randomVector = new Vector2(Random.Range(-randomForceFactor, randomForceFactor)/2, Random.Range(-randomForceFactor, randomForceFactor)/2);

		body.AddForce(randomVector.normalized*forceRate*cr.speed);
	}

	void OnCollisionStay2D(Collision2D col) {
		//Vector2 towardsCollision = new Vector2(col.transform.position.x-gameObject.transform.position.x, col.transform.position.y-gameObject.transform.position.y);

		//towardsCollision.x += Random.Range(-randomForceFactor, randomForceFactor)/10f;
		//towardsCollision.y += Random.Range(-randomForceFactor, randomForceFactor)/10f;

		Vector2 randomVector = new Vector2(Random.Range(-randomForceFactor, randomForceFactor)/2, Random.Range(-randomForceFactor, randomForceFactor)/2);

		body.AddForce(randomVector.normalized*forceRate*cr.speed);
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
		cr = c;
	}
}
