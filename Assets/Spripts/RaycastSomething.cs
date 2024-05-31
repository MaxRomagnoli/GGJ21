using UnityEngine;
using UnityEngine.Events;

public class RaycastSomething : MonoBehaviour {

    [SerializeField] private LayerMask rayMask;
    [SerializeField] private float raycastLenght = 15f;
    //[SerializeField] private UnityEvent eventsTriggerEnter;
    //[SerializeField] private UnityEvent eventsTriggerExit;
    //private bool calledInvoke = false;
    [HideInInspector] public Vector3 hittedPosition; //variables you can call from another script
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

	void FixedUpdate ()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastLenght, rayMask))
        {
            if(hit.collider.gameObject.name == "Rin-chan") {
                gameManager.StartTalking();
            }
            else if(hit.collider.gameObject.tag == "pickable") {
                gameManager.TakeObject(hit.collider.gameObject);
            }

            /*if (!calledInvoke)
            {
                hittedPosition = hit.collider.transform.position;
                Debug.Log("invoke enter: " + thisTag);
                eventsTriggerEnter.Invoke();
                calledInvoke = true;
            }*/
        }
        /*else if(calledInvoke)
        {
            Debug.Log("invoke exit");
            eventsTriggerExit.Invoke();
            calledInvoke = false;
        }*/
    }

    void OnDrawGizmos()
    {
        DrawHelperAtCenter(this.transform.position, this.transform.forward, Color.blue, raycastLenght);
    }

    private void DrawHelperAtCenter(Vector3 origin, Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(origin, origin + direction * scale);
    }

}
