using System;
using UnityEngine;

namespace Project._Scripts.Runtime.InGame.TileGenerator
{
  public class MeshCombiner : MonoBehaviour
  {
    private void Start()
    {
      Invoke(nameof(CombineMeshes), 1f);
    }
    public void CombineMeshes()
    {
      MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
      CombineInstance[] combine = new CombineInstance[meshFilters.Length];

      for (int i = 0; i < meshFilters.Length; i++)
      {
        combine[i].mesh = meshFilters[i].sharedMesh;
        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        meshFilters[i].gameObject.SetActive(false);
      }

      var meshFilter = GetComponent<MeshFilter>();
      
      meshFilter.mesh = new Mesh();
      meshFilter.mesh.CombineMeshes(combine);
      GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
      transform.gameObject.SetActive(true);
    }
  }
}
