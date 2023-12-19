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
    [SerializeField] private Slider NoiseSpeedSlider;
    [Space]
    [SerializeField] private TMP_Text WidthText;
    [SerializeField] private TMP_Text HeightText;
    [SerializeField] private TMP_Text NoiseSpeedText;
    
    public void SetWidthSlider(ref int value)
    {
      var sliderValue = Mathf.Clamp(Mathf.CeilToInt(WidthSlider.value * 100f), 1, 100);
      value = sliderValue;
      WidthText.text = $"{sliderValue}";
    }
    
    public void SetLengthSlider(ref int value)
    {
      var sliderValue = Mathf.Clamp(Mathf.CeilToInt(HeightSlider.value * 100f), 1, 100);
      value = sliderValue;
      HeightText.text = $"{sliderValue}";
    }

    public void SetNoiseSpeedSlider(ref float value)
    {
      var currentSliderValue = NoiseSpeedSlider.value * 10f;
      var sliderValue = Mathf.Clamp(currentSliderValue, 0, 10);
      value = sliderValue;
      NoiseSpeedText.text = $"{(double)currentSliderValue:F1}";
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
