using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    [SerializeField] private int _XgridSize = 1;
    [SerializeField] private int _YgridSize = 1;
    [SerializeField] private float _spacing = 1f;
    [SerializeField] private Color _gridColor = Color.gray;

    private void OnDrawGizmos()
    {
        DisplayGrid();
    }

    private void DisplayGrid()
    {
        Gizmos.color = _gridColor;

        // Calculate half grid size.
        float XhalfGridSize = _XgridSize * 0.5f * _spacing;
        float YhalfGridSize = _YgridSize * 0.5f * _spacing;

        // Draw vertical grid lines.
        for (int i = 0; i <= _XgridSize; i++)
        {
            float xPos = i * _spacing - XhalfGridSize;
            Vector3 start = new Vector3(xPos, -YhalfGridSize, 0);
            Vector3 end = new Vector3(xPos, YhalfGridSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal grid lines.
        for (int i = 0; i <= _YgridSize; i++)
        {
            float yPos = i * _spacing - YhalfGridSize;
            Vector3 start = new Vector3(-XhalfGridSize, yPos, 0);
            Vector3 end = new Vector3(XhalfGridSize, yPos, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}