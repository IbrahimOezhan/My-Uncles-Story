using UnityEngine;

public class A_SplineEmitter : MonoBehaviour
{
    private Vector3[] splinePointsArray;
    private int splineCount;
    public bool debugDrawSpline = true;

    private void Start()
    {
        FMOD_ReTransform_Spline();
    }

    public void FMOD_ReTransform_Spline()
    {
        splineCount = transform.childCount;
        splinePointsArray = new Vector3[splineCount];
        for (int i = 0; i < splineCount; i++) splinePointsArray[i] = transform.GetChild(i).position;
    }

    private void FixedUpdate()
    {
        if (splineCount > 1)
        {
            for (int i = 1; i < splineCount; i++) Debug.DrawLine(splinePointsArray[i], splinePointsArray[i - 1], Color.blue);
        }
    }

    public Vector3 WhereOnSpline(Vector3 pos) //functions bellow feed values to where
    {
        int ClosestSplinePoint = GetClosestSplinePoint(pos);
        if (ClosestSplinePoint == 0) return splineSegment(splinePointsArray[0], splinePointsArray[1], pos);

        else if (ClosestSplinePoint == splineCount - 1)
        {
            if (AudioManager.instance.player == null) return new Vector3(0, 0, 0);
            else return splineSegment(splinePointsArray[splineCount - 1], splinePointsArray[splineCount - 2], pos);
        }
        else //gives what was the previous point what is the next point and the math for it
        {
            Vector3 leftSeg = splineSegment(splinePointsArray[ClosestSplinePoint - 1], splinePointsArray[ClosestSplinePoint], pos);
            Vector3 rightSeg = splineSegment(splinePointsArray[ClosestSplinePoint + 1], splinePointsArray[ClosestSplinePoint], pos);

            if ((pos - leftSeg).sqrMagnitude <= (pos - rightSeg).sqrMagnitude) return leftSeg;
            else return rightSeg;
        }
    }

    private int GetClosestSplinePoint(Vector3 pos) //closest spline point to the player
    {
        int closestPoint = -1;
        float shortestDistance = 0.0f;

        for (int i = 0; i < splineCount; i++)
        {
            float sqrDistance = (splinePointsArray[i] - pos).sqrMagnitude;
            if (shortestDistance == 0.0f || sqrDistance < shortestDistance)
            {
                shortestDistance = sqrDistance;
                closestPoint = i;
            }
        }
        return closestPoint;
    }

    public Vector3 splineSegment(Vector3 v1, Vector3 v2, Vector3 pos) //direction where we moving in spline
    {
        Vector3 v1ToPos = pos - v1;
        Vector3 seqDirection = (v2 - v1).normalized;

        float distanceFromV1 = Vector3.Dot(seqDirection, v1ToPos); //makes value always 1 or -1 depending on the direction it's going

        if (distanceFromV1 < 0.0f) return v1;
        else if (distanceFromV1 * distanceFromV1 > (v2 - v1).sqrMagnitude) return v2;
        else return v1 + (seqDirection * distanceFromV1);
    }
}
