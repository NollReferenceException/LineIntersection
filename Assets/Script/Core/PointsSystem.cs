using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PointsSystem : MonoBehaviour
{
    private List<StartPoint> _startPoints;
    private FinishPoint _finishPoint;

    private bool renderingStarted;

    public FinishPoint FinishPoint { get => _finishPoint; set => _finishPoint = value; }
    public List<StartPoint> StartPoints { get => _startPoints; set => _startPoints = value; }
    public UnityAction FinishPointsPlacing { get; set; }

    public void Init()
    {
        _startPoints = new List<StartPoint>();

        FinishPointsPlacing += SwitchRendering;
    }

    void Update()
    {
        if (renderingStarted)
        {
            CalculateStartPointsOffset();

            MixingLines();

            RenderLines();
        }
    }

    void MixingLines()
    {
        for (int i = 0; i < _startPoints.Count; i++)
        {
            List<Vector3> tempIntersectionNegativePoints = new List<Vector3>();
            List<Vector3> tempIntersectionPositivePoints = new List<Vector3>();

            Vector2 intersectionNegativePoint = _startPoints[i].NegativeFinishPoint;
            Vector2 intersectionPositivePoint = _startPoints[i].PositiveFinishPoint;

            for (int j = 0; j < _startPoints.Count; j++)
            {
                if (_startPoints[j] == _startPoints[i])
                {
                    continue;
                }

                NegativeSideCalc(_startPoints[i], _startPoints[j], ref intersectionNegativePoint, ref tempIntersectionNegativePoints);
                PositiveSideCalc(_startPoints[i], _startPoints[j], ref intersectionPositivePoint, ref tempIntersectionPositivePoints);
            }


            if (tempIntersectionNegativePoints.Count > 1)
                intersectionNegativePoint = GetNearbyintersectionPoint(tempIntersectionNegativePoints, _startPoints[i].NegativeStartPoint);
            if (tempIntersectionPositivePoints.Count > 1)
                intersectionPositivePoint = GetNearbyintersectionPoint(tempIntersectionPositivePoints, _startPoints[i].PositiveStartPoint);

            _startPoints[i].IntersectionPositivePoint = intersectionPositivePoint;
            _startPoints[i].IntersectionNegativePoint = intersectionNegativePoint;
        }
    }

    void NegativeSideCalc(StartPoint currentPoint, StartPoint otherPoint, ref Vector2 intersectionNegativePoint, ref List<Vector3> tempList)
    {
        if (MathUtils.LineIntersection(currentPoint.NegativeStartPoint, currentPoint.NegativeFinishPoint, otherPoint.PositiveStartPoint, otherPoint.IntersectionPositivePoint, ref intersectionNegativePoint))
        {
            tempList.Add(intersectionNegativePoint);
        }
        else if (MathUtils.LineIntersection(currentPoint.NegativeStartPoint, currentPoint.NegativeFinishPoint, _finishPoint.transform.position, otherPoint.IntersectionPositivePoint, ref intersectionNegativePoint))
        {
            tempList.Add(intersectionNegativePoint);
        }
        else if (MathUtils.LineIntersection(currentPoint.NegativeStartPoint, currentPoint.NegativeFinishPoint, otherPoint.PositiveStartPoint, otherPoint.PositiveFinishPoint, ref intersectionNegativePoint))
        {
            tempList.Add(intersectionNegativePoint);
        }
    }

    void PositiveSideCalc(StartPoint currentPoint, StartPoint otherPoint, ref Vector2 intersectionPositivePoint, ref List<Vector3> tempList)
    {
        if (MathUtils.LineIntersection(currentPoint.PositiveStartPoint, currentPoint.PositiveFinishPoint, otherPoint.NegativeStartPoint, otherPoint.IntersectionNegativePoint, ref intersectionPositivePoint))
        {
            tempList.Add(intersectionPositivePoint);
        }
        else if (MathUtils.LineIntersection(currentPoint.PositiveStartPoint, currentPoint.PositiveFinishPoint, _finishPoint.transform.position, otherPoint.IntersectionNegativePoint, ref intersectionPositivePoint))
        {
            tempList.Add(intersectionPositivePoint);
        }
        else if (MathUtils.LineIntersection(currentPoint.PositiveStartPoint, currentPoint.PositiveFinishPoint, otherPoint.NegativeStartPoint, otherPoint.NegativeFinishPoint, ref intersectionPositivePoint))
        {
            tempList.Add(intersectionPositivePoint);
        }
    }

    Vector3 GetNearbyintersectionPoint(List<Vector3> IntersectionPoints, Vector3 startPoint)
    {
        float minDistance = 99999f;
        int minIndex = 0;

        for (int i = 0; i < IntersectionPoints.Count; i++)
        {
            float currentDistance = Vector3.Distance(startPoint, IntersectionPoints[i]);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                minIndex = i;
            }
        }

        return IntersectionPoints[minIndex];
    }

    void CalculateStartPointsOffset()
    {
        for (int i = 0; i < _startPoints.Count; i++)
        {
            _startPoints[i].OffsetCalc();
        }
    }

    void RenderLines()
    {
        for (int i = 0; i < _startPoints.Count; i++)
        {
            LinkedList<Vector3> vertices = new LinkedList<Vector3>();

            vertices.AddLast(_startPoints[i].NegativeStartPoint);
            vertices.AddLast(_startPoints[i].PositiveStartPoint);
            vertices.AddLast(_startPoints[i].IntersectionPositivePoint);
            vertices.AddLast(_startPoints[i].IntersectionNegativePoint);

            vertices.AddLast(_finishPoint.transform.position);

            _startPoints[i].UpdateMesh(vertices);
        }
    }

    void SwitchRendering()
    {
        renderingStarted = !renderingStarted;

        if (renderingStarted)
        {
            ///nothing
        }
        else
        {
            for (int i = 0; i < _startPoints.Count; i++)
            {
                Destroy(_startPoints[i].MeshObj.gameObject);
                Destroy(_startPoints[i].gameObject);
            }
            _startPoints.Clear();
        }
    }
}
