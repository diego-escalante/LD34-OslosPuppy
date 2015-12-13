using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {

  private float size = 1;

  //Camera scaling stuff.
  private Camera cam;
  private Transform camTrans;
  private float camSize;

  //===================================================================================================================

  private void Start() {
    //Get camera stuff.
    cam = Camera.main;
    camTrans = cam.transform.parent;
    camSize = Camera.main.orthographicSize;
  }

  //===================================================================================================================

  public void grow(float amount) {
    //Increase the size of the monster.
    size += amount;
    transform.localScale = new Vector3(1 * size * (transform.localScale.x >= 0 ? 1 : -1), 1 * size, 1);

    //Scale the camera.
    float monsView = cam.WorldToViewportPoint(transform.position).y;
    cam.orthographicSize = camSize * size;
    float deltaWorldPos = cam.ViewportToWorldPoint(new Vector3(0, monsView, cam.nearClipPlane)).y - transform.position.y;
    camTrans.Translate(new Vector3(0, -deltaWorldPos, 0));
  }

}
