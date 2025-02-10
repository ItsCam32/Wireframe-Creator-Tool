using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private TMP_InputField gridSizeInputField;
    [SerializeField] private GameObject mainMenu;

    private int gridSize = 25;

    private void Start()
    {
        gridSizeInputField.text = "25";
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
}
