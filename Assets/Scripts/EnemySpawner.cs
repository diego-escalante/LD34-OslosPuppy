using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

  public GameObject[] enemies;
  private int counter = 0;

  private Transform center;

  //===================================================================================================================

  private void Start(){
    center = GameObject.FindWithTag("Monster").transform;
    StartCoroutine("spawn");
  }

  //===================================================================================================================

  private IEnumerator spawn(){

    while(true) {
      counter++;

      if(center == null) center = GameObject.FindWithTag("Monster").transform;

      Instantiate(enemies[0], center.position + new Vector3(5, 0, 0), Quaternion.identity);

      yield return new WaitForSeconds(5 - counter/100f);
    }  

  }

}
