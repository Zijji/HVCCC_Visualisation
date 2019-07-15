using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WSMGameStudio.Splines;

[CustomEditor(typeof(SplineMeshRenderer))]
public class SplineMeshRendererInspector : Editor
{
    private SplineMeshRenderer _splineMeshRenderer;

    private GUIContent _btnPrintMeshDetails = new GUIContent("Print Mesh Details", "Prints the generated mesh details on console window when Realtime Mesh Generation property is enabled");
    private GUIContent _btnRenderMesh = new GUIContent("Render Mesh", "Manually generates the mesh when Realtime Mesh Generation property is disabled");
    private GUIContent _btnBakeMesh = new GUIContent("Bake Mesh", "Exports the generated mesh as a prefab (Mesh Baker Window)");
    private GUIContent _btnConnectNewRenderer = new GUIContent("Connect New Renderer", string.Format("Connects a new Mesh Renderer at the end of the current one. Usefull to improve performance with occlusion culling.", System.Environment.NewLine));

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _splineMeshRenderer = (SplineMeshRenderer)target;

        GUILayout.BeginHorizontal();

        if (_splineMeshRenderer.realtimeMeshGeneration)
        {
            if (GUILayout.Button(_btnPrintMeshDetails))
            {
                _splineMeshRenderer.PrintMeshDetails();
            }
        }
        else
        {
            if (GUILayout.Button(_btnRenderMesh))
            {
                _splineMeshRenderer.ExtrudeMesh();
            }
        }

        if (GUILayout.Button(_btnBakeMesh))
        {
            BakeMeshWindow.ShowWindow();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_btnConnectNewRenderer))
        {
            Selection.activeGameObject = _splineMeshRenderer.ConnectNewRenderer();
            MarkSceneAlteration();
        }
        GUILayout.EndHorizontal();
    }

    private void MarkSceneAlteration()
    {
        EditorUtility.SetDirty(_splineMeshRenderer);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
