using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControls : MonoBehaviour
{
    public string player;

    public float speed;
    public float turnForce; // 35
    public float airTurnForce;
    public float jumpForce;
    public float brakeForce;
    public float fallGravityMultiplier;
    public float shortJumpGravityMultiplier;
    public bool isOnGround;
    public bool preserveMomentum;
    public float airDrag;
    public Collider groundDetector;
    public Transform checkpoint;
    public Camera carCamera;

    public float speedSoundThreshold;
    public AudioSource carAudio;
    public AudioClip IdleSound;
    public AudioClip acceleratingSound;
    public AudioClip speedingSound;

    public int minFOV;
    public int FOVscaling;
    public int maxFOV;

    private Rigidbody rb;
    private bool isBreaking;

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update() {
        // Vi laver ikke noget efter det her if-statement hvis ikke vi er p? jorden.
        if (!isOnGround) {
            return;
        }

        // Hop n?r vi trykker p? space.
        if (Input.GetButtonDown("Jump_" + player)) {
            Jump();
        }

        // Hold gang i motor lyden
        //MakeMotorSounds();

        // Brems n?r vi trykker p? ctrl
        isBreaking = Input.GetButtonDown("Brake_" + player);


        //Og FOV baseret p? bilens hastighed
        UpdateFOV();
    }

    private void LateUpdate() {
        // S?rg for at bilens rotation forbliver l?st
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    void FixedUpdate()
    {
        // Se om vi st?r p? jorden
        DetectGround();

        // Vi vil kun kunne accelerere og bremse hvis vi er p? jorden
        if (isOnGround) {
            GroundControls();
        } // Hvis vi er i luften kan vi holde den nuv?rende fart ved at holde frem nede
        else {
            AirControls();

            // F? hop til at f?les bedre ved at ?ge tyngdekraften n?r vi falder.
            tweakJumpForces();
        }

        if (isBreaking) {
            Brake();
        }  
    }

    private void UpdateFOV() {
        //Find bilens nuv?rende hastighed og FOV:
        float currentSpeed = rb.velocity.magnitude;
        float currentFOV = carCamera.fieldOfView;

        //Vi vil have 60 FOV ved 0 speed og 100 FOV ved 200 speed, s? vi laver noget hurtigt procentregning:
        float FOVincrease = currentSpeed / 100f * FOVscaling;
        float targetFOV = minFOV + FOVincrease;

        // Vi s?rger for at FOVen aldrig kommer over maxFov, for det ser m?rkelig ud.
        targetFOV = Mathf.Min(targetFOV, maxFOV);
        
        // Lav en glidende overgang mellem de to FOV tal.
        float actualFOV = Mathf.Lerp(currentFOV, targetFOV, 0.2f);

        // S?t FOV v?rdien p? cameraet.
        carCamera.fieldOfView = actualFOV;
    }

    private void MakeMotorSounds() {
        bool playerIsMoving = Input.GetAxis("Vertical_" + player) != 0;
        if (!playerIsMoving) {
            KeepPlayingSound(IdleSound);
        }
        if (playerIsMoving) {
            float currentSpeed = rb.velocity.magnitude;

            // If the car is still slow, play the acceleracting sound
            if (currentSpeed < speedSoundThreshold) {
                KeepPlayingSound(acceleratingSound);
            }
            else {
                KeepPlayingSound(speedingSound);
            }
        }
    }

    private void KeepPlayingSound(AudioClip sound) {

        // Find out if we are already playing the correct sound.
        bool IsAlreadyPlayingSound = carAudio.clip == sound && carAudio.isPlaying;
        
        // If we are already playing the sound, dont do anything.
        if (IsAlreadyPlayingSound) {
            // end the method early.
            return;
        }

        // otherwise, start assign the new sound and start it over.
        carAudio.clip = IdleSound;
        carAudio.Play();
    }


    private void DetectGround() {
        // create shorthand name
        Collider gdc = groundDetector;

        // Base the boxcast on the dimensions of the groundDetector object.
        isOnGround = Physics.CheckBox(gdc.bounds.center, gdc.transform.localScale, gdc.transform.rotation, LayerMask.GetMask("Ground"));
    }


    private void tweakJumpForces() {
        // F? hop til at f?les bedre ved at ?ge tyngdekraften n?r vi falder.
        bool isFalling = rb.velocity.y < -0.1;
        bool isRising = rb.velocity.y > 0.1;
        bool isNotHoldingJump = !Input.GetButton("Jump_" + player);
        bool isShortJumping = isRising && isNotHoldingJump;

        if (isFalling) {
            // tr?k 1 fra multiplieren fordi gravity allerede bliver applied 1 gang automatisk.
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1));
        }
        else if (isShortJumping) {
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1));
        }
    }

    private void AirControls() {
        // Behold det nuv?rende momentum hvis frem holdes nede
        preserveMomentum = Input.GetAxis("Vertical_" + player) == 1;
        if (preserveMomentum) {
            rb.drag = airDrag;
        }
        else {// ellers lader vi luftmodstanden s?nke farten
            rb.drag = 1;
        }
        

        HandleRotation(airTurnForce);
    }

    private void GroundControls() {
        // Make sure the drag has been enabled again when not in air.
        rb.drag = 1;
        HandleAcceleration();
        HandleRotation(turnForce);
    }

    private void HandleAcceleration() {
        // Udregn ny kraft. Aksen g?r fra -1 til 1.
        Vector3 fartRetning = new Vector3(speed * Input.GetAxis("Vertical_" + player), 0, 0);

        // Tilf?j kraft til bil.
        rb.AddRelativeForce(fartRetning);
    }

    private void HandleRotation(float force) {
        // Udregn ny kraft. Aksen g?r fra -1 til 1.
        Vector3 rotationsRetning = new Vector3(0, force * Input.GetAxis("Horizontal_" + player), 0);

        // Tilf?j kraft til bil.
        rb.AddTorque(rotationsRetning);
    }


    public void Jump() {
        Vector3 jumpVector = new Vector3(0, jumpForce, 0);
        rb.AddRelativeForce(jumpVector);

        isOnGround = false;
    }

    public void Brake() {
        // f? nuv?rende speed
        Vector3 currentSpeed = rb.velocity;

        Vector3 deacceleration = currentSpeed * brakeForce;

        rb.AddForce(deacceleration);
    }
}
