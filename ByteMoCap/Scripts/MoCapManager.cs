using ByteScript.MoCap.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ByteScript.MoCap
{
    // Replay controller limitation is to be able to record or play 1 group at a time.
    public class MoCapManager : MonoBehaviour
    {
        //EVENT DELEGATES
        public delegate void RecordingDelegate(Group group, State state, string customPrefix);
        public delegate void PlaybackDelegate(Group group, State state, string customPrefix);

        public enum State { Preload, Ongoing, Paused, Stopped };
        public enum Group { Defualt };

        //EVENTS

        /// <summary>
        /// Announces the time to any timeline objects so that they can sample their stored values at that time
        /// </summary>
        public static PlaybackDelegate playbackEvent;

        /// <summary>
        /// Announces to actors whether to record or not
        /// </summary>
        public static RecordingDelegate recordingEvent;

        [Header("Recording Settings")]
        public float keyframeInterval = 0.2f;
        public float maxTime = 60f;

        //PUBLIC VARIABLES
        [HideInInspector] public static MoCapManager instance; //instance of self

        private Dictionary<Tuple<Group, string>, MoCapTimeRecorder> moCapTimeRecorders =
            new Dictionary<Tuple<Group, string>, MoCapTimeRecorder>();
        private Dictionary<Tuple<Group, string>, MoCapTimePlayback> moCapTimePlaybacks =
            new Dictionary<Tuple<Group, string>, MoCapTimePlayback>();

        void Awake()
        {
            //create instance
            if (instance == null)
                instance = this;
        }

        public void startRecording(Group group = Group.Defualt, string customPrefix = "")
        {
            var key = new Tuple<Group, string>(group, customPrefix);
            if (!moCapTimeRecorders.ContainsKey(key))
            {
                var timeRecorder = new MoCapTimeRecorder(group, customPrefix);
                moCapTimeRecorders[key] = timeRecorder;
            }
            recordingEvent?.Invoke(group, State.Ongoing, customPrefix);
        }

        public void pauseRecording(Group group = Group.Defualt, string customPrefix = "")
        {
            recordingEvent?.Invoke(group, State.Paused, customPrefix);
        }

        public void stopRecording(Group group = Group.Defualt, string customPrefix = "")
        {
            recordingEvent?.Invoke(group, State.Stopped, customPrefix);
            var key = new Tuple<Group, string>(group, customPrefix);
            if (moCapTimeRecorders.ContainsKey(key))
            {
                moCapTimeRecorders.Remove(key);
            }
        }

        public void startPlayback(Group group = Group.Defualt, string customPrefix = "")
        {
            var key = new Tuple<Group, string>(group, customPrefix);
            if (!moCapTimePlaybacks.ContainsKey(key))
            {
                var timePlayback = new MoCapTimePlayback(group, customPrefix);
                timePlayback.onPlaybackFinishedEvent += OnPlaybackFinished; ;
                moCapTimePlaybacks[key] = timePlayback;
            }
            playbackEvent?.Invoke(group, State.Ongoing, customPrefix);
        }

        public void pausePlayback(Group group = Group.Defualt, string customPrefix = "")
        {
            playbackEvent?.Invoke(group, State.Paused, customPrefix);
        }

        public void stopPlayback(Group group = Group.Defualt, string customPrefix = "")
        {
            playbackEvent?.Invoke(group, State.Stopped, customPrefix);
            var key = new Tuple<Group, string>(group, customPrefix);
            if (moCapTimePlaybacks.ContainsKey(key))
            {
                moCapTimePlaybacks.Remove(key);
            }
        }

        internal static string GenerateName(Group group, string customPrefix, string objectName)
        {
            string prefix = "ByteMoCap_" + group;
            if (string.IsNullOrEmpty(customPrefix))
            {
                return prefix + "_" + objectName;
            }

            return prefix + "_" + customPrefix + "_" + objectName;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            List<MoCapTimeRecorder> recorders = new List<MoCapTimeRecorder>(moCapTimeRecorders.Values);
            foreach (MoCapTimeRecorder recorder in recorders)
            {
                recorder.UpdateTime(deltaTime);
            }
            List<MoCapTimePlayback> playbacks = new List<MoCapTimePlayback>(moCapTimePlaybacks.Values);
            foreach (MoCapTimePlayback playback in playbacks)
            {
                playback.UpdateTime(deltaTime);
            }
        }

        private void OnPlaybackFinished(Group group, string customPrefix)
        {
            var key = new Tuple<Group, string>(group, customPrefix);
            if (moCapTimePlaybacks.ContainsKey(key))
            {
                moCapTimePlaybacks.Remove(key);
            }

            playbackEvent?.Invoke(group, State.Stopped, customPrefix);
            Debug.Log(group + " " + customPrefix + " is Done Playing");
        }
    }

    [System.Serializable]
    public struct ReplayAnimation
    {
        // This should be false if we failed to load it from file.
        public bool isValid;

        public bool recordPosition;
        public bool recordRotation;
        public bool recordExtra;

        public KeyframeVector3 positionKey;
        public KeyframeQuaternion rotationKey;
        public KeyframeVector3 extraKey;
    }
}

