using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    public AudioSource WalkingSoundEffect;
    public AudioClip WalkingClip;
    
    void Start()
    {
        WalkingSoundEffect = GetComponent<AudioSource>();

    }

    void Update()
    {
        playSoundOnWalk();

    }
    public void walking()
    {
        WalkingSoundEffect.clip = WalkingClip;
        WalkingSoundEffect.Play();
    }
    public void playSoundOnWalk()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            walking();

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            walking();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            walking();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            walking();
        }

        //När spelaren slutar gå slutar ljudet
        if (Input.GetKeyUp(KeyCode.W))
        {
            WalkingSoundEffect.Stop();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            WalkingSoundEffect.Stop();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            WalkingSoundEffect.Stop();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            WalkingSoundEffect.Stop();
        }
    }
}
