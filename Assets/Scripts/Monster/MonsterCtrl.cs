using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

public class MonsterCtrl : MonoBehaviour {

  private float size = 1;
  private int currentTier = 1;

  //Action stuff.
  private MonsterBase currentAction;
  
  private float hp;
  public float atkCharge = 0f;
  private float foodDistance;

  private float atkChargeSpeed = 2f;
  private float hpThreshold = 0.8f;
  private float foodDistanceMax = 10f;

  private float nearestEnemy;
  private float nearestEnemyThreshold = 10f;

  //Camera scaling stuff.
  private Camera cam;
  private Transform camTrans;
  private float camSize;
  private float camSizeMultiplier;

  //Components.
  private HealthManager healthMgmt;
  private BoxCollider2D coll;

  //Animation.
  private Animator anim;
  public AnimatorController animCtrl1;
  public AnimatorController animCtrl2;
  public AnimatorController animCtrl3;
  public AnimatorController animCtrl4;

  public GameObject poof;

  //===================================================================================================================

  private void Start() {

    anim = GetComponent<Animator>();
    coll = GetComponent<BoxCollider2D>();

    //Get camera stuff.
    cam = Camera.main;
    camTrans = cam.transform.parent;
    camSize = Camera.main.orthographicSize;
    camSizeMultiplier = size;

    //Get components.
    healthMgmt = GetComponent<HealthManager>();
    
    currentAction = GetComponent<MonsterIdle>();
    StartCoroutine("chargeAttack");
  }

  //===================================================================================================================

  private void Update() {
    updateHP();
    updateFoodDistance();
    updateEnemyDistance();
    //updateAttack

    brain();

    if(currentTier == 1 && size >= 3) evolve();

  }

  //===================================================================================================================

  public void grow(float amount) {
    //Increase the size of the monster.
    size += amount;
    camSizeMultiplier += amount;
    transform.localScale = new Vector3(1 * size * (transform.localScale.x >= 0 ? 1 : -1), 1 * size, 1);

    //Scale the camera.
    float monsView = cam.WorldToViewportPoint(transform.position).y;
    cam.orthographicSize = camSize * camSizeMultiplier;
    float deltaWorldPos = cam.ViewportToWorldPoint(new Vector3(0, monsView, cam.nearClipPlane)).y - transform.position.y;
    camTrans.Translate(new Vector3(0, -deltaWorldPos, 0));
  }

  //===================================================================================================================

  private void switchAction(string className) {

    System.Type tName = System.Type.GetType(className);
    //Find the desired action, turn off previous one, turn new one on.
    MonsterBase[] actions = GetComponents<MonsterBase>();
    foreach(MonsterBase action in actions) {
      if(tName == action.GetType()) {
        if(currentAction == action) return;
        currentAction.enabled = false;
        currentAction = action;
        currentAction.enabled = true;
        return;
      }
    }
  }

  //===================================================================================================================

  private void brain(){
    if(atkCharge == 1) switchAction("MonsterAttack");
    else if(hp < hpThreshold && nearestEnemy < nearestEnemyThreshold) switchAction("MonsterEvade");
    else if(foodDistance < foodDistanceMax) switchAction("MonsterEat");
    else {
      //Idle, follow, roam.
      switchAction("MonsterFollow");
    }

  }

  //===================================================================================================================

  private void updateHP(){
    hp = healthMgmt.getHealth();
  }

  //===================================================================================================================

  private IEnumerator chargeAttack() {
    while(true) {
      if(atkCharge < 1) {
        atkCharge += 0.01f;
        atkCharge = Mathf.Min(atkCharge, 1);
      }
      yield return new WaitForSeconds(atkChargeSpeed);
    }
  }

  //===================================================================================================================

  private void updateFoodDistance() {
    float minDistance = Mathf.Infinity;
    GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
    foreach(GameObject food in foods) {
      float distance = Mathf.Abs(food.transform.position.x - transform.position.x);
      if(distance < minDistance) minDistance = distance;
    }
    foodDistance = minDistance;
  }

  //===================================================================================================================

  private void updateEnemyDistance() {
    float minDistance = Mathf.Infinity;
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    foreach(GameObject enemy in enemies) {
      float distance = Mathf.Abs(enemy.transform.position.x - transform.position.x);
      if(distance < minDistance) minDistance = distance;
    }
    nearestEnemy = minDistance;    
  }

  //===================================================================================================================

  private void evolve(){
    currentTier++;

    switchAction("MonsterIdle");
    Instantiate(poof, transform.position + new Vector3(0, coll.size.y/2,0), Quaternion.identity);
    switch(currentTier){
      case 2:
        anim.runtimeAnimatorController = animCtrl2;
        transform.localScale = Vector3.one;
        coll.size = new Vector2(1.5f, 2);
        coll.offset = new Vector2(0, 1);
        break;
      case 3:
        break;
      case 4:
        break;
    }

    Invoke("reenable", 1f);
    this.enabled = false;
    size = 1;

  }

  private void reenable(){
    this.enabled = true;
  }
}
