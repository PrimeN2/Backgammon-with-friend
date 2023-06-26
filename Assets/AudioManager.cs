using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

	[SerializeField] private AudioSource _audioSource;

	private void Awake()
	{
		Instance = this;
	}

	public void MakeSound(AudioClip clip)
	{
		_audioSource.PlayOneShot(clip);
	}
}
