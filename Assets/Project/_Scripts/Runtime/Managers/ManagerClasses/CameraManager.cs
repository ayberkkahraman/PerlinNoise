using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;

namespace Project._Scripts.Runtime.Managers.ManagerClasses
{
  public class CameraManager : MonoBehaviour
  {
    public Camera MainCamera { get; set; }
    public Camera OrtoCamera;
    
    [SerializeField]private CinemachineVirtualCamera StageCamera;
    [SerializeField]private CinemachineVirtualCamera MonoCamera;
    [SerializeField]private CinemachineVirtualCamera TopCamera;
    [SerializeField]private CinemachineTargetGroup TargetGroup;
    
    public CinemachineVirtualCamera ActiveCamera { get; set; }

    private List<CinemachineVirtualCamera> _cameras;
    public delegate void OnCameraChangeRequest([CanBeNull]Transform firstTarget, [CanBeNull]Transform secondTarget);
    public static OnCameraChangeRequest OnCameraChangeRequestHandler;

    private bool _ortoActive;

    private void Awake()
    {
      MainCamera = Camera.main;

      _cameras = new List<CinemachineVirtualCamera>();
      _cameras = FindObjectsOfType<CinemachineVirtualCamera>().ToList();
    }

    private void OnEnable()
    {
      OnCameraChangeRequestHandler += UpdateCameraDistance;
    }

    private void OnDisable()
    {
      OnCameraChangeRequestHandler -= UpdateCameraDistance;
    }
    
    public void UpdateCameraDistance([CanBeNull]Transform firstTarget, [CanBeNull]Transform secondTarget)
    {
      TargetGroup.m_Targets[0].target = firstTarget;
      TargetGroup.m_Targets[1].target = secondTarget;
      
      ChangeActiveCamera(secondTarget != null ? (_ortoActive ? TopCamera : StageCamera) : MonoCamera);
    }

    public void SwitchCameraType()
    {
      _ortoActive = !_ortoActive;
      
      OrtoCamera.enabled = _ortoActive;
      MainCamera.enabled = !_ortoActive;

      ChangeActiveCamera(_ortoActive ? TopCamera : StageCamera);
    }

    public void ChangeActiveCamera(CinemachineVirtualCamera activeCamera)
    {
      _cameras.ForEach(x => x.Priority = 0);
      activeCamera.Priority = 10;
      ActiveCamera = activeCamera;
    }
  }
}
