using UnityEngine;
using System.Collections;

  public class MonsterBehavior : MonoBehaviour {

  [SerializeField] private float sizeMultiplier = 1f;
  private float camSize;
  private Camera cam;
  private Transform camTrans;

  //===================================================================================================================

  private void Start() {
    //Get camera stuff.
    cam = Camera.main;
    camTrans = cam.transform.parent;
    camSize = Camera.main.orthographicSize;
  }

  private void FixedUpdate() { setSize(sizeMultiplier, false);}

  //===================================================================================================================

  private void setSize(float f, bool relative=true) {
    //Increase the size of the monster.
    sizeMultiplier = relative ? sizeMultiplier + f : f;
    transform.localScale = new Vector3(1 * sizeMultiplier, 1 * sizeMultiplier, 1);


    float monsView = cam.WorldToViewportPoint(transform.position).y;
    cam.orthographicSize = camSize * sizeMultiplier;
    float deltaWorldPos = cam.ViewportToWorldPoint(new Vector3(0, monsView, cam.nearClipPlane)).y - transform.position.y;


    camTrans.Translate(new Vector3(0, -deltaWorldPos, 0));
  }
}
