using UnityEngine;
using System.Collections;

public class ManaDropBehavior : MonoBehaviour {

  private void Start() {
    Destroy(gameObject, 10f);
  }

  private void OnTriggerEnter2D(Collider2D other){
    if(other.gameObject.tag == "Player"){
      other.GetComponent<PlayerAttackBehavior>().manaAmount = Mathf.Min(other.GetComponent<PlayerAttackBehavior>().manaAmount + 30, 100);
      Destroy(gameObject);
    }
  }
}
