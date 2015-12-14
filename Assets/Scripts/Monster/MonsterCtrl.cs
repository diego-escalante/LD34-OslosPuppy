using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {

  private float size = 1;

  //Action stuff.
  private MonsterBase currentAction;
  
  private float hp;
  private float atkCharge;
  private float foodDistance;

  private float atkChargeSpeed = 1f;
  private float hpThreshold = 0.8f;
  private float foodDistanceMax = 10f;

  //Camera scaling stuff.
  private Camera cam;
  private Transform camTrans;
  private float camSize;

  //Components.
  private HealthManager healthMgmt;

  //===================================================================================================================

  private void Start() {
    //Get camera stuff.
    cam = Camera.main;
    camTrans = cam.transform.parent;
    camSize = Camera.main.orthographicSize;

    //Get components.
    healthMgmt = GetComponent<HealthManager>();
    
    currentAction = GetComponent<MonsterIdle>();
  }

  //===================================================================================================================

  private void Update() {
    updateHP();
    updateFoodDistance();
    //updateAttack

    brain();

  }

  //===================================================================================================================

  public void grow(float amount) {
    //Increase the size of the monster.
    size += amount;
    transform.localScale = new Vector3(1 * size * (transform.localScale.x >= 0 ? 1 : -1), 1 * size, 1);

    //Scale the camera.
    float monsView = cam.WorldToViewportPoint(transform.position).y;
    cam.orthographicSize = camSize * size;
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
    else if(hp < hpThreshold) switchAction("MonsterEvade");
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
      if(atkCharge < 1) atkCharge += 0.01f;
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
}
