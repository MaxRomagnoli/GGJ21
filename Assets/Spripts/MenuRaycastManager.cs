using UnityEngine;
//using UnityEngine.UI;

public class MenuRaycastManager : MonoBehaviour {

    [SerializeField] private GameObject popUpObj;
    private RaycastSomething raycastSomething;

	void Start ()
    {
        HideObj();
        raycastSomething = FindObjectOfType<RaycastSomething>();
    }
	
	public void PopUpThisPosition () //called from raycast something
    {
        popUpObj.transform.position = raycastSomething.hittedPosition;
        popUpObj.SetActive(true);
        //AudioManager.instance.Play("Soundtrack");
    }

    public void HideObj()
    {
        popUpObj.SetActive(false);
        //AudioManager.instance.StopAll();
    }

}
