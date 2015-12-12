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

  //Shield input.
  public static event InputButton shieldPressed;
  public static event InputButton shieldReleased;

  //===================================================================================================================

  private void Update() {
    if(Input.GetButtonDown("Jump") && jumpPressed != null) jumpPressed();
    if(Input.GetButtonDown("Attack") && attackPressed != null) attackPressed();
    if(Input.GetButtonDown("Shield") && shieldPressed != null) shieldPressed();
    if(Input.GetButtonUp("Shield") && shieldReleased != null) shieldReleased();
    if(horizontalAxis != null) horizontalAxis(Input.GetAxisRaw("Horizontal"));
  }
}
