using UnityEngine;

public class RayRange
{
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private Vector2 _direction;

    public RayRange(Vector2 startPoint, Vector2 endPoint, Vector2 direction)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;
        _direction = direction;
    }

    public Vector2 StartPoint => _startPoint;

    public Vector2 EndPoint => _endPoint;

    public Vector2 Direction => _direction;

    public Ray GetRay(float t)
    {
        Vector2 point = Vector2.Lerp(_startPoint, _endPoint, t);
        Ray ray = new Ray(point, _direction);
        return ray;
    }

    public Ray GetRay(Vector2 offsetFromStart)
    {
        float t = offsetFromStart.sqrMagnitude / (_startPoint.sqrMagnitude - _endPoint.sqrMagnitude);
        return GetRay(t);
    }
}