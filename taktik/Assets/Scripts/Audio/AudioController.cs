using UnityEngine;
using System.Collections;

public class AudioController : UKUnitySingletonManuallyCreated<AudioController> {
    public AudioDb audioDb;
    public string nextMusic;

    private static UKObjectRecycler recycler;
    private static GameObject audioContainer;
    private static AudioSource currentMusicSource;

	// Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        if (audioContainer == null)
        {
            audioContainer = new GameObject("AudioContainer");
            recycler = audioContainer.AddComponent<UKObjectRecycler>();
            GameObject.DontDestroyOnLoad(audioContainer);
        }

        UKMessenger.AddListener("efx.click", gameObject, () =>
        {
            PlaySound("click");
        });

        UKMessenger.AddListener("COLLECTED_MOON", gameObject, () =>
        {
            PlaySound("coin");
        });

        StartCoroutine(CoMusic());
	}

    private IEnumerator CoMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / 15f);

            var isEnabled = true;

            // mute but playing?
            if (!isEnabled && currentMusicSource != null && currentMusicSource.isPlaying)
            {
                currentMusicSource.Stop();
            }

            if (isEnabled && !string.IsNullOrEmpty(nextMusic))
            {
                var found = false;

                foreach (var music in audioDb.Musics)
                {
                    // found
                    if (music.Name == nextMusic)
                    {
                        if (currentMusicSource != null)
                        {
                            // already playing?
                            if (currentMusicSource.clip == music.Clip)
                            {
                                found = true;
                                break;
                            }

                            currentMusicSource.Stop();
                        }
                        else
                        {
                            var o = new GameObject("music");
                            o.transform.parent = audioContainer.transform;
                            currentMusicSource = o.AddComponent<AudioSource>();
                            GameObject.DontDestroyOnLoad(o);
                        }

                        // fade in & out
                        var fadeTime = 1f;
                        yield return StartCoroutine(Fade(currentMusicSource, 0f, fadeTime));

                        currentMusicSource.Stop();
                        currentMusicSource.clip = music.Clip;
                        currentMusicSource.volume = 0f;
                        currentMusicSource.loop = true;
                        currentMusicSource.Play();

                        yield return StartCoroutine(Fade(currentMusicSource, music.Volume, fadeTime));

                        nextMusic = null;
                        found = true;
                        break;
                    }
                }

                if (!found) Debug.LogError(string.Format("music {0} not found!", nextMusic));
            }
        }
    }

    private IEnumerator Fade(AudioSource source, float to, float time)
    {
        var start = source.volume;
        var startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + time)
        {
            source.volume = UKMathHelper.MapIntoRange(Time.realtimeSinceStartup, startTime, startTime + time, start, to);
            yield return null;
        }

        source.volume = to;
    }

    private AudioSource SpawnSource(AudioClip clip)
    {
        GameObject o = recycler.GetObject("audio." + clip.name, () => {
            GameObject s = new GameObject(clip.name);
            s.AddComponent<AudioSource>().clip = clip;
            GameObject.DontDestroyOnLoad(s);
            s.transform.parent = audioContainer.transform;
            return s;
        });

        return o.GetComponent<AudioSource>();
    }

    public void PlaySound(string name)
    {
        foreach (var sound in audioDb.Sounds)
        {
            if (sound.Name == name)
            {
                var s = SpawnSource(sound.Clip);
                s.Play();
                s.volume = sound.Volume;
                UKObjectRecycler.DepositObject(s.gameObject, sound.Clip.length + 0.5f);
                return;
            }
        }

        Debug.LogError(string.Format("sound {0} not found!", name));
    }

    public void PlayMusic(string name)
    {
        nextMusic = name;
    }
}
