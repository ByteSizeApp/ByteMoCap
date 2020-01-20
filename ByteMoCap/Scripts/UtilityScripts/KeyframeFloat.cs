using UnityEngine;
using System.Collections.Generic;

namespace ByteScript.MoCap.Util
{
    [System.Serializable]
    public class KeyframeFloat
    {
        public AnimationCurve curve = new AnimationCurve();

        public List<Keyframe> keys = new List<Keyframe>();

        public void AddKeyframe(float value, float time)
        {
            keys.Add(new Keyframe(time, value));
        }

        public void SetCurves()
        {
            curve.keys = keys.ToArray();
            curve = CurveUtilities.SetTangents(curve, CurveTangentMode.smooth);
        }

        public float SampleCurves(float time)
        {
            //if time received is larger than curve, sample last keyframe
            if (curve.length > 0 && time > curve.keys[curve.keys.Length - 1].time)
            {
                time = curve.keys[curve.keys.Length - 1].time;
            }

            float newFloat = curve.Evaluate(time);

            return newFloat;
        }
    }
}