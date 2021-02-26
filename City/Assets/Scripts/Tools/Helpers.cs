using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Helpers
{
    public static LineRenderer DrawCircle(LineRenderer LR, int segmentsPerCircle, float radius, float lineWidth)
    {
        LR.useWorldSpace = false;
        LR.startWidth = lineWidth;
        LR.endWidth = lineWidth;
        LR.positionCount = segmentsPerCircle + 1;

        var pointCount = segmentsPerCircle + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segmentsPerCircle);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        LR.SetPositions(points);
        return LR;
    }
}