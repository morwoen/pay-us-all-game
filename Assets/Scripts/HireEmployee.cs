using UnityEngine;

public class HireEmployee : MonoBehaviour
{
  public void OnClick() {
    var manager = FindObjectOfType<GameManager>();
    manager.HireEmployee();
  }
}
