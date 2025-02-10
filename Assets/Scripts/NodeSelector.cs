using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class NodeSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject exportedText;
    [SerializeField] private Material defaultNodeMat, selectedNodeMat;
    [SerializeField] private GameObject lineRendererPrefab, nodeEntryPrefab;
    [SerializeField] private Transform nodesPanel;
    [SerializeField] private float nodeEntryStartY;
    [SerializeField] private float nodeEntryYSpacing;

    [SerializeField] private Grid grid;

    private List<Vector3> edges = new List<Vector3>();
    private List<GameObject> lineRenderers = new List<GameObject>();
    private List<GameObject> nodeEntries = new List<GameObject>();

    private GameObject firstNode, secondNode;
    private int nodes = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.name == "Node")
                {
                    // Same node selected twice
                    if (firstNode && hitObj == firstNode)
                    {
                        firstNode.GetComponent<MeshRenderer>().material = defaultNodeMat;
                        firstNode = null;
                    }

                    // Selecting unique 2nd node
                    else if (firstNode)
                    {
                        secondNode = hitObj;
                        secondNode.GetComponent<MeshRenderer>().material = selectedNodeMat;

                        nodesPanel.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 40 + nodeEntryYSpacing);

                        for (int i = 0; i < 2; i++)
                        {
                            edges.Add(i == 0 ? firstNode.transform.position : secondNode.transform.position);

                            GameObject node = Instantiate(nodeEntryPrefab, Vector3.zero, Quaternion.identity);
                            nodeEntries.Add(node);

                            NodeEntry nodeEntry = node.GetComponent<NodeEntry>();
                            nodeEntry.UpdateData(nodes, i == 0 ? firstNode.transform.position : secondNode.transform.position, grid.scale);

                            Transform nodeTransform = nodeEntry.gameObject.transform;
                            nodeTransform.SetParent(nodesPanel);
                            nodeTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            nodeTransform.localPosition = new Vector3(0.0f, nodeEntryStartY - (nodes * nodeEntryYSpacing), 0.0f);

                            nodes++;
                        }

                        GameObject lineRendererObj = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
                        lineRenderers.Add(lineRendererObj);

                        LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();
                        lineRenderer.positionCount = 2;
                        lineRenderer.SetPosition(0, firstNode.transform.position);
                        lineRenderer.SetPosition(1, secondNode.transform.position);

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

    public void OnResetButtonClicked()
    {
        OnExportButtonClicked();

        while (lineRenderers.Count > 0)
        {
            OnUndoButtonClicked();
        }
    }

    public void OnUndoButtonClicked()
    {
        if (lineRenderers.Count == 0) return;

        GameObject line = lineRenderers[lineRenderers.Count - 1];
        lineRenderers.Remove(line);
        Destroy(line);

        edges.RemoveAt(edges.Count - 1);
        edges.RemoveAt(edges.Count - 1);

        Destroy(nodeEntries[nodeEntries.Count - 1]);
        Destroy(nodeEntries[nodeEntries.Count - 2]);
        nodeEntries.RemoveAt(nodeEntries.Count - 1);
        nodeEntries.RemoveAt(nodeEntries.Count - 1);

        nodes -= 2;

        nodesPanel.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 40 + nodeEntryYSpacing);
    }

    public void OnExportButtonClicked()
    {
        string dataString = "";
        for (int i = 0; i < edges.Count; i++)
        {
            dataString += $"Vector3({edges[i].x / grid.scale}, {edges[i].y / grid.scale}, {edges[i].z / grid.scale})";

            if (i < edges.Count - 1)
            {
                dataString += ", ";
            }
        }

        string path = Application.dataPath + Path.DirectorySeparatorChar + "/StreamingAssets/WireframeData.txt";
        using StreamWriter writer = new StreamWriter(path, false);
        writer.Write(dataString);
        writer.Close();

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = true
        });

        exportedText.SetActive(true);
        Invoke("HideExportConfirmation", 3.0f);
    }

    private void HideExportConfirmation()
    {
        exportedText.SetActive(false);
    }
}
