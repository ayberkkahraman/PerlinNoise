using System;
using Project._Scripts.Runtime.InGame.UIElements.UIPanel;
using UnityEngine;

namespace Project._Scripts.Runtime.Managers.ManagerClasses
{
  public class UIManager : MonoBehaviour
  {
    [SerializeField]private UIPanel UIPanel;

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Tab))
      {
        UIPanel.Interact();
      }
    }
  }
}
