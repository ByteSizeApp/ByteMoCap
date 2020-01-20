using UnityEngine;
using System.Collections.Generic;

namespace ByteScript.MoCap.Util
{
    [System.Serializable]
    public class KeyframeQuaternion
    {
        public AnimationCurve curveX = new AnimationCurve();
        public AnimationCurve curveY = new AnimationCurve();
        public AnimationCurve curveZ = new AnimationCurve();
        public AnimationCurve curveW = new AnimationCurve();

        public List<Keyframe> keysX = new List<Keyframe>();
        public List<Keyframe> keysY = new List<Keyframe>();
        public List<Keyframe> keysZ = new List<Keyframe>();
        public List<Keyframe> keysW = new List<Keyframe>();

        public void AddKeyframe(Quaternion value, float time)
        {
            keysX.Add(new Keyframe(time, value.x));
            keysY.Add(new Keyframe(time, value.y));
            keysZ.Add(new Keyframe(time, value.z));
            keysW.Add(new Keyframe(time, value.w));
        }

        public void SetCurves()
        {
            curveX.keys = keysX.ToArray();
            curveX = CurveUtilities.SetTangents(curveX, CurveTangentMode.smooth);

            curveY.keys = keysY.ToArray();
            curveY = CurveUtilities.SetTangents(curveY, CurveTangentMode.smooth);

            curveZ.keys = keysZ.ToArray();
            curveZ = CurveUtilities.SetTangents(curveZ, CurveTangentMode.smooth);

            curveW.keys = keysW.ToArray();
            curveW = CurveUtilities.SetTangents(curveW, CurveTangentMode.smooth);
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

        }

        public Quaternion? SampleCurves(float time)
        {
            //if time received is larger than curve, sample last keyframe
            if (curveX.length > 0 && time > curveX.keys[curveX.keys.Length - 1].time)
            {
                return null;
            }

            Quaternion quaternion = new Quaternion(
                curveX.Evaluate(time),
                curveY.Evaluate(time),
                curveZ.Evaluate(time),
                curveW.Evaluate(time)
            );

            return quaternion;
        }
    }
}