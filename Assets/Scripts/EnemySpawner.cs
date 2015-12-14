using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

  public GameObject[] enemies;
  private int difficulty = 0;

  private Transform center;
  //[Random.Range(0,enemies.Length)]

  //===================================================================================================================

  private void Start(){
    center = GameObject.FindWithTag("Monster").transform;
    // StartCoroutine(spawn(enemies[0]));
    InvokeRepeating("increaseDifficulty", 3f, 10f);

  }

  //===================================================================================================================

  private IEnumerator spawn(GameObject enemy){
    difficulty++;
    float offset = 0;

    while(true) {
      offset += 0.5f;
      if(center == null) center = GameObject.FindWithTag("Monster").transform;

      float cWidth = calcScreenWidth();

      float side = Random.Range(-0.5f, 0.5f);
      float spawnx;
      if(side > 0) {
        spawnx = center.position.x + cWidth/2 + cWidth*0.05f;
        if(spawnx > GameController.WORLD_SIZE/2) spawnx -= cWidth;
      }
      else {
        spawnx = center.position.x - cWidth/2 - cWidth*0.05f;
        if(spawnx < -GameController.WORLD_SIZE/2) spawnx += cWidth * 1.1f;
      }

      Instantiate(enemy, new Vector3(spawnx, 0.5f, 0), Quaternion.identity);

      yield return new WaitForSeconds(Random.Range(3f, 8f) + offset);
    }  

  }

  //===================================================================================================================

  private float calcScreenWidth() {
    float a = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
    float b = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
    return b - a;
  }

  //===================================================================================================================

  private void increaseDifficulty(){
    difficulty++;
    /*if(difficulty > 2) */StartCoroutine(spawn(enemies[Random.Range(0,Mathf.Min(difficulty,enemies.Length))]));
    // else StartCoroutine(spawn(enemies[0]));
  }

}
