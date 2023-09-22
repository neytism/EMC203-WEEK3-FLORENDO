using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private bool _showBezierCurves = true;
    
    [SerializeField] private GameObject _lineRendererPrefabQuadratic;
    [SerializeField] private GameObject _lineRendererPrefabCubic;
    
    [SerializeField] private LineRenderer _lineRendererStraight;
    private List<Transform> waypoints;

    public void Initialize(List<Transform> wp)
    {
        waypoints = wp;

        if (wp == null) return;
        
        DrawLineRendererStraight();
        if (!_showBezierCurves) return;
        DrawLineRendererQuadratic();
        DrawLineRendererCubic();
    }
    
     private void DrawLineRendererStraight()
    {
        _lineRendererStraight.positionCount = waypoints.Count;
        
        for (int i = 0; i < waypoints.Count; i++)
        {
            _lineRendererStraight.SetPosition(i, waypoints[i].position);
        }
    }

     private void DrawLineRendererQuadratic()
     {
         for (int j = 0; j < waypoints.Count - 2; j++)
         {
             float numOfPoints = 100;

             Vector3 bezier = new Vector3(0, 0, 0);

             // Instantiate a new LineRenderer prefab for each curve
             LineRenderer lineRendererQuadratic = Instantiate(_lineRendererPrefabQuadratic.GetComponent<LineRenderer>(), gameObject.transform, true);
             lineRendererQuadratic.positionCount = (int)numOfPoints;
             
             float t = 0;
             for (int i = 0; i < lineRendererQuadratic.positionCount; i++)
             {
                 bezier = EvaluateQuadraticCurve(waypoints[j].position, waypoints[j + 1].position, waypoints[j+2].position, t);
                 lineRendererQuadratic.SetPosition(i, bezier);
                 t += 1 / numOfPoints;
             }
         }
     }



    private void DrawLineRendererCubic()
    {
        for (int j = 0; j < waypoints.Count - 3; j++)
        {
            float numOfPoints = 100;
             
            Vector3 bezier = new Vector3(0,0,0);

            // Instantiate a new LineRenderer prefab for each curve
            LineRenderer lineRendererCubic = Instantiate(_lineRendererPrefabCubic.GetComponent<LineRenderer>(), gameObject.transform, true);
            lineRendererCubic.positionCount = (int)numOfPoints;

            float t = 0;
            for (int i = 0; i < lineRendererCubic.positionCount; i++)
            {
                bezier = EvaluateCubicCurve(waypoints[j].position, waypoints[j + 1].position, waypoints[j+2].position, waypoints[j+3].position, t);
                lineRendererCubic.SetPosition(i, bezier);
                t += 1 / numOfPoints;
            }
        }
    }

    private Vector3 EvaluateQuadraticCurve(Vector3 start, Vector3 mid, Vector3 end, float t)
    {
        Vector3 p0 = Mathf.Pow(1- t, 2) * start;
        Vector3 p1 = (1 - t) * 2 * t * mid;
        Vector3 p2 = Mathf.Pow(t, 2) * end;

        return p0 + p1 + p2;
    }

    private Vector3 EvaluateCubicCurve(Vector3 start, Vector3 mid0, Vector3 mid1, Vector3 end, float t)
    {
        Vector3 p0 = Mathf.Pow(1- t, 3) * start;;
        Vector3 p1 = Mathf.Pow(1- t, 2) * 3 * t * mid0;
        Vector3 p2 = 3 * (1 - t) * Mathf.Pow(t, 2) * mid1;
        Vector3 p3 = Mathf.Pow(t, 3) * end ;
        
        return p0 + p1 + p2 + p3;
    }

}
