using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

  [SerializeField] private float maxSpeed = 0.25f;
  [SerializeField] private float acceleration = 0.0075f;
  [SerializeField] private float damage = 1f;

  //Movement stuff.
  private float targetSpeed = 0;
  private Vector2 velocity = Vector2.zero;
  private bool facingRight = true;

  //AI.
  private delegate void Action();                       //Delegate holding an action.
  private Stack<Action> actions = new Stack<Action>();  //Stack holding all the actions. Top action is performed.

  private bool choosing = false;
  private bool chose = false;
  private bool isAttacking = false;

  //Collision Stuff.
  private Vector2 enemySize = Vector2.zero;
  private LayerMask solidMask = new LayerMask();

  //Other.
  private Transform atkPoint;
  private Collider2D targetColl;
  private HealthManager targetHM;
  private Transform target;

  //===================================================================================================================

  private void Start() { 
    //Get the height of the player.
    Collider2D coll = GetComponent<BoxCollider2D>();
    enemySize = new Vector2(coll.bounds.size.x, coll.bounds.size.y);

    //Get the solid layermask for raycasting.
    solidMask = LayerMask.GetMask("Solid");

    //Get attack point position.
    atkPoint = transform.Find("Attack Point");

    //Get something to chase.
    switchTarget(GameObject.FindWithTag("Player"));

    //Set initial action.
    actions.Push(idle);
  }

  //===================================================================================================================

  private void FixedUpdate() {

    //Take current action.
    // updateState();
    takeAction();
    velocity.x = move();

    //Horizontal collision checking.
    if(velocity.x != 0) checkHorizontalCollisions();

    //Face the correct way.
    if((targetSpeed > 0 && !facingRight) || (targetSpeed < 0) && facingRight) turnAround();
    
    //Move the enemy.
    transform.Translate(velocity);
  }

  //===================================================================================================================

  private float move(){
    if(velocity.x == targetSpeed) return velocity.x;

    return (velocity.x > targetSpeed) ? Mathf.Max(targetSpeed, velocity.x - acceleration):
                                        Mathf.Min(velocity.x + acceleration, targetSpeed);
  }

  //===================================================================================================================

  private void setTargetSpeed(float f) { targetSpeed = f * maxSpeed; }

  //===================================================================================================================

  private void checkHorizontalCollisions() {
    float distance = Mathf.Abs(velocity.x) + enemySize.x/2;
    int direction = velocity.x > 0 ? 1 : -1;
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, distance, solidMask);

    if(hit) {
      float gap = hit.distance;
      transform.Translate(Vector2.right * direction * (gap - enemySize.x/2));
      velocity.x = 0;
    }
  }

  //===================================================================================================================

  private void turnAround() {
    facingRight = !facingRight;
    Vector3 temp = transform.localScale;
    temp.x *= -1;
    transform.localScale = temp;
  }

  //===================================================================================================================

  private void takeAction() {
    if(actions.Count != 0) {
      Action currentAction = actions.Peek();
      currentAction();
    }
  }

  //===================================================================================================================

  private void idle() {
    print("Idle");
    if(!choosing) StartCoroutine("chooseDirection");
    else if(chose) {
      chose = false;
      actions.Pop();
      actions.Push(chase);
    }
  }

  //===================================================================================================================

  private void chase(){
    print("Chase");
    if(targetColl.OverlapPoint(atkPoint.position)) {
      actions.Pop();
      actions.Push(attack);
    }
    else {
      int desiredDirection = getDirection(false);
      int actualDirection = getDirection(true);

      if(desiredDirection != actualDirection && actualDirection != 0) {
        actions.Pop();
        actions.Push(idle);
      }
    }
  }

  //===================================================================================================================

  private void attack() {
    print("Attack!");
    if(!isAttacking) StartCoroutine("attacking");
    else if(!targetColl.OverlapPoint(atkPoint.position)) {
      actions.Pop();
      actions.Push(idle);
    }
  }

  //===================================================================================================================

  private IEnumerator chooseDirection(){
    choosing = true;
    setTargetSpeed(0);
    yield return new WaitForSeconds(0.5f);

    setTargetSpeed((float)getDirection());

    choosing = false;
    chose = true;
  }

  //===================================================================================================================

  private int getDirection(bool actual=false){
    if(!actual) {
      if(target.position.x - transform.position.x > 0) return 1;
      else if(target.position.x - transform.position.x < 0) return -1;
      else return 0;
    }
    else {
      if(targetSpeed > 0) return 1;
      else if(targetSpeed < 0) return -1;
      else return 0;
    }
  }

  //===================================================================================================================

  private IEnumerator attacking() {
    isAttacking = true;
    targetSpeed = 0;
    print("Attacking!");
    yield return new WaitForSeconds(1f);
    if(targetColl.OverlapPoint(atkPoint.position)) targetHM.modifyHealth(-damage);
    yield return new WaitForSeconds(1f);
    isAttacking = false;
  }

  //===================================================================================================================

  private void switchTarget(GameObject newTarget) {
    target = newTarget.transform;
    targetColl = newTarget.GetComponent<Collider2D>();
    targetHM = newTarget.GetComponent<HealthManager>();
  }
}
