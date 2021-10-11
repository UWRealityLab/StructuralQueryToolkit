using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stereonet : MonoBehaviour
{
    public int id;

    [SerializeField] GameObject pointPrefab;
    public Transform whalebackFlagsParent;
    private List<Transform> flagsList;
    public Transform pointPlanesParent;
    [SerializeField] Transform stereonetPlanesParent;
    [SerializeField] Transform stereonetLinearParent;
    [SerializeField] Transform linearWorldLinesParent;
    [SerializeField] Transform polePointsParentObj;
    private List<Transform> pointsList;
    [SerializeField] GameObject finalPointPrefab;
    [SerializeField] GameObject model;
    [SerializeField] GameObject specialPointPrefab;
    [SerializeField] Material stalePointMaterial;

    int stereonetLayer;

    private LinkedList<Vector3> points;
    private Transform finalPoint;
    [HideInInspector] public LineRenderer lineRenderer;
    private AvgStereonetPoleData data;

    // Used to store the default measurement groups' names 
    [HideInInspector] public Measurement defaultPlaneMeasurement;
    [HideInInspector] public Measurement defaultLineationMeasurement;

    [HideInInspector] public Transform latestPoint;

    [HideInInspector] public LinkedList<Vector3> normals;  // Normals for the current stereonet

    // For measuring planes (where measuring 3 points will create a plane)
    //[SerializeField] GameObject planePointPrefab;
    [SerializeField] GameObject planeLineRendererPrefab;
    [SerializeField] GameObject planeWorldPrefab;
    [SerializeField] GameObject planeWorldTwoPointPrefab; // Two point variant (it's a square plane)
    [SerializeField] Gradient normalStereonetPlaneLineGradient;
    [SerializeField] Gradient desaturatedStereonetPlaneLineGradient;
    private LinkedList<Transform> planePoints;
    [HideInInspector] public LinkedList<PiPlotPlane> stereonetPlanes;
    private LinkedList<Transform> worldPlanes;
    
    // For measuring lines (2 points)
    [SerializeField] GameObject linearLinePrefab;
    [SerializeField] GameObject linearStereonetPointPrefab;
    [SerializeField] Material normalStereonetLinearLineMaterial;
    [SerializeField] Material desaturatedStereonetLinearPointMaterial;
    private LinkedList<Transform> worldLines;
    [HideInInspector] public LinkedList<PiPlotLine> stereonetLinearPoints;
    private LinkedList<Transform> worldLinePoints; // In reality, this will only be 1 or 2 points 
    
    public Material[] flagMaterials;
    public Material twoPointPlaneMaterial; // For plane-plotting

    [HideInInspector]
    public bool isPiPlotEnabled = false;
    
    private const int NUM_INITIAL_POINTS = 5; // Points to add to the middle of the stereonet to have better line fitting (for pole plotting)

    private void Awake()
    {
        points = new LinkedList<Vector3>();
        normals = new LinkedList<Vector3>();
        poleElevations = new List<float>();

        flagsList = new List<Transform>();
        
        lineRenderer = GetComponent<LineRenderer>();
        stereonetLayer = LayerMask.NameToLayer("Stereonet");
        finalPoint = Instantiate(finalPointPrefab, Vector3.zero, Quaternion.identity, transform).transform;

        data = new AvgStereonetPoleData();
        data.strikeDipPairs = new LinkedList<(float, float)>();
        data.trendPlungePairs = new LinkedList<(float, float)>();

        stereonetPlanes = new LinkedList<PiPlotPlane>();
        planePoints = new LinkedList<Transform>();
        worldPlanes = new LinkedList<Transform>();

        worldLinePoints = new LinkedList<Transform>();
        worldLines = new LinkedList<Transform>();
        stereonetLinearPoints = new LinkedList<PiPlotLine>();

        for (int i = 0; i < NUM_INITIAL_POINTS; i++)
        {
            points.AddFirst(new Vector3(0.0f, 0.0f, 0.0f));
        }
    }

    private void Start()
    {
        finalPoint.gameObject.SetActive(false); // You have to put this line in Start() instead of Awake() for it to work

        defaultPlaneMeasurement = new PiPlotPlane();
        defaultLineationMeasurement = new PiPlotLine();

    }

    private void Update()
    {
        if (hasChanged)
        {
            hasChanged = false;
            FitPlane();
        }
    }

    private void OnDestroy()
    {
        foreach (var flag in flagsList)
        {
            if (flag)
            {
                Destroy(flag.gameObject);
            }
        }
    }

    void FitPlane()
    {
        // Only fit the plane if the user has 3 measurements or more
        if (GetNumPoints() < 3)
        {
            finalPoint.gameObject.SetActive(false);
            lineRenderer.enabled = false;
            isPiPlotEnabled = false;
            PIPlotButton.instance.isToggled = false;
            return;
        }
        finalPoint.gameObject.SetActive(true);


        List<Vector3> pointsList = new List<Vector3>();

        Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
        foreach (Vector3 pos in points)
        {
            pointsList.Add(pos);
            sum += pos;
        }

        Vector3 centroid = sum * (1.0f / pointsList.Count);

        //Calculate determinants from matrix components
        float xx = 0.0f; float xy = 0.0f; float xz = 0.0f;
        float yy = 0.0f; float yz = 0.0f; float zz = 0.0f;

        foreach (Vector3 pos in points)
        {
            Vector3 r = pos - centroid;
            xx += r.x * r.x;
            xy += r.x * r.y;
            xz += r.x * r.z;
            yy += r.y * r.y;
            yz += r.y * r.z;
            zz += r.z * r.z;
        }

        float det_x = yy * zz - yz * yz;
        float det_y = xx * zz - xz * xz;
        float det_z = xx * yy - xy * xy;

        float det_max = Mathf.Max(det_x, det_y, det_z);
        Vector3 dir = new Vector3(0.0f, 0.0f, 0.0f);
        if (det_max == det_x)
        {
            dir.x = det_x;
            dir.y = xz * yz - xy * zz;
            dir.z = xy * yz - xz * yy;
        }
        else if (det_max == det_y)
        {
            dir.x = xz * yz - xy * zz;
            dir.y = det_y;
            dir.z = xy * xz - yz * xx;
        }
        else
        {
            dir.x = xy * yz - xz * yy;
            dir.y = xy * xz - yz * xx;
            dir.z = det_z;
        }
        Vector3 normal = -dir.normalized;
        StereonetsController.singleton.finalPlane.forward = normal;

        var finalPlane = StereonetsController.singleton.finalPlane;
        var finalPlaneLeftCorner = StereonetsController.singleton.finalPlaneLeftCorner;
        var finalPlaneRightCorner = StereonetsController.singleton.finalPlaneRightCorner;
        
        var isOverTurned = normal.y > 0f; 
        finalPoint.position = StereonetsController.singleton.originTransform.position + (isOverTurned ? (-normal * 5f) : (normal * 5f));

        int iters = 50;
        lineRenderer.positionCount = 0;
        for (int i = 0; i < iters; i++)
        {
            Vector3 pos = Vector3.Lerp(finalPlaneLeftCorner.position, finalPlaneRightCorner.position, Mathf.SmoothStep(0f, 1f, (float)i / (iters - 1)));

            /*
            var stereonetPointPosition = pos - StereonetsController.singleton.finalPlane.up * 10f;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(stereonetPointPosition.x, stereonetPointPosition.y + 0.01f, stereonetPointPosition.z)); // Offset in Y to account for z fighting
            */

            var hits = Physics.RaycastAll(pos, -finalPlane.up, 100f, ~stereonetLayer);
            foreach (RaycastHit hit in hits)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z)); // Offset in Y to account for z fighting
            }
        }
        // For the two edges of the line renderer
        var planeSideDir = (finalPlaneLeftCorner.position - finalPlaneRightCorner.position).normalized;
        lineRenderer.SetPosition(0, transform.position + planeSideDir * 4.9f);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position - planeSideDir * 4.9f);

    }

    private Vector3[] GenerateStereonetLine(Vector3 planeDown, Vector3 leftCorner, Vector3 rightCorner)
    {
        int itrCount = 50;
        Vector3[] points = new Vector3[itrCount];

        for (int i = 0; i < itrCount; i++)
        {
            Vector3 pos = Vector3.Lerp(leftCorner, rightCorner, Mathf.SmoothStep(0f, 1f, (float)i / (itrCount - 1)));

            points[i] = pos + planeDown * 15f; 

            //Instantiate(specialPointPrefab, points[i], Quaternion.identity);

            Debug.DrawLine(StereonetsController.singleton.originTransform.position, points[i], Color.blue, 3f);
        }

        return points;
    }


    // Draws a point in the 3D stereonet
    public void AddPole(Vector3 normal, Flag flag)
    {
        normal = -normal;

        var isOverturnedBedding = normal.y > 0f;
        var dirNormal = isOverturnedBedding ? normal : -normal;
        var stereonetPointPosition = StereonetsController.singleton.originTransform.position - dirNormal * 4.9f;

        if (latestPoint != null)
        {
            latestPoint.GetComponent<MeshRenderer>().material = stalePointMaterial;
        }
        latestPoint = Instantiate(isOverturnedBedding ? specialPointPrefab : pointPrefab, stereonetPointPosition, Quaternion.identity, polePointsParentObj).transform;
        
        flag.stereonetPoint = latestPoint;
        points.AddFirst(stereonetPointPosition);

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.CreatePoint(-normal);
        }

        FitPlane();
    }

    private bool hasChanged = false;
    public void ChangePoleData(Vector3 flagUp, Transform stereonetPoint)
    {
        hasChanged = true;
        var oldPos = stereonetPoint.position;

        var normal = flagUp;
        var isOverturnedBedding = normal.y > 0f;
        var dirNormal = isOverturnedBedding ? normal : -normal;
        var stereonetPointPosition = StereonetsController.singleton.originTransform.position - dirNormal * 4.9f;

        stereonetPoint.position = stereonetPointPosition;

        points.AddLast(stereonetPointPosition);
        points.Remove(oldPos);
    }

    /// <summary>
    /// Note: is aligned with the points stack
    /// </summary>
    private List<float> poleElevations;
    // Draws a point in the 3D stereonet
    public void AddPole(Vector3 normal, float elevation, Flag flag)
    {
        normal = -normal;
        
        var isOverturnedBedding = normal.y > 0f;
        var dirNormal = isOverturnedBedding ? normal : -normal;
        var stereonetPointPosition = StereonetsController.singleton.originTransform.position - dirNormal * 4.9f;

        if (latestPoint != null)
        {
            latestPoint.GetComponent<MeshRenderer>().material = stalePointMaterial;
        }
        latestPoint = Instantiate(isOverturnedBedding ? specialPointPrefab : pointPrefab, stereonetPointPosition, Quaternion.identity, polePointsParentObj).transform;
        
        
        flag.stereonetPoint = latestPoint;
        points.AddFirst(stereonetPointPosition);
        poleElevations.Add(elevation);

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.CreatePoint(-normal);
        }

        FitPlane();
    }


    public void AddPlanePointThreePoint(Transform point)
    {
        point.parent = pointPlanesParent;
        planePoints.AddFirst(point);

        if (planePoints.Count == 3)
        {
            // Create gameobject prefab
            var worldPlane = Instantiate(planeWorldPrefab, pointPlanesParent);
            worldPlane.transform.position = Vector3.zero;
            var stereonetPlane = Instantiate(planeLineRendererPrefab, stereonetPlanesParent); // Essentially the line renderer
            PiPlotPlane piPlotPlane = stereonetPlane.GetComponent<PiPlotPlane>();

            // Get normal of the 3 points, which is the plane forward
            var a = planePoints.First.Value;
            planePoints.RemoveFirst();
            a.parent = worldPlane.transform;
            var b = planePoints.First.Value;
            planePoints.RemoveFirst();
            b.parent = worldPlane.transform;
            var c = planePoints.First.Value;
            planePoints.RemoveFirst();
            c.parent = worldPlane.transform;

            Vector3 normal = Vector3.Cross(a.position - b.position, a.position - c.position).normalized;
            bool isFlipped = Vector3.Dot(normal, point.up) < 0 ? true : false;

            if (isFlipped)
            {
                normal = -normal;
            }


            piPlotPlane.SetForward(normal);

            // Create mesh
            Mesh planeMesh = worldPlane.GetComponent<MeshFilter>().mesh;

            // TEMP? we're moving the vertices up a little
            //var offset = normal * 0.1f;
            planeMesh.SetVertices(new Vector3[] { a.position, b.position, c.position });
            planeMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };

            // Triangles depend on which orientation (clockwise or anti clockwise) the user placed the plane points
            if (isFlipped)
            {
                planeMesh.triangles = new int[] { 2, 1, 0 };
            } else
            {
                planeMesh.triangles = new int[] { 0, 1, 2 };
            }

            worldPlane.GetComponent<MeshRenderer>().material = flagMaterials[1];
            
            worldPlanes.AddFirst(worldPlane.transform);
            stereonetPlanes.AddFirst(piPlotPlane);

            PiPlotPlaneButton.instance.UpdateButton();
            LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane.strike, piPlotPlane.dip);
        }
    }

    public void AddPlanePointTwoPoint(Transform point)
    {
        point.parent = pointPlanesParent;
        planePoints.AddFirst(point);

        if (planePoints.Count == 2)
        {
            // Create gameobject prefab
            var worldPlaneParent = Instantiate(planeWorldTwoPointPrefab, pointPlanesParent); // Contains the plane and the two points
            var worldPlane = worldPlaneParent.transform.GetChild(0);
            worldPlane.position = Vector3.zero;
            var stereonetPlane = Instantiate(planeLineRendererPrefab, stereonetPlanesParent); // Essentially the line renderer
            PiPlotPlane piPlotPlane = stereonetPlane.GetComponent<PiPlotPlane>();

            var a = planePoints.First.Value;
            planePoints.RemoveFirst();
            var b = planePoints.First.Value;
            planePoints.RemoveFirst();
            a.SetParent(worldPlaneParent.transform);
            b.SetParent(worldPlaneParent.transform);

            // Derive normal of the plane, which is the cross product of the two points
            Vector3 normal = (a.up + b.up) / 2;
            Vector3 halfwayPoint = Vector3.Lerp(a.position, b.position, 0.5f);
            Debug.DrawLine(halfwayPoint, halfwayPoint + normal * 5f, Color.blue, 50f);


            // Set plane size to touch the two points
            var dist = Vector3.Distance(a.position, b.position);
            var worldPlaneTrans = worldPlane;
            Vector3 axis = a.position - b.position;
            Vector3 planeForwardDir = Vector3.Cross(normal, axis);
            worldPlaneTrans.LookAt(planeForwardDir, axis);
            worldPlaneTrans.Rotate(Vector3.up, 90f);
            worldPlaneTrans.position = halfwayPoint;
            worldPlaneTrans.localScale = new Vector3(dist, dist, 1f);

            piPlotPlane.SetForward(normal);

            // Update slider
            PlaneTwoPointerSlider.instance.UpdateValues(worldPlane, piPlotPlane);

            worldPlane.GetComponent<MeshRenderer>().material = twoPointPlaneMaterial;

            worldPlanes.AddFirst(worldPlaneParent.transform);
            stereonetPlanes.AddFirst(piPlotPlane);

            PiPlotPlaneButton.instance.UpdateButton();
            LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane.strike, piPlotPlane.dip);
        }
    }

    // Calculates the strike and dip of the current measurement's vector
    private void UpdateLatestMeasurementUIPlane(Vector3 normal)
    {
        float trend;
        float plunge;

        if (normal.y <= 0f)
        {
            normal = -normal;
        }

        if (normal.z < 0f)
        {
            trend = (Mathf.Atan2(normal.x, normal.z) * 180 / Mathf.PI) + 180;
        }
        else
        {
            trend = (Mathf.Atan2(normal.x, normal.z) * 180 / Mathf.PI);
        }

        plunge = (Mathf.Asin(normal.y) * 180 / Mathf.PI);

        if (trend < 0f)
        {
            trend += 180;
        }


        float strike = trend + 90;
        float dip = 90 - plunge;

        // Minus strike by 360 if it's larger than 360
        if (strike >= 360f)
        {
            strike -= 360;
        }

        LatestMeasurementUI.instance.SetPlaneMeasurementInformation(strike, dip);
    }

    // Draws a point in the stereonet (for the line measurements)
    public void AddLinePoint(Transform point)
    {
        point.parent = stereonetLinearParent;
        worldLinePoints.AddFirst(point);

        if (worldLinePoints.Count == 2)
        {
            // Create gameobject prefab, and set its line renderer to connect the two points
            var lineGameObject = Instantiate(linearLinePrefab, linearWorldLinesParent);

            // Get normal of the 2 points, which is the plane forward
            var a = worldLinePoints.First.Value;
            worldLinePoints.RemoveFirst();
            a.parent = lineGameObject.transform;
            var b = worldLinePoints.First.Value;
            worldLinePoints.RemoveFirst();
            b.parent = lineGameObject.transform;

            var lineRenderer = lineGameObject.GetComponent<LineRenderer>();
            lineRenderer.widthMultiplier *= Settings.instance.ObjectScaleMultiplier;
            lineRenderer.SetPositions(new Vector3[] { a.position, b.position});
            lineRenderer.material = flagMaterials[1];

            Vector3 normal = (a.position - b.position).normalized;

            // Instaniate the prefab that will go into the stereonet
            var stereonetPoint = Instantiate(linearStereonetPointPrefab, stereonetLinearParent).GetComponent<PiPlotLine>();

            // Creating a point in the stereonet 
            RaycastHit hit;
            if (!Physics.Raycast(StereonetsController.singleton.originTransform.position, -normal, out hit, 10f, ~stereonetLayer))
            {
                // Second raycast and setting the prefab as unique
                Physics.Raycast(StereonetsController.singleton.originTransform.position, normal, out hit, 10f, ~stereonetLayer);
            }
            stereonetPoint.transform.position = hit.point;
            stereonetPoint.SetData(-hit.normal);

            stereonetLinearPoints.AddFirst(stereonetPoint);
            worldLines.AddFirst(lineGameObject.transform);

            PiPlotLinearButton.instance.UpdateButton();
            UpdateLatestMeasurementUILinear(normal);
        }
    }

    void UpdateLatestMeasurementUILinear(Vector3 vector)
    {
        float trend;
        float plunge;

        if (vector.y <= 0f)
        {
            vector = -vector;
        }

        if (vector.z < 0f)
        {
            trend = (Mathf.Atan2(vector.x, vector.z) * 180 / Mathf.PI) + 180;
        }
        else
        {
            trend = (Mathf.Atan2(vector.x, vector.z) * 180 / Mathf.PI);
        }

        plunge = (Mathf.Asin(vector.y) * 180 / Mathf.PI);

        if (trend < 0f)
        {
            trend += 180;
        }

        LatestMeasurementUI.instance.SetTrendPlungeInformation(trend, plunge);
    }

    private int numCombinedLines = 0;
    public void CalculateAverageLine()
    {
        var combinedNormal = Vector3.zero;
        var numLines = worldLines.Count;

        // Instaniate the prefab that will go into the stereonet
        var stereonetPoint = Instantiate(linearStereonetPointPrefab, stereonetLinearParent).GetComponent<PiPlotLine>();
        stereonetPoint.ConvertToCombinedLine();

        while (worldLines.Count > 0)
        {
            var stereonetLinearPoint = stereonetLinearPoints.First.Value;
            stereonetLinearPoints.RemoveFirst();
            stereonetLinearPoint.GetComponent<MeshRenderer>().material = desaturatedStereonetLinearPointMaterial;
            stereonetPoint.AddCombinedLinearLine(stereonetLinearPoint);

            var currWorldLine = worldLines.First.Value;
            worldLines.RemoveFirst();
            stereonetPoint.AddCombinedWorldLine(currWorldLine);

            combinedNormal += currWorldLine.GetComponent<LineRenderer>().GetPosition(1) - currWorldLine.GetComponent<LineRenderer>().GetPosition(0);
        }
        combinedNormal /= numLines;

        // Creating a point in the stereonet 
        RaycastHit hit;
        if (!Physics.Raycast(StereonetsController.singleton.originTransform.position, -combinedNormal, out hit, 10f, ~stereonetLayer))
        {
            Debug.Log("Pole does not contact the stereonet conventionally - flipping pole");
            // Second raycast and setting the prefab as unique
            Physics.Raycast(StereonetsController.singleton.originTransform.position, combinedNormal, out hit, 10f, ~stereonetLayer);
        }
        stereonetPoint.transform.position = hit.point;
        stereonetPoint.SetData(-hit.normal);
        stereonetLinearPoints.AddFirst(stereonetPoint);
        numCombinedLines++;
    }

    private int numCombinedPlanes = 0;
    /// <summary>
    /// Creates a new stereonet line renderer, and adds it to the top of the stack
    /// so that it's the first one to be undone (nothing else changes) 
    /// </summary>
    public void CalculateAveragePlane()
    {
        var combinedNormal = Vector3.zero;
        var numPlanes = worldPlanes.Count;

        var combinedLine = Instantiate(planeLineRendererPrefab, stereonetPlanesParent).transform;
        var piPlotPlane = combinedLine.GetComponent<PiPlotPlane>();
        piPlotPlane.ConvertToCombinedPlane();
        //piPlotPlane = (PiPlotPlaneCombined)piPlotPlane;

        // Calculates average normal
        // Lowers the importance of the line by desaturating its material
        while (worldPlanes.Count > 0)
        {
            var worldPlane = worldPlanes.First.Value;
            worldPlanes.RemoveFirst();

            var stereonetPlane = stereonetPlanes.First.Value;
            stereonetPlanes.RemoveFirst();
            stereonetPlane.lineRenderer.colorGradient = desaturatedStereonetPlaneLineGradient;

            combinedNormal += stereonetPlane.plane.forward.normalized;

            piPlotPlane.AddCombinedPlane(worldPlane);
            piPlotPlane.AddCombinedPLaneLine(stereonetPlane);
        }
        
        combinedNormal /= numPlanes;

        piPlotPlane.SetForward(combinedNormal);
        stereonetPlanes.AddFirst(piPlotPlane);
        numCombinedPlanes++;
    }

    
    public void ClearPoles() => StartCoroutine(ClearPolesCoroutine());
    IEnumerator ClearPolesCoroutine()
    {
        int numPoints = polePointsParentObj.transform.childCount;
        for (int i = 0; i < numPoints; i++)
        {
            UndoPole();
            yield return new WaitForEndOfFrame();
        }
    }

    public void UndoPole()
    {
        if (polePointsParentObj.transform.childCount <= 0)
        {
            return;
        }

        if (poleElevations.Count > 0)
        {
            poleElevations.RemoveAt(poleElevations.Count - 1);
        }

        Destroy(polePointsParentObj.transform.GetChild(polePointsParentObj.transform.childCount - 1).gameObject);
        Destroy(flagsList[flagsList.Count - 1].gameObject);
        flagsList.RemoveAt(flagsList.Count - 1);
        normals.RemoveFirst();
        points.RemoveFirst();
        data.strikeDipPairs.RemoveFirst();
        data.trendPlungePairs.RemoveFirst();
        FitPlane();

        PIPlotButton.instance.UpdateButton();
    }

    public void UndoPlane()
    {
        if (planePoints.Count > 0)
        {
            Destroy(planePoints.First.Value.gameObject);
            planePoints.RemoveFirst();
        }
        else if (stereonetPlanes.Count > 0)
        {
            
            var stereonetPlane = stereonetPlanes.First.Value;
            stereonetPlanes.RemoveFirst();


            if (stereonetPlane.isCombined)
            {
                // Deleting a combined plane means we need to bring back all of the previous planes
                foreach (var plane in stereonetPlane.combinedPlanes)
                {
                    worldPlanes.AddFirst(plane);
                }
                foreach (var combinedLine in stereonetPlane.combinedPlaneLines)
                {
                    combinedLine.lineRenderer.colorGradient = normalStereonetPlaneLineGradient;
                    stereonetPlanes.AddFirst(combinedLine);
                }
                numCombinedPlanes--;
            } else
            {
                Destroy(worldPlanes.First.Value.gameObject);
                worldPlanes.RemoveFirst();
            }
            Destroy(stereonetPlane.gameObject);
        }
        PiPlotPlaneButton.instance.UpdateButton();

    }

    public void UndoLine()
    {
        if (worldLinePoints.Count > 0)
        {
            Destroy(worldLinePoints.First.Value.gameObject);
            worldLinePoints.RemoveFirst();
        }
        else if (stereonetLinearPoints.Count > 0)
        {
            var piPlotLine = stereonetLinearPoints.First.Value;
            stereonetLinearPoints.RemoveFirst();

            if (piPlotLine.isCombined)
            {
                // Deleting a combined plane means we need to bring back all of the previous planes
                foreach (var combinedWorldLine in piPlotLine.combinedWorldLines)
                {
                    worldLines.AddFirst(combinedWorldLine);
                }
                foreach (var combinedStereonetLinearPoint in piPlotLine.combinedStereonetPoints)
                {
                    combinedStereonetLinearPoint.GetComponent<MeshRenderer>().material = normalStereonetLinearLineMaterial;
                    stereonetLinearPoints.AddFirst(combinedStereonetLinearPoint.GetComponent<PiPlotLine>());
                }
                numCombinedLines--;
            }
            else
            {
                Destroy(worldLines.First.Value.gameObject);
                worldLines.RemoveFirst();
            }

            Destroy(piPlotLine.gameObject);
        } 

        PiPlotLinearButton.instance.UpdateButton();

    }

    public void ClearUnfinishedPlanePoints() => StartCoroutine(ClearUnfinishedPlanePointsCoroutine());
    IEnumerator ClearUnfinishedPlanePointsCoroutine()
    {
        int numPoints = planePoints.Count;
        for (int i = 0; i < numPoints; i++)
        {
            UndoPlane();
            yield return new WaitForEndOfFrame();
        }
    }
    public struct AvgStereonetPoleData
    {
        // Average pole measurement data
        public float avgPoleTrend;
        public float avgPolePlunge;

        public LinkedList<(float, float)> strikeDipPairs;
        public LinkedList<(float, float)> trendPlungePairs;

        public void Clear()
        {
            avgPoleTrend = 0f;
            avgPolePlunge = 0f;
            strikeDipPairs.Clear();
            trendPlungePairs.Clear();
        }

        public bool IsEmpty()
        {
            return strikeDipPairs.Count == 0 && trendPlungePairs.Count == 0;
        }
    }


    public void AddStrikeAndDip(float strike, float dip)
    {
        data.strikeDipPairs.AddFirst((strike, dip));
    }

    public void AddTrendPlunge(float trend, float plunge)
    {
        data.trendPlungePairs.AddFirst((trend, plunge));
    }
    

    /// <summary>
    /// Calculates the trend and plunge of the current list of normals.
    /// Note that this is unique from planes and lineations calculations because
    /// this requires fitting the final final plane again (inefficient).
    /// </summary>
    /// <returns></returns>
    public AvgStereonetPoleData GetAvgStereonetPoleData()
    {
        if (normals.Count == 0)
        {
            data.Clear();
            return data;
        }

        // Bug fix: Since this can be called when switching cards in the dashboard, the finalPlane is actually not updated, so... call FitPlane()
        FitPlane();

        Vector3 planeVector = StereonetsController.singleton.finalPlane.forward;
        StereonetUtils.CalculateTrendAndPlunge(planeVector, out data.avgPoleTrend, out data.avgPolePlunge);

        return data;
    }

    public List<float> GetPoleElevationData()
    {
        return poleElevations;
    }

    // Returns the strike and dip data of all the pole measurements
    private void CalculatePoleStrikeAndDip(out float strike, out float dip)
    {
        strike = 0f;
        dip = 0f;
    }   

    public void SetLineRenderer(bool isEnabled)
    {
        lineRenderer.enabled = isEnabled;
    }

    // ha
    // returns the actual number of points
    private int GetInflatedNumPoints()
    {
        return points.Count;
    }

    public int GetNumPoints()
    {
        return points.Count - NUM_INITIAL_POINTS;
    }

    /// <summary>
    /// Returns the number of un-combined planes
    /// </summary>
    /// <returns></returns>
    public int GetNumPlanes()
    {
        return stereonetPlanes.Count - numCombinedPlanes;
    }

    public int GetNumPlanePoints()
    {
        return planePoints.Count;
    }

    /// <summary>
    /// Returns the number of un-combined lines
    /// </summary>
    /// <returns></returns>
    public int GetNumLines()
    {
        return stereonetLinearPoints.Count - numCombinedLines;
    }
    
    public void AddPoleFlag(Transform flag)
    {
        flagsList.Add(flag);
    }
    
    public void ChangeFlagsMaterial(Material mat, Material twoPointPlaneMaterial)
    {
        flagMaterials = new Material[] { flagMaterials[0], mat };
        this.twoPointPlaneMaterial = twoPointPlaneMaterial;

        // For pole measurements
        if (flagsList.Count != 0)
        {
            var materials = flagsList[0].GetComponent<Flag>().flagMeshRenderer.materials;
            materials[1] = mat;

            foreach (var poleFlag in flagsList)
            {
                poleFlag.GetComponent<Flag>().flagMeshRenderer.materials = materials;
            }
        }
        
        // For the plane measurements
        foreach (var worldPlane in worldPlanes)
        {
            // Note: there are two different types of planes (two point and three point)
            // One way to differentiate them is to check their material properties

            // However, both have the same materials for their child points (the spheres)
            var childPoints = worldPlane.GetComponentsInChildren<MeshRenderer>();
            foreach (var childPoint in childPoints)
            {
                childPoint.material = mat;
            }

            var planeMeshRenderer = worldPlane.GetComponent<MeshRenderer>();
            if (planeMeshRenderer == null)
            {
                // This is a two-point plane
                planeMeshRenderer = worldPlane.GetChild(0).GetComponent<MeshRenderer>();
                planeMeshRenderer.material = twoPointPlaneMaterial;
            }
            else
            {
                // This is a three-point plane
                planeMeshRenderer.material = mat;
            }
        }
        foreach (var point in planePoints)
        {
            point.GetComponent<MeshRenderer>().material = mat;
        }
        foreach (var steronetPlane in stereonetPlanes)
        {
            if (steronetPlane.combinedPlaneLines != null)
            {
                foreach (var combinedPlane in steronetPlane.combinedPlanes)
                {
                    var meshes = combinedPlane.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mesh in meshes)
                    {
                        mesh.material = mat;
                    }
                }
            }
        }

        // For line measurements
        foreach (var worldLine in worldLines)
        {
            worldLine.GetComponent<LineRenderer>().material = mat;

            // Change colors of their children
            for (int i = 0; i < worldLine.childCount; i++)
            {
                worldLine.GetChild(i).GetComponent<MeshRenderer>().material = mat;
            }

        }
        foreach (var worldLinearPoint in worldLinePoints)
        {
            worldLinearPoint.GetComponent<MeshRenderer>().material = mat;
        }
        foreach (var stereonetPoint in stereonetLinearPoints)
        {
            var piPlotLine = stereonetPoint.GetComponent<PiPlotLine>();

            if (piPlotLine.isCombined)
            {
                foreach (var combinedLine in piPlotLine.combinedWorldLines)
                {
                    combinedLine.GetComponent<LineRenderer>().material = mat;
                    var meshes = combinedLine.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mesh in meshes)
                    {
                        mesh.material = mat;
                    }
                }
            }
        }
    }

    // Hid everything except for the flags in the scene
    public void Hide()
    {
        polePointsParentObj.gameObject.SetActive(false);
        model.SetActive(false);
        SetLineRenderer(false);
        finalPoint.gameObject.SetActive(false);

        stereonetPlanesParent.gameObject.SetActive(false);

        stereonetLinearParent.gameObject.SetActive(false);
    }

    public void Show()
    {
        polePointsParentObj.gameObject.SetActive(true);
        model.SetActive(true);
        if (isPiPlotEnabled)
        {
            SetLineRenderer(true);
            finalPoint.gameObject.SetActive(true);
        }
        stereonetPlanesParent.gameObject.SetActive(true);

        stereonetLinearParent.gameObject.SetActive(true);
    }

    public void SetPoleMeasurementAsStale()
    {
        if (latestPoint)
        {
            latestPoint.GetComponent<MeshRenderer>().material = stalePointMaterial;
        }
    }
}
