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

    //Get all the objects nearby, compare enemy distances and get the nearest one.
    Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position, searchRadius);
    foreach (Collider2D candidate in candidates) {

      if(candidate.gameObject.tag != "Enemy") continue;

      float distance = Mathf.Abs(candidate.transform.position.x - transform.position.x);

      //If this is the nearest enemy so far.
      if(distance < nearestDistance) {
        nearestDistance = distance;
        nearestEnemy = candidate.transform;
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
