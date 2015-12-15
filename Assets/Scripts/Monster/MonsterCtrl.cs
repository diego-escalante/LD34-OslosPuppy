using UnityEngine;
using System.Collections;
// using UnityEditor.Animations;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour {

  public GameObject nextMonster;
  public float sizeGoal;
  public GameObject poof;
  public float atkChargeDuration = 5f;
  public float foodDistanceMax = 10f;
  public float nearestEnemyThreshold = 3f;
  public bool roamEnabled = false;

  private float atkChargeElapsed = 0;
  private bool chargeReady = false;

  private float size = 1;

  //Action stuff.
  private MonsterBase currentAction;
  
  private float foodDistance;
  private float nearestEnemy;
  

  //Camera scaling stuff.
  private Camera cam;
  private Transform camTrans;
  private float camSize;
  private float camSizeMultiplier;

  //Components.
  private BoxCollider2D coll;

  private EnemyMovement enemyMove;

  private Image atkImg;

  //===================================================================================================================

  private void Start() {

    coll = GetComponent<BoxCollider2D>();

    atkImg = GameObject.Find("Attack Image").GetComponent<Image>();

    //Get camera stuff.
    cam = Camera.main;
    camTrans = cam.transform.parent;
    camSize = Camera.main.orthographicSize;
    camSizeMultiplier = size;

    
    currentAction = GetComponent<MonsterIdle>();
    StartCoroutine("chargeAttack");
  }

  //===================================================================================================================

  private void Update() {
    atkImg.fillAmount = Mathf.Clamp(atkChargeElapsed/atkChargeDuration,0,1);
    updateFoodDistance();

    brain();

    if(size >= sizeGoal) evolve();

  }

  //===================================================================================================================

  public void grow(float amount) {
    //Increase the size of the monster.
    size += amount;
    camSizeMultiplier += amount;
    transform.localScale = new Vector3(1 * size * (transform.localScale.x >= 0 ? 1 : -1), 1 * size, 1);

    //Scale the camera.
    float monsView = cam.WorldToViewportPoint(transform.position).y;
    cam.orthographicSize = Mathf.Min(camSize * camSizeMultiplier, 8);
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
    if(chargeReady) switchAction("MonsterAttack");
    else if(foodDistance < foodDistanceMax) switchAction("MonsterEat");
    else if(roamEnabled) switchAction("MonsterRoam");
    else switchAction("MonsterFollow");
  }

  //===================================================================================================================

  public void atkDone(){
    StartCoroutine("chargeAttack");
  }

  //===================================================================================================================

  private IEnumerator chargeAttack() {
    chargeReady = false;
    while(atkChargeElapsed < atkChargeDuration) {
      atkChargeElapsed += Time.deltaTime;
      yield return null;
    }
    atkChargeElapsed = 0;
    chargeReady = true;
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

  private void evolve(){
      Instantiate(poof, transform.position + new Vector3(0, coll.size.y/2,0), Quaternion.identity);
      MonsterCtrl newCtrl = ((GameObject)Instantiate(nextMonster, transform.position, Quaternion.identity)).GetComponent<MonsterCtrl>();
      newCtrl.atkChargeElapsed = atkChargeElapsed;
      Destroy(gameObject);
    }
}
