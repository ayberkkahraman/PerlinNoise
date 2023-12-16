using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;


namespace Project._Scripts.Runtime.InGame.TileGenerator
{
  public class PerlinTileGenerator : MonoBehaviour
  {
    public PerlinCube.PerlinCube PerlinCube;
    public Transform CubesHolder;
    public Vector2Int Size;
    public Vector2 Offset;

    private List<PerlinCube.PerlinCube> _cubes;
    
    [Range(.01f, 3f)][SerializeField] private float NoiseSpeed = 1f;

    [MinMaxSlider(0f, 1f)]
    public Vector2 HeightBoundaries;

    private void Start()
    {
      _cubes = new List<PerlinCube.PerlinCube>();
      
      GenerateTile();
    }
    private void Update()
    {
      if(Input.GetKeyDown(KeyCode.K)) GenerateTile();

      if (_cubes.Any())
      {
        _cubes.ForEach(x =>
        {
          if(x != null)
            x.CubeUpdate(NoiseSpeed, HeightBoundaries);
        });
      }
    }

    #if UNITY_EDITOR
    // private void OnValidate()
    // {
    //   GenerateTile();
    // }
    #endif

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

          PerlinCube.PerlinCube cube = Instantiate(PerlinCube, position, Quaternion.identity, CubesHolder);
          cube.TraverseScale(NoiseSpeed, HeightBoundaries);
          _cubes.Add(cube);
        }
      }
    }
  }
}
