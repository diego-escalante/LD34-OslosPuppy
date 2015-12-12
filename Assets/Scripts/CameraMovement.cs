using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

  [SerializeField] private float smoothness = 3f;
  private Transform target;
  private Animator anim;
  
  //===================================================================================================================

  private void Start(){
    target = GameObject.FindWithTag("Monster").transform;
    anim = GetComponent<Animator>();
  }

  //===================================================================================================================

  private void FixedUpdate(){
    follow();
  }

  //===================================================================================================================

  private void follow(){
    Vector3 newPos = new Vector3(target.position.x, transform.parent.position.y, transform.parent.position.z);

    if(transform.parent.position == newPos) return;
    if(Vector2.Distance(transform.parent.position, target.position) < 0.1f) transform.parent.position = newPos;
    else transform.parent.position = Vector3.Lerp(transform.parent.position, newPos, Time.deltaTime * smoothness);
  }

  //===================================================================================================================

  public void shake() {
    anim.SetTrigger("Shake");
  }
}
