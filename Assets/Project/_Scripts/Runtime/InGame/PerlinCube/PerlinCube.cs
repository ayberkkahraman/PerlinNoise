using System;
using DG.Tweening;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    private float _perlinValue;
    private float _perlinHeight;

    private Vector3 _defaultScale;
    private Vector3 _currentScale;

    private bool _ready;
    private Color _currentColor;

    private void Awake()
    {
      Initialize();
    }

    public void CubeUpdate(float speed, Vector2 limits, float time)
    {
        UpdateScale(speed, limits, time);
        UpdateColor(limits);
    }

    private void UpdateScale(float speed, Vector2 limits, float time)
    {
      CalculatePerlinHeight(speed, limits, time);

      _currentScale = transform.localScale;
      _currentScale.y = _perlinHeight;
      transform.localScale = _currentScale;
    }

    private void UpdateColor(Vector2 limits)
    {
      _currentColorValue = math.lerp(0.2f, 1f, (_perlinHeight - limits.x) / (limits.y - limits.x));

      _currentColor.r = _currentColorValue;
      _currentColor.g = _currentColorValue;
      _currentColor.b = _currentColorValue;
      _material.color = _currentColor;
    }

    private void CalculatePerlinHeight(float speed, Vector2 limits, float time)
    {
      _perlinValue = Mathf.PerlinNoise(time * _perlinScale * speed, 0.0f);
      _perlinHeight = math.lerp(limits.x, limits.y, _perlinValue);
    }

    private void Initialize()
    {
      _meshRenderer = GetComponent<MeshRenderer>();
      _material = _meshRenderer.material;
      _material.enableInstancing = true;
      _perlinScale = Random.Range(0f, 1f);
    }
  }
}
