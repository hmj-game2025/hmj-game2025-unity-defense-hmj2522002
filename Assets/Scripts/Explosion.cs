using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	[SerializeField] List<AudioClip> m_sounds;

    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioClip clip in m_sounds)
		{
			if (SeCoolDown.Instance.CanPlay(clip))
			{
				AudioSource.PlayClipAtPoint(clip, transform.position);
			}
		}
    }
}
