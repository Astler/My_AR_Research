// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Niantic.ARVoyage
{
    /// <summary>
    /// A list of audio clips for a category
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClipsDescription", menuName = "ScriptableObjects/AudioClipList")]
    public class AudioClipList : ScriptableObject
    {
        public string category;
        public List<AudioClip> clips;
    }
}
