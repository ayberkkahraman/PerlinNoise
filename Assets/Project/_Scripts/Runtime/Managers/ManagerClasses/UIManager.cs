using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project._Scripts.Runtime.Managers.ManagerClasses
{
  public class UIManager : MonoBehaviour
  {
    [SerializeField] private Slider WidthSlider;
    [SerializeField] private Slider HeightSlider;
    [Space]
    [SerializeField] private TMP_Text WidthText;
    [SerializeField] private TMP_Text HeightText;
    
    public void SetWidthSlider(ref int widthValue)
    {
      var sliderValue = Mathf.Clamp(Mathf.CeilToInt(WidthSlider.value * 100f), 1, 100);
      widthValue = sliderValue;
      WidthText.text = $"{sliderValue}";
    }
    
    public void SetHeightSlider(ref int widthValue)
    {
      var sliderValue = Mathf.Clamp(Mathf.CeilToInt(HeightSlider.value * 100f), 1, 100);
      widthValue = sliderValue;
      HeightText.text = $"{sliderValue}";
    }
    // [SerializeField]private UIPanel UIPanel;
    //
    // private void Update()
    // {
    //   if (Input.GetKeyDown(KeyCode.Tab))
    //   {
    //     UIPanel.Interact();
    //   }
    // }
  }
}
