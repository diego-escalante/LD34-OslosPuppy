using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballBehavior : MonoBehaviour {

  [SerializeField] private float speed = 0.5f;
  [SerializeField] private float lifetime = 0.25f;
  [SerializeField] private float damage = 1f;

  private List<Collider2D> victims = new List<Collider2D>();

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

    Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position, 0.5f);

    foreach(Collider2D candidate in candidates){
      if(candidate.gameObject.tag != "Enemy" || victims.Contains(candidate)) continue;
      victims.Add(candidate);
      candidate.gameObject.SendMessage("modifyHealth", -damage, SendMessageOptions.DontRequireReceiver);
    }
  }
}
