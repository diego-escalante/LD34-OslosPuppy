using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour {

  [SerializeField] private float maxHealth = 100;

  private float currentHealth;
  private SpriteRenderer sr;
  private Color originalColor;

  private bool isPlayer = false;
  private PlayerAttackBehavior playerAtk;
  private Animator anim;

  //===================================================================================================================

  private void Start() {
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

    else if(gameObject.tag == "Monster") 
      GetComponent<MonsterBehavior>().enabled = false; //YEAH YEAH. I'll get to it soon.

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
