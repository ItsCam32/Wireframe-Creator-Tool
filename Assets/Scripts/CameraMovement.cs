using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    // vv Private Exposed vv //

    [Range(0.01f, 1000.0f)]
    [SerializeField]
    private float cameraSpeed;

    [Range(0.01f, 1000.0f)]
    [SerializeField]
    private float cameraPanSpeed;

    [SerializeField]
    private Grid grid;

    // vv Private vv //

    private float cameraXRotation = 0.0f;
    private float cameraYRotation = 0.0f;

    ////////////////////////////////////////

    #region Private Functions

    private void Update()
    {
        UpdateCameraSpeed();
        MoveCamera();
        PanCamera();
    }

    private void UpdateCameraSpeed()
    {
        // Scroll forward
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            cameraSpeed += 10.0f;
        }

        // Scroll backward
        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            cameraSpeed -= 10.0f;
        }

        cameraSpeed = Mathf.Clamp(cameraSpeed, 10f, 500.0f);
        UI_Manager.Instance.UpdateCameraSpeedText($"Camera Speed: {(cameraSpeed / 10.0f).ToString("F0")}");
    }

    private void MoveCamera()
    {
        // Direction from WASD
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += -transform.right;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += -transform.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += transform.right;
        }

        // Will this new position be within grid bounds?
        Vector3 newPos = transform.position + direction * cameraSpeed * Time.deltaTime;
        float bounds = grid ? grid.Bounds : 0.0f;
        if
        (
            newPos.x > bounds || newPos.x < -bounds
            || newPos.y > bounds || newPos.y < -bounds
            || newPos.z > bounds || newPos.z < -bounds
        ) return;

        transform.position += direction * cameraSpeed * Time.deltaTime;
    }

    private void PanCamera()
    { 
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            float mouseX = Input.GetAxis("Mouse X") * cameraPanSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * cameraPanSpeed;
            transform.Rotate(Vector3.up * mouseX);
            cameraXRotation -= mouseY;
            cameraXRotation = Mathf.Clamp(cameraXRotation, -90.0f, 90.0f);
            cameraYRotation += mouseX;
            transform.localRotation = Quaternion.Euler(cameraXRotation, cameraYRotation, 0f);
        }
        
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    #endregion
}
