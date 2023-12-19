using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Project._Scripts.Runtime.Managers.BaseManager.ManagerContainer;
using Project._Scripts.Runtime.Managers.ManagerClasses;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;


namespace Project._Scripts.Runtime.InGame.TileGenerator
{
  public class PerlinTileGenerator : MonoBehaviour
  {
    [SerializeField]private PerlinCube.PerlinCube PerlinCube;
    [SerializeField]private Vector2Int Size;
    [SerializeField]private Vector2 Offset;

    private List<PerlinCube.PerlinCube> _cubes;
    
    [Range(.01f, 10f)][SerializeField] private float NoiseSpeed = 1f;
    
    [MinMaxSlider(0f, 2f)]
    [SerializeField]private Vector2 HeightBoundaries;

    private PerlinCubeJob _job;
    private JobHandle _jobHandle;

    private Transform[] _cubeTransforms;
    
    private NativeArray<float> _nativeScaleValues;
    private NativeArray<float> _nativePerlinValues;
    private float[] _randomScales;
    private TransformAccessArray _accessArray;

    private void Awake()
    {
      _cubes = new List<PerlinCube.PerlinCube>();
    }
    private void Start()
    {
      GenerateTile();
      _jobHandle = new JobHandle();
    }

    private void Update()
    {
      _job.ScaleValues = _nativeScaleValues;

      for (int i = 0; i < _nativeScaleValues.Length; i++)
      {
        _job.ScaleValues[i] = CalculatePerlinHeight(i);
        UpdateCubeColor(_cubes[i], GetColorValue(i));
      }
      
      _jobHandle = _job.Schedule(_accessArray);
    }

    private void LateUpdate()
    {
      _jobHandle.Complete();
    }
    
    public float GetColorValue(int index)
    {
      return math.lerp(0.2f, 1f, (_nativeScaleValues[index] - HeightBoundaries.x) / (HeightBoundaries.y - HeightBoundaries.x));
    }

    public void UpdateCubeColor(PerlinCube.PerlinCube cube, float colorValue)
    {
      cube.CurrentColor = new Color(colorValue, colorValue, colorValue, 1);
      cube.MaterialPropertyBlock.SetColor("_Color", cube.CurrentColor);
      cube.Renderer.SetPropertyBlock(cube.MaterialPropertyBlock);
    }

    private float CalculatePerlinHeight(int index)
    {
      var time = Time.time;
      var perlinValue = Mathf.PerlinNoise(time * _randomScales[index] * NoiseSpeed, 0.0f);
      return math.lerp(HeightBoundaries.x, HeightBoundaries.y, perlinValue);
    }

    private void OnDestroy()
    {
      _nativeScaleValues.Dispose();
      _nativePerlinValues.Dispose();
      _jobHandle.Complete();
      _accessArray.Dispose();
    }

    public void GenerateTile()
    {
      _cubes.ForEach(x => Destroy(x.gameObject));
      _cubes = new List<PerlinCube.PerlinCube>();
      var cubeSize = PerlinCube.transform.localScale;
      for (int x = 0; x < Size.x; x++)
      {
        for (int y = 0; y < Size.y; y++)
        {
          Vector3 position = new Vector3(
            x * cubeSize.x - (Size.x * cubeSize.x * 0.5f) + Offset.x * x,
            0f,
            y * cubeSize.z - (Size.y * cubeSize.z * 0.5f) + Offset.y * y
          );

          PerlinCube.PerlinCube cube = Instantiate(PerlinCube, position, Quaternion.identity, transform);
      
          _cubes.Add(cube);
        }
      }

      var secondTransform = _cubes[0].transform == _cubes[^1].transform ? null : _cubes[^1].transform;
      CameraManager.OnCameraChangeRequestHandler(_cubes[0].transform, secondTransform);

      _cubeTransforms = new Transform[Size.x * Size.y];
      _randomScales = new float[Size.x * Size.y];
      
      for (int i = 0; i < _cubes.Count; i++)
      {
        _cubeTransforms[i] = _cubes[i].transform;
        _randomScales[i] = Random.Range(.2f, 2f);
      }
      
      _nativeScaleValues = new NativeArray<float>(_cubeTransforms.Length, Allocator.Persistent);
      _nativePerlinValues = new NativeArray<float>(_cubeTransforms.Length, Allocator.Persistent);
      _accessArray = new TransformAccessArray(_cubeTransforms);
    }
    public void SetWidth()
    {
      int sizeX = Size.x;
      ManagerContainer.Instance.GetInstance<UIManager>().SetWidthSlider(ref sizeX);
      Size.x = sizeX;
    }
    public void SetHeight()
    {
      int sizeY = Size.y;
      ManagerContainer.Instance.GetInstance<UIManager>().SetHeightSlider(ref sizeY);
      Size.y = sizeY;
    }
  }
  
  [BurstCompile]
  public struct PerlinCubeJob : IJobParallelForTransform
  {
    public NativeArray<float> ScaleValues;
    public void Execute(int index, TransformAccess transform)
    {
      var newScale = transform.localScale;
      newScale.y = ScaleValues[index];
      transform.localScale = newScale;
    }
  }
}
