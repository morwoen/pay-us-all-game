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
  public float salary;

  public Text employeeName;
  public Image avatar;

  void Start() {

  }

  void Update() {

  }

  public IEnumerator SetImage(string url) {
    if (url == null) yield break;

    using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url)) {
      yield return webRequest.SendWebRequest();

      if (!webRequest.isNetworkError) {
        var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
        avatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
      }
    }
  }
}
