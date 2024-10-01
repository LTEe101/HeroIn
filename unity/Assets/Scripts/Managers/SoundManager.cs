using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    Dictionary<string, float> _audioVolumes = new Dictionary<string, float>(); // 음량을 저장할 딕셔너리 추가
    private bool _isMuted = false;
    private float _masterVolume = 1.0f; // 전체 볼륨 값을 저장할 변수
    // MP3 Player   -> AudioSource
    // MP3 음원     -> AudioClip
    // 관객(귀)     -> AudioListener

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
        AudioListener.volume = _masterVolume;

    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float volume = 1.0f, float pitch = 1.0f)
    {
        // 특정 AudioClip의 음량을 설정
        SetAudioVolume(path, volume, type);

        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            if (audioSource.isPlaying && audioSource.clip == audioClip)
                return;

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = GetAudioVolume(audioClip.name, type); // 음량 설정
            audioSource.mute = _isMuted;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.volume = GetAudioVolume(audioClip.name, type); // 음량 설정
            audioSource.mute = _isMuted;
            audioSource.PlayOneShot(audioClip);
        }
    }

    // 음량 설정 메서드
    private void SetAudioVolume(string clipName, float volume, Define.Sound type)
    {
        string soundTypeName = System.Enum.GetName(typeof(Define.Sound), type);
        if (_audioVolumes.ContainsKey(soundTypeName))
        {
            _audioVolumes[soundTypeName] = volume;
        }
        else
        {
            _audioVolumes.Add(soundTypeName, volume); // 추가된 부분: 처음 음량이 설정될 때 딕셔너리에 값 추가
        }
    }

    // 음량 가져오는 메서드
    private float GetAudioVolume(string clipName, Define.Sound type)
    {
        string soundTypeName = System.Enum.GetName(typeof(Define.Sound), type);
        return _audioVolumes.ContainsKey(soundTypeName) ? _audioVolumes[soundTypeName] : 1.0f; // 기본값 1.0f 반환
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }
    public void SetVolume(float volume)
    {
        _masterVolume = volume;
        AudioListener.volume = _masterVolume; // AudioListener의 볼륨을 조정
    }

    public float GetVolume()
    {
        return _masterVolume;
    }
    public void ToggleMute()
    {
        _isMuted = !_isMuted;

        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource != null)
            {
                audioSource.mute = _isMuted; // 모든 AudioSource의 음소거 상태를 갱신
            }
        }

        Debug.Log($"Sound muted: {_isMuted}");
    }

    public bool IsMuted()
    {
        return _isMuted;
    }
    public void StopBGM()
    {
        _audioSources[(int)Define.Sound.Bgm].Stop();
    }

}
