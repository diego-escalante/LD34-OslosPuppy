using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

  [SerializeField] private float smoothness = 3f;
  private Transform target;

  //For screen shakes.
  private float duration = 1f;
  //===================================================================================================================

  private void Start(){
    target = GameObject.FindWithTag("Monster").transform;
  }

  //===================================================================================================================

  private void FixedUpdate(){
    
  }

  private void follow(){
    Vector3 newPos = new Vector3(target.position.x, transform.position.y, transform.position.z);

    if(transform.position == newPos) return;
    if(Vector2.Distance(transform.position, target.position) < 0.1f) transform.position = newPos;
    else transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * smoothness);
  }

  private void shake(){

  }

  private IEnumerator shaking(){
    float endTime = Time.time + duration;

    while(Time.time < endTime){
      //SHAKE IT.
      yield return new WaitForFixedUpdate();
    }
  }
}
