using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Michsky.UI.ModernUIPack;

public class GameManager : MonoBehaviour
{
  public float cash = 10000;
  public float payGap = 0;
  public float revenue = 0;
  public float hireCooldown = 10;

  public GameObject hirePrefab;
  public GameObject employeePrefab;
  public ScrollRect scrollRect;
  public ModalWindowManager modal;

  private GameObject hireButton;
  private float currentHireCooldown;
  private List<EmployeeScript> employees = new List<EmployeeScript>();

  // Start is called before the first frame update
  void Start() {
    AddHireButton();
  }

  void Update() {
    if (currentHireCooldown > 0) {
      currentHireCooldown -= Time.deltaTime;
    } else {
      AddHireButton();
    }
  }

  public void HireEmployee() {
    Destroy(hireButton);
    hireButton = null;
    currentHireCooldown = hireCooldown;

    modal.descriptionText = "WE ARE DOING IT";
    modal.UpdateUI();
    modal.OpenWindow();

    StartCoroutine(CreateEmployee());
  }

  IEnumerator CreateEmployee() {
    using (UnityWebRequest webRequest = UnityWebRequest.Get("https://randomuser.me/api/")) {
      yield return webRequest.SendWebRequest();

      if (!webRequest.isNetworkError) {
        var res = JsonUtility.FromJson<PersonResponse>(webRequest.downloadHandler.text);
        var person = res.results[0];

        var employee = Instantiate(employeePrefab, scrollRect.content).GetComponent<EmployeeScript>();
        employees.Add(employee);
        employee.employeeName.text = person?.name?.first + " " + person?.name?.last + " " + (person?.gender == "male" ? "♂" : "♀");
        employee.gender = person?.gender == "male" ? EmployeeScript.Gender.Male : EmployeeScript.Gender.Female;
        scrollRect.normalizedPosition = Vector2.zero;
        StartCoroutine(employee.SetImage(person?.picture?.large));
      }
    }
  }

  void AddHireButton() {
    if (hireButton == null) {
      hireButton = Instantiate(hirePrefab, scrollRect.content);
    }
  }

  [System.Serializable]
  private class PersonData
  {
    public string gender;
    public PersonName name;
    public PersonPicture picture;
  }

  [System.Serializable]
  private class PersonResponse
  {
    public PersonData[] results;
  }

  [System.Serializable]
  private class PersonName
  {
    public string first;
    public string last;
  }

  [System.Serializable]
  private class PersonPicture
  {
    public string large;
  }
}
