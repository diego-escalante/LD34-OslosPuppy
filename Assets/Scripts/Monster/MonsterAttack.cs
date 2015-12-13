using UnityEngine;
using System.Collections;

public class MonsterAttack : MonsterBase {

  private bool reachedEnemy = false;
  private bool attacking = false;

  //===================================================================================================================

  protected override void FixedUpdate() {
    
    //If not attacking, pick a target, or if at the target, attack.
    if(!attacking) {
      if(target == null) target = findNearestEnemy();
      if(reachedEnemy) StartCoroutine("attack");
    }

    base.FixedUpdate();
  }

  //===================================================================================================================

  private void OnDisable() { StopCoroutine("attack"); }

  //===================================================================================================================

  private IEnumerator attack() {
    attacking = true;
    reachedEnemy = false;
    target = null;
    print("attack");
    yield return new WaitForSeconds(2f);
    attacking = false;
  }

  //===================================================================================================================

  private Transform findNearestEnemy() {
    
    Transform nearestEnemy = null;
    float nearestDistance = Mathf.Infinity;

    //Get all enemies, find the closest one.
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    foreach(GameObject enemy in enemies){
      float distance = Mathf.Abs(enemy.transform.position.x - transform.position.x);
      if(distance < nearestDistance){
        nearestDistance = distance;
        nearestEnemy = enemy.transform;
      }
    }
    return nearestEnemy;
  }

  //===================================================================================================================

  protected override float calcTargetSpeed(){

    //If there's nothing to move to stop.
    if(target == null) return 0;

    //If it is close enough, stop, otherwise, get the correct targetSpeed.
    float distance = target.position.x - transform.position.x;
    if(Mathf.Abs(distance) < distanceThreshold) {
      reachedEnemy = true;
      return 0;
    }
    else return maxSpeed * (distance > 0 ? 1 : -1);
  }

  //===================================================================================================================

}
