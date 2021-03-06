using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmployeeScript : MonoBehaviour
{
  public enum Gender
  {
    Male,
    Female,
  }

  public Gender gender;
  public float productivity;
  public float productivityRate;
  public float salary;
  public float happiness;
  public float happinessRate;
  public float happinessRateChange = 0.001f;
  public Sprite manFace;
  public Sprite womanFace;

  public Text employeeName;
  public Image avatar;
  public Text salaryTextField;
  public Text happinessTextField;

  private string salaryText;
  private string happinessText;

  void Start() {
    salaryText = salaryTextField.text;
    happinessText = happinessTextField.text;
    happiness = 100;
  }

  void Update() {
    salaryTextField.text = salaryText.Replace("{salary}", salary.ToString("0")).Replace("{productivity}", productivity.ToString("0"));
    happinessTextField.text = happinessText.Replace("{happy}", happiness.ToString("0"));
    happinessTextField.color = Color.Lerp(Color.red, Color.green, happiness/100);

    happiness = Mathf.Clamp(happiness - (happinessRate * Time.deltaTime), 0, 100);
    happinessRate += happinessRateChange * Time.deltaTime;
    productivity = Mathf.Clamp(productivity + (productivityRate * Time.deltaTime * (happiness/100)), 0, 100);

    if (happiness <= 0) {
      FindObjectOfType<GameManager>().Resign(this);
    }
  }

  public void SetImage() {
    if (gender == Gender.Female) {
      avatar.sprite = womanFace;
    } else {
      avatar.sprite = manFace;
    }
  }

  public void IncreaseSalary() {
    salary += 1000;
    happiness = Mathf.Clamp(happiness + 5, 0, 100);
    happinessRate -= happinessRateChange * 10;
  }

  public void DecreaseSalary() {
    salary -= 1000;
    happiness = Mathf.Clamp(happiness - 10, 0, 100);
    happinessRate += happinessRateChange * 5;
  }
}
