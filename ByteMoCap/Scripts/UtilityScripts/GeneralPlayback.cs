using UnityEngine;

namespace ByteScript.MoCap.Util
{
    public class GeneralPlayback : MonoBehaviour
    {
        //PUBLIC
        [Header("Playback Settings")]
        public MoCapManager.Group group = MoCapManager.Group.Defualt;

        protected string customPrefix = "";
        protected string recordName = "";
        protected bool isValid = false;

        private MoCapManager.State playbackState = MoCapManager.State.Stopped;
        private float time;

        void Awake()
        {
            //add to events
            MoCapManager.playbackEvent += OnPlaybackEvent;
            recordName = name;
        }
        void OnDestroy()
        {
            //remove from events when object is destroyed
            MoCapManager.playbackEvent -= OnPlaybackEvent;
        }

        void OnPlaybackEvent(MoCapManager.Group group, MoCapManager.State state,
            string customPrefix)
        {
            if (this.group != group)
            {
                Debug.Log(
                    "Found recording group " + this.group +
                    ", not responding to " + group + " call");
                return;
            }

            this.customPrefix = customPrefix;

            switch (state)
            {
                case MoCapManager.State.Preload:
                    {
                        LoadFromDisk();
                    }
                    break;
                case MoCapManager.State.Ongoing:
                    if (playbackState == MoCapManager.State.Stopped || playbackState == MoCapManager.State.Preload)
                    {
                        Debug.Log("Starting to playback");
                        time = 0;
                        LoadFromDisk();
                    }
                    break;
                case MoCapManager.State.Paused:
                    // Only valid when recording is ongoing.
                    if (playbackState != MoCapManager.State.Ongoing)
                    {
                        Debug.LogWarning("Playback state is " + playbackState);
                        return;
                    }
                    break;
                case MoCapManager.State.Stopped:
                    // Only invalid when no recording is going on.
                    if (playbackState == MoCapManager.State.Stopped)
                    {
                        Debug.LogError("No ongoing playback");
                        return;
                    }
                    Debug.Log("playback has stopped");
                    break;
            }
            playbackState = state;
        }
        protected virtual void LoadFromDiskIfNeeded()
        {
            if (playbackState == MoCapManager.State.Preload)
            {
                return;
            }

            LoadFromDisk();
        }

        protected virtual void LoadFromDisk()
        {
            Debug.LogError("LoadFromDisk function has not been implemented");
        }
        protected virtual void Playback(float time)
        {
            Debug.LogError("LoadFromDisk function has not been implemented");
        }

        private void Update()
        {
            if (!isValid)
            {
                return;
            }

            // Keep time updated;
            if (playbackState == MoCapManager.State.Ongoing)
            {
                Playback(time);
                time += Time.deltaTime;
            }
        }
    }
}