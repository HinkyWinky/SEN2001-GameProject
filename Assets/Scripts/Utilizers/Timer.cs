using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Timer
    {
        private static readonly List<TimeRequest> TimeRequestList = new List<TimeRequest>();

        private readonly struct TimeRequest
        {
            public readonly string name;
            public readonly float startTime;

            public TimeRequest(string name, float startTime)
            {
                this.name = name;
                this.startTime = startTime;
            }
        }

        public static void StartTime(string name)
        {
            float startTime = Time.time;
            TimeRequest newTimeRequest = new TimeRequest(name, startTime);
            TimeRequestList.Add(newTimeRequest);
        }

        public static void EndTime(string name)
        {
            TimeRequest curTimeRequest = TimeRequestList.Find(timeRequest => timeRequest.name == name);
            if (TimeRequestList.Contains(curTimeRequest))
            {
                float endTime = Time.time;
                float elapsedTime = endTime - curTimeRequest.startTime;
                Debug.Log($"{curTimeRequest.name}: {elapsedTime}");
                TimeRequestList.Remove(curTimeRequest);
            }
        }
    }
}
