using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;


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

    [MinMaxSlider(0f, 2f)]
    public Vector2 HeightBoundaries;

    public float InternalDelay = .01f;

    private async void Start()
    {
      _cubes = new List<PerlinCube.PerlinCube>();
      
      GenerateTile();
      
      while (true)
      {
        await ExecuteEveryInterval(InternalDelay);
      }
    }

    private async Task ExecuteEveryInterval(float interval)
    {
      Execute();
      await Task.Delay(Mathf.RoundToInt(interval * 1000));
    }
    
    private void Execute()
    {
      foreach (var t in _cubes)
      {
        t.CubeUpdate(NoiseSpeed, HeightBoundaries, Time.time);
      }
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

          PerlinCube.PerlinCube cube = Instantiate(PerlinCube, position, Quaternion.identity, CubesHolder);
      
          _cubes.Add(cube);
        }
      }
    }
  }
}
