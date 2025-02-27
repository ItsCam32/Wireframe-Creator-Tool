using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UI_Manager : MonoBehaviour
{
    // vv Private Exposed vv //

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private TMP_InputField gridSizeInputField;

    [SerializeField]
    private TMP_InputField pivotXInputField;

    [SerializeField]
    private TMP_InputField pivotYInputField;

    [SerializeField]
    private TMP_InputField pivotZInputField;

    [SerializeField]
    private Transform pivotCube;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private TextMeshProUGUI cameraSpeedText;

    [SerializeField]
    private TextMeshProUGUI nodesText;

    // vv Private vv //

    private int gridSize = 24;

    // vv Public vv //

    public static UI_Manager Instance { get; private set; }

    [HideInInspector]
    public float pivotX = 0.0f;

    [HideInInspector]
    public float pivotY = 0.0f;

    [HideInInspector]
    public float pivotZ = 0.0f;

    ////////////////////////////////////////

    #region Private Functions

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gridSizeInputField.text = "24";
        pivotXInputField.text = "0";
        pivotYInputField.text = "0";
        pivotZInputField.text = "0";
    }
    #endregion

    #region Public Functions

    public void UpdateCameraSpeedText(string newText)
    {
        cameraSpeedText.text = newText;
    }

    public void OnGridSizeInputFieldChanged()
    {
        if (string.IsNullOrEmpty(gridSizeInputField.text) || gridSizeInputField.text.Contains("-"))
        {
            nodesText.text = "Nodes: 0";
        }

        else if (int.TryParse(gridSizeInputField.text, out int result))
        {
            nodesText.text = $"Nodes: {Mathf.Pow(result, 3)}";
        }
    }

    public void OnPivotInputFieldsChanged()
    {
        if (string.IsNullOrEmpty(pivotXInputField.text) || pivotXInputField.text == "-")
        {
            return;
        }

        if (float.TryParse(pivotXInputField.text, out float xResult))
        {
            pivotX = xResult;
        }

        if (string.IsNullOrEmpty(pivotYInputField.text) || pivotYInputField.text == "-")
        {
            return;
        }

        if (float.TryParse(pivotYInputField.text, out float yResult))
        {
            pivotY = yResult;
        }

        if (string.IsNullOrEmpty(pivotZInputField.text) || pivotZInputField.text == "-")
        {
            return;
        }

        if (float.TryParse(pivotZInputField.text, out float zResult))
        {
            pivotZ = zResult;
        }

        pivotCube.position = new Vector3(pivotX * 10, pivotY * 10, pivotZ * 10);
    }

    public void OnStartButtonClicked()
    {
        if (gridSizeInputField.text == string.Empty || gridSizeInputField.text.Contains("-"))
        {
            gridSize = 24;
            gridSizeInputField.text = gridSize.ToString();
            
            return;
        }

        gridSize = int.Parse(gridSizeInputField.text);
        if (gridSize < 5 || gridSize > 200)
        {
            gridSize = 24;
            gridSizeInputField.text = gridSize.ToString();
            return;
        }

        grid.BuildGrid(gridSize);
        mainMenu.SetActive(false);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
    #endregion
}
