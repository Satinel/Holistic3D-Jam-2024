using UnityEngine;

public class SetPriceAudio : MonoBehaviour
{
    [SerializeField] AudioClip _increasePriceClip, _decreasePriceClip;

    [SerializeField] AudioSource _audioSource;

    public void PlayIncreaseClip()
    {
        _audioSource.PlayOneShot(_increasePriceClip);
    }

    public void PlayDecreaseClip()
    {
        _audioSource.PlayOneShot(_decreasePriceClip);
    }
}
