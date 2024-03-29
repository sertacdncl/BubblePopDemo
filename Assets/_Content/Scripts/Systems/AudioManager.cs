using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private List<AudioItem> audioItems;
	
	public void PlaySoundOnce(string tag, bool randomPitch = false)
	{
		var audioItem = audioItems.Find(x => x.tag == tag);
		if (audioItem == null)
		{
			Debug.LogError("AudioItem with tag " + tag + " not found");
			return;
		}
		
		if(randomPitch)
			audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
		else
			audioSource.pitch = audioItem.pitch;
		audioSource.PlayOneShot(audioItem.audioClip, audioItem.volume);
	}
}


[Serializable]
public class AudioItem
{
	public string tag;
	public AudioClip audioClip;
	public float volume = 1f;
	public float pitch = 1f;
	public float delay = 0f;
	public bool loop = false;
}