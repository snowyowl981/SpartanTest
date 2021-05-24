using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 8.0f;
    public float jumpPower = 10.0f;
    public float gravity = 9.8f;
    public float yVelocty;

    private Transform tr;

    private Rigidbody rig;

    public bool onGround = false;

    Vector3 moveDir = Vector3.zero;

    public float lookSensitivity;
    public float cameraRotationLimit;
    public float currentCameraRotationX;

    public Camera theCamera;

    private Animator playerAnim;

    private Transform spine;

    private float h;
    private float v;

    Vector3 ChestOffset = new Vector3(0, -40, -100);
    Vector3 ChestDir = new Vector3();

    private readonly int hashFront = Animator.StringToHash("IsWalkFront");
    private readonly int hashBack = Animator.StringToHash("IsWalkBack");
    private readonly int hashLeft = Animator.StringToHash("IsWalkLeft");
    private readonly int hashRight = Animator.StringToHash("IsWalkRight");
    private readonly int hashJump = Animator.StringToHash("Jump");
    private readonly int hashSprint = Animator.StringToHash("IsSprint");

    // Start is called before the first frame update
    IEnumerator Start()
    {
        lookSensitivity = 0.0f;

        tr = GetComponent<Transform>();
        rig = GetComponent<Rigidbody>();

        playerAnim = GetComponent<Animator>();
        spine = playerAnim.GetBoneTransform(HumanBodyBones.Spine);

        yield return new WaitForSeconds(0.3f);
        lookSensitivity = 200.0f;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();

        if(onGround)
        {
            PlayerJump();
        }

        CameraRotation();
        CharacterRotation();

        PlayerAnimation();

        PlayerSprint();
    }

    public void LateUpdate()
    {
        Operation_boneRotation();
    }

    void Operation_boneRotation()
    {
        ChestDir = theCamera.transform.position + theCamera.transform.forward * 50f;
        spine.LookAt(ChestDir);
        spine.rotation = spine.rotation * Quaternion.Euler(ChestOffset);

    }

    void PlayerMove()
    {   
        // 좌우
        h = Input.GetAxis("Horizontal");

        // 상하
        v = Input.GetAxis("Vertical");

        moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);

        yVelocty -= gravity * Time.deltaTime;
        moveDir.y = yVelocty;


    }

    void PlayerJump()
    {   
        if(Input.GetKeyDown(KeyCode.Space))
        {
            onGround = false;
            rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            Debug.Log("공중");
            playerAnim.SetTrigger(hashJump);
        }
    }

    void PlayerSprint()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            playerAnim.SetBool(hashSprint, true);
            moveSpeed = 14.0f;
        }
        else
        {
            playerAnim.SetBool(hashSprint, false);
            moveSpeed = 8.0f;
        }
    }

    void OnCollisionEnter(Collision coll)
    {   
        if(coll.gameObject.CompareTag("GROUND"))
        {
            onGround = true;
            Debug.Log("지상");
        }
    }

    void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity * Time.deltaTime;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _charaterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity * Time.deltaTime;
        rig.MoveRotation(rig.rotation * Quaternion.Euler(_charaterRotationY));
    }

    void PlayerAnimation()
    {   
        if(h == 0 && v == 0)
        {
            playerAnim.SetBool(hashFront, false);
            playerAnim.SetBool(hashBack, false);
            playerAnim.SetBool(hashLeft, false);
            playerAnim.SetBool(hashRight, false);
        }
        else if(v >= 0.01f)
        {
            playerAnim.SetBool(hashFront, true);
        }
        else if(v <= -0.01f)
        {
            playerAnim.SetBool(hashBack, true);
        }
        else if(h >= 0.01f)
        {
            playerAnim.SetBool(hashRight, true);
        }
        else if(h <= 0.01f)
        {
            playerAnim.SetBool(hashLeft, true);
        }
    }
}
