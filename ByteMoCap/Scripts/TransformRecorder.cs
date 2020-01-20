using ByteScript.MoCap.Util;

namespace ByteScript.MoCap
{
    public class TransformRecorder : GeneralRecorder
    {
        public bool useLocalCoordinates = false;

        protected KeyframeVector3 positionKey = new KeyframeVector3();
        protected KeyframeQuaternion rotationKey = new KeyframeQuaternion();

        protected override void AddKeyFrame(float time)
        {
            //position
            if (useLocalCoordinates)
            {
                positionKey.AddKeyframe(transform.localPosition, time);
            }
            else
            {
                positionKey.AddKeyframe(transform.position, time);
            }

            //rotation
            if (useLocalCoordinates)
            {
                rotationKey.AddKeyframe(transform.localRotation, time);
            }
            else
            {
                rotationKey.AddKeyframe(transform.rotation, time);
            }
        }

        protected override void SaveToDisk()
        {
            FileManager.SerializeObject<TransformRecord>(
                getTransformRecord(), group, customPrefix, recordName);
        }

        protected TransformRecord getTransformRecord()
        {
            positionKey.SetCurves();
            rotationKey.SetCurves();

            TransformRecord transformRecord = new TransformRecord();
            transformRecord.isValid = true;

            transformRecord.positionKey = positionKey;
            transformRecord.rotationKey = rotationKey;
            return transformRecord;
        }
    }

    [System.Serializable]
    public class TransformRecord
    {
        public bool isValid;
        public KeyframeVector3 positionKey;
        public KeyframeQuaternion rotationKey;
    }
}