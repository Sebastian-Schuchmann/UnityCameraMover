using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraMover : MonoBehaviour
{
    [Tooltip("This allows you to move the camera via key presses: 1, 2, 3 etc...")]
    public bool debugMode = false;
    [Tooltip("This determines where the different Targets are located in your hierarchy.")]
    public GameObject camPositionsParent;
    public AnimationCurve animCurve;
    public float timeToMoveInSeconds = 1f;
    public List<GameObject> targetPositions;

    private GameObject _targetObject;
    private float _currentAnimationTime = 0f;
    private bool _needsUpdate = true;
    private GameObject _fromGo;
    private AnimationCurve _nullCurve;

    private void Start()
    {
        //Creates a Cube that we use as a starting point for interpolation
        _fromGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _fromGo.GetComponent<MeshRenderer>().enabled = false;
        _fromGo.GetComponent<Collider>().enabled = false;

        _nullCurve = new AnimationCurve();
    }

    private void Update()
    {
        if (debugMode)
        {
            for (var i=0; i < targetPositions.Count; i++)
            {
                if (Input.GetKeyDown((i+1).ToString()))
                {
                    MoveToTarget(targetPositions[i]);
                }
            }
        }
        
        if (!_needsUpdate || _targetObject == null) return;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (animCurve.Equals(_nullCurve))
        {
            Debug.LogWarning("Animation Curve was set automatically!");
            animCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
        
        _currentAnimationTime += Time.deltaTime / timeToMoveInSeconds;
        transform.position = Vector3.Lerp(_fromGo.transform.position, _targetObject.transform.position,
            animCurve.Evaluate(_currentAnimationTime));
        transform.rotation = Quaternion.Slerp(_fromGo.transform.rotation, _targetObject.transform.rotation,
            animCurve.Evaluate(_currentAnimationTime));

        if (_currentAnimationTime > 1f)
        {
            _needsUpdate = false;
        }
    }
    
    public void MoveToTarget(GameObject go)
    {
        var cameraTransform = transform;
        _fromGo.transform.position = cameraTransform.position;
        _fromGo.transform.rotation = cameraTransform.rotation;

        _targetObject = go;
        _currentAnimationTime = 0f;
        _needsUpdate = true;
    }

    [ExecuteInEditMode]
    public void InstantSetToTarget(GameObject go)
    {
        var goTransform = go.transform;
        var cameraTransform = transform;
        
        cameraTransform.position = goTransform.position;
        cameraTransform.rotation = goTransform.rotation;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraMover))]
public class CameraMoverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CameraMover cameraMover = (CameraMover)target;
        
        GUILayout.Space(10);

        if (cameraMover.targetPositions.Count > 0)
        {
            GUILayout.Label("Move to Target Position");

            foreach (var targetPos in cameraMover.targetPositions)
            {
                if (GUILayout.Button(targetPos.name))
                {
                    cameraMover.InstantSetToTarget(targetPos);
                }
            }
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Create New Camera Target Positions");

        if (GUILayout.Button("Add Target Position"))
        {
            if (cameraMover.camPositionsParent == null)
            {
                Debug.LogWarning("You have to set Cam Positions Parent in order to Add a Target Position!");
                return;
            }
            
            var cameraPositionTarget = new GameObject("Camera Position - " + (cameraMover.targetPositions.Count).ToString());
            var camTransform = cameraMover.transform;
            cameraPositionTarget.transform.SetPositionAndRotation(camTransform.position, camTransform.rotation);
            cameraPositionTarget.transform.SetParent(cameraMover.camPositionsParent.transform);
       
            cameraMover.targetPositions.Add(cameraPositionTarget);
        }

        if (GUILayout.Button("Delete Last Target Position"))
        {
            if (cameraMover.targetPositions.Count == 0)
            {
                Debug.LogWarning("There are no Positions to delete!");
                return;
            }
            
            var gameObjectToDelete = cameraMover.targetPositions[cameraMover.targetPositions.Count - 1];
            cameraMover.targetPositions.Remove(gameObjectToDelete);
            DestroyImmediate(gameObjectToDelete);
        }
    }
}
#endif

