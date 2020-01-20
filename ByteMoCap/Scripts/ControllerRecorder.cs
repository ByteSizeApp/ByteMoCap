using ByteScript.MoCap.Util;

namespace ByteScript.MoCap
{
    public class ControllerRecorder : TransformRecorder
    {
        public float triggerValue = 0;
        protected KeyframeFloat triggerKey = new KeyframeFloat();

        // To be used with unity events.
        public void setTriggerValue(float triggerValue)
        {
            this.triggerValue = triggerValue;
        }

        protected override void AddKeyFrame(float time)
        {
            base.AddKeyFrame(time);
            triggerKey.AddKeyframe(triggerValue, time);
        }

        protected override void SaveToDisk()
        {
            FileManager.SerializeObject<ControllerRecord>(
                getControllerRecord(), group, customPrefix, recordName);
        }

        protected ControllerRecord getControllerRecord()
        {
            ControllerRecord controllerRecord = new ControllerRecord(getTransformRecord());
            triggerKey.SetCurves();

            controllerRecord.triggerKey = triggerKey;
            return controllerRecord;
        }
    }

    [System.Serializable]
    public class ControllerRecord : TransformRecord
    {
        public KeyframeFloat triggerKey;
        public TransformRecord transformRecord;
        public ControllerRecord() { }

        public ControllerRecord(TransformRecord transformRecord)
        {
            this.transformRecord = transformRecord;
        }
    }
}