using UnityEngine;
using System.Collections;
using System.Collections.Generic;

  public class MonsterBehavior : MonoBehaviour {

  [SerializeField] private float maxSpeed = 0.25f;
  [SerializeField] private float acceleration = 0.0075f;
  // [SerializeField] private float damage = 1f;

  private float targetSpeed = 0;
  private Vector2 velocity = Vector2.zero;
  private bool facingRight = true;
  private float sizeMultiplier = 1f;
  private float growthDelta = 0.0005f;

  //Camera scaling stuff.
  private float camSize;
  private Camera cam;
  private Transform camTrans;

  //AI.
  private delegate void Action();
  private Stack<Action> actions = new Stack<Action>();
  private Transform target;
  // private bool choosing = false;
  // private bool chose = false;
  private float actionCooldown = 3f;
  private float elapsedTime = 0f;

  //Collision.
  private LayerMask solidMask = new LayerMask();
  private Vector2 monsterSize = Vector2.zero;

  //Anim
  private Animator anim;

  //===================================================================================================================

  private void Start() {
    //Get camera stuff.
    cam = Camera.main;
    camTrans = cam.transform.parent;
    camSize = Camera.main.orthographicSize;


    //Get the height of the monster.
    Collider2D coll = GetComponent<BoxCollider2D>();
    monsterSize = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
    //Get the solid layermask for raycasting.
    solidMask = LayerMask.GetMask("Solid");

    //Set initial action.
    switchTarget(GameObject.FindWithTag("Player"));
    actions.Push(idle);

    //Animation
    anim = GetComponent<Animator>();

    //Initialize growing!
    StartCoroutine("grow");
  }

  //===================================================================================================================

  private void FixedUpdate() {

    takeAction();

    velocity.x = move();

    //Choose a new action when necessary.
    elapsedTime += Time.deltaTime;
    if(elapsedTime > actionCooldown) {
      elapsedTime = 0;
      chooseAction();
    }

    if(velocity.x != 0) checkHorizontalCollisions();
    if((targetSpeed > 0 && !facingRight) || (targetSpeed < 0) && facingRight) turnAround();

    //Animation
    anim.SetFloat("Speed", Mathf.Abs(velocity.x));

    transform.Translate(velocity);

  }

  //===================================================================================================================

  private void setSize(float f, bool relative=true) {
    //Increase the size of the monster.
    sizeMultiplier = relative ? sizeMultiplier + f : f;

    transform.localScale = new Vector3(1 * sizeMultiplier * (transform.localScale.x >= 0 ? 1 : -1), 1 * sizeMultiplier, 1);


    float monsView = cam.WorldToViewportPoint(transform.position).y;
    cam.orthographicSize = camSize * sizeMultiplier;
    float deltaWorldPos = cam.ViewportToWorldPoint(new Vector3(0, monsView, cam.nearClipPlane)).y - transform.position.y;


    camTrans.Translate(new Vector3(0, -deltaWorldPos, 0));
  }

  //===================================================================================================================

  private void evade() {
  }

  private void attack() {
  }

  private void eat() {
  }

  private void follow() {
  }

  private void idle() {
    switchTarget(gameObject);
    actions.Pop();
    // if(!isDirectionCorrect()) {
    //   if(!choosing) StartCoroutine("chooseDirection");
    //   else if(chose) {
    //     chose = false;
    //     actions.Pop();
    //   }
    // }
  }

  private void roam() {
  }

  //===================================================================================================================

  private float move() {
    
    //Figure out which way to go.
    if(Mathf.Abs(target.position.x - transform.position.x) < 2.5f) setTargetSpeed(0);
    else setTargetSpeed(getDirection(false));

    //If we are already moving optimally, return early.
    if(velocity.x == targetSpeed) return velocity.x;

    //Accelerate towards target speed.
    return (velocity.x > targetSpeed) ? Mathf.Max(targetSpeed, velocity.x - acceleration):
                                        Mathf.Min(velocity.x + acceleration, targetSpeed);
  }

  //===================================================================================================================

  // private IEnumerator chooseDirection(){
  //   choosing = true;
  //   setTargetSpeed(0);
  //   yield return new WaitForSeconds(0.5f);

  //   setTargetSpeed((float)getDirection());

  //   choosing = false;
  //   chose = true;
  // }

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

  private void setTargetSpeed(float f) { targetSpeed = f * maxSpeed; }

  //===================================================================================================================

  private void checkHorizontalCollisions() {
    float distance = Mathf.Abs(velocity.x) + monsterSize.x/2;
    int direction = velocity.x > 0 ? 1 : -1;
    RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,0.5f,0), Vector2.right * direction, distance, solidMask);

    if(hit) {
      float gap = hit.distance;
      transform.Translate(Vector2.right * direction * (gap - monsterSize.x/2));
      velocity.x = 0;
    }
  }

  //===================================================================================================================

  private void takeAction() {
    if(actions.Count != 0) {
      Action currentAction = actions.Peek();
      currentAction();
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

  private void switchTarget(GameObject newTarget) {
    target = newTarget.transform;
  }

  //===================================================================================================================

  private IEnumerator grow(){
    
    while(true) {
      setSize(growthDelta);
      yield return new WaitForFixedUpdate();
    }
  }

  //===================================================================================================================

  private bool isDirectionCorrect() {
    int desiredDirection = getDirection(false);
    int actualDirection = getDirection(true);

    return !(desiredDirection != actualDirection && actualDirection != 0);
  }

  //===================================================================================================================

  private void chooseAction() {
    if(actions.Count > 0) actions.Pop();

    float rand = Random.value;

    //Half of the time, be idle.
    if(rand < 0.5f) switchTarget(gameObject);

    //Half ot the time, follow player.
    else {
      switchTarget(GameObject.FindWithTag("Player"));
      // actions.Push(follow);
    }
  }
}
