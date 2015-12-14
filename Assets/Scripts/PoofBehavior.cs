using UnityEngine;
using System.Collections;

public class PoofBehavior : MonoBehaviour {

  private void Start(){
    Destroy(gameObject, 6f);
  }
}
