using UnityEngine;
using System.Collections;

public class MonsterRoam : MonsterBase {

  public float maxRoamDistance = 10f;
  public float chooseCD = 10f;
  public float chooseCDDelta = 3f;

  private float range = GameController.WORLD_SIZE/2;

  //===================================================================================================================

  private void OnEnable() {
    target = (new GameObject("Roam Target")).transform;
    StartCoroutine("setRoamLocation");
  }

  //===================================================================================================================

  private void OnDisable() {
    StopCoroutine("setRoamLocation");
    if(target != null) Destroy(target.gameObject);
  }

  //===================================================================================================================

  private IEnumerator setRoamLocation(){
    float cooldown;
    float x;
    while(true) {

      //Move target to somewhere in the world.
      x = Mathf.Clamp(transform.position.x + Random.Range(-maxRoamDistance, maxRoamDistance), -range, range);
      target.position = new Vector3(x, 0, 0);

      //Set cooldown and repeat everything again after the time passes.
      cooldown = chooseCD + Random.Range(-chooseCDDelta, chooseCDDelta);
      yield return new WaitForSeconds(cooldown);
    }
  }

}
