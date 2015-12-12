using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

  //Delegates.
  public delegate void InputButton();
  public delegate void InputAxis(float f);

  //Jump input.
  public static event InputButton jumpPressed;

  //Horizontal axis input.
  public static event InputAxis horizontalAxis;

  //Attack input.
  public static event InputButton attackPressed;

  //===================================================================================================================

  private void Update() {
    if(Input.GetButtonDown("Jump") && jumpPressed != null) jumpPressed();
    if(Input.GetButtonDown("Attack") && attackPressed != null) attackPressed();
    if(horizontalAxis != null) horizontalAxis(Input.GetAxisRaw("Horizontal"));
  }
}
