using UnityEngine;
using TMPro;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private TextMeshProUGUI cameraSpeedText;

    [SerializeField] private Grid grid;

    public bool looking
    {
        get; private set;
    }

    private float xRot = 0.0f;
    private float yRot = 0.0f;

    private void Update()
    {
        UpdateCameraSpeed();
        MoveCamera();
        MouseLook();
    }

    private void UpdateCameraSpeed()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            cameraSpeed += 10.0f;
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            cameraSpeed -= 10.0f;
        }

        cameraSpeed = Mathf.Clamp(cameraSpeed, 10f, 500.0f);
        cameraSpeedText.text = $"Camera Speed: {(cameraSpeed / 10.0f).ToString("F0")}";
    }

    private void MoveCamera()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += cameraTransform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += -cameraTransform.right;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += -cameraTransform.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += cameraTransform.right;
        }

        // Will this new position be within grid bounds?
        Vector3 newPos = cameraTransform.position + direction * cameraSpeed * Time.deltaTime;
        if
        (
            newPos.x > grid.bounds || newPos.x < -grid.bounds
            || newPos.y > grid.bounds || newPos.y < -grid.bounds
            || newPos.z > grid.bounds || newPos.z < -grid.bounds
        )
        return;

        cameraTransform.position += direction * cameraSpeed * Time.deltaTime;
    }

    private void MouseLook()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            looking = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            cameraTransform.Rotate(Vector3.up * mouseX);
            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, -90.0f, 90.0f);
            yRot += mouseX;
            cameraTransform.localRotation = Quaternion.Euler(xRot, yRot, 0f);
        }
        
        else
        {
            looking = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
