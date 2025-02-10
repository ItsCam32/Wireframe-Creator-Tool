using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab, edgeNodePrefab;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int nodeSpacing;
    [SerializeField] private float renderRange;
    [SerializeField] private float cameraUpdateFrequency;

    public float bounds
    {
        get; private set;
    }

    public int scale
    {
        get; private set;
    }

    // Types: Node object, is edge node
    private Dictionary<GameObject, bool> nodes = new Dictionary<GameObject, bool>();

    private List<MeshRenderer> nodeRenderers = new List<MeshRenderer>();
    private float timeSinceCameraUpdated = 0.0f;
    private bool gridVisible = true;

    public void BuildGrid(int gridSize)
    {
        scale = nodeSpacing;

        if (gridSize % 2 == 0) gridSize++;
        float min = 0.0f - (gridSize - 1) / 2 * nodeSpacing;
        float max = 0.0f + (gridSize - 1) / 2 * nodeSpacing;
        bounds = max;

        for (float width = min; width <= max; width += nodeSpacing)
        {
            for (float height = min; height <= max; height += nodeSpacing)
            {
                for (float depth = min; depth <= max; depth += nodeSpacing)
                {
                    // Is this node at the edge of our grid?
                    bool isEdgeNode =
                        width == min || width == max
                        || height == min || height == max
                        || depth == min || depth == max;

                    // Create node
                    GameObject node = Instantiate(isEdgeNode ? edgeNodePrefab : nodePrefab, new Vector3(width, height, depth), Quaternion.identity);
                    node.name = isEdgeNode ? "Edge" : "Node";
                    nodes.Add(node, isEdgeNode);
                    nodeRenderers.Add(node.GetComponent<MeshRenderer>());
                }
            }
        }
    }

    private void Update()
    {
        // Don't update nodes every frame
        timeSinceCameraUpdated += Time.deltaTime;
        if (timeSinceCameraUpdated < cameraUpdateFrequency || !gridVisible) return;
        timeSinceCameraUpdated = 0.0f;

        foreach (KeyValuePair<GameObject, bool> node in nodes)
        {
            if (node.Value) continue;

            // Hide nodes at a certain distance
            float distance = Vector3.Distance(node.Key.transform.position, cameraTransform.position);
            node.Key.SetActive(distance < renderRange ? true : false);

            // Scale nodes down as they get further away
            distance = 50.0f / distance;
            distance = Mathf.Clamp(distance, 0.0f, 1.0f);
            node.Key.transform.localScale = new Vector3(distance, distance, distance);
        }
    }

    public void OnGridToggleButtonClicked()
    {
        gridVisible = !gridVisible;

        foreach (KeyValuePair<GameObject, bool> node in nodes)
        {
            // Manually enable edge nodes, others are done dynamically
            if (gridVisible)
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
}
