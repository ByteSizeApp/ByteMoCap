using ByteScript.MoCap.Util;
using UnityEngine;
using UnityEngine.Events;

namespace ByteScript.MoCap
{
    [System.Serializable]
    public class TriggerChanged : UnityEvent<float> { }
    public class ControllerPlayback : TransformPlayback
    {
        public TriggerChanged onTriggerChanged;

        // Keeps track of the state, so we dont fire a bunch of event when the controller is not pressed.
        private bool isTriggerDown;

        private KeyframeFloat triggerKey = new KeyframeFloat();

        protected override void LoadFromDisk()
        {
            ControllerRecord record =
                FileManager.DeSerializeObject<ControllerRecord>(group, customPrefix, recordName);
            if (setTransformPlayback(record.transformRecord))
            {
                triggerKey = record.triggerKey;
            }
        }
        protected override void Playback(float time)
        {
            base.Playback(time);
            float? triggerValue = triggerKey.SampleCurves(time);
            if (triggerValue.HasValue)
            {
                if (triggerValue.Value > 0.1)
                {
                    isTriggerDown = true;
                    onTriggerChanged.Invoke(triggerValue.Value);
                }
                else if (isTriggerDown)
                {
                    // Fire trigger value 0 only once after and reset trigger down state.
                    isTriggerDown = false;
                    onTriggerChanged.Invoke(0);
                }
            }
        }
    }
}