using System.IO;
using SpaceStation.Utils;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace SpaceStation.Pathfinding
{
    [CustomEditor(typeof(PathfindingManager))]
    public class PathfindingManagerEditor : Editor
    {
        private BoxBoundsHandle _gridSizeHandle;
        private ObjectField _bakePosition;

        private void OnEnable()
        {
            _gridSizeHandle = new BoxBoundsHandle();
            _gridSizeHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Z;
            _gridSizeHandle.size = Vector3.one;
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var manager = (PathfindingManager)target;
            
            manager.RegenerateGrid();
            
            var root = new VisualElement();
            root.Bind(new SerializedObject(manager));

            var gridCellSize = new FloatField();
            gridCellSize.label = "Grid Cell Size";
            gridCellSize.bindingPath = nameof(PathfindingManager._gridCellSize);
            gridCellSize.RegisterValueChangedCallback(_ => manager.RegenerateGrid());

            var gridBoundsCenter = new Vector2Field();
            gridBoundsCenter.label = "Grid Bounds Center";
            gridBoundsCenter.bindingPath = nameof(PathfindingManager._gridBoundsCenter);
            gridBoundsCenter.RegisterValueChangedCallback(_ => manager.RegenerateGrid());
            
            var gridBoundsSize = new Vector2Field();
            gridBoundsSize.label = "Grid Bounds Size";
            gridBoundsSize.bindingPath = nameof(PathfindingManager._gridBoundsSize);
            gridBoundsSize.RegisterValueChangedCallback(_ => manager.RegenerateGrid());

            var bakeEditorFoldout = new Foldout();
            bakeEditorFoldout.text = "Bake Editor";

            _bakePosition = new ObjectField();
            _bakePosition.objectType = typeof(Transform);
            bakeEditorFoldout.Add(_bakePosition);

            var bakeButton = new Button();
            bakeButton.text = "Bake to position!";
            bakeButton.clickable.clicked += () =>
            {
                var transform = (Transform)_bakePosition.value;
                manager.BakeToPosition(transform.position);
            };
            _bakePosition.Add(bakeButton);
 
            root.Add(gridCellSize);
            root.Add(gridBoundsCenter);
            root.Add(gridBoundsSize);
            root.Add(bakeEditorFoldout);

            return root;
        }
        
        #region Scene GUI

        private void OnSceneGUI()
        {
            var manager = (PathfindingManager)target;
            var matrix = Matrix4x4.TRS( Vector3.zero, Quaternion.identity, Vector3.one );

            if (manager.Grid == null)
            {
                manager.RegenerateGrid();
            }
            
            Handles.zTest = CompareFunction.Always;

            DrawBounds(matrix, manager);
            DrawGridPreview(manager);
            DrawBakePositionHandler();
        }

        private void DrawBounds(Matrix4x4 p_matrix, PathfindingManager p_manager)
        {
            using (new Handles.DrawingScope(Color.blue, p_matrix))
            {
                _gridSizeHandle.center = p_matrix.inverse.MultiplyPoint3x4(new Vector3(p_manager._gridBoundsCenter.x, 0f, p_manager._gridBoundsCenter.y));
                _gridSizeHandle.size = new Vector3(p_manager._gridBoundsSize.x, 0f, p_manager._gridBoundsSize.y);

                EditorGUI.BeginChangeCheck();

                _gridSizeHandle.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    p_manager._gridBoundsCenter = p_matrix.MultiplyPoint3x4(_gridSizeHandle.center.XZ());
                    p_manager._gridBoundsSize = _gridSizeHandle.size.XZ();
                    EditorUtility.SetDirty(p_manager);
                    
                    p_manager.RegenerateGrid();
                }
            }
        }

        private void DrawGridPreview(PathfindingManager p_manager)
        {
            foreach (var cell in p_manager.Grid)
            {
                Handles.color = Color.white;
                Handles.Label(new Vector3(cell.WorldPosition.x, 0.5f, cell.WorldPosition.y), cell.Weight.ToString());
                
                var position = new Vector3(cell.WorldPosition.x, 0f, cell.WorldPosition.y);
                var size = new Vector3(p_manager._gridCellSize, 0f, p_manager._gridCellSize);

                Handles.color = Color.red;
                var directionNeighbour = p_manager.GetLowestWeightNeighbour(cell.GridPosition);
                
                if (directionNeighbour.HasValue)
                {
                    Handles.DrawLine(position, new Vector3(directionNeighbour.Value.WorldPosition.x, 0f, directionNeighbour.Value.WorldPosition.y));
                }

                Handles.color = Color.cyan;
                Handles.DrawWireCube(position, size);
            }
        }

        private void DrawBakePositionHandler()
        {
            if (_bakePosition == null || _bakePosition.value == null)
            {
                return;
            }

            var transform = (Transform)_bakePosition.value;

            EditorGUI.BeginChangeCheck();

            var pos = transform.position;
            var rot = transform.rotation;
            
            Handles.TransformHandle(ref pos, ref rot);

            if (EditorGUI.EndChangeCheck())
            {
                transform.position = pos;
                transform.rotation = rot;
                EditorUtility.SetDirty(transform);
            }
        }

        #endregion Scene GUI
    }
}