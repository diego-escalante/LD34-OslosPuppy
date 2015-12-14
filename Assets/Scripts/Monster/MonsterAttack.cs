using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAttack : MonsterBase {

  public float damage = 3f;
  public float range = 3f;

  private bool reachedEnemy = false;
  private bool attacking = false;

  private ParticleSystem psys;
  private float fireDuration;

  private MonsterCtrl ctrl;

  //===================================================================================================================

  protected override void Start() {
    base.Start();

    //Set up particle system stuff.
    psys = transform.Find("Particle System").GetComponent<ParticleSystem>();
    fireDuration = psys.duration + psys.startLifetime -1;

    ctrl = GetComponent<MonsterCtrl>();
  }

  //===================================================================================================================

  protected override void FixedUpdate() {
    
    //If not attacking, pick a target, or if at the target, attack.
    if(!attacking) {
      if(target == null) target = findNearestEnemy();
      if(reachedEnemy) {
        float distance = target.transform.position.x - transform.position.x;
        turnAround(distance);
        StartCoroutine("attack");
      }
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
    
    anim.SetTrigger("StartAttacking");
    psys.Play();
    float elapsedTime = 1f;
    List<GameObject> memory = new List<GameObject>();

    yield return new WaitForSeconds(elapsedTime);
    while(elapsedTime < fireDuration) {

      GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
      foreach(GameObject enemy in enemies){
        if(memory.Contains(enemy)) continue;
        float distance = enemy.transform.position.x - transform.position.x;
        
        if((facingRight && 0 <= distance && distance <= range) ||
          (!facingRight && -range <= distance && distance <= 0)) {
          enemy.GetComponent<HealthManager>().modifyHealth(-damage);
          memory.Add(enemy);
        }
      }

      elapsedTime += Time.deltaTime;
      yield return null;
    }
    psys.Stop();
    anim.SetTrigger("StopAttacking");
    attacking = false;
    ctrl.atkDone();
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

}
