using System;
using System.Collections.Generic;
using System.Linq;
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

    public AnimationCurve InitializeEase;

    private List<PerlinCube.PerlinCube> _cubes;
    
    [Range(.01f, 3f)][SerializeField] private float NoiseSpeed = 1f;
    
    [MinMaxSlider(.2f, 1f)]
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
        _cubes.ForEach(x => x.CubeUpdate(NoiseSpeed, HeightBoundaries));
      }
    }
    
    

    public void GenerateTile()
    {
      _cubes.ForEach(x => Destroy(x.gameObject));
      _cubes = new List<PerlinCube.PerlinCube>();

      for (int x = 0; x < Size.x; x++)
      {
        for (int y = 0; y < Size.y; y++)
        {
          Vector2 position = new Vector2
          (
            x + (PerlinCube.transform.localScale.x / 2) + Offset.x*x - (Size.x / 2), 
            y + (PerlinCube.transform.localScale.z / 2) + Offset.y*y - (Size.y / 2)
          );
          
          Vector3 initialPosition = new Vector3(position.x, 0f, position.y);
          PerlinCube.PerlinCube cube = Instantiate(PerlinCube, initialPosition, Quaternion.identity, CubesHolder);
          _cubes.Add(cube);
        }
      }
    }
  }
}
