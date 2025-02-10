using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText, positionText;
    [SerializeField] private Button focusButton;

    private Vector3 nodePosition;

    public void UpdateData(int index, Vector3 position, int scale)
    {
        nodePosition = position;
        indexText.text = index.ToString();
        positionText.text = $"{(position.x / scale).ToString("F0")}, {(position.y / scale).ToString("F0")}, {(position.z / scale).ToString("F0")}";
    }

    public void OnFocusButtonClicked()
    {
        Camera.main.transform.position = nodePosition - Camera.main.transform.forward * 3;
    }
}
