using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeEntry : MonoBehaviour
{
    // vv Private Exposed vv //

    [SerializeField]
    private TextMeshProUGUI indexText;

    [SerializeField]
    private TextMeshProUGUI positionText;

    [SerializeField]
    private Button focusButton;

    // vv Private vv //

    private Vector3 nodePosition;

    ////////////////////////////////////////

    #region Public Functions

    public void UpdateData(int index, Vector3 position, int scale)
    {
        indexText.text = index.ToString();
        nodePosition = position;

        position /= scale;
        positionText.text = $"{position.x.ToString("F0")}, {position.y.ToString("F0")}, {position.z.ToString("F0")}";
    }

    public void OnFocusButtonClicked()
    {
        Camera.main.transform.position = nodePosition - Camera.main.transform.forward * 3;
    }
    #endregion
}
