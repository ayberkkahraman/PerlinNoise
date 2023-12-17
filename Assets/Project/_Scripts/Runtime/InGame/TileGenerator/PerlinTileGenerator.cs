using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
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
    public PerlinCube.PerlinCube PerlinCube;
    public Transform CubesHolder;
    public Vector2Int Size;
    public Vector2 Offset;

    private List<PerlinCube.PerlinCube> _cubes;
    
    [Range(.01f, 10f)][SerializeField] private float NoiseSpeed = 1f;
    private float[] _randomScales;

    [MinMaxSlider(0f, 2f)]
    public Vector2 HeightBoundaries;

    public float InternalDelay = .01f;

    private PerlinCubeJob _job;
    private JobHandle _jobHandle;

    private Transform[] _cubeTransforms;
    private NativeArray<float> _nativeValues;
    private TransformAccessArray _accessArray;

    private void Start()
    {
      _cubes = new List<PerlinCube.PerlinCube>();
      GenerateTile();
      _cubeTransforms = new Transform[Size.x * Size.y];
      _randomScales = new float[_cubeTransforms.Length];

      for (int i = 0; i < _randomScales.Length; i++) _randomScales[i] = Random.Range(.2f, 2f);
      
      _jobHandle = new JobHandle();
    
      for (int i = 0; i < _cubes.Count; i++) { _cubeTransforms[i] = _cubes[i].transform;}
      
      _nativeValues = new NativeArray<float>(_cubeTransforms.Length, Allocator.Persistent);
      _accessArray = new TransformAccessArray(_cubeTransforms);
    }

    private void Update()
    {
      _job.Values = _nativeValues;

      for (int i = 0; i < _nativeValues.Length; i++)
      {
        _job.Values[i] = CalculatePerlinHeight(i);
        _cubes[i].UpdateColor(HeightBoundaries, _job.Values[i], GetColorValue(i));
      }
      
      _jobHandle = _job.Schedule(_accessArray);
      
    }

    private void LateUpdate()
    {
      _jobHandle.Complete();
    }
    
    private void Execute()
    {
      foreach (var cube in _cubes)
      {
        cube.CubeUpdate(NoiseSpeed, HeightBoundaries, Time.time);
      }
    }

    public float GetColorValue(int index)
    {
      return math.lerp(0.2f, 1f, (_nativeValues[index] - HeightBoundaries.x) / (HeightBoundaries.y - HeightBoundaries.x));
    }

    private float CalculatePerlinHeight(int index)
    {
      var perlinValue = Mathf.PerlinNoise(Time.time * _randomScales[index] * NoiseSpeed, 0.0f);
      return math.lerp(HeightBoundaries.x, HeightBoundaries.y, perlinValue);
    }
    private void OnDestroy()
    {
      _nativeValues.Dispose();
      _jobHandle.Complete();
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
    }
  }
  
  [BurstCompile]
  public struct PerlinCubeJob : IJobParallelForTransform
  {
    public NativeArray<float> Values;
    public float2 HeightLimit;
    public void Execute(int index, TransformAccess transform)
    {
      transform.localScale = new Vector3(transform.localScale.x, Values[index], transform.localScale.z);
    }
  }
}
