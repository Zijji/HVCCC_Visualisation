using UnityEngine;
using UnityEditor;
using WSMGameStudio.Splines;
using System.IO;

public class BakeMeshWindow : EditorWindow
{
    private string meshName;
    private string outputDirectory = string.Empty;
    private string defaultDiretory = "WSM Game Studio/Train Controller_v3/Railroad Builder/Baked/";
    private string txtMessage;
    private Color txtColor = Color.black;

    private GUIStyle errorMessageStyle;

    [MenuItem("Window/WSM Game Studio/Mesh Baker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<BakeMeshWindow>("Mesh Baker");
    }

    /// <summary>
    /// Render Window
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Mesh Baker", EditorStyles.boldLabel);

        if (outputDirectory == string.Empty)
            outputDirectory = defaultDiretory;

        meshName = EditorGUILayout.TextField("Mesh Name", meshName);
        outputDirectory = EditorGUILayout.TextField("Output Directory", outputDirectory);

        if (GUILayout.Button("Bake"))
        {
            Bake();
        }

        errorMessageStyle = new GUIStyle();
        errorMessageStyle.normal.textColor = txtColor;
        GUILayout.Label(txtMessage, errorMessageStyle);
    }

    /// <summary>
    /// Bake mesh into folder
    /// </summary>
    private void Bake()
    {
        txtMessage = string.Empty;

        if (meshName == string.Empty)
        {
            ShowMessage("Please, enter a name for the mesh before proceding", WSMGameStudio.Splines.MessageType.Error);
            return;
        }

        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject == null)
        {
            ShowMessage("No object selected", WSMGameStudio.Splines.MessageType.Error);
            return;
        }

        SplineMeshRenderer splineMeshRenderer = selectedGameObject.GetComponent<SplineMeshRenderer>();
        if (splineMeshRenderer == null)
        {
            ShowMessage("Selected object is not a Spline Mesh Renderer", WSMGameStudio.Splines.MessageType.Error);
            return;
        }

        if (splineMeshRenderer.GeneratedMesh == null)
        {
            ShowMessage("Generated mesh not found. Please generate a mesh before proceding.", WSMGameStudio.Splines.MessageType.Error);
            return;
        }

        MeshRenderer baseMeshRenderer = splineMeshRenderer.GetComponent<MeshRenderer>();

        if (baseMeshRenderer == null)
        {
            ShowMessage("Selected object does not contains a Mesh Renderer.", WSMGameStudio.Splines.MessageType.Error);
            return;
        }

        //Create unique mesh instead of referencing original mesh
        Mesh mesh = (Mesh)Instantiate(splineMeshRenderer.GeneratedMesh);

        DirectoryInfo info = new DirectoryInfo(Application.dataPath);
        string folderPath = Path.Combine(info.Name, outputDirectory);
        string meshFilePath = Path.Combine(folderPath, meshName + "_mesh.asset");
        string prefabFilePath = Path.Combine(folderPath, meshName + "_prefab.prefab");

        //Check folder existence
        if (!Directory.Exists(folderPath))
        {
            ShowMessage(string.Format("Folder does not exist: {0}", folderPath), WSMGameStudio.Splines.MessageType.Error);
            return;
        }

        //Overwrite dialog
        if (Exists(meshFilePath) || Exists(prefabFilePath))
        {
            if (!ShowOverwriteDialog(meshName))
                return;
        }

        //Save mesh as .asset file
        mesh = SaveMeshFile(mesh, meshFilePath);

        GameObject sceneObject = new GameObject(meshName);

        MeshFilter meshFilter = sceneObject.AddComponent<MeshFilter>();
        MeshRenderer meshRend = sceneObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = sceneObject.AddComponent<MeshCollider>();
        sceneObject.transform.position = splineMeshRenderer.transform.position;
        sceneObject.transform.rotation = splineMeshRenderer.transform.rotation;
        sceneObject.tag = splineMeshRenderer.gameObject.tag;
        sceneObject.layer = splineMeshRenderer.gameObject.layer;

