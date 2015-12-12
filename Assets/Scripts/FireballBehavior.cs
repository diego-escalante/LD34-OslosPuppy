using UnityEngine;
using System.Collections;

public class FireballBehavior : MonoBehaviour {

  [SerializeField] private float speed = 0.5f;
  [SerializeField] private float lifetime = 0.25f;

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
    transform.Translate(Vector2.right * speed);
  }
}
