using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FinalizeScore : MonoBehaviour {

  public Transform t;
  private ScoreKeeper sk;
  private Text txt;

  void Start(){
    sk = t.GetComponent<ScoreKeeper>();
    txt = GetComponent<Text>();
  }

	// Update is called once per frame
	void Update () {
	 txt.text = "Score: " + sk.score.ToString();
	}
}
