using UnityEngine;
using System.Collections;

public class PlayerAttackBehavior : MonoBehaviour {

  [SerializeField] private GameObject fireballPrefab;
  [SerializeField] private GameObject shieldPrefab;

  private GameObject shield;
  private SpriteRenderer shieldSR;
  private bool isShielding = false;
  private Color shieldColor;

  //Properties.
  public bool IsShielding {get {return isShielding;}}

  //===================================================================================================================

  private void OnEnable() {
    InputManager.attackPressed += attack;
    InputManager.shieldPressed += shieldUp;
    InputManager.shieldReleased += shieldDown;

  }

  //===================================================================================================================

  private void OnDisable() {
    InputManager.attackPressed -= attack;
    InputManager.shieldPressed -= shieldUp;
    InputManager.shieldReleased += shieldDown;
  }

  //===================================================================================================================

  private void attack() {
    if(!isShielding) Instantiate(fireballPrefab, transform.position, Quaternion.identity);
  }

  //===================================================================================================================

  private void shieldUp() {
    isShielding = true;
    shield = (GameObject)Instantiate(shieldPrefab, transform.position, Quaternion.identity);
    shieldSR = shield.GetComponent<SpriteRenderer>();
    shieldColor = shieldSR.color;
    StartCoroutine("shielding");
  }

  //===================================================================================================================

  private void shieldDown() {
    isShielding = false;
  }

  //===================================================================================================================

  private IEnumerator shielding() {
    //Update the position of the shield while it is up.
    while(isShielding) {
      shield.transform.position = new Vector3(transform.position.x, transform.position.y, shield.transform.position.z);
      yield return new WaitForFixedUpdate();
    }

    //Destroy the shield after we are done.
    Destroy(shield);
  }

  //===================================================================================================================

  public void block(){
    StartCoroutine("flashColor", Color.blue);
  }

  //===================================================================================================================

  private IEnumerator flashColor(Color c){
    float currentTime = 0;
    float endTime = 0.5f;

    while(currentTime < endTime) {
      currentTime += Time.deltaTime;
      shieldSR.color = Color.Lerp(c, shieldColor, currentTime/endTime);
      yield return new WaitForFixedUpdate();
    }
  }
}
