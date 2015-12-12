using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour {

  [SerializeField] private float maxHealth = 100;

  private float currentHealth;
  private SpriteRenderer sr;
  private Color originalColor;

  //===================================================================================================================

  private void Start() {
    currentHealth = maxHealth;
    sr = GetComponent<SpriteRenderer>();
    originalColor = sr.color;
  }

  //===================================================================================================================

  public void modifyHealth(float amount) {
    currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    if(currentHealth == 0) death();

    if(amount < 0) StartCoroutine("flashColor",Color.white);
  }

  //===================================================================================================================

  private void death() {
    if(gameObject.name == "Player") SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Restart.
    Destroy(gameObject);
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

}
