using UnityEngine;
using System.Collections;

public class MonsterFollow : MonsterBase {
  
  protected override void Start() {
    base.Start();
    
    //Follow the player.
    target = GameObject.FindWithTag("Player").transform;
  }
}
