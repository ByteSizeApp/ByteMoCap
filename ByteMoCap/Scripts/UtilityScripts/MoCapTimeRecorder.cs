namespace ByteScript.MoCap.Util
{
    public class MoCapTimeRecorder
    {
        private MoCapManager.Group group;
        private string customPrefix;
        private float elaspedTime = 0;
        private MoCapManager.State recordingState;

        public MoCapTimeRecorder(MoCapManager.Group group, string customPrefix)
        {
            this.recordingState = MoCapManager.State.Ongoing;
            this.group = group;
            this.customPrefix = customPrefix;
            MoCapManager.recordingEvent += OnRecordingEvent;

        }
        public void UpdateTime(float deltaTime)
        {
            if (recordingState == MoCapManager.State.Ongoing)
                elaspedTime += deltaTime;
        }

        private void OnRecordingEvent(MoCapManager.Group group, MoCapManager.State state, string customPrefix)
        {
            // Make sure this is the correct group and prefix.
            if (this.group != group || !string.Equals(this.customPrefix, customPrefix))
            {
                return;
            }

            if (state == MoCapManager.State.Stopped && recordingState != MoCapManager.State.Stopped)
            {
                RecordTime();
            }

            recordingState = state;
        }

        private void RecordTime()
        {
            string mixedPrefix =
                string.IsNullOrEmpty(customPrefix) ?
                group.ToString() :
                group.ToString() + "_" + customPrefix;

            FileManager.SerializeObject<float>(
                elaspedTime, group, customPrefix, "time");
        }
    }
}