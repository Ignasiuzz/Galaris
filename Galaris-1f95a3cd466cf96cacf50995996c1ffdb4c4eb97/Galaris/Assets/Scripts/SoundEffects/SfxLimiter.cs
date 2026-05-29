using System.Collections.Generic;
using UnityEngine;

public static class SfxLimiter
{
    private static readonly Dictionary<string, float> LastPlayTimes = new Dictionary<string, float>();
    private static readonly Dictionary<string, Queue<float>> RecentPlayTimes = new Dictionary<string, Queue<float>>();

    public static bool TryPlay(AudioSource source, string key, float minInterval, int burstLimit, float burstWindow)
    {
        if (source == null || string.IsNullOrEmpty(key))
        {
            return false;
        }

        float now = Time.unscaledTime;

        if (LastPlayTimes.TryGetValue(key, out float lastPlayTime) && now - lastPlayTime < minInterval)
        {
            return false;
        }

        if (!RecentPlayTimes.TryGetValue(key, out Queue<float> timestamps))
        {
            timestamps = new Queue<float>();
            RecentPlayTimes[key] = timestamps;
        }

        while (timestamps.Count > 0 && now - timestamps.Peek() > burstWindow)
        {
            timestamps.Dequeue();
        }

        if (timestamps.Count >= burstLimit)
        {
            return false;
        }

        LastPlayTimes[key] = now;
        timestamps.Enqueue(now);
        source.Play();
        return true;
    }
}
