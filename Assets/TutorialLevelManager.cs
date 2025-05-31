using System.Collections;
using UnityEngine;

public class TutorialLevelManager : MonoBehaviour
{
    int timeBetweenAnnouncement = 5;
    int timePerAnnouncement = 4000;
    [SerializeField] announcement a;
    
    void Start()
    {
        StartCoroutine(StartEnumerators());
    }

    IEnumerator StartEnumerators()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PickUpWeapon(timeBetweenAnnouncement));
    }

    IEnumerator PickUpWeapon(int time)
    {
        a.SetAnnouncementText("Press E to pick up the Weapon", timePerAnnouncement);
        yield return new WaitForSeconds(time);
        StartCoroutine(KillEnemy(timeBetweenAnnouncement));
    }

    IEnumerator KillEnemy(int time)
    {
        a.SetAnnouncementText("Use Mouse1 to shoot the Enemy", timePerAnnouncement);
        yield return new WaitForSeconds(time);
        StartCoroutine(PickUpKeycard(timeBetweenAnnouncement));
    }

    IEnumerator PickUpKeycard(int time)
    {
        a.SetAnnouncementText("Find and press E to pick up the Keycard", timePerAnnouncement);
        yield return new WaitForSeconds(time);
        StartCoroutine(ElevatorInteract(timeBetweenAnnouncement));
    }

    IEnumerator ElevatorInteract(int time)
    {
        a.SetAnnouncementText("Press E on the elevator to finish tutorial", timePerAnnouncement);
        yield return new WaitForSeconds(time);
    }
}
