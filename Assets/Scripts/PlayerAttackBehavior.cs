using UnityEngine;
using System.Collections;

public class PlayerAttackBehavior : MonoBehaviour {

  [SerializeField] private GameObject fireballPrefab;

  //===================================================================================================================

  private void OnEnable() {
    InputManager.attackPressed += attack;
  }

  //===================================================================================================================

  private void OnDisable() {
    InputManager.attackPressed -= attack;
  }

  //===================================================================================================================

  private void attack() {
    Instantiate(fireballPrefab, transform.position, Quaternion.identity);
  }

}
