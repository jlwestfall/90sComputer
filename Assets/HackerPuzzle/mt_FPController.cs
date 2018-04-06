using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]

public class mt_FPController : MonoBehaviour
{
    public float m_WalkSpeed = 7;
    public float m_RunSpeed = 20;
    public float m_JumpSpeed = 10;
    public float m_CrouchSpeed = 0.2f;
    [Range(0f, 1f)]
    public float m_RunstepLenghten = 0.7f;
    public float m_Gravity = 10;
    public bool m_UseFovKick = true;
    public bool m_UseHeadBob = true;
    public float m_StepInterval = 5;
    public AudioClip[] m_FootstepSounds;
    public AudioClip m_JumpSound;
    public AudioClip m_LandSound;

    [SerializeField]
    public MouseLook m_MouseLook;
    [SerializeField]
    public FOVKick m_FovKick;
    [SerializeField]
    public HeadBob m_HeadBob;
    [SerializeField]
    public JumpBob m_JumpBob;

    [HideInInspector]
    public bool freeze = false;
    [HideInInspector]
    public bool m_IsWalking = true;
    protected bool m_Jump = false;
    protected bool m_Crouch = false;
    protected bool m_PreviouslyGrounded;
    [HideInInspector]
    public bool m_Jumping = false;
    [HideInInspector]
    public bool m_Crouching = false;
    float m_CrouchHeight;
    protected float m_YRotation;
    protected float m_StepCycle;
    protected float m_NextStep;

    protected Camera m_Camera;
    protected Vector2 m_Input;
    protected Vector3 m_MoveDir = Vector3.zero;
    protected CharacterController m_CharacterController;
    protected CollisionFlags m_CollisionFlags;
    protected Vector3 m_OriginalCameraPosition;
    protected AudioSource m_AudioSource;

    protected mt_Hacking m_Hacking = null;
   public mt_Hacking Hacking
    {
        get
        {
            if (m_Hacking == null) m_Hacking = FindObjectOfType<mt_Hacking>();
            return m_Hacking;
        }
    }

    protected void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_CrouchHeight = m_CharacterController.height / 2;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_AudioSource = GetComponent<AudioSource>();
        m_MouseLook.Init(transform, m_Camera.transform);

#if !UNITY_ANDROID && !UNITY_IPHONE
        Hacking.HideCursor = true;
#endif
    }

    protected virtual void Update()
    {
        if (freeze || Time.timeScale == 0) return;

        RotateView();

        GetInput();

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoJumpBob());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    [HideInInspector]
    public float speed;
    protected virtual void FixedUpdate()
    {
        if (freeze || Time.timeScale == 0) return;

        GetInput(out speed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;

        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_Gravity;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;
            }

            if (m_Crouching)
            {
                m_MoveDir.x = desiredMove.x * speed * m_CrouchSpeed;
                m_MoveDir.z = desiredMove.z * speed * m_CrouchSpeed;
            }

        }
        else
        {
            m_MoveDir += Physics.gravity * 2 * Time.fixedDeltaTime;
        }
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

        ProgressStepCycle(speed);

        UpdateCameraPosition(speed);
    }

    protected void StartCrouch()
    {
        m_CharacterController.height -= m_CrouchHeight;
        m_CharacterController.center -= new Vector3(0, -m_CrouchHeight / 2, 0);
        m_Crouching = true;
    }

    protected void StopCrouch()
    {
        m_CharacterController.height += m_CrouchHeight;
        m_CharacterController.center += new Vector3(0, -m_CrouchHeight / 2, 0);
        m_Crouching = false;
    }

    protected void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.pitch = Time.timeScale;
        m_AudioSource.Play();
    }

    protected void PlayLandingSound()
    {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }

    protected void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;
        PlayFootStepAudio();
    }

    protected void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.pitch = Time.timeScale;
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    protected void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob) return;

        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }

    protected virtual void GetInput()
    {
        if (!m_Jump && !m_Crouching) m_Jump = Input.GetButtonDown("Jump");

        m_Crouch = Input.GetButton("Crouch");
        if (m_Crouch && !m_Crouching) StartCrouch();
        else if (!m_Crouch && m_Crouching) StopCrouch();
    }

    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;
    protected virtual void GetInput(out float speed)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        bool waswalking = m_IsWalking;

        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        if (m_Input.sqrMagnitude > 1) m_Input.Normalize();

        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }

    protected void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public float smoothTime = 5f;

        [HideInInspector]
        public float yRot;
        [HideInInspector]
        public float xRot;

        [HideInInspector]
        public Quaternion m_CharacterTargetRot;
        [HideInInspector]
        public Quaternion m_CameraTargetRot;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = Quaternion.Euler(Vector3.zero);
        }

        public virtual void LookRotation(Transform character, Transform camera)
        {
            yRot = Input.GetAxis("Mouse X") * XSensitivity * 0.1f;
            xRot = Input.GetAxis("Mouse Y") * YSensitivity * 0.1f;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smoothTime > 0)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }

    [Serializable]
    public class FOVKick
    {
        public float FOVIncrease = 12f;
        public float TimeToIncrease = 1f;
        public float TimeToDecrease = 1f;
        float originalFov;
        public AnimationCurve IncreaseCurve = new AnimationCurve();
        Camera Cam;

        public void Setup(Camera camera)
        {
            Cam = Camera.main;
            originalFov = camera.fieldOfView;
        }

        public IEnumerator FOVKickUp()
        {
            float t = Mathf.Abs((Cam.fieldOfView - originalFov) / FOVIncrease);
            while (t < TimeToIncrease)
            {
                Cam.fieldOfView = originalFov + (IncreaseCurve.Evaluate(t / TimeToIncrease) * FOVIncrease);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator FOVKickDown()
        {
            float t = Mathf.Abs((Cam.fieldOfView - originalFov) / FOVIncrease);
            while (t > 0)
            {
                Cam.fieldOfView = originalFov + (IncreaseCurve.Evaluate(t / TimeToDecrease) * FOVIncrease);
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            //make sure that fov returns to the original size
            Cam.fieldOfView = originalFov;
        }
    }

    [Serializable]
    public class HeadBob
    {
        public float HorizontalBobRange = 0.33f;
        public float VerticalBobRange = 0.33f;
        public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob
        public float VerticaltoHorizontalRatio = 1f;

        protected float m_CyclePositionX;
        protected float m_CyclePositionY;
        protected float m_BobBaseInterval;
        protected Vector3 m_OriginalCameraPosition;
        protected float m_Time;

        public void Setup(Camera camera, float bobBaseInterval)
        {
            m_BobBaseInterval = bobBaseInterval;
            m_OriginalCameraPosition = camera.transform.localPosition;
            m_Time = Bobcurve[Bobcurve.length - 1].time;
        }

        public Vector3 DoHeadBob(float speed)
        {
            float xPos = m_OriginalCameraPosition.x + (Bobcurve.Evaluate(m_CyclePositionX) * HorizontalBobRange);
            float yPos = m_OriginalCameraPosition.y + (Bobcurve.Evaluate(m_CyclePositionY) * VerticalBobRange);

            m_CyclePositionX += (speed * Time.deltaTime) / m_BobBaseInterval;
            m_CyclePositionY += ((speed * Time.deltaTime) / m_BobBaseInterval) * VerticaltoHorizontalRatio;

            if (m_CyclePositionX > m_Time)
            {
                m_CyclePositionX = m_CyclePositionX - m_Time;
            }
            if (m_CyclePositionY > m_Time)
            {
                m_CyclePositionY = m_CyclePositionY - m_Time;
            }

            return new Vector3(xPos, yPos, 0f);
        }
    }

    [Serializable]
    public class JumpBob
    {
        public float BobDuration = 0.2f;
        public float BobAmount = 0.1f;

        float m_Offset = 0f;
        public float Offset()
        {
            return m_Offset;
        }

        public IEnumerator DoJumpBob()
        {
            // make the camera move down slightly
            float t = 0f;
            while (t < BobDuration)
            {
                m_Offset = Mathf.Lerp(0f, BobAmount, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // make it move back to neutral
            t = 0f;
            while (t < BobDuration)
            {
                m_Offset = Mathf.Lerp(BobAmount, 0f, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            m_Offset = 0f;
        }
    }

}