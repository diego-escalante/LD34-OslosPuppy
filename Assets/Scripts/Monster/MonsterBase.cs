using UnityEngine;
using System.Collections;

public class MonsterBase : MonoBehaviour {

  //Movement.
  public float maxSpeed = 0.05f;
  public float acceleration = 0.01f;
  private Vector2 velocity = Vector2.zero;
  private bool facingRight = true;

  //Collision.
  private LayerMask solidMask = new LayerMask();
  private Collider2D coll;

  //Target: What to move to.
  public float distanceThreshold = 1f;
  protected Transform target;

  //Animation.
  protected Animator anim;

  //===================================================================================================================

  protected virtual void Start() {

    //Set up collision stuff.
    coll = GetComponent<BoxCollider2D>();
    solidMask = LayerMask.GetMask("Solid");

    //Set up animation stuff.
    anim = GetComponent<Animator>();
  }

  //===================================================================================================================

  protected virtual void FixedUpdate() {

    //Move and check collisions.
    move();
    if(velocity.x != 0) checkCollisions();

    //Send speed to animator.
    anim.SetFloat("Speed", Mathf.Abs(velocity.x));

    //Translate the monster.
    transform.Translate(velocity);
  }

  //===================================================================================================================

  private void checkCollisions() {

    //Do some preliminary calculations and cast a ray with that.
    Vector2 monsterSize = coll.bounds.size;
    float distance = Mathf.Abs(velocity.x) + monsterSize.x/2;
    int direction = velocity.x > 0 ? 1 : -1;
    Vector3 origin = transform.position + new Vector3(0, monsterSize.y/2, 0);
    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, distance, solidMask);

    //If we hit a wall, stop and move to contact.
    if(hit) {
      velocity.x = 0;
      transform.Translate(Vector2.right * direction * (hit.distance - monsterSize.x/2));
    }
  }

  //===================================================================================================================

  private void turnAround() {
    //Flip the x-scale.
    facingRight = !facingRight;
    Vector3 temp = transform.localScale;
    temp.x *= -1;
    transform.localScale = temp;
  }

  //===================================================================================================================

  private void move() {
    //Update the target speed.
    float targetSpeed = calcTargetSpeed();

    //Face the correct way.
    if((targetSpeed > 0 && !facingRight) || (targetSpeed < 0) && facingRight) turnAround();

    //Approach target velocity.
    if(velocity.x != targetSpeed) {
      velocity.x = (velocity.x > targetSpeed) ? Mathf.Max(targetSpeed, velocity.x - acceleration):
                                                Mathf.Min(velocity.x + acceleration, targetSpeed);
    }
  }

  //===================================================================================================================

  protected virtual float calcTargetSpeed(){

    //If there's nothing to move to stop.
    if(target == null) return 0;

    //If it is close enough, stop, otherwise, get the correct targetSpeed.
    float distance = target.position.x - transform.position.x;
    if(Mathf.Abs(distance) < distanceThreshold) return 0;
    else return maxSpeed * (distance > 0 ? 1 : -1);
  }

}
