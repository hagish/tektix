using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AudioDb : ScriptableObject {
    [System.Serializable]
    public class Music
    {
        public string Name;
        public AudioClip Clip;
        public float Volume;
    }

    [System.Serializable]
    public class Sound
    {
        public string Name;
        public AudioClip Clip;
        public float Volume;
    }

    public List<Music> Musics;
    public List<Sound> Sounds;
}
