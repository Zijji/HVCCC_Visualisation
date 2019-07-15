using UnityEngine;
using WSMGameStudio.RailroadSystem;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;

[CustomEditor(typeof(TrainController_v3))]
public class TrainControllerInspector : Editor
{
    private TrainController_v3 _trainController_v3;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _trainController_v3 = target as TrainController_v3;
        if (GUILayout.Button("Connect Wagons"))
        {
#if UNITY_EDITOR
            Undo.RecordObject(_trainController_v3, "Wagons Connected");
#endif
            SetWagonsPositions();
#if UNITY_EDITOR
            MarkSceneAlteration();
#endif
        }
    }

    private void SetWagonsPositions()
    {
        if (_trainController_v3.wagons == null)
        {
            Debug.LogWarning("Wagons list cannot be null");
            return;
        }

        _trainController_v3.transform.position = new Vector3(0, _trainController_v3.transform.position.y, 0);
        _trainController_v3.transform.rotation = Quaternion.identity;

        float lastWagonJointDistance = 0f;
        float totalDistance = 0f;

        for (int index = 0; index < _trainController_v3.wagons.Count; index++)
        {
            _trainController_v3.wagons[index].transform.rotation = Quaternion.identity;

            //if (index == 0)
            //    _trainController_v3.wagons[index].transform.position = new Vector3(0, _trainController_v3.wagons[index].transform.position.y, -8);
            //else
            //    _trainController_v3.wagons[index].transform.position = new Vector3(0, _trainController_v3.wagons[index].transform.position.y, ((-15 * index) - 8));

            //Teste
            if (index == 0)
            {
                lastWagonJointDistance = Mathf.Abs(_trainController_v3.backJoint.transform.localPosition.z);
                totalDistance -= ((_trainController_v3.wagons[index].JoinDistance / 2f) + lastWagonJointDistance);
            }
            else
                totalDistance -= ((_trainController_v3.wagons[index].JoinDistance / 2f) + (lastWagonJointDistance / 2f));

            _trainController_v3.wagons[index].transform.position = new Vector3(0, _trainController_v3.wagons[index].transform.position.y, totalDistance);

            lastWagonJointDistance = _trainController_v3.wagons[index].JoinDistance;

            //Teste

            EditorUtility.SetDirty(_trainController_v3.wagons[index]);
        }
    }

    private void MarkSceneAlteration()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(_trainController_v3);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
    }
}
