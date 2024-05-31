using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BeltMover : MonoBehaviour
{
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private float timeToRespawn = 5f;
    private float currentTimeToRespawn;
    private AudioSource audioSource;
    private GameManager gameManager;
    private Rigidbody rigidbody;
    private GameObject currentBelt;

    void Reset()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        if (!groundCheck)
            groundCheck = GroundCheck.Create(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Aggiunge velocità presa da carrello
        if(groundCheck.isGrounded && groundCheck.objGrounded.tag == "belt") {

            //rigidbody.velocity = groundCheck.objGrounded.transform.forward * gameManager.BeltSpeed();

            //Limita velocità
            if(rigidbody.velocity.sqrMagnitude < gameManager.BeltSpeed()) {
                rigidbody.AddForce(groundCheck.objGrounded.transform.forward * gameManager.SuitcaseForce() * Time.fixedDeltaTime);
            } else {
                rigidbody.velocity = rigidbody.velocity.normalized * gameManager.BeltSpeed();
            }
        }

        // Se è uscito dalla mappa
        else if(gameManager.IsOutOfMap(this.transform)) {
            if(this.gameObject.tag == "Player" && gameManager.GetDamage()) {
                gameManager.GameOver(false);
            }
            Teleport();
        }

        // Tempo di respawn
        if(this.gameObject.tag != "Player") {
            if( rigidbody.velocity.sqrMagnitude < 0.1f) {
                currentTimeToRespawn += Time.fixedDeltaTime;
                if(currentTimeToRespawn > timeToRespawn) {
                    currentTimeToRespawn = 0;
                    Teleport();
                }
            } else {
                currentTimeToRespawn = 0;
            }
        } 
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "spawnPoint") {
            Teleport();
        }
    }

    void Teleport() {
        StopMoving() ;
        if(this.gameObject.tag == "Player") {
            gameManager.StartStorageSound();
            Transform spawnPosition = gameManager.GetSpawnPointPosition();
            this.transform.position = spawnPosition.position;
            this.transform.rotation = spawnPosition.rotation;
        } else {
            gameManager.AddInPool(this.transform);
        }
    }

    void StopMoving() {
        //currentBelt = null;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Se collide contro un nastro, assegna la forza corrente dal nastro e lo assegna come attivo
        if(collision.gameObject.tag == "belt" && currentBelt != collision.gameObject) {
            if(audioSource != null && !audioSource.isPlaying) { audioSource.Play(); }
            currentBelt = collision.gameObject;
        }
    }

    /*void OnCollisionExit(Collision collision)
    {
        // Se esce dalla collisione col nastro attivo, smette di aggiungere forza
        if(this.gameObject.tag == "Player") { //currentBelt == collision.gameObject && 
            currentBelt = null;
        }
    }*/

}
