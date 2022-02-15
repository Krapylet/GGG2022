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

    private Rigidbody rb;
    private bool isBreaking;

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update() {
        // Vi laver ikke noget efter det her if-statement hvis ikke vi er p� jorden.
        if (!isOnGround) {
            return;
        }

        // Hop n�r vi trykker p� space.
        if (Input.GetButtonDown("Jump_" + player)) {
            Jump();
        }

        // Brems n�r vi trykker p� ctrl
        isBreaking = Input.GetButtonDown("Brake_" + player);
    }

    private void LateUpdate() {
        // S�rg for at bilens rotation forbliver l�st
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    void FixedUpdate()
    {
        // Se om vi st�r p� jorden
        DetectGround();

        // Vi vil kun kunne accelerere og bremse hvis vi er p� jorden
        if (isOnGround) {
            GroundControls();
        } // Hvis vi er i luften kan vi holde den nuv�rende fart ved at holde frem nede
        else {
            AirControls();
        }

        if (isBreaking) {
            Brake();
        }

        tweakJumpForces();
    }

    // create shorthand name
    RaycastHit boxHit;
    private void DetectGround() {
        // create shorthand name
        Collider gdc = groundDetector;

        // Base the boxcast on the dimensions of the groundDetector object.
        isOnGround = Physics.CheckBox(gdc.bounds.center, gdc.transform.localScale, gdc.transform.rotation, LayerMask.GetMask("Ground"));
    }


    private void tweakJumpForces() {
        // F� hop til at f�les bedre ved at �ge tyngdekraften n�r vi falder.
        bool isFalling = rb.velocity.y < -0.1;
        bool isRising = rb.velocity.y > 0.1;
        bool isNotHoldingJump = !Input.GetButton("Jump_" + player);
        bool isShortJumping = isRising && isNotHoldingJump;

        if (isFalling) {
            // tr�k 1 fra multiplieren fordi gravity allerede bliver applied 1 gang automatisk.
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1));
        }
        else if (isShortJumping) {
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1));
        }
    }

    private void AirControls() {
        // Behold det nuv�rende momentum hvis frem holdes nede
        preserveMomentum = Input.GetAxis("Vertical_" + player) == 1;
        if (preserveMomentum) {
            rb.drag = airDrag;
        }
        else {// ellers lader vi luftmodstanden s�nke farten
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
        // Udregn ny kraft. Aksen g�r fra -1 til 1.
        Vector3 fartRetning = new Vector3(speed * Input.GetAxis("Vertical_" + player), 0, 0);

        // Tilf�j kraft til bil.
        rb.AddRelativeForce(fartRetning);
    }

    private void HandleRotation(float force) {
        // Udregn ny kraft. Aksen g�r fra -1 til 1.
        Vector3 rotationsRetning = new Vector3(0, force * Input.GetAxis("Horizontal_" + player), 0);

        // Tilf�j kraft til bil.
        rb.AddTorque(rotationsRetning);
    }


    public void Jump() {
        Vector3 jumpVector = new Vector3(0, jumpForce, 0);
        rb.AddRelativeForce(jumpVector);

        isOnGround = false;
    }

    public void Brake() {
        // f� nuv�rende speed
        Vector3 currentSpeed = rb.velocity;

        Vector3 deacceleration = currentSpeed * brakeForce;

        rb.AddForce(deacceleration);
    }
}
