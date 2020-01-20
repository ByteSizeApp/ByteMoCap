namespace ByteScript.MoCap.Util
{
    public delegate void OnPlaybackFinishedEvent(MoCapManager.Group group, string customPrefix);
    public class MoCapTimePlayback
    {
        public event OnPlaybackFinishedEvent onPlaybackFinishedEvent;
        private MoCapManager.Group group;
        private string customPrefix;
        private float durationLeft = 0;
        private MoCapManager.State playbackState;

        public MoCapTimePlayback(MoCapManager.Group group, string customPrefix)
        {
            this.group = group;
            this.customPrefix = customPrefix;
            this.playbackState = MoCapManager.State.Ongoing;
            MoCapManager.playbackEvent += OnPlaybackEvent;
            durationLeft = FileManager.DeSerializeObject<float>(group, customPrefix, "time");
        }

        public void UpdateTime(float deltaTime)
        {
            if (playbackState == MoCapManager.State.Ongoing)
            {
                durationLeft -= deltaTime;

                if (durationLeft < 0)
                {
                    playbackState = MoCapManager.State.Stopped;
                    onPlaybackFinishedEvent?.Invoke(group, customPrefix);
                }
            }
        }

        private void OnPlaybackEvent(MoCapManager.Group group, MoCapManager.State state, string customPrefix)
        {
            // Make sure this is the correct group and prefix.
            if (this.group != group || !string.Equals(this.customPrefix, customPrefix))
            {
                return;
            }

            if (state == MoCapManager.State.Stopped && playbackState != MoCapManager.State.Stopped)
            {
                onPlaybackFinishedEvent?.Invoke(group, customPrefix);
            }

            playbackState = state;
        }
    }
}