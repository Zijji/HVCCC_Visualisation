using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using WSMGameStudio.Splines;

[CustomEditor(typeof(Spline))]
public class SplineInspector : Editor
{
    private const int _lineSteps = 10;

    private Spline _spline;
    private Transform _handleTransform;
    private Quaternion _handleRotation;
    private const float _handleSize = 0.04f;
    private const float _pickSize = 0.06f;
    private int _selectedIndex = -1;

    private GUIContent _btnAddNewCurve = new GUIContent("Add Curve", "Adds a new curve at the end of the spline");
    private GUIContent _btnRemoveCurve = new GUIContent("Remove Curve", "Removes last curve added");
    private GUIContent _btnResetRotations = new GUIContent("Reset Rotations", "Resets all control points rotations");
    private GUIContent _btnResetSpline = new GUIContent("Reset Spline", "Restarts spline from scratch");
    private GUIContent _btnFollowTerrain = new GUIContent("Follow Terrain", string.Format("Adjusts control points height values to follow terrain elevations.{0}{0}OBS: Works better with high resolution splines. Use the \"New Curve Length\" min value while creating your spline for better results", System.Environment.NewLine));
    private GUIContent _btnFlatten = new GUIContent("Flatten", "Resets all control points height values, making a flat spline");

    private static Color[] _modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    /// <summary>
    /// Draw spline on editor
    /// </summary>
    private void OnSceneGUI()
    {
        _spline = target as Spline;
        _handleTransform = _spline.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local ? _handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < _spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        ShowDirections();
    }

    /// <summary>
    /// Show spline points forward direction on Editor
    /// </summary>
    private void ShowDirections()
    {
        Handles.color = Color.red;
        Vector3 point = _spline.GetPoint(0f);
        Handles.DrawLine(point, point + _spline.GetDirection(0f) * SplineDefaultValues.DirectionScale);
        int steps = SplineDefaultValues.StepsPerCurve * _spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = _spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + _spline.GetDirection(i / (float)steps) * SplineDefaultValues.DirectionScale);
        }
    }

    /// <summary>
    /// Show spline point handles on Editor
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector3 ShowPoint(int index)
    {
        Vector3 pointPosition = _handleTransform.TransformPoint(_spline.GetControlPointPosition(index));
        Quaternion pointRotation = _spline.GetControlPointRotation(index);

        float size = HandleUtility.GetHandleSize(pointPosition);
        if (index == 0)
            size *= 2f;
        else
            size *= 1.5f;

        //Start of new spline section will be green for easy identification
        if (index == 0 || ((index) % 3 == 0))
            Handles.color = Color.green;
        else
            Handles.color = _modeColors[(int)_spline.GetControlPointMode(index)];

        if (Handles.Button(pointPosition, _handleRotation, (size * _handleSize), (size * _pickSize), Handles.DotHandleCap))
        {
            _selectedIndex = index;
            Repaint();
        }

        if (_selectedIndex == index)
        {
            //Position handle
            EditorGUI.BeginChangeCheck();
            pointPosition = Handles.DoPositionHandle(pointPosition, _handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_spline, "Move Point");
                MarkSceneAlteration();
                _spline.SetControlPointPosition(index, _handleTransform.InverseTransformPoint(pointPosition));
            }

            //Rotation handle
            EditorGUI.BeginChangeCheck();
            pointRotation = Handles.DoRotationHandle(pointRotation, pointPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_spline, "Rotate Point");
                MarkSceneAlteration();
                _spline.SetControlPointRotation(index, pointRotation);
            }
        }

        return pointPosition;
    }

    /// <summary>
    /// Draw inspector elements and UI
    /// </summary>
    public override void OnInspectorGUI()
    {
        _spline = target as Spline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", _spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Toggle Loop");
            MarkSceneAlteration();
            _spline.Loop = loop;
        }

        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("New Curve Length");
        float newCurveLength = EditorGUILayout.Slider(_spline.newCurveLength, 15f, 100f);
        GUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Change Curve Length");
            MarkSceneAlteration();
            _spline.newCurveLength = newCurveLength;
        }

        if (_selectedIndex >= 0 && _selectedIndex < _spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(_btnAddNewCurve))
        {
            Undo.RecordObject(_spline, "Add Curve");
            _spline.AddCurve();
            MarkSceneAlteration();
        }

        if (GUILayout.Button(_btnRemoveCurve))
        {
            Undo.RecordObject(_spline, "Remove Curve");
            _spline.RemoveCurve();
            MarkSceneAlteration();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_btnResetRotations))
        {
            Undo.RecordObject(_spline, "Reset Rotations");
            _spline.ResetRotations();
            MarkSceneAlteration();
        }

        if (GUILayout.Button(_btnResetSpline))
        {
            Undo.RecordObject(_spline, "Reset");
            _spline.Reset();
            MarkSceneAlteration();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_btnFollowTerrain))
        {
            Undo.RecordObject(_spline, "Follow Terrain");
            _spline.FollowTerrain();
            MarkSceneAlteration();
        }

        if (GUILayout.Button(_btnFlatten))
        {
            Undo.RecordObject(_spline, "Flatten");
            _spline.Flatten();
            MarkSceneAlteration();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draw inpector elements form selected spline point
    /// </summary>
    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", _spline.GetControlPointPosition(_selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Move Point");
            MarkSceneAlteration();
            _spline.SetControlPointPosition(_selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 rotation = EditorGUILayout.Vector3Field("Rotation", Convert.QuaternionToVector3(_spline.GetControlPointRotation(_selectedIndex)));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Rotate Point");
            MarkSceneAlteration();
            _spline.SetControlPointRotation(_selectedIndex, Convert.Vector3ToQuaternion(rotation));
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)
            EditorGUILayout.EnumPopup("Mode", _spline.GetControlPointMode(_selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Change Point Mode");
            _spline.SetControlPointMode(_selectedIndex, mode, true);
            MarkSceneAlteration();
        }
    }

    private void MarkSceneAlteration()
    {
        EditorUtility.SetDirty(_spline);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}