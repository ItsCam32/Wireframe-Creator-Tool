using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class NodeSelector : MonoBehaviour
{
    // vv Private Exposed vv //

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject exportedTextObj;

    [SerializeField]
    private Material defaultNodeMat;

    [SerializeField]
    private Material selectedNodeMat;

    [SerializeField]
    private GameObject lineRendererPrefab;

    [SerializeField]
    private GameObject nodeUIPrefab;

    [SerializeField]
    private Transform nodesPanel;

    [SerializeField]
    private float nodeEntryStartY;

    [Range(0.0f, 100.0f)]
    [SerializeField]
    private float nodeEntryYSpacing;

    [SerializeField]
    private Grid grid;

    // vv Private vv //

    private List<Vector3> vertices = new List<Vector3>();
    private List<GameObject> lineRendererObjs = new List<GameObject>();
    private List<GameObject> uiNodes = new List<GameObject>();

    private GameObject firstNode = null;
    private GameObject secondNode = null;
    private int nodes = 0;
    private int magicListSpacingOffset = 40;
    private float exportConfirmationDuration = 3.0f;

    ////////////////////////////////////////

    #region Private Functions

    private void Update()
    {
        // On left click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Mouse cursor screen to world point
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.name == "Node")
                {
                    // Same node selected twice, so remove selections
                    if (firstNode != null && hitObj == firstNode)
                    {
                        firstNode.GetComponent<MeshRenderer>().material = defaultNodeMat;
                        firstNode = null;
                    }

                    // Selected unique 2nd node, so create the edge
                    else if (firstNode)
                    {
                        secondNode = hitObj;
                        secondNode.GetComponent<MeshRenderer>().material = selectedNodeMat;

                        // Increase scroll view length
                        nodesPanel.GetComponent<RectTransform>().sizeDelta += new Vector2(0, magicListSpacingOffset + nodeEntryYSpacing);

                        for (int i = 0; i < 2; i++)
                        {
                            vertices.Add(i == 0 ? firstNode.transform.position : secondNode.transform.position);

                            // Create UI entry for list
                            NodeEntry uiNodeEntry = Instantiate(nodeUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<NodeEntry>();

                            if (grid)
                            {
                                uiNodeEntry.UpdateData(nodes, i == 0 ? firstNode.transform.position : secondNode.transform.position, grid.Scale);
                            }
                            
                            uiNodes.Add(uiNodeEntry.gameObject);

                            Transform nodeTransform = uiNodeEntry.gameObject.transform;
                            nodeTransform.SetParent(nodesPanel);
                            nodeTransform.localScale = Vector3.one;
                            nodeTransform.localPosition = new Vector3(0.0f, nodeEntryStartY - (nodes * nodeEntryYSpacing), 0.0f);

                            nodes++;
                        }

                        // Create line between the 2 vertices 
                        LineRenderer renderer = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
                        renderer.positionCount = 2;
                        renderer.SetPosition(0, firstNode.transform.position);
                        renderer.SetPosition(1, secondNode.transform.position);
                        lineRendererObjs.Add(renderer.gameObject);

                        // Reset selections
                        firstNode.GetComponent<MeshRenderer>().material = defaultNodeMat;
                        secondNode.GetComponent<MeshRenderer>().material = defaultNodeMat;
                        firstNode = null;
                        secondNode = null;
                    }

                    // Selecting first node
                    else
                    {
                        firstNode = hitObj;
                        firstNode.GetComponent<MeshRenderer>().material = selectedNodeMat;
                    }
                }
            }
        }
    }

    private void HideExportConfirmation()
    {
        exportedTextObj.SetActive(false);
    }
    #endregion

    #region Public Functions

    public void OnResetButtonClicked()
    {
        // Get data before wiping
        ExportData();

        while (lineRendererObjs.Count > 0)
        {
            UndoMostRecentEdge();
        }
    }

    public void UndoMostRecentEdge()
    {
        // No drawn edges
        if (lineRendererObjs.Count == 0) return;

        // Destroy the renderer for the most recent edge
        GameObject mostRecentLineObj = lineRendererObjs[lineRendererObjs.Count - 1];
        lineRendererObjs.Remove(mostRecentLineObj);
        Destroy(mostRecentLineObj);

        // Remove 2 vertices that made this edge
        for (int i = 0; i < 2; i++)
        {
            vertices.RemoveAt(vertices.Count - 1);
            Destroy(uiNodes[uiNodes.Count - 1]);
            uiNodes.RemoveAt(uiNodes.Count - 1);
        }

        nodes -= 2;

        // Resize scroll view
        nodesPanel.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, magicListSpacingOffset + nodeEntryYSpacing);
    }

    public void ExportData()
    {
        string dataString = "";
        for (int i = 0; i < vertices.Count; i++)
        {
            // Simplify Vector (set gap to 1 for easy scaling externally)
            if (grid)
            {
                Vector3 simplifiedVertex = vertices[i] / grid.Scale;
                string xString = simplifiedVertex.x % 1 == 0 ? $"{simplifiedVertex.x}.0f" : $"{simplifiedVertex.x}f";
                string yString = simplifiedVertex.y % 1 == 0 ? $"{simplifiedVertex.y}.0f" : $"{simplifiedVertex.y}f";
                string zString = simplifiedVertex.z % 1 == 0 ? $"{simplifiedVertex.z}.0f" : $"{simplifiedVertex.z}f";

                dataString += $"Vector3({xString}, {yString}, {zString})";
            }

            // Separator
            if (i < vertices.Count - 1)
            {
                dataString += ", ";
            }

            if ((i + 1) % 3 == 0)
            {
                dataString += "\n";
            }
        }

        string xPivotString = UI_Manager.Instance.pivotX % 1 == 0 ? $"{UI_Manager.Instance.pivotX}.0f" : $"{UI_Manager.Instance.pivotX}f";
        string yPivotString = UI_Manager.Instance.pivotY % 1 == 0 ? $"{UI_Manager.Instance.pivotY}.0f" : $"{UI_Manager.Instance.pivotY}f";
        string zPivotString = UI_Manager.Instance.pivotZ % 1 == 0 ? $"{UI_Manager.Instance.pivotZ}.0f" : $"{UI_Manager.Instance.pivotZ}f";
        dataString += $"\n\nPivot: Vector3({xPivotString}, {yPivotString}, {zPivotString})";

        string path = Application.dataPath + Path.DirectorySeparatorChar + "/StreamingAssets/WireframeData.txt";
        using StreamWriter writer = new StreamWriter(path, false);
        writer.Write(dataString);
        writer.Close();

        // Open in default text editor (most likely notepad)
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = true
        });

        // Show confirmation
        exportedTextObj.SetActive(true);
        Invoke("HideExportConfirmation", exportConfirmationDuration);
    }
    #endregion
}
