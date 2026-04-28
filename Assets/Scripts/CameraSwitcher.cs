using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("Settings")]
    public KeyCode switchKey = KeyCode.V;

    private bool isFirstPerson = false;

    void Start()
    {
        SetCameraMode(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            isFirstPerson = !isFirstPerson;
            SetCameraMode(isFirstPerson);
        }
    }

    void SetCameraMode(bool firstPerson)
    {
        if (firstPersonCamera != null)
            firstPersonCamera.gameObject.SetActive(firstPerson);

        if (thirdPersonCamera != null)
            thirdPersonCamera.gameObject.SetActive(!firstPerson);
    }
}