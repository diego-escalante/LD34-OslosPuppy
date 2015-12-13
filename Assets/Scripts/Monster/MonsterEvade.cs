using UnityEngine;
using System.Collections;

public class MonsterEvade : MonsterBase {

  public float searchRadius = 3f;

  //===================================================================================================================

  protected override void FixedUpdate() {
    target = findNearestEnemy();
    base.FixedUpdate();
  }

  //===================================================================================================================

  private Transform findNearestEnemy() {
    
    Transform nearestEnemy = null;
    float nearestDistance = Mathf.Infinity;

    //Get all enemies, find the closest one within the searchRadius.
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    foreach(GameObject enemy in enemies){
      float distance = Mathf.Abs(enemy.transform.position.x - transform.position.x);
      if(distance < nearestDistance && distance < searchRadius){
        nearestDistance = distance;
        nearestEnemy = enemy.transform;
      }
    }

    return nearestEnemy;
  }

  //===================================================================================================================

  protected override float calcTargetSpeed() {
    //If there's nothing to move to stop.
    if(target == null) return 0;

    //Run away!
    float distance = target.position.x - transform.position.x;
    return maxSpeed * (distance > 0 ? -1 : 1);
  }
}
