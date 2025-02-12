using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // vv Private Exposed vv //

    [SerializeField]
    private GameObject nodePrefab, edgePrefab;

    [SerializeField]
    private Transform cameraTransform;

    [Range(5, 100)]
    [SerializeField]
    private int nodeSpacing;

    [Range(10f, 100000.0f)]
    [SerializeField]
    private float nodeRenderRange;

    [Range(0.01f, 5.0f)]
    [SerializeField]
    private float UpdateFrequency;

    // vv Private vv //

    // Key: Node object
    // Value: Edge node
    private Dictionary<GameObject, bool> nodes = new Dictionary<GameObject, bool>();

    private List<MeshRenderer> nodeMeshRenderers = new List<MeshRenderer>();
    private float timeSinceUpdated = 0.0f;
    private bool isGridVisible = true;

    // vv Public vv //

    public float Bounds { get; private set; }
    public int Scale
    {
        get { return nodeSpacing; }
    }

    ////////////////////////////////////////

    #region Private Functions

    private void Update()
    {
        // Don't update nodes every frame or if the grid is toggled off
        timeSinceUpdated += Time.deltaTime;
        if (timeSinceUpdated < UpdateFrequency || !isGridVisible) return;
        timeSinceUpdated = 0.0f;

        foreach (KeyValuePair<GameObject, bool> node in nodes)
        {
            // Always show edge nodes while the grid is toggled on
            if (node.Value) continue;

            // Hide nodes at a certain distance
            float distance = Vector3.Distance(node.Key.transform.position, cameraTransform.position);
            node.Key.SetActive(distance < nodeRenderRange ? true : false);

            // Scale nodes down as they get further away
            distance = 50.0f / distance;
            distance = Mathf.Clamp(distance, 0.0f, 1.0f);
            node.Key.transform.localScale = new Vector3(distance, distance, distance);
        }
    }
    #endregion

    #region Public Functions

    public void BuildGrid(int gridSize)
    {
        if (gridSize % 2 == 0) gridSize++;
        gridSize += 2;

        float min = -((gridSize - 1) / 2 * nodeSpacing);
        float max = -min;
        Bounds = max;

        for (float width = min; width <= max; width += nodeSpacing)
        {
            for (float height = min; height <= max; height += nodeSpacing)
            {
                for (float depth = min; depth <= max; depth += nodeSpacing)
                {
                    // Is this position at the edge of our grid?
                    bool isEdge =
                        width == min || width == max
                        || height == min || height == max
                        || depth == min || depth == max;

                    // Create node
                    GameObject node = Instantiate(isEdge ? edgePrefab : nodePrefab, new Vector3(width, height, depth), Quaternion.identity);
                    node.name = isEdge ? "Edge" : "Node";
                    nodes.Add(node, isEdge);
                    nodeMeshRenderers.Add(node.GetComponent<MeshRenderer>());
                }
            }
        }
    }

    public void OnGridToggleButtonClicked()
    {
        isGridVisible = !isGridVisible;

        foreach (KeyValuePair<GameObject, bool> node in nodes)
        {
            // Manually enable edge nodes, others are done dynamically
            if (isGridVisible)
            {
                if (node.Value)
                {
                    node.Key.SetActive(true);
                }
            }

            else
            {
                node.Key.SetActive(false);
            }
        }
    }
    #endregion
}
