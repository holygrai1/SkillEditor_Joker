using System;
using System.Reflection;
using UnityEngine;

public static class EditorAudioUnility
{
    private static MethodInfo playClipMehthodInfo;
    private static MethodInfo stopAllClipMehthodInfo;
    static EditorAudioUnility()
    {
        Assembly editorAssembly = typeof(UnityEditor.AudioImporter).Assembly;
        Type utilClassType = editorAssembly.GetType("UnityEditor.AudioUtil");
        playClipMehthodInfo = utilClassType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null,
                                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
        stopAllClipMehthodInfo = utilClassType.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="start">0~1的播放进度</param>
    public static void PlayAudio(AudioClip clip, float start)
    {
        playClipMehthodInfo.Invoke(null, new object[] { clip, (int)(start * clip.frequency), false });
    }

    public static void StopAllAudios()
    {
        stopAllClipMehthodInfo.Invoke(null, null);
    }
}
