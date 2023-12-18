using System;
using UnityEngine;

namespace Project._Scripts.Runtime.InGame.UIElements.UIPanel
{
  public class UIPanel : MonoBehaviour
  {
    private Animator _animator;

    private Action _openPanelCallback;
    private Action _closePanelCallback;
    
    private static readonly int Open = Animator.StringToHash("Open");
    private static readonly int Close = Animator.StringToHash("Close");

    private bool _isOpen;
    
    private void Awake()
    {
      _animator = GetComponent<Animator>();
      
      _openPanelCallback = () => _animator.SetTrigger(Open);
      _closePanelCallback = () => _animator.SetTrigger(Close);
    }

    public void Interact()
    {
      if(!_isOpen) _openPanelCallback?.Invoke();
      else _closePanelCallback?.Invoke();

      _isOpen = !_isOpen;
    }
  }
}
