using System.IO;
using SpaceStation.Utils;
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
        private GridCell[,] _grid;

        private void OnEnable()
        {
            _gridSizeHandle = new BoxBoundsHandle();
            _gridSizeHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Z;
            _gridSizeHandle.size = Vector3.one;
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var manager = (PathfindingManager)target;
            
            var root = new VisualElement();
            root.Bind(new SerializedObject(manager));

            var gridCellSize = new FloatField();
            gridCellSize.label = "Grid Cell Size";
            gridCellSize.bindingPath = nameof(PathfindingManager._gridCellSize);

            var gridBoundsCenter = new Vector2Field();
            gridBoundsCenter.label = "Grid Bounds Center";
            gridBoundsCenter.bindingPath = nameof(PathfindingManager._gridBoundsCenter);
            
            var gridBoundsSize = new Vector2Field();
            gridBoundsSize.label = "Grid Bounds Size";
            gridBoundsSize.bindingPath = nameof(PathfindingManager._gridBoundsSize);

            var bakeEditorFoldout = new Foldout();
            bakeEditorFoldout.text = "Bake Editor";

            var bakePosition = new ObjectField();
            bakePosition.label = "Position";
            bakePosition.objectType = typeof(Transform);
            bakeEditorFoldout.Add(bakePosition);

            var bakeButton = new Button();
            bakeButton.text = "Bake!";
            bakeButton.clickable.clicked += () =>
            {
                var transform = (Transform)bakePosition.value;
                manager.BakeToPosition(transform.position);
            };
            bakePosition.Add(bakeButton);
 
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

            _grid ??= manager.CreateGrid();
            
            Handles.zTest = CompareFunction.Always;

            DrawBounds(matrix, manager);
            DrawGridPreview(manager);
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
                    
                    _grid = p_manager.CreateGrid();
                }
            }
        }

        private void DrawGridPreview(PathfindingManager p_manager)
        {
            foreach (var cell in _grid)
            {
                var position = new Vector3(cell.WorldPosition.x, 0f, cell.WorldPosition.y);
                var size = new Vector3(p_manager._gridCellSize, 0f, p_manager._gridCellSize);

                Handles.color = Color.red;
                Handles.DrawLine(position, position + new Vector3(cell.Direction.x, 0f, cell.Direction.y) / 2f);

                Handles.color = Color.cyan;
                Handles.DrawWireCube(position, size);
            }
        }

        #endregion Scene GUI
    }
}