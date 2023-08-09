using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Forgotten_Files.DungeonPainter.Runtime;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;

namespace Forgotten_Files.DungeonPainter.Editor
{
    [CustomEditor(typeof(DungeonGenerator))]
    public class DungeonGeneratorEditor : UnityEditor.Editor
    {

        private BoxBoundsHandle _boundsHandles = new BoxBoundsHandle();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DungeonGenerator dungeonGenerator = (DungeonGenerator)target;

            if (dungeonGenerator.DungeonData == null) return;

            if (GUILayout.Button("Regenerate"))
            {
                ClearAll(dungeonGenerator.transform);
                dungeonGenerator.GenerateAll();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Add Room"))
            {
                dungeonGenerator.AddRoom();

                EditorUtility.SetDirty(dungeonGenerator.DungeonData);
            
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Remove Room"))
            {
                dungeonGenerator.RemoveRoom();

                // so it can save
                EditorUtility.SetDirty(dungeonGenerator.DungeonData);

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        private void OnSceneGUI()
        {
            DungeonGenerator dungeonGenerator = (DungeonGenerator)target;
            if (dungeonGenerator.DungeonData == null) return;

            for (int i = 0; i < dungeonGenerator.Rooms.Count; ++i)
            {
                Bounds room = dungeonGenerator.Rooms[i];

                // allows ctrl z
                EditorGUI.BeginChangeCheck();

                _boundsHandles.center = room.center;
                _boundsHandles.size = room.size;
                _boundsHandles.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(dungeonGenerator.DungeonData, "Resize Room");
                    dungeonGenerator.Rooms[i] = new Bounds(_boundsHandles.center, _boundsHandles.size);
                }
            }

            EditorUtility.SetDirty(dungeonGenerator.DungeonData);
        }

        public void ClearAll(Transform transform)
        {
            List<Transform> toDestroy = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform container = transform.GetChild(i);
                for (int j = 0; j < container.childCount; j++)
                {
                    toDestroy.Add(container.GetChild(j));
                }
            }

            foreach (Transform t in toDestroy)
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }
}
