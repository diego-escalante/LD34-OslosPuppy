using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

  //Delegates.
  public delegate void InputButton();
  public delegate void InputAxis(float f);

  //Jump input.
  public static event InputButton jumpPressed;
  public static event InputAxis horizontalAxis;

  //===================================================================================================================

  private void Update() {
    if(Input.GetButtonDown("Jump") && jumpPressed != null) jumpPressed();

    if(horizontalAxis != null) horizontalAxis(Input.GetAxisRaw("Horizontal"));
  }
}
