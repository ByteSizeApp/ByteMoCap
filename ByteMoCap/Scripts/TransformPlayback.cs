using ByteScript.MoCap.Util;
using UnityEngine;

namespace ByteScript.MoCap
{
    public class TransformPlayback : GeneralPlayback
    {
        //PUBLIC
        [Header("Recording Settings")]
        public bool useLocalCoordinates = false;

        private KeyframeVector3 positionKey = new KeyframeVector3();
        private KeyframeQuaternion rotationKey = new KeyframeQuaternion();
        protected override void LoadFromDisk()
        {
            TransformRecord record =
                FileManager.DeSerializeObject<TransformRecord>(group, customPrefix, recordName);

            setTransformPlayback(record);
        }

        // True if valid.
        protected bool setTransformPlayback(TransformRecord transformRecord)
        {
            if (transformRecord.isValid)
            {
                isValid = transformRecord.isValid;
                positionKey = transformRecord.positionKey;
                rotationKey = transformRecord.rotationKey;
                return true;
            }

            return false;
        }

        protected override void Playback(float time)
        {
            Vector3? position = positionKey.SampleCurves(time);
            if (position.HasValue)
            {
                if (useLocalCoordinates)
                {
                    this.transform.localPosition = position.Value;
                }
                else
                {
                    this.transform.position = position.Value;
                }
            }

            Quaternion? rotation = rotationKey.SampleCurves(time);
            if (rotation.HasValue)
            {
                if (useLocalCoordinates)
                {
                    this.transform.localRotation = rotation.Value;
                }
                else
                {
                    this.transform.rotation = rotation.Value;
                }
            }
        }
    }
}