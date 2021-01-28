using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

/// <summary>
/// Main script for interacting with terrain data - controls placing, tutorial, scaling, rotation, points of interest
/// </summary>
public class ARObjectInteraction : MonoBehaviour
{
    private ARRaycastManager rayManager;
    private ARPlaneManager planeManager;
    private GameObject sessionOrigin;
    [SerializeField]
    private GameObject subject;
    GameObject spawnedObject;

    [Header("Object Interaction")]
    [SerializeField]
    private float scaleMin;
    [SerializeField]
    private float scaleMax;
    [SerializeField]
    private float rotateSpeed = 1f;
    [SerializeField]
    private int swipeMargin = 300;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject intro_findPlane;
    [SerializeField]
    private GameObject intro_lockObj;
    [SerializeField]
    private GameObject intro_interactUI;
    [SerializeField]
    private GameObject lockButton;
    [SerializeField]
    private GameObject interactionUI;
    [SerializeField]
    private Slider scaleSlider;

    private GameObject pointsOfInterest;

    private bool introShown = false;
    private bool placingMode = true;
    private bool activePOI = false;
    private float swipeStartPos;
    bool touchIsSwipe = false;

    [Header("Flair")]
    [SerializeField]
    private Material terrainMat;
    [SerializeField]
    private Material placingMat;
    private Renderer meshRend;

    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        sessionOrigin = planeManager.gameObject;
        lockButton.SetActive(false);
        intro_findPlane.SetActive(true);
        interactionUI.SetActive(false);
        intro_lockObj.SetActive(false);
        intro_interactUI.SetActive(false);

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
                    if (intro_interactUI.activeSelf) intro_interactUI.SetActive(false);
                    // if swipe is starting near the center of the screen, then do swipe things
                    if (/*(Screen.width / 2 - swipeMargin) <= touch.position.x && */ touch.position.x <= (Screen.width / 2 + swipeMargin))
                    {
                        swipeStartPos = touch.position.x;
                        touchIsSwipe = true;
                    }
                    break;
                case TouchPhase.Moved:
                    if (touchIsSwipe) spawnedObject.transform.Rotate(Vector3.up, -rotateSpeed * touch.deltaPosition.x);
                    break;
                case TouchPhase.Ended:
                    if (touchIsSwipe) touchIsSwipe = false;
                    break;
            }

        }
    }

    // instantiate object when first plane is found
    void CreateObj(Vector3 placePosition, Quaternion placeRotation)
    {
        print("instantiating obj");
        spawnedObject = Instantiate(subject, placePosition, placeRotation);
        meshRend = spawnedObject.transform.Find("Terrain").GetComponent<Renderer>();
        meshRend.material = placingMat;
        pointsOfInterest = spawnedObject.transform.Find("Markers").gameObject;
        pointsOfInterest.SetActive(false);
        intro_findPlane.SetActive(false);
        intro_lockObj.SetActive(true);
        lockButton.SetActive(true);
    }


    // lock and unlock object's position, show and hide the plane visualization
    public void LockObj()
    {
        if (placingMode)
        {
            placingMode = false;

            if (!introShown)
            {
                introShown = true;
                intro_lockObj.SetActive(false);
                intro_interactUI.SetActive(true);
            }

            interactionUI.SetActive(true);
            meshRend.material = terrainMat;

            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            sessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        }
        else
        {
            placingMode = true;
            interactionUI.SetActive(false);
            if (activePOI)
            {
                activePOI = false;
                pointsOfInterest.SetActive(false);
            }

            meshRend.material = placingMat;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
            sessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
        }

    }

    // enable points of interest
    public void ShowPOI()
    {
        if (activePOI)
        {
            activePOI = false;
            pointsOfInterest.SetActive(false);
        }
        else
        {
            activePOI = true;
            pointsOfInterest.SetActive(true);
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

