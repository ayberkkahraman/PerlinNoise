using Project._Scripts.Runtime.InGame.TileGenerator;
using UnityEditor;
using UnityEngine;

namespace Project._Scripts.Editor
{
  [CustomEditor(typeof(PerlinTileGenerator))]
    public class GenerateTileButton : UnityEditor.Editor
    {
      public override void OnInspectorGUI()
      {
        DrawDefaultInspector();
  
        PerlinTileGenerator generator = (PerlinTileGenerator)target;
  
        if (GUILayout.Button("Generate"))
        {
          generator.GenerateTile();
        }
      }
    }
}
