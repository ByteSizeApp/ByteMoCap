using UnityEngine;
using System.Collections.Generic;

namespace ByteScript.MoCap.Util
{
    [System.Serializable]
    public class KeyframeVector3
    {
        public AnimationCurve curveX = new AnimationCurve();
        public AnimationCurve curveY = new AnimationCurve();
        public AnimationCurve curveZ = new AnimationCurve();

        private List<Keyframe> keysX = new List<Keyframe>();
        private List<Keyframe> keysY = new List<Keyframe>();
        private List<Keyframe> keysZ = new List<Keyframe>();

        public void AddKeyframe(Vector3 value, float time)
        {
            keysX.Add(new Keyframe(time, value.x));
            keysY.Add(new Keyframe(time, value.y));
            keysZ.Add(new Keyframe(time, value.z));
        }

        public void SetCurves()
        {
            curveX.keys = keysX.ToArray();
            curveX = CurveUtilities.SetTangents(curveX, CurveTangentMode.smooth);

            curveY.keys = keysY.ToArray();
            curveY = CurveUtilities.SetTangents(curveY, CurveTangentMode.smooth);

            curveZ.keys = keysZ.ToArray();
            curveZ = CurveUtilities.SetTangents(curveZ, CurveTangentMode.smooth);
        }

        public Vector3? SampleCurves(float time)
        {
            //if time received is larger than curve, sample last keyframe
            if (curveX.length > 0 && time > curveX.keys[curveX.keys.Length - 1].time)
            {
                return null;
            }

            Vector3 vec3 = new Vector3(
                curveX.Evaluate(time),
                curveY.Evaluate(time),
                curveZ.Evaluate(time)
            );

            return vec3;
        }

        public void RemoveFramesAfterTime(float time)
        {
            int index = 0;

            for (int i = 0; i < keysX.Count; i++)
            {
                if (keysX[i].time > time)
                {
                    index = i;
                    break;
                }
            }
            keysX.RemoveRange(index, keysX.Count - index);
            Debug.Log(index + " " + (keysX.Count - index));
        }
    }
}