using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

/// <summary>
/// Currently only used to request camera permissions, if this were a full app would manage more things
/// </summary>
public class GameManager : MonoBehaviour
{

    void Start()
    {
        // request camera permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }


    }

}
