using UnityEngine;

public class EnemyVoicelines : MonoBehaviour
{

    public AudioSource currentSoundEffect;
    public AudioClip[] EnemySearchVoicelines;
    public AudioClip[] EnemyIdleVoicelines;
    public AudioClip[] EnemyAttackVoicelines;
    private int RandomTime;
    bool isEnemyIdle;
    bool isEnemySearching;
    bool isEnemyAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSoundEffect = GetComponent<AudioSource>();
        isEnemyIdle = false;
        isEnemySearching = false;
        isEnemyAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        RandomTime = Random.Range(0, 15000);
        EnemyIdleVoice();
        EnemySearchVoice();
        EnemyAttackVoice();
        
        
    }

    //Dessa metoderna h�mtas av scripts som s�tter ig�ng de olika stadierna i enemy
    //D�r enemyscriptsen s�tter de olika boolsen till true n�r det beh�vs
    public void SetIdleEnemyVoiceline(bool a) 
    { 
        isEnemyIdle = a;
    }
    public void SetSearchEnemyVoiceline(bool b)
    {
        isEnemySearching = b;
    }
    public void SetAttackEnemyVoiceline(bool c)
    {
        isEnemyAttacking = c;
    }

    public void SetEnemyVoicelines(int which) 
    {
        if (which == 1) //Idle voicelines
        {
            //SetIdleEnemyVoiceline(true);
            //SetAttackEnemyVoiceline(false);
            //SetSearchEnemyVoiceline(false);

            isEnemyIdle = true;
            isEnemySearching = false;
            isEnemyAttacking = false;

        }
        else if (which == 2) //searchvoicelines
        { 
            //SetIdleEnemyVoiceline(false);
            //SetAttackEnemyVoiceline(false);
            //SetSearchEnemyVoiceline(true);
            isEnemyIdle = false;
            isEnemySearching = true;
            isEnemyAttacking = false;

        }
        else if(which == 3) //Attck voicelines
        {
            //SetIdleEnemyVoiceline(false);
            //SetAttackEnemyVoiceline(true);
            //SetSearchEnemyVoiceline(false);
            isEnemyIdle = false;
            isEnemySearching = false;
            isEnemyAttacking = true;

        }
    
    }


    //Metoderna f�r att de olika voicelinesen ska kunna spelas upp
    //N�r deras respektive bools s�tts som true i enemy scriptet kommer randomtime g�ra att det 
    //beh�vs vara en specifik siffra f�r att s�ga en voiceline
    //Detta kommer g�ra att n�r voiclinesen s�gs �r p� ett spann inom 60 sekunder randomly
    void EnemyIdleVoice() 
    {
        if (isEnemyIdle == true && RandomTime == 1) 
        {
            PlayIdleVoiceline();
        
        }
    
    }
    void EnemySearchVoice()
    {
        if (isEnemySearching == true && RandomTime == 1)
        {
            PlaySearchVoiceline();

        }

    }
    void EnemyAttackVoice()
    {
        if (isEnemyAttacking == true && RandomTime == 1)
        {
            PlayAttackVoiceline();
        }

    }


    //De olika voicelines som kan spelas randomiserat
    void PlayIdleVoiceline() 
    { 
        currentSoundEffect.clip = EnemyIdleVoicelines[Random.Range(0, EnemyIdleVoicelines.Length)];
        currentSoundEffect.PlayOneShot(currentSoundEffect.clip, 2);
    
    }
    void PlaySearchVoiceline()
    {
        currentSoundEffect.clip = EnemySearchVoicelines[Random.Range(0, EnemySearchVoicelines.Length)];
        currentSoundEffect.PlayOneShot(currentSoundEffect.clip, 2);

    }
    void PlayAttackVoiceline()
    {
        currentSoundEffect.clip = EnemyAttackVoicelines[Random.Range(0, EnemyAttackVoicelines.Length)];
        currentSoundEffect.PlayOneShot(currentSoundEffect.clip, 2);

    }

}
