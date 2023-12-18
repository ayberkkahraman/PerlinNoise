using System;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;

namespace Project._Scripts.Runtime.Managers.ManagerClasses
{
  public class CameraManager : MonoBehaviour
  {
    public Camera MainCamera { get; set; }
    [SerializeField]private CinemachineVirtualCamera StageCamera;
    [SerializeField] private CinemachineTargetGroup TargetGroup;

    private void Awake()
    {
      MainCamera = Camera.main;
    }
    public void UpdateCameraDistance([CanBeNull]Transform firstTarget, [CanBeNull]Transform secondTarget)
    {
      TargetGroup.m_Targets[0].target = firstTarget;
      if(secondTarget != null)
        TargetGroup.m_Targets[1].target = secondTarget;
    }
  }
}
