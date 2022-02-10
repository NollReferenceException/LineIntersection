using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private int _frameRate = 60;

    private PointsSystem _pointsSystem;
    private GameObject _pointsSystemObject;

    bool _placementStage = true;

    Vector3 _clickPos;


    void Start()
    {
        Application.targetFrameRate = _frameRate;

        PreInitPlacePoint();
    }

    void Update()
    {
        if(_placementStage)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                PlacePoints();
            }
        }
        if (Input.GetButtonUp("Fire2"))
        {
            _placementStage = !_placementStage;
            _pointsSystem.FinishPointsPlacing?.Invoke();
        }
    }

    void PlacePoints()
    {
        MouseToWorldPositionCalculate();

        if (_pointsSystem.FinishPoint == null)
        {
            _pointsSystem.FinishPoint = CreateFinishPoint();
        }
        else
        {
            if (CheckFreePlace())
            {
                _pointsSystem.StartPoints.Add(CreateStartPoint());
            }
        }
    }

    void MouseToWorldPositionCalculate()
    {
        _clickPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    bool CheckFreePlace()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            return false;
        }

        return true;
    }

    StartPoint CreateStartPoint()
    {
        StartPoint startPoint = Instantiate(_pointPrefab, _clickPos, Quaternion.identity).AddComponent<StartPoint>();
        startPoint.Init(_pointsSystem.FinishPoint);

        startPoint.GetComponent<Renderer>().material.color = Color.green;
        
        return startPoint;
    }

    FinishPoint CreateFinishPoint()
    {
        FinishPoint finishPoint = Instantiate(_pointPrefab, _clickPos, Quaternion.identity).AddComponent<FinishPoint>();

        finishPoint.GetComponent<Renderer>().material.color = Color.red;

        finishPoint.gameObject.AddComponent<Dragable>();

        return finishPoint;
    }

    void PreInitPlacePoint()
    {
        _pointsSystemObject = new GameObject("PointsSystem");
        _pointsSystem = _pointsSystemObject.AddComponent<PointsSystem>();
        _pointsSystem.Init();
    }
}
