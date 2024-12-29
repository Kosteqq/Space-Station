using System.Collections.Generic;
using SpaceStation.Utils;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace SpaceStation.PathFinding
{
    [CustomEditor(typeof(PathFindingManager))]
    public class PathfindingManagerEditor : Editor
    {
        private BoxBoundsHandle _gridSizeHandle;
        private ObjectField _bakePosition;
        private ObjectField _pathFinderPosition;
        private List<Vector2> _pathPoints;

        private void OnEnable()
        {
            _gridSizeHandle = new BoxBoundsHandle();
            _gridSizeHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Z;
            _gridSizeHandle.size = Vector3.one;
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var manager = (PathFindingManager)target;
            
            manager.RegenerateGrid();
            
            var root = new VisualElement();
            root.Bind(new SerializedObject(manager));

            var gridCellSize = new FloatField();
            gridCellSize.label = "Grid Cell Size";
            gridCellSize.bindingPath = nameof(PathFindingManager._gridCellSize);
            gridCellSize.RegisterValueChangedCallback(_ => manager.RegenerateGrid());

            var gridBoundsCenter = new Vector2Field();
            gridBoundsCenter.label = "Grid Bounds Center";
            gridBoundsCenter.bindingPath = nameof(PathFindingManager._gridBoundsCenter);
            gridBoundsCenter.RegisterValueChangedCallback(_ => manager.RegenerateGrid());
            
            var gridBoundsSize = new Vector2Field();
            gridBoundsSize.label = "Grid Bounds Size";
            gridBoundsSize.bindingPath = nameof(PathFindingManager._gridBoundsSize);
            gridBoundsSize.RegisterValueChangedCallback(_ => manager.RegenerateGrid());

            var editorFoldout = new Foldout();
            editorFoldout.text = "Editor";

            _bakePosition = new ObjectField();
            _bakePosition.objectType = typeof(Transform);
            editorFoldout.Add(_bakePosition);

            var bakeButton = new Button();
            bakeButton.text = "Bake To Position!";
            bakeButton.clickable.clicked += () =>
            {
                var transform = (Transform)_bakePosition.value;
                manager.BakeToPosition(transform.position.XZ());
            };
            _bakePosition.Add(bakeButton);

            _pathFinderPosition = new ObjectField();
            _pathFinderPosition.objectType = typeof(Transform);
            editorFoldout.Add(_pathFinderPosition);

            var findButton = new Button();
            findButton.text = "Find Path!";
            findButton.clickable.clicked += () =>
            {
                var targetTransform = (Transform)_bakePosition.value;
                var srcTransform = (Transform)_pathFinderPosition.value;
                _pathPoints = manager.GetPathToPoint(srcTransform.position.XZ(), targetTransform.position.XZ());
            };
            _pathFinderPosition.Add(findButton);
 
            root.Add(gridCellSize);
            root.Add(gridBoundsCenter);
            root.Add(gridBoundsSize);
            root.Add(editorFoldout);

            return root;
        }
        
        #region Scene GUI

        private void OnSceneGUI()
        {
            var manager = (PathFindingManager)target;
            var matrix = Matrix4x4.TRS( Vector3.zero, Quaternion.identity, Vector3.one );

            if (manager.Grid == null)
            {
                manager.RegenerateGrid();
            }
            
            Handles.zTest = CompareFunction.Always;

            DrawBounds(matrix, manager);
            DrawGridPreview(manager);
            DrawEditorPositionHandler(_bakePosition);
            DrawEditorPositionHandler(_pathFinderPosition);
            DrawEditorPath();
        }

        private void DrawBounds(Matrix4x4 p_matrix, PathFindingManager p_manager)
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

        private void DrawGridPreview(PathFindingManager p_manager)
        {
            foreach (var cell in p_manager.Grid)
            {
                Handles.color = Color.white;
                Handles.Label(new Vector3(cell.WorldPosition.x, 0.5f, cell.WorldPosition.y), cell.Weight.ToString());
                
                var position = new Vector3(cell.WorldPosition.x, 0f, cell.WorldPosition.y);
                var size = new Vector3(p_manager._gridCellSize, 0f, p_manager._gridCellSize);

                Handles.color = Color.red;
                var directionNeighbour = p_manager.TryGetLowestWeightNeighbour(cell.GridPosition);
                
                if (directionNeighbour.HasValue)
                {
                    ref var neighbourCell = ref p_manager.Grid[directionNeighbour.Value.x, directionNeighbour.Value.y];
                    Handles.DrawLine(position, new Vector3(neighbourCell.WorldPosition.x, 0f, neighbourCell.WorldPosition.y));
                }

                Handles.color = Color.cyan;
                Handles.DrawWireCube(position, size);
            }
        }

        private void DrawEditorPositionHandler(ObjectField p_transformField)
        {
            if (p_transformField == null || p_transformField.value == null)
            {
                return;
            }

            var transform = (Transform)p_transformField.value;

            EditorGUI.BeginChangeCheck();

            var pos = transform.position;
            var rot = transform.rotation;

            Handles.Label(pos, transform.gameObject.name);
            Handles.TransformHandle(ref pos, ref rot);

            if (EditorGUI.EndChangeCheck())
            {
                transform.position = pos;
                transform.rotation = rot;
                EditorUtility.SetDirty(transform);
            }
        }

        private void DrawEditorPath()
        {
            if (_pathPoints == null)
            {
                return;
            }
            
            Handles.color = Color.yellow;

            for (var i = 0; i < _pathPoints.Count - 1; i++)
            {
                Handles.DrawLine(
                    new Vector3(_pathPoints[i].x, 0f, _pathPoints[i].y),
                    new Vector3(_pathPoints[i + 1].x, 0f, _pathPoints[i + 1].y));
            }
        }

        #endregion Scene GUI
    }
}