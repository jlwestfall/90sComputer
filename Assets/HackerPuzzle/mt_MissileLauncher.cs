using UnityEngine;
using System.Collections;

public class mt_MissileLauncher : MonoBehaviour
{
    public float ignitionDelay = 0.5f;
    public AnimationClip siloOpen;
    public AnimationClip siloClose;
    public GameObject missilePrefab;
    public Transform spawnPoint;
    public Transform target;
    public string targetTag;

    public bool lookatMissile = true;
    public bool canFire = false;

    protected Animation m_Animator = null;
    Animation siloAnimator
    {
        get
        {
            if (m_Animator == null) m_Animator = GetComponent<Animation>();
            return m_Animator;
        }
    }

    protected GameObject m_Player = null;
    GameObject Player
    {
        get
        {
            if (m_Player == null) m_Player = GameObject.FindWithTag("Player");
            return m_Player;
        }
    }

    protected mt_FPController m_Controller = null;
    mt_FPController FPController
    {
        get
        {
            if (m_Controller == null) m_Controller = FindObjectOfType<mt_FPController>();
            return m_Controller;
        }
    }

    void Start()
    {
        if (target == null && targetTag == "") return;
        if (canFire) Fire();
    }

    void Update()
    {
        if (lookatMissile && missile)
        {
            Quaternion aimRotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(missile.transform.position - Player.transform.position), Time.deltaTime * 5);

            Player.transform.rotation = Quaternion.Euler(new Vector2(aimRotation.eulerAngles.x, aimRotation.eulerAngles.y));
            FPController.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(new Vector3(0, aimRotation.eulerAngles.y, 0));
            FPController.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(new Vector3(aimRotation.eulerAngles.x, 0, 0));
        }

        if (targetTag == "") return;
        if (target == null)
        {
            FindClosestTarget();
            target = closestTarget;
        }
    }

    public void Fire()
    {
        StartCoroutine(FireOnce());
    }

    IEnumerator FireOnce()
    {
        if (siloAnimator && siloOpen)
        {
            ignitionDelay = siloOpen.length;
            siloAnimator.Play(siloOpen.name);
        }


        yield return new WaitForSeconds(ignitionDelay);

        canFire = true;

        FPController.freeze = true;

        Respawn();

        while (missile != null) yield return 0;

        canFire = false;

        if (siloAnimator && siloClose)
        {
            siloAnimator[siloClose.name].speed = -1;
            siloAnimator.Play(siloClose.name);
        }

        FPController.freeze = false;
    }

    [HideInInspector]
    public GameObject missile;
    void Respawn()
    {
        if (target == null || !canFire) return;

        missile = (GameObject)Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation);
        missile.transform.SetParent(spawnPoint);
        missile.name = missile.name.Replace("(Clone)", "");
    }

    Transform closestTarget;
    public Transform FindClosestTarget()
    {
        if (targetTag == null | targetTag == "") return null;
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);

        float distance = Mathf.Infinity;
        foreach (GameObject go in allTargets)
        {
            float curDistance = Vector3.Distance(go.transform.position, transform.position);
            if (curDistance < distance)
            {
                distance = curDistance;
                closestTarget = go.transform;
            }
        }
        return closestTarget;
    }

}