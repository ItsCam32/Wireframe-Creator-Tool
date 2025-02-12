using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    // vv Private Exposed vv //

    [SerializeField] private Grid grid;
    [SerializeField] private TMP_InputField gridSizeInputField;
    [SerializeField] private GameObject mainMenu;

    [SerializeField]
    private TextMeshProUGUI cameraSpeedText;

    [SerializeField]
    private TextMeshProUGUI nodesText;

    [SerializeField]
    private NodeSelector nodeSelector;

    // vv Private vv //

    private int gridSize = 25;

    // vv Public vv //

    public static UI_Manager Instance { get; private set; }

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
        gridSizeInputField.text = "25";
    }
    #endregion

    #region Public Functions

    public void UpdateCameraSpeedText(string newText)
    {
        cameraSpeedText.text = newText;
    }

    public void OnGridSizeInputFieldChanged()
    {
        if (gridSizeInputField.text == string.Empty)
        {
            nodesText.text = "Nodes: 0";
        }

        else
        {
            nodesText.text = $"Nodes: {Mathf.Pow(int.Parse(gridSizeInputField.text), 3)}";
        }
    }

    public void OnStartButtonClicked()
    {
        if (gridSizeInputField.text == string.Empty)
        {
            gridSizeInputField.text = "25";
            gridSize = 25;
            return;
        }

        gridSize = int.Parse(gridSizeInputField.text);
        if (gridSize < 5 || gridSize > 200)
        {
            gridSizeInputField.text = "25";
            gridSize = 25;
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
