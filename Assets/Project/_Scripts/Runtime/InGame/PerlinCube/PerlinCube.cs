using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project._Scripts.Runtime.InGame.PerlinCube
{
  public class PerlinCube : MonoBehaviour
  {
    #region Components
    private MeshRenderer _meshRenderer;
    private Material _material;
    #endregion
    private float _perlinScale = 1f;
    

    private float _currentColorValue;
    private float _targetHeight;

    private Vector3 _defaultScale;
    private Vector3 _currentScale;

    private bool _ready;

    private void Start()
    {
      Initialize();
    }

    public void TraverseScale(float speed, Vector2 limits)
    {
      
    }
    
    public void CubeUpdate(float speed, Vector2 limits)
    {
        UpdateScale(speed, limits);
        UpdateColor(limits);
    }

    private void UpdateScale(float speed, Vector2 limits)
    {
      _targetHeight = HeightCalculation(speed, limits);
      
      _currentScale = transform.localScale;
      _currentScale.y = _targetHeight;
      transform.localScale = _currentScale;
    }

    private void UpdateColor(Vector2 limits)
    {
      _currentColorValue = Mathf.Lerp(0.2f, 1f, (_targetHeight - limits.x) / (limits.y - limits.x));
      
      Color currentColor = _material.color;
      currentColor.r = _currentColorValue;
      currentColor.g = _currentColorValue;
      currentColor.b = _currentColorValue;
      _material.color = currentColor;
    }

    private float HeightCalculation(float speed, Vector2 limits)
    {
      float perlinValue = Mathf.PerlinNoise(Time.time * _perlinScale * speed, 0.0f);
      float height = Mathf.Lerp(limits.x, limits.y, perlinValue);
      return height;
    }

    private void Initialize()
    {
      _meshRenderer = GetComponent<MeshRenderer>();
      _material = _meshRenderer.material;
      _perlinScale = Random.Range(.01f, 1f);
    }
  }
}
