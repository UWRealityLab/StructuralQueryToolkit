using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StereonetCameraStack : MonoBehaviour
{
    public static StereonetCameraStack instance;

    [Tooltip("Stereonet Aligning")]
    [SerializeField] Transform bowl;
    [SerializeField] Transform bowlParent;
    [SerializeField] Transform player;
    [SerializeField] Transform playerCamera; // For some reason, the X-axis rotation happens on the camera itself, not the player parent GameObject

    public float offset = 1f;

    [Tooltip("Stereonet Rendering")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform pointsParent;
    [SerializeField] GameObject pointPrefab;

    public LinkedList<GameObject>[] points;
    public int activeStereonetID;

    private Camera cam;
    private int raycastLayer;

    public TextMeshProUGUI[] compassImages;

    private void Awake()
    {
        instance = this;
        cam = GetComponent<Camera>();
        points = new LinkedList<GameObject>[6];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new LinkedList<GameObject>();
        }
        
        var undoEvent = ToolManager.instance.undoEvent;
        undoEvent.AddListener(() =>
        {
            StereonetCameraStack.instance.Undo();
        });
    }

    private void Start()
    {
        raycastLayer = LayerMask.GetMask("Stereonet Bowl UI");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        bowl.transform.forward = -player.transform.forward;
        bowl.transform.localRotation = Quaternion.Euler(0f, bowl.rotation.eulerAngles.y, bowl.rotation.eulerAngles.z);

        bowlParent.transform.localRotation = Quaternion.Euler (playerCamera.rotation.eulerAngles.x, 0f, 0f);

        SetCompassImagesAlpha();
    }

    public void CreatePoint(Vector3 dir)
    {
        RaycastHit hit;
        Vector3 adjustedDir = lineRenderer.transform.TransformDirection(dir);

        if (!Physics.Raycast(lineRenderer.transform.position, adjustedDir, out hit, 5f, raycastLayer))
        {
            print("This should never run");
            lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(lineRenderer.transform.position + adjustedDir));
            return;
        }

        lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(hit.point));
        var point = Instantiate(pointPrefab, hit.point, Quaternion.identity, pointsParent);
        points[activeStereonetID].AddLast(point);

    }

    private void SetCompassImagesAlpha()
    {

        foreach (var image in compassImages)
        {
            float a = Vector3.Dot(image.transform.forward, transform.forward);
            image.color = new Color(1f, 1f, 1f, a );

        }

    }

    public void Undo()
    {
        if (points[activeStereonetID].Count == 0)
        {
            return;
        }

        Destroy(points[activeStereonetID].Last.Value);
        points[activeStereonetID].RemoveLast();

        UpdateLineRenderer();
    }

    public void SwitchStereonet(int stereonetID)
    {
        var oldPoints = points[activeStereonetID];
        foreach (GameObject point in oldPoints)
        {
            point.SetActive(false);
        }

        activeStereonetID = stereonetID;

        var newPoints = points[activeStereonetID];
        foreach (GameObject point in newPoints)
        {
            point.SetActive(true);
        }

        UpdateLineRenderer();
    }

    public void Delete(int stereonetID)
    {
        var pointsToDelete = points[stereonetID];
        foreach (var point in pointsToDelete)
        {
            Destroy(point);
        }

        pointsToDelete.Clear();
    }

    private void UpdateLineRenderer()
    {
        if (points[activeStereonetID].Count > 0)
        {
            lineRenderer.SetPosition(1, lineRenderer.transform.InverseTransformPoint(points[activeStereonetID].Last.Value.transform.position));
        }
        else
        {
            lineRenderer.SetPosition(1, Vector3.zero);
        }
    }

    public void DeleteAll()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Delete(i);
        }
    }

    public void SwitchToOrtho()
    {
        cam.orthographic = true;
    }

    public void SwitchToPerspective()
    {
        cam.orthographic = false;
    }
}
