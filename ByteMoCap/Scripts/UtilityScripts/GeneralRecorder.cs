using System.Collections;
using UnityEngine;

namespace ByteScript.MoCap.Util
{
    public class GeneralRecorder : MonoBehaviour
    {
        //PUBLIC
        [Header("Recording Settings")]
        public MoCapManager.Group group = MoCapManager.Group.Defualt;

        protected string customPrefix = "";
        protected string recordName = "";

        //PRIVATE
        // keyframeInterval set at global level.
        private float keyframeInterval = 0.2f;
        private float maxTime = 30f;

        private MoCapManager.State recordingState = MoCapManager.State.Stopped;
        private float time;

        void Awake()
        {
            //add to events
            MoCapManager.recordingEvent += OnRecordingEvent;
            recordName = name;
        }
        void OnDestroy()
        {
            //remove from events when object is destroyed
            MoCapManager.recordingEvent -= OnRecordingEvent;
        }

        void OnRecordingEvent(MoCapManager.Group group, MoCapManager.State state, string customPrefix)
        {
            if (this.group != group)
            {
                Debug.Log("Found recording group " + this.group + ", not responding to " + group + " call");
                return;
            }

            this.customPrefix = customPrefix;
            Debug.Log("Found recording group " + this.group + state);

            switch (state)
            {
                case MoCapManager.State.Ongoing:
                    // If stopped, start.
                    if (recordingState == MoCapManager.State.Stopped)
                    {
                        Debug.Log("Starting recording");
                        // This is needed here as we start recording coroutine right here. Waiting until it leaves the switch will be too late.
                        recordingState = state;
                        maxTime = MoCapManager.instance.maxTime;
                        this.keyframeInterval = MoCapManager.instance.keyframeInterval;
                        StartCoroutine("WriteCurves");
                    }
                    // If paused, resume. 
                    break;
                case MoCapManager.State.Paused:
                    // Only valid when recording is ongoing.
                    if (recordingState != MoCapManager.State.Ongoing)
                    {
                        Debug.LogWarning("Recording state is " + recordingState);
                        return;
                    }
                    break;
                case MoCapManager.State.Stopped:
                    // Only invalid when no recording is going on.
                    if (recordingState == MoCapManager.State.Stopped)
                    {
                        Debug.LogError("No ongoing record");
                        return;
                    }
                    break;
            }
            recordingState = state;
        }

        IEnumerator WriteCurves()
        {
            //reset variables
            time = 0;

            while (recordingState == MoCapManager.State.Ongoing || recordingState == MoCapManager.State.Paused)
            {
                if (recordingState == MoCapManager.State.Paused)
                {
                    yield return new WaitForSeconds(keyframeInterval);
                }

                if (time > maxTime)
                {
                    Debug.LogError(name + " is recording overtime");
                    yield return new WaitForSeconds(keyframeInterval);
                }

                AddKeyFrame(time);
                yield return new WaitForSeconds(keyframeInterval);
            }

            SaveToDisk();

            yield return null;
        }

        protected virtual void AddKeyFrame(float time)
        {
            Debug.LogError("AddKeyFrame function has not been implemented");
        }

        protected virtual void SaveToDisk()
        {
            Debug.LogError("SaveToDisk function has not been implemented");
        }

        private void Update()
        {
            // Keep time updated;
            if (recordingState == MoCapManager.State.Ongoing)
            {
                time += Time.deltaTime;
            }
        }
    }
}