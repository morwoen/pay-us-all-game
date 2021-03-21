using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
  public GameManager manager;
  public Text cash;
  public Text gap;
  public Text gain;

  private string cashMessage;
  private string gapMessage;
  private string gainMessage;

  void Start() {
    cashMessage = cash.text;
    gapMessage = gap.text;
    gainMessage = gain.text;
  }

  void Update() {
    var payGap = float.IsNaN(manager.payGap) ? 0 : manager.payGap;
    cash.text = cashMessage.Replace("{cash}", manager.cash.ToString("0"));
    gap.text = gapMessage.Replace("{gap}", payGap.ToString("0"));
    gain.text = gainMessage.Replace("{gain}", manager.revenue.ToString("0"));
  }
}
