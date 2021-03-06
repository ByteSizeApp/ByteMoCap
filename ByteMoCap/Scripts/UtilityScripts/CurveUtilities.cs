using UnityEngine;
using System.Collections.Generic;

namespace ByteScript.MoCap.Util
{
    public enum CurveTangentMode
    {
        smooth,
        stepped
    }

    public class CurveUtilities : MonoBehaviour
    {
        public static Vector3[] SmoothLinePoints(Vector3[] inputPoints, float segmentSize)
        {
            //create curves
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            //create keyframe sets
            Keyframe[] keysX = new Keyframe[inputPoints.Length];
            Keyframe[] keysY = new Keyframe[inputPoints.Length];
            Keyframe[] keysZ = new Keyframe[inputPoints.Length];

            //set keyframes
            for (int i = 0; i < inputPoints.Length; i++)
            {
                keysX[i] = new Keyframe(i, inputPoints[i].x);
                keysY[i] = new Keyframe(i, inputPoints[i].y);
                keysZ[i] = new Keyframe(i, inputPoints[i].z);
            }

            //apply keyframes to curves
            curveX.keys = keysX;
            curveY.keys = keysY;
            curveZ.keys = keysZ;

            //smooth curve tangents
            for (int i = 0; i < inputPoints.Length; i++)
            {
                curveX.SmoothTangents(i, 0);
                curveY.SmoothTangents(i, 0);
                curveZ.SmoothTangents(i, 0);
            }

            //list to write smoothed values to
            List<Vector3> lineSegments = new List<Vector3>();

            //find segments in each section
            for (int i = 0; i < inputPoints.Length; i++)
            {
                //add first point
                lineSegments.Add(inputPoints[i]);

                //make sure within range of array
                if (i + 1 < inputPoints.Length)
                {
                    //find distance to next point
                    float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);

                    //number of segments
                    int segments = (int)(distanceToNext / segmentSize);

                    //add segments
                    for (int s = 1; s < segments; s++)
                    {
                        //interpolated time on curve
                        float time = ((float)s / (float)segments) + (float)i;

                        //sample curves to find smoothed position
                        Vector3 newSegment = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));

                        //add to list
                        lineSegments.Add(newSegment);
                    }
                }
            }

            return lineSegments.ToArray();
        }

        /// <summary>
        /// Returns an array of curves (x=0,y=1,z=2) in between the time of 0-1
        /// </summary>
        /// <returns>The line curve time01.</returns>
        /// <param name="inputPoints">Input points.</param>
        public static AnimationCurve[] SmoothLineCurveTime01(Vector3[] inputPoints)
        {
            //create curves
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            //create keyframe sets
            Keyframe[] keysX = new Keyframe[inputPoints.Length];
            Keyframe[] keysY = new Keyframe[inputPoints.Length];
            Keyframe[] keysZ = new Keyframe[inputPoints.Length];

            //set keyframes
            for (int i = 0; i < inputPoints.Length; i++)
            {
                float time = (float)i / ((float)inputPoints.Length - 1);

                keysX[i] = new Keyframe(time, inputPoints[i].x);
                keysY[i] = new Keyframe(time, inputPoints[i].y);
                keysZ[i] = new Keyframe(time, inputPoints[i].z);
            }

            //apply keyframes to curves
            curveX.keys = keysX;
            curveY.keys = keysY;
            curveZ.keys = keysZ;

            //smooth curve tangents
            for (int i = 0; i < inputPoints.Length; i++)
            {
                curveX.SmoothTangents(i, 0);
                curveY.SmoothTangents(i, 0);
                curveZ.SmoothTangents(i, 0);
            }

            AnimationCurve[] packedCurves = new AnimationCurve[3];
            packedCurves[0] = curveX;
            packedCurves[1] = curveY;
            packedCurves[2] = curveZ;

            return packedCurves;
        }

        /// <summary>
        /// Sets the tangents for a given curve.
        /// </summary>
        /// <returns>The tangents.</returns>
        /// <param name="curve">Curve.</param>
        /// <param name="tangentMode">Tangent mode.</param>
        public static AnimationCurve SetTangents(AnimationCurve curve, CurveTangentMode tangentMode)
        {
            for (int i = 0; i < curve.keys.Length; i++)
            {
                //smooth
                if (tangentMode == CurveTangentMode.smooth)
                {
                    curve.SmoothTangents(i, 0);
                }

                //stepped
                if (tangentMode == CurveTangentMode.stepped)
                {
                    curve.keys[i].inTangent = Mathf.Infinity;
                    curve.keys[i].outTangent = Mathf.Infinity;
                }
            }

            return curve;
        }
    }
}