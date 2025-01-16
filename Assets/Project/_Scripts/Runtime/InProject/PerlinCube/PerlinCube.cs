using UnityEngine;

namespace Project._Scripts.Runtime.InProject.PerlinCube
{
  public class PerlinCube : MonoBehaviour
  {
    public Renderer Renderer { get; set; }
    public MaterialPropertyBlock MaterialPropertyBlock { get; set; }
    public Color CurrentColor { get; set; }

    private void Awake()
    {
      Initialize();
    }
    
    private void Initialize()
    {
      MaterialPropertyBlock = new MaterialPropertyBlock();
      Renderer = GetComponent<Renderer>();
    }
  }
}
