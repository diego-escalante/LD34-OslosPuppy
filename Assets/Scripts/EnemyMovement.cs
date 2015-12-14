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
  private Collider2D atkColl;
  private Collider2D targetColl;
  private HealthManager targetHM;
  private Transform target;

  // static private Transform monsterTran;
  // static private Transform playerTran;

  //Anim.
  private Animator anim;

  private float range = 2f;

  //===================================================================================================================

  private void Start() { 

    //Get Monster and Player Transform.
    // if(monsterTran == null) monsterTran = GameObject.FindWithTag("Monster").transform;
    // if(playerTran == null) playerTran = GameObject.FindWithTag("Player").transform;

    //Get the height of the enemy.
    Collider2D coll = GetComponent<BoxCollider2D>();
    enemySize = new Vector2(coll.bounds.size.x, coll.bounds.size.y);

    //Get the solid layermask for raycasting.
    solidMask = LayerMask.GetMask("Solid");

    //Get attack point position.
    atkPoint = transform.Find("Attack Point");
    atkColl = atkPoint.GetComponent<Collider2D>();

    //get anim
    anim = GetComponent<Animator>();

    //Get something to chase.
    // switchTarget();

    //Set initial action.
    actions.Push(idle);
  }

  //===================================================================================================================

  // public void updateMonster() {
  //   if(monsterTran == null) {
  //     monsterTran = GameObject.FindWithTag("Monster").transform;

  //   }
  //   switchTarget();
  // }

  //===================================================================================================================

  private void FixedUpdate() {

    if(target == null) {
      target = GameObject.FindWithTag("Monster").transform;
      targetColl = target.GetComponent<Collider2D>(); 
      targetHM = target.GetComponent<HealthManager>();
    }

    //Take current action.
    // updateState();
    takeAction();
    velocity.x = move();

    //Horizontal collision checking.
    if(velocity.x != 0) checkHorizontalCollisions();

    //Face the correct way.
    if((targetSpeed > 0 && !facingRight) || (targetSpeed < 0) && facingRight) turnAround();

    //Anim
    anim.SetFloat("Speed", Mathf.Abs(velocity.x));
    
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
    Vector3 origin = transform.position + new Vector3(0, enemySize.y/2, 0);
    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, distance, solidMask);

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
    if(!choosing) StartCoroutine("chooseDirection");
    else if(chose) {
      chose = false;
      actions.Pop();
      actions.Push(chase);
    }
  }

  //===================================================================================================================

  private void chase(){
    if(targetColl.OverlapPoint(atkPoint.position)) {
      actions.Pop();
      actions.Push(attack);
    }
    else {
      int desiredDirection = getDirection(false);
      int actualDirection = getDirection(true);

      if(desiredDirection != actualDirection && actualDirection != 0) {
        if(atkColl.IsTouching(targetColl)) {
          actions.Pop();
          actions.Push(attack);
          return;
        }
        actions.Pop();
        actions.Push(idle);
      }
    }
  }

  //===================================================================================================================

  private void attack() {
    if(!isAttacking) StartCoroutine("attacking");
    else if(!targetColl.OverlapPoint(atkPoint.position) && !atkColl.IsTouching(targetColl)) {
      actions.Pop();
      actions.Push(idle);
    }
  }

  //===================================================================================================================

  private IEnumerator chooseDirection(){
    choosing = true;
    setTargetSpeed(0);
    // switchTarget();
    yield return new WaitForSeconds(0.5f);

    setTargetSpeed((float)getDirection());

    choosing = false;
    chose = true;
  }

  //===================================================================================================================

  private int getDirection(bool actual=false){
    if(target == null) {
      target = GameObject.FindWithTag("Monster").transform;
      targetColl = target.GetComponent<Collider2D>(); 
      targetHM = target.GetComponent<HealthManager>();
    }
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
    anim.SetTrigger("Attack");
    yield return new WaitForSeconds(0.25f);
    // if(atkColl.IsTouching(targetColl)) targetHM.modifyHealth(-damage);
    if(target == null) {
      target = GameObject.FindWithTag("Monster").transform;
      targetColl = target.GetComponent<Collider2D>(); 
      targetHM = target.GetComponent<HealthManager>();
    }
    float distance = target.transform.position.x - transform.position.x;
    if(Mathf.Abs(distance) < range) 
    // if((facingRight && 0 <= distance && distance <= range) || (!facingRight && -range <= distance && distance <= 0))
      targetHM.modifyHealth(-damage);


    // switchTarget();
    yield return new WaitForSeconds(1f);
    isAttacking = false;
  }

  //===================================================================================================================

  // private void switchTarget() {

  //   if(!playerTran.GetComponent<HealthManager>().enabled) target = monsterTran;
  //   else if(!monsterTran.GetComponent<HealthManager>().enabled) target = playerTran;
  //   else {
  //     float dragonDistance = Vector2.Distance(transform.position, monsterTran.position);
  //     float playerDistance = Vector2.Distance(transform.position, playerTran.position);
  //     target = playerDistance < dragonDistance ? playerTran : monsterTran;      
  //   }

  //   targetColl = target.GetComponent<Collider2D>(); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  //   targetHM = target.GetComponent<HealthManager>();
  // }
}
