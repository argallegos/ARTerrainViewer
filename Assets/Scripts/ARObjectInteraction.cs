using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARObjectInteraction : MonoBehaviour
{
    private ARRaycastManager rayManager;
    [SerializeField]
    private GameObject subject;
    GameObject spawnedObject;

    [Header("Object Interaction")]
    [SerializeField]
    private float scaleMin;
    [SerializeField]
    private float scaleMax;
    [SerializeField]
    private float rotateSpeed = 50f;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject intro_findPlane;
    [SerializeField]
    private GameObject intro_lockObj;
    [SerializeField]
    private GameObject lockButton;
    [SerializeField]
    private GameObject interactionUI;
    [SerializeField]
    private Slider scaleSlider;

    private bool introShown = false;
    private bool placingMode = true;
    private float swipeStartPos;

    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
        lockButton.SetActive(false);
        interactionUI.SetActive(false);
        intro_lockObj.SetActive(false);

        scaleSlider.onValueChanged.AddListener(delegate { ScaleObj(); });
        scaleSlider.minValue = scaleMin;
        scaleSlider.maxValue = scaleMax;

    }


    void Update()
    {
        // create object, then follow center of screen if in placing mode
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        if (hits.Count > 0 && placingMode == true)
        {
            if (spawnedObject == null)
            {
                CreateObj(hits[0].pose.position, hits[0].pose.rotation);

            }
            else spawnedObject.transform.position = hits[0].pose.position;
        }

        // rotate object left and right
        if (Input.touchCount > 0 && placingMode == false)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    swipeStartPos = touch.position.x;
                    break;
                case TouchPhase.Moved:
                    if (swipeStartPos > touch.position.x)
                    {
                        spawnedObject.transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);
                    }
                    else if (swipeStartPos < touch.position.x)
                    {
                        spawnedObject.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
                    }
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }
    }

    // instantiate object when first plane is found
    void CreateObj(Vector3 placePosition, Quaternion placeRotation)
    {
        print("instantiating obj");
        spawnedObject = Instantiate(subject, placePosition, placeRotation);
        intro_findPlane.SetActive(false);
        intro_lockObj.SetActive(true);
        lockButton.SetActive(true);
    }


    // lock and unlock object's position
    public void LockObj()
    {
        if (placingMode)
        {
            placingMode = false;
            print("disable placing mode");

            if (!introShown)
            {
                introShown = true;
                intro_lockObj.SetActive(false);
            }

            interactionUI.SetActive(true);

        }
        else
        {
            placingMode = true;
            print("placing mode on");

            interactionUI.SetActive(false);
        }
    }

    // use the slider to scale object
    public void ScaleObj()
    {
        if (spawnedObject.transform.localScale.x >= scaleMin && spawnedObject.transform.localScale.x <= scaleMax)
        {
            spawnedObject.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value);
        }
        else if (spawnedObject.transform.localScale.x < scaleMin) spawnedObject.transform.localScale = new Vector3(scaleMin, scaleMin, scaleMin);
        else if (spawnedObject.transform.localScale.x > scaleMax) spawnedObject.transform.localScale = new Vector3(scaleMax, scaleMax, scaleMax);
    }

}

