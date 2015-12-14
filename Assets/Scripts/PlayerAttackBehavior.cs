using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerAttackBehavior : MonoBehaviour {

  [SerializeField] private GameObject fireballPrefab;
  [SerializeField] private GameObject shieldPrefab;

  private GameObject shield;
  private SpriteRenderer shieldSR;
  private bool isShielding = false;
  private Color shieldColor;

  private Transform atkPoint;

  //Animation.
  private Animator anim;
  //Properties.
  public bool IsShielding {get {return isShielding;}}

  private float manaAmount = 100;
  private float timeperpoint = 0.5f;
  // private float chargeDuration = 30f;
  private Image manaImg;

  //===================================================================================================================

  private void Start() {
    anim = GetComponent<Animator>();
    atkPoint = transform.Find("Attack Point");
    StartCoroutine("charge");
    manaImg = GameObject.Find("Mana Image").GetComponent<Image>();
  }

  //===================================================================================================================

  private void FixedUpdate() {
    manaImg.fillAmount = Mathf.Clamp((float)manaAmount/100, 0 ,1);
  }

  //===================================================================================================================

  private IEnumerator charge() {
    while(true){
      manaAmount = Mathf.Clamp(manaAmount + 1, 0, 100);
      yield return new WaitForSeconds(timeperpoint);
    }
  }

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
    if(!isShielding && manaAmount >= 10) {
      manaAmount -= 10;
      Instantiate(fireballPrefab, atkPoint.position, Quaternion.identity);
      anim.SetTrigger("Fire");
    }
  }

  //===================================================================================================================

  private void shieldUp() {
    isShielding = true;
    shield = (GameObject)Instantiate(shieldPrefab, transform.position, Quaternion.identity);
    shieldSR = shield.GetComponent<SpriteRenderer>();
    shieldColor = shieldSR.color;
    anim.SetTrigger("Cast");
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
