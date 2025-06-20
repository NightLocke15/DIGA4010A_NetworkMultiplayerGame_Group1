using Mirror;
using UnityEngine;

public class PlayerPuckSound : NetworkBehaviour
{
    [SerializeField] private AudioClip pull;
    [SerializeField] private AudioClip slide;
    [SerializeField] private AudioClip wallHit;
    private AudioSource audioSource;
    private PuckScript puckScript;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        puckScript = GetComponent<PuckScript>();
    }

    [ClientCallback]
    private void OnMouseDown()
    {
        audioSource.clip = pull;
        audioSource.Play();
    }

    [ClientCallback]
    private void OnMouseUp()
    {
        PlaySlideSound();
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall" || collision.collider.tag == "Tower" || collision.collider.tag == "Floor")
        {
            PlayWallHitSound();
        }
    }

    [Command(requiresAuthority = false)]
    public void PlaySlideSound()
    {
        SlideSound();
    }

    [Command(requiresAuthority = false)]    
    public void PlayWallHitSound()
    {
        WallHitSound();
    }

    [ClientRpc]
    public void SlideSound()
    {
        audioSource.clip = slide;
        audioSource.Play();
    }

    [ClientRpc]
    public void WallHitSound()
    {
        audioSource.clip = wallHit;
        audioSource.Play();
    }
}
