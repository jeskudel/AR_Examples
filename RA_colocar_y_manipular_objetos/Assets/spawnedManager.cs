using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class spawnedManager : MonoBehaviour
{
    public uint qsize = 15;  // number of messages to keep
    public int fontSize = 40;
    Queue myLogQueue = new Queue();

    [SerializeField]
    ARRaycastManager m_RaycastManager;
    [SerializeField]
    GameObject spawnablePrefab;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    Camera arCam;
    GameObject spawnedObject;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] " + logString);
        if (type == LogType.Exception) myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }
    void OnGUI()
    {
        GUI.skin.label.fontSize = fontSize;
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started up logging.");
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
            return;

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        //Debug.Log(Input.GetTouch(0).phase);
        
        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if (Physics.Raycast(ray,out hit))
                {
                    //Debug.Log(ray);
                    //Debug.Log(hit.collider.gameObject);
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                        Debug.Log("Objeto ya existente");
                        Debug.Log(spawnedObject);
                    }
                    else
                    {
                        SpanwPrefab(m_Hits[0].pose.position);
                        //Debug.Log("Objeto nuevo!");
                        //Debug.Log(spawnedObject);
                    }
                }
            }
        }
        if ((Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary) && spawnedObject != null)
        {
            spawnedObject.transform.position = m_Hits[0].pose.position;
            Debug.Log("Movimiento!!!");
            Debug.Log(spawnedObject);
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            spawnedObject = null;
            //Debug.Log("Fase acabado");
            //Debug.Log(spawnedObject);
        }
    }

    private void SpanwPrefab(Vector3 position)
    {
        spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);
    }
}
