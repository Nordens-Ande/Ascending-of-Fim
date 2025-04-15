using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Shoot : MonoBehaviour
{
    int rayLength;
    [SerializeField] GameObject bulletOrigin; //placera child objectet som vapnet ska ha här, skottets/rayens origin.
    [SerializeField] GameObject BulletTrail; // Placera Bullet trail prefab här. 

    void CreateBulletTrail(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 offsetStart = start + direction * 0.2f;
        //Debug.DrawLine(offsetStart, end, Color.green, 2f); // för testing

        GameObject trail = Instantiate(BulletTrail, offsetStart, Quaternion.identity);
        LineRenderer line = trail.GetComponent<LineRenderer>();
        //line.SetPosition(0, start);
        //line.SetPosition(1, end);
        //Destroy(trail, 0.15f); // Förstör trail efter ett tag
        StartCoroutine(AnimateBulletTrail(line, offsetStart, end));

    }

    IEnumerator AnimateBulletTrail(LineRenderer line, Vector3 start, Vector3 end)
    {
        float travelDuration = 0.05f; // Hur lång tid det tar för trailen att nå objektet
        float fadeDuration = 0.1f; // Hur lång tid det tar för trailen att fadea
        float timer = 0f;

        line.SetPosition(0, start);
        line.SetPosition(1, start); // Start both points at origin

        while (timer < travelDuration)
        {
            timer += Time.deltaTime;
            Vector3 current = Vector3.Lerp(start, end, timer / travelDuration);
            line.SetPosition(1, current);
            yield return null;
        }

        line.SetPosition(1, end);

        // Fade
        timer = 0f;
        Gradient gradient = new Gradient();
        Color startColor = line.startColor;
        Color endColor = line.endColor;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = 1 - (timer / fadeDuration); // Gå mot 0, "fadea ut"

            Color fadedColor = new Color(startColor.r, startColor.g, startColor.b, t);
            line.startColor = fadedColor;
            line.endColor = fadedColor;

            yield return null;
        }

        Destroy(line.gameObject);
    }

    void Start()
    {
        rayLength = 10000;
    }

    Vector3 GetDirection()
    {
        Vector3 direction = bulletOrigin.transform.forward;
        direction.y = 0;
        return direction;
    }

    Ray BuildRay()
    {
        Ray ray = new Ray(bulletOrigin.transform.position, GetDirection());
        Debug.DrawRay(ray.origin, ray.direction * rayLength);
        return ray;
    }

    public RaycastHit ShootRay()
    {
        Ray ray = BuildRay();
        RaycastHit hit;

        Vector3 endPoint = ray.origin + ray.direction * rayLength;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            endPoint = hit.point;
        }
        CreateBulletTrail(ray.origin, endPoint);
        return hit;
    }

}
