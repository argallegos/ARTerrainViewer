using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectInteraction : MonoBehaviour
{
    private ARRaycastManager rayManager;
    [SerializeField]
    private GameObject subject;
    GameObject spawnedObject;

    private bool placingMode = true;

    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();

    }


    void Update()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // follow center of screen if in placing mode
        if (hits.Count > 0 && placingMode == true)
        {
            if (spawnedObject == null)
            {
                print("instantiating obj");
                spawnedObject = Instantiate(subject, hits[0].pose.position, hits[0].pose.rotation);
            }
            spawnedObject.transform.position = hits[0].pose.position;
        }

        if (Input.touchCount > 0)
        {
            print("somethings touching");
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                //turn off placing mode to keep the object in one place, or else turn it back on
                if (placingMode)
                {
                    placingMode = false;
                    print("disable placing mode");
                }
                else
                {
                    placingMode = true;
                    print("placing mode on");
                }
            }

            // run place obj at this position
            // set object tethered to true
        }

    }



    void PlaceObj (Vector3 position, GameObject obj)
    {
        //subject.transform.position = 
        //obj.AddComponent<ARAnchor>();
    }
}
