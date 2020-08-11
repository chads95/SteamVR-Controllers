using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;


public class LaserPointer : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Boolean teleportAction;
    public GameObject mainCamera;
    [HideInInspector]
    public static RaycastHit laserhit;

    public GameObject rayCurrentlyHitting;

    public GameObject laserPrefab; // 1
    private GameObject laser; // 2
    private Transform laserTransform; // 3
    private Vector3 hitPoint; // 4

    // 1
    public Transform cameraRigTransform;
    // 2
    public GameObject teleportReticlePrefab;
    // 3
    private GameObject reticle;
    // 4
    private Transform teleportReticleTransform;
    // 5
    public Transform headTransform;
    // 6
    public Vector3 teleportReticleOffset;
    // 7
    public LayerMask teleportMask;
    // 8
    private bool shouldTeleport;

    private Camera m_camera;



    public void Start()
    {
        // 1
        laser = Instantiate(laserPrefab);
        // 2
        laserTransform = laser.transform;

        // 1
        reticle = Instantiate(teleportReticlePrefab);
        // 2
        teleportReticleTransform = reticle.transform;

        m_camera = mainCamera.GetComponent<Camera>();
    }

    public void Update()
    {
        // 1
        if (teleportAction.GetState(handType))
        {
            // 2
            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out laserhit, 100))
            {
                rayCurrentlyHitting = laserhit.collider.gameObject;
                if (laserhit.collider.gameObject.tag == "TeleportArea")
                {
                    hitPoint = laserhit.point;
                    ShowLaser(laserhit);
                    // 1
                    reticle.SetActive(true);
                    // 2
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                    // 3
                    shouldTeleport = true;
                }
                else if(laserhit.collider.gameObject.tag == "Button")
                {
                    hitPoint = laserhit.point;
                    ShowLaser(laserhit);
                    if (grabAction.GetLastStateDown(handType))
                    {
                        StartButton(laserhit);
                    }
                }
                else if(laserhit.collider.gameObject.tag == "TestTrigger")
                {
                    hitPoint = laserhit.point;
                    ShowLaser(laserhit);
                }
            }
        }
        else // 3
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (teleportAction.GetStateUp(handType) && shouldTeleport)
        {
            Teleport();
        }
    }

    public void ShowLaser(RaycastHit hit)
    {
        // 1
        laser.SetActive(true);
        // 2
        laserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);
        // 3
        laserTransform.LookAt(hitPoint);
        // 4
        laserTransform.localScale = new Vector3(laserTransform.localScale.x,
                                                laserTransform.localScale.y,
                                                hit.distance);
    }

    private void Teleport()
    {
        // 1
        shouldTeleport = false;
        // 2
        reticle.SetActive(false);
        // 3
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // 4
        difference.y = 0;
        // 5
        cameraRigTransform.position = hitPoint + difference;
    }

    private void StartButton(RaycastHit hit)
    {
        if (hit.collider.gameObject.name == "Scene1Button")
            SceneManager.LoadScene(1);
        else if (hit.collider.gameObject.name == "Scene2Button")
            SceneManager.LoadScene(2);
    }
}
