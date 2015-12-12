using UnityEngine;
using System.Collections;

  public class MonsterBehavior : MonoBehaviour {

  private float sizeMultiplier = 1f;
  private float camSize;

  //===================================================================================================================

  private void Start() {
    //Get the original camera size.
    camSize = Camera.main.orthographicSize;
  }

  //===================================================================================================================

  private void setSize(float f, bool relative=true) {
    //Increase the size of the monster, keeping the camera perfectly scaled to it.
    sizeMultiplier = relative ? sizeMultiplier + f : f;
    transform.localScale = new Vector3(1 * sizeMultiplier, 1 * sizeMultiplier, 1);
    Camera.main.orthographicSize = camSize * sizeMultiplier;
  }
}
