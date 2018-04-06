using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(mt_Destructible))]

public class mt_Missile : MonoBehaviour
{
    [Range(1, 100)]
    public float velocity = 10;
    [Range(0.5f, 100)]
    public float torque = 5;
    public float duration = 8;
    public bool selfDestruct = true;
    public float maxAltitude = 200;
    public float damage = 500;

    protected mt_MissileLauncher m_Launcher = null;
    mt_MissileLauncher Launcher
    {
        get
        {
            if (m_Launcher == null) m_Launcher = FindObjectOfType<mt_MissileLauncher>();
            return m_Launcher;
        }
    }

    protected Collider m_Collider = null;
    Collider Col
    {
        get
        {
            if (m_Collider == null) m_Collider = GetComponent<Collider>();
            return m_Collider;
        }
    }

    void Start()
    {
        Col.enabled = false;
    }

    float currentDuration = 0;
    void Update()
    {
        if (transform.localPosition.y > Launcher.spawnPoint.localPosition.y + (transform.localScale.y * 10)) Col.enabled = true;

        Vector3 direction = (Launcher.target.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up) * Quaternion.LookRotation(direction);
        if (transform.position.y >= maxAltitude / 1.25f) transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * torque);

        currentDuration += Time.deltaTime;
        transform.position = Vector3.Slerp(transform.position, CalculateBezierPoint(currentDuration / duration, Launcher.spawnPoint.position, Launcher.target.position, new Vector3(transform.position.x, maxAltitude * 3, transform.position.z)), Time.deltaTime * velocity);

        if (selfDestruct && transform.position.y >= maxAltitude)
        {
            Boom();
        }
    }

    void Boom()
    {
        GetComponent<mt_Destructible>().ProcessDamage(GetComponent<mt_Destructible>().currentHealth + 1);
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.transform == transform || collider.transform == transform.parent) return;

        if (collider.transform.GetComponent<mt_Destructible>())
        {
            collider.transform.GetComponent<mt_Destructible>().ProcessDamage(damage);
        }

        Boom();
    }

    void OntriggerEnter(Collision collision)
    {
        if (collision.transform.GetComponent<mt_Destructible>())
        {
            collision.transform.GetComponent<mt_Destructible>().ProcessDamage(damage);
        }
    }

    private Vector3 CalculateBezierPoint(float time, Vector3 startPosition, Vector3 endPosition, Vector3 controlPoint)
    {
        float f = 1 - time;
        float ff = f * f;
        Vector3 point = ff * startPosition;
        point += 2 * f * time * controlPoint;
        point += time * time * endPosition;
        return point;
    }

}