using SpaceStation.Utils;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Rendering;

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
                _gridSizeHandle.center = p_matrix.inverse.MultiplyPoint3x4(new Vector3(p_manager._gridBoundsPosition.x, 0f, p_manager._gridBoundsPosition.y));
                _gridSizeHandle.size = new Vector3(p_manager._gridBoundsSize.x, 0f, p_manager._gridBoundsSize.y);

                EditorGUI.BeginChangeCheck();

                _gridSizeHandle.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    p_manager._gridBoundsPosition = p_matrix.MultiplyPoint3x4(_gridSizeHandle.center.XZ());
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
    }
}