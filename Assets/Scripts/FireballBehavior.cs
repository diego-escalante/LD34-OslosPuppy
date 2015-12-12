using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballBehavior : MonoBehaviour {

  [SerializeField] private float speed = 0.5f;
  [SerializeField] private float lifetime = 0.25f;
  [SerializeField] private float damage = 1f;

  private List<GameObject> victims = new List<GameObject>();

  //Static, since we can just share this among all fireballs.
  static private PlayerMovement move;

  //===================================================================================================================

  private void Start() {
    //Set the fireball velocity relative to the player's.
    if(move == null) move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    speed += Mathf.Abs(move.Velocity.x);

    //Add some randomness to the angle.
    float angle = Random.Range(-2f,2f);
    angle += move.FacingRight ? 0 : 180;
    transform.eulerAngles = new Vector3(0, 0, angle);

    //Make sure the fireball dies.
    Destroy(gameObject, lifetime);
  }

  //===================================================================================================================

  private void FixedUpdate() {
    checkHits();
    transform.Translate(Vector2.right * speed);
  }

  //===================================================================================================================

  private void checkHits(){
    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right, speed);

    foreach(RaycastHit2D hit in hits) {

      GameObject candidate = hit.collider.gameObject;

      //If it is not an enemy or if we have seen this enemy before, ignore it.
      if(candidate.tag != "Enemy" || victims.Contains(candidate)) continue;

      //Remember this enemy, and damage it.
      victims.Add(candidate);
      candidate.SendMessage("modifyHealth", -damage, SendMessageOptions.DontRequireReceiver);
    }
  }
}
