using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour {

  private int score = 0;
  private Text scoreTxt;

  //===================================================================================================================

  private void Start(){
    scoreTxt = GetComponent<Text>();
  }

  //===================================================================================================================

  public void modifyScore(int amount){
    score += amount;
    scoreTxt.text = "Score: " + score.ToString();
  }

}
