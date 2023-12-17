using System;
using DG.Tweening;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Project._Scripts.Runtime.InGame.PerlinCube
{
  public class PerlinCube : MonoBehaviour
  {
    #region Components
    private Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
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
        // UpdateScale(speed, limits, time);
        // UpdateColor(limits);
    }

    private void UpdateScale(float speed, Vector2 limits, float time)
    {
      CalculatePerlinHeight(speed, limits, time);

      transform.localScale = new Vector3(transform.localScale.x, _perlinHeight, transform.localScale.z);
    }
    
    private void CalculatePerlinHeight(float speed, Vector2 limits, float time)
    {
      _perlinValue = Mathf.PerlinNoise(time * _perlinScale * speed, 0.0f);
      _perlinHeight = math.lerp(limits.x, limits.y, _perlinValue);
    }

    public void UpdateColor(Vector2 limits, float value, float colorValue)
    {
      _currentColor = new Color(colorValue, colorValue, colorValue, 1);
      _materialPropertyBlock.SetColor("_Color", _currentColor);
      _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

 

    private void Initialize()
    {
      _materialPropertyBlock = new MaterialPropertyBlock();
      _renderer = GetComponent<Renderer>();
      _perlinScale = Random.Range(0f, 1f);
    }
  }
}
