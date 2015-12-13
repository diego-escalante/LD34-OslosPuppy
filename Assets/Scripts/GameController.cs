using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

  private HealthManager monsterHM;
  private HealthManager playerHM;

  //===================================================================================================================

  private void Start() {
    monsterHM = GameObject.FindWithTag("Monster").GetComponent<HealthManager>();
    playerHM = GameObject.FindWithTag("Player").GetComponent<HealthManager>();
  }

  //===================================================================================================================

  private void Update() {
    if(Input.GetKeyDown("r")) restart();
    if(loseCheck()) Invoke("restart", 3); 
  }

  private bool loseCheck() {
    return !monsterHM.enabled && !playerHM.enabled;
  }

  private void restart() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
