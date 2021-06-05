using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;

public class GameManager : MonoBehaviour
{
  public float cash = 10000;
  public float payGap = 0;
  public float payGapLose = 20000;
  public float revenue = 0;
  public float hireCooldown = 10;
  public float companyScaleFactor = 1;
  public float companyScaleFactorRate = 0.1f;

  public GameObject hirePrefab;
  public GameObject employeePrefab;
  public ScrollRect scrollRect;
  public ModalWindowManager modal;
  public ModalWindowManager quitModal;

  private GameObject hireButton;
  private float currentHireCooldown;
  private List<EmployeeScript> employees = new List<EmployeeScript>();
  private bool firstResign = true;

  // Start is called before the first frame update
  void Start() {
    currentHireCooldown = float.MaxValue;
    StartCoroutine(InitialRun());

    // TODO: Wanting a promotion
    // TODO: score factor, display it at the final screen
  }

  void Update() {
    if (Input.GetButtonDown("Cancel")) {
      quitModal.OpenWindow();
    }

    if (currentHireCooldown > 0) {
      currentHireCooldown -= Time.deltaTime;
    } else {
      AddHireButton();
    }

    cash += revenue * Time.deltaTime;
    companyScaleFactor += companyScaleFactorRate * Time.deltaTime;

    revenue = 0;
    var salaries = new Dictionary<EmployeeScript.Gender, float>();
    var totals = new Dictionary<EmployeeScript.Gender, int>();
    salaries.Add(EmployeeScript.Gender.Male, 0);
    salaries.Add(EmployeeScript.Gender.Female, 0);
    totals.Add(EmployeeScript.Gender.Male, 0);
    totals.Add(EmployeeScript.Gender.Female, 0);
    foreach (var employee in employees) {
      revenue += employee.productivity * companyScaleFactor;
      salaries[employee.gender] += employee.salary * ((100 - employee.productivity) / 100);
      totals[employee.gender] += 1;
    }

    payGap = Mathf.Abs((salaries[EmployeeScript.Gender.Male] / totals[EmployeeScript.Gender.Male]) - (salaries[EmployeeScript.Gender.Female] / totals[EmployeeScript.Gender.Female]));
    cash -= (salaries[EmployeeScript.Gender.Male] + salaries[EmployeeScript.Gender.Female]) / 1000 * Time.deltaTime;

    if (payGap > payGapLose) {
      StartCoroutine(LoseGame());
    }
  }

  public void HireEmployee() {
    Destroy(hireButton);
    hireButton = null;
    currentHireCooldown = hireCooldown + employees.Count * hireCooldown / 2;

    StartCoroutine(CreateEmployee());
  }

  IEnumerator CreateEmployee() {
    using (UnityWebRequest webRequest = UnityWebRequest.Get("https://randomuser.me/api/")) {
      yield return webRequest.SendWebRequest();

      if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
        var res = JsonUtility.FromJson<PersonResponse>(webRequest.downloadHandler.text);
        var person = res.results[0];

        AddEmployee(person);
      }
    }
  }

  void AddEmployee(PersonData person) {
    if (person == null) return;

    var employee = Instantiate(employeePrefab, scrollRect.content).GetComponent<EmployeeScript>();
    employees.Add(employee);
    employee.employeeName.text = person?.name?.first + " " + person?.name?.last + " " + (person?.gender == "male" ? "♂" : "♀");
    employee.gender = person?.gender == "male" ? EmployeeScript.Gender.Male : EmployeeScript.Gender.Female;
    employee.happinessRate = Random.Range(0.01f, 0.1f);
    employee.salary = Random.Range(20, 30) * 1000;
    employee.productivity = Random.Range(5, 50);
    employee.productivityRate = Random.Range(0.01f, 0.05f);

    scrollRect.normalizedPosition = Vector2.zero;
    StartCoroutine(employee.SetImage(person?.picture?.large));
  }
  
  public void Resign(EmployeeScript employee) {
    employees.Remove(employee);
    Destroy(employee.gameObject);
    if (firstResign) {
      firstResign = false;
      modal.titleText = "Tip";
      modal.descriptionText = "One of your employees just resigned\nHappy employees will stay and be more productive";
      modal.UpdateUI();
      modal.OpenWindow();
    }
  }

  IEnumerator InitialRun() {
    modal.titleText = "Tutorial";
    modal.descriptionText = "Welcome to Pay Us All\nA game about solving the gender pay gap";
    modal.UpdateUI();
    modal.OpenWindow();
    
    while (modal.isOn) {
      yield return null;
    }

    yield return new WaitForSeconds(1.5f);

    var female = new PersonData();
    female.gender = "female";
    female.name = new PersonName();
    female.name.first = "Christa";
    female.name.last = "Blanchard";
    female.picture = new PersonPicture();
    female.picture.large = "https://randomuser.me/api/portraits/women/2.jpg";
    AddEmployee(female);

    yield return new WaitForSeconds(1.5f);

    var male = new PersonData();
    male.gender = "male";
    male.name = new PersonName();
    male.name.first = "Arnaud";
    male.name.last = "Lam";
    male.picture = new PersonPicture();
    male.picture.large = "https://randomuser.me/api/portraits/men/1.jpg";
    AddEmployee(male);

    yield return new WaitForSeconds(1.5f);

    modal.titleText = "Tutorial";
    modal.descriptionText = "Oh! Look!\nYou have two new employees";
    modal.UpdateUI();
    modal.OpenWindow();

    while (modal.isOn) {
      yield return null;
    }

    yield return new WaitForSeconds(1.5f);

    modal.titleText = "Tutorial";
    modal.descriptionText = "You can Increase and Decrease their salaries\nto keep them happy without a pay gap";
    modal.UpdateUI();
    modal.OpenWindow();

    while (modal.isOn) {
      yield return null;
    }

    yield return new WaitForSeconds(5f);

    modal.titleText = "Tutorial";
    modal.descriptionText = "Keep your employees happy\nThey will increase their productivity over time";
    modal.UpdateUI();
    modal.OpenWindow();

    while (modal.isOn) {
      yield return null;
    }

    yield return new WaitForSeconds(10f);

    currentHireCooldown = 0;

    modal.titleText = "Grow your company";
    modal.descriptionText = "You can now hire a new employee\nThey are hard to find, so treat them well";
    modal.UpdateUI();
    modal.OpenWindow();

    // TODO: Show how the male wants raise
  }

  IEnumerator LoseGame() {
    modal.titleText = "Oh NO!";
    modal.descriptionText = "The gender pay gap in your company exceeded £20000";

    var btn = modal.confirmButton.GetComponent<ButtonManagerWithIcon>();
    btn.buttonText = "Quit";
    btn.UpdateUI();

    modal.UpdateUI();
    modal.OpenWindow();

    while (modal.isOn) {
      yield return null;
    }

    SceneManager.LoadScene(0);
  }

  void AddHireButton() {
    if (hireButton == null) {
      hireButton = Instantiate(hirePrefab, scrollRect.content);
    }
  }

  public void ToMenu() {
    SceneManager.LoadScene(0);
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
