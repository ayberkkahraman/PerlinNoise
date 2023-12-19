using System;
using System.Collections;
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
    private RenderParams _rp;
    private PerlinCubeJob _job;
    private JobHandle _jobHandle;
    
    [SerializeField]private PerlinCube.PerlinCube PerlinCube;
    [SerializeField]private Mesh Mesh;
    [SerializeField]private Material Material;
    [SerializeField]private Material LitMaterial;
    
    [Space]
    [SerializeField]private Vector2Int Size;
    [SerializeField]private Vector2 Offset;
    
    [Space]
    [Range(.01f, 10f)]
    [SerializeField] private float NoiseSpeed = 1f;
    
    [MinMaxSlider(0f, 2f)]
    [SerializeField]private Vector2 HeightBoundaries;

    private List<PerlinCube.PerlinCube> _cubes;
    private NativeArray<Matrix4x4> _nativeMatrices;
    private Transform[] _cubeTransforms;
    
    private float[] _randomScales;
    
    private NativeArray<float3> _nativePositions;
    private NativeArray<float> _nativeScaleValues;
    private NativeArray<float> _nativePerlinValues;
    
    private TransformAccessArray _accessArray;

    private bool _applyColorChange;
    private bool _applyScaleChange;
    private void Awake()
    {
      _cubes = new List<PerlinCube.PerlinCube>();
    }
    private void Start()
    {
      // ToggleColor();
      GenerateTile();
      _jobHandle = new JobHandle();

      _rp = new RenderParams(Material);
    }

    private void Update()
    {
      _job.ScaleValues = _nativeScaleValues;
      _job.Matrices = _nativeMatrices;

      for (int i = 0; i < _nativeScaleValues.Length; i++)
      {
        if(_applyScaleChange)_job.ScaleValues[i] = CalculatePerlinHeight(i);
        if(_applyColorChange)UpdateCubeColor(_cubes[i], GetColorValue(i));
      }
      
      _jobHandle = _job.Schedule(_accessArray);
      _jobHandle.Complete();
      
      Graphics.RenderMeshInstanced(_rp, Mesh, 0, _nativeMatrices);
    }

    public float GetColorValue(int index)
    {
      return math.lerp(0.2f, 1f, (_nativeScaleValues[index] - HeightBoundaries.x) / (HeightBoundaries.y - HeightBoundaries.x));
    }

    public void ToggleColor()
    {
      _applyColorChange = !_applyColorChange;
      _cubes.ForEach(x => x.Renderer.material = _applyColorChange ? Material : LitMaterial);
    }
    
    public void ToggleScale()
    {
      _applyScaleChange = !_applyScaleChange;
      _job.Apply = _applyScaleChange;
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
      _nativeMatrices.Dispose();
      _jobHandle.Complete();
      _accessArray.Dispose();
    }

    public void GenerateTile()
    {
      foreach (var perlinCube in _cubes)
      {
        GameObject cube = perlinCube.gameObject;
        Destroy(cube);
      }
      
      _cubes.Clear();
      
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
          cube.Renderer.material = _applyColorChange ? Material : LitMaterial;
          _cubes.Add(cube);
        }
      }

      var secondTransform = _cubes[0].transform == _cubes[^1].transform ? null : _cubes[^1].transform;
      CameraManager.OnCameraChangeRequestHandler(_cubes[0].transform, secondTransform);

      _cubeTransforms = new Transform[Size.x * Size.y];
      _randomScales = new float[Size.x * Size.y];
      _nativeScaleValues = new NativeArray<float>(_cubes.Count, Allocator.Persistent);
      _nativePerlinValues = new NativeArray<float>(_cubes.Count, Allocator.Persistent);
      _nativePositions = new NativeArray<float3>(_cubeTransforms.Length, Allocator.Persistent);
      _nativeMatrices = new NativeArray<Matrix4x4>(_cubeTransforms.Length, Allocator.Persistent);
      
      for (int i = 0; i < _cubes.Count; i++)
      {
        _nativePositions[i] = _cubes[i].transform.position;
        _cubeTransforms[i] = _cubes[i].transform;
        _randomScales[i] = Random.Range(.2f, 2f);
      }
      
      _job.Positions = _nativePositions;
      

      _accessArray = new TransformAccessArray(_cubeTransforms);
    }
    public void SetWidth()
    {
      int sizeX = Size.x;
      ManagerContainer.Instance.GetInstance<UIManager>().SetWidthSlider(ref sizeX);
      Size.x = sizeX;
    }
    public void SetLength()
    {
      int sizeY = Size.y;
      ManagerContainer.Instance.GetInstance<UIManager>().SetLengthSlider(ref sizeY);
      Size.y = sizeY;
    }

    public void SetNoiseSpeed()
    {
      float speed = NoiseSpeed;
      ManagerContainer.Instance.GetInstance<UIManager>().SetNoiseSpeedSlider(ref speed);
      NoiseSpeed = speed;
    }
  }
  
  [BurstCompile]
  public struct PerlinCubeJob : IJobParallelForTransform
  {
    public NativeArray<Matrix4x4> Matrices;
    public NativeArray<float> ScaleValues;
    public NativeArray<float3> Positions;
    public bool Apply;
    public void Execute(int index, TransformAccess transform)
    {
      
      if (Apply)
      {
        var newScale = transform.localScale;
        newScale.y = ScaleValues[index];
        transform.localScale = newScale;
      }


      var pos = new Vector3(Positions[index].x, 0f, Positions[index].z);
      Matrices[index] = Matrix4x4.TRS(pos, Quaternion.identity, transform.localScale);
    }
  }
}
