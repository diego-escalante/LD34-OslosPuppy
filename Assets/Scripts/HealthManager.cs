using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

  [SerializeField] private float maxHealth = 100;

  private float currentHealth;
  private SpriteRenderer sr;
  private Color originalColor;

  private bool isPlayer = false;
  private PlayerAttackBehavior playerAtk;
  private Animator anim;

  private Image hpImg;

  //===================================================================================================================

  private void Start() {
    if(transform.gameObject.tag == "Monster") hpImg = GameObject.Find("Health Image").GetComponent<Image>();
    currentHealth = maxHealth;
    sr = GetComponent<SpriteRenderer>();
    originalColor = sr.color;
    anim = GetComponent<Animator>();

    if(gameObject.tag == "Player") {
      isPlayer = true;
      playerAtk = GetComponent<PlayerAttackBehavior>();
    }
  }

  //===================================================================================================================

  public void modifyHealth(float amount) {
    if(!this.enabled) return;
    if(isPlayer && playerAtk.IsShielding) {
      playerAtk.block();
      return;
    }

    currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    if(transform.gameObject.tag == "Monster") hpImg.fillAmount = Mathf.Clamp((float)currentHealth/maxHealth, 0 ,1);
    if(currentHealth == 0) death();

    if(amount < 0) StartCoroutine("flashColor",Color.red);
  }

  //===================================================================================================================

  private void death() {
    anim.SetTrigger("Death");
    
    if(gameObject.tag == "Enemy") {
      GetComponent<EnemyMovement>().enabled = false;
      gameObject.tag = "Food";
      Destroy(gameObject, 5f);
    }

    else if(gameObject.tag == "Monster") {
      GetComponent<MonsterCtrl>().enabled = false;
      MonsterBase[] comps = GetComponents<MonsterBase>();
      foreach(MonsterBase comp in comps) comp.enabled = false;
    }

    else if(gameObject.tag == "Player") {
      GetComponent<PlayerMovement>().enabled = false;
      GetComponent<PlayerAttackBehavior>().enabled = false;
    }
    
    this.enabled = false;
  }

  //===================================================================================================================

  private IEnumerator flashColor(Color c){
    float currentTime = 0;
    float endTime = 0.25f;

    while(currentTime < endTime) {
      currentTime += Time.deltaTime;
      sr.color = Color.Lerp(c, originalColor, currentTime/endTime);
      yield return new WaitForFixedUpdate();
    }
  }

  //===================================================================================================================

  public float getHealth(){
    return currentHealth/maxHealth;
  }
}
