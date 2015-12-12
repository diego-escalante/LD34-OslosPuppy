using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

  [SerializeField] private float smoothness = 3f;
  private Transform target;

  //For screen shakes.
  private Animator anim;
  // public bool shakeTemp = false;
  
  //===================================================================================================================

  private void Start(){
    target = GameObject.FindWithTag("Monster").transform;
    anim = GetComponent<Animator>();
  }

  //===================================================================================================================

  private void FixedUpdate(){
    follow();
    // if(shakeTemp) shake();
  }

  //===================================================================================================================

  private void follow(){
    Vector3 newPos = new Vector3(target.position.x, transform.parent.position.y, transform.parent.position.z);

    if(transform.parent.position == newPos) return;
    if(Vector2.Distance(transform.parent.position, target.position) < 0.1f) transform.parent.position = newPos;
    else transform.parent.position = Vector3.Lerp(transform.parent.position, newPos, Time.deltaTime * smoothness);
  }

  // private void shake() {
  //   shakeTemp = false;
  //   anim.SetTrigger("Shake");
  // }
}