        meshCollider.enabled = splineMeshRenderer.enableCollision;
        meshCollider.sharedMesh = mesh;
        meshCollider.sharedMaterial = selectedGameObject.GetComponent<MeshCollider>().sharedMaterial;

        meshFilter.sharedMesh = mesh;
        meshRend.sharedMaterials = baseMeshRenderer.sharedMaterials;

        if (splineMeshRenderer.customMeshColliders != null)
        {
            foreach (var customCollider in splineMeshRenderer.customMeshColliders)
            {
                if (customCollider.GeneratedMesh == null)
                {
                    Debug.LogWarning(string.Format("Generated mesh not found for {0}. Please generate a mesh before proceding.", customCollider.gameObject.name));
                    continue;
                }

                Mesh customMeshCollider = (Mesh)Instantiate(customCollider.GeneratedMesh);
                string customMeshFilePath = Path.Combine(folderPath, meshName + "_" + customCollider.gameObject.name + "_mesh.asset");
                customMeshCollider = SaveMeshFile(customMeshCollider, customMeshFilePath);

                GameObject sceneObjectChild = new GameObject(customCollider.gameObject.name);

                MeshCollider customMeshColliderCollider = sceneObjectChild.AddComponent<MeshCollider>();
                sceneObjectChild.transform.position = customCollider.transform.position;
                sceneObjectChild.transform.rotation = customCollider.transform.rotation;
                sceneObjectChild.tag = customCollider.gameObject.tag;
                sceneObjectChild.layer = customCollider.gameObject.layer;

                customMeshColliderCollider.enabled = true;
                customMeshColliderCollider.sharedMesh = customMeshCollider;
                customMeshColliderCollider.sharedMaterial = customCollider.GetComponent<MeshCollider>().sharedMaterial;

                sceneObjectChild.transform.parent = sceneObject.transform;
            }
        }

        //Save prefab as .prefab file
        GameObject prefab = SavePrefabFile(sceneObject, prefabFilePath, true);

        ShowMessage(string.Format("Mesh {0} Created Succesfuly!", meshName), WSMGameStudio.Splines.MessageType.Success);
    }

    #region Auxiliar Methods
    /// <summary>
    /// Save prefab as .prefab file
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="prefabFilePath"></param>
    /// <returns></returns>
    private GameObject SavePrefabFile(GameObject sceneObject, string prefabFilePath, bool destroySceneObject)
    {
        GameObject prefab = PrefabUtility.CreatePrefab(prefabFilePath, sceneObject);

        if (destroySceneObject)
            GameObject.DestroyImmediate(sceneObject);

        return prefab;
    }

    /// <summary>
    /// Save mesh as .asset file
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="meshFilePath"></param>
    /// <returns>Saved mesh reference</returns>
    private static Mesh SaveMeshFile(Mesh mesh, string meshFilePath)
    {
        MeshUtility.Optimize(mesh);
        AssetDatabase.CreateAsset(mesh, meshFilePath);
        AssetDatabase.SaveAssets();
        return mesh;
    }

    /// <summary>
    /// Show message on Window
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    private void ShowMessage(string message, WSMGameStudio.Splines.MessageType type)
    {
        txtMessage = message;

        switch (type)
        {
            case WSMGameStudio.Splines.MessageType.Success:
                txtColor = new Color(0, 0.5f, 0); //Dark green;
                break;
            case WSMGameStudio.Splines.MessageType.Warning:
                txtColor = Color.yellow;
                break;
            case WSMGameStudio.Splines.MessageType.Error:
                txtColor = Color.red;
                break;
        }
    }

    /// <summary>
    /// Show overwrite dialog confirmation
    /// </summary>
    /// <param name="meshName"></param>
    /// <returns></returns>
    private bool ShowOverwriteDialog(string meshName)
    {
        return EditorUtility.DisplayDialog("Are you sure?",
                        string.Format("A mesh named {0} already exists. Do you want to overwrite it?", meshName.ToUpper()),
                        "Yes",
                        "No");
    }

    /// <summary>
    /// Check if file already exists
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private bool Exists(string filePath)
    {
        return File.Exists(filePath);
    } 
    #endregion
}
