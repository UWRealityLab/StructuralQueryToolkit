
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stereonet3D : Stereonet
{
    [SerializeField] GameObject pointPrefab;
    public Transform pointPlanesParent;
    [SerializeField] Transform stereonetPlanesParent;
    [SerializeField] Transform stereonetLinearParent;
    [SerializeField] Transform linearWorldLinesParent;
    [SerializeField] Transform polePointsParentObj;
    [SerializeField] GameObject finalPointPrefab;
    [SerializeField] GameObject model;
    [SerializeField] GameObject specialPointPrefab;
    [SerializeField] Material stalePointMaterial;

    int stereonetLayer;

    private Transform finalPoint;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public Transform latestPoint;
    
    // For measuring planes (where measuring 3 points will create a plane)
    [SerializeField] GameObject planeLineRendererPrefab;
    [SerializeField] Gradient normalStereonetPlaneLineGradient;
    [SerializeField] Gradient desaturatedStereonetPlaneLineGradient;
    
    // For measuring lines (2 points)
    [SerializeField] GameObject linearStereonetPointPrefab;
    [SerializeField] Material normalStereonetLinearLineMaterial;
    [SerializeField] Material desaturatedStereonetLinearPointMaterial;

    private void Awake()
    {
        base.Awake();
        
        lineRenderer = GetComponent<LineRenderer>();
        stereonetLayer = LayerMask.NameToLayer("Stereonet");
        finalPoint = Instantiate(finalPointPrefab, Vector3.zero, Quaternion.identity, transform).transform;
    }

    private void Start()
    {
        base.Start();
        finalPoint.gameObject.SetActive(false); // You have to put this line in Start() instead of Awake() for it to work
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
    
    private void UpdatePolePlottingState()
    {
        if (GetNumPoints() < 3)
        {
            finalPoint.gameObject.SetActive(false);
            lineRenderer.enabled = false;
            isPiPlotEnabled = false;
            PIPlotButton.instance.isToggled = false;
            return;
        }
        finalPoint.gameObject.SetActive(true);
    }

    private void FitPlane()
    {
        // Only fit the plane if the user has 3 measurements or more
        if (GetNumPoints() < 3)
        {
            return;
        }

        List<Vector3> pointsList = new List<Vector3>();

        Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
        foreach (var pole in polePoints)
        {
            pointsList.Add(pole.Position);
            sum += pole.Position;
        }

        Vector3 centroid = sum * (1.0f / pointsList.Count);

        //Calculate determinants from matrix components
        float xx = 0.0f; float xy = 0.0f; float xz = 0.0f;
        float yy = 0.0f; float yz = 0.0f; float zz = 0.0f;

        foreach (var pole in polePoints)
        {
            Vector3 r = pole.Position - centroid;
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
        StereonetsController.instance.finalPlane.forward = normal;

        var finalPlane = StereonetsController.instance.finalPlane;
        var finalPlaneLeftCorner = StereonetsController.instance.finalPlaneLeftCorner;
        var finalPlaneRightCorner = StereonetsController.instance.finalPlaneRightCorner;
        
        var isOverTurned = normal.y > 0f; 
        finalPoint.position = StereonetsController.instance.originTransform.position + (isOverTurned ? (-normal * 5f) : (normal * 5f));

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

            Debug.DrawLine(StereonetsController.instance.originTransform.position, points[i], Color.blue, 3f);
        }

        return points;
    }


    // Draws a point in the 3D stereonet
    public override void AddPole(Vector3 normal, Flag flag)
    {
        normal = -normal;

        var isOverturnedBedding = normal.y > 0f;
        var dirNormal = isOverturnedBedding ? normal : -normal;
        var stereonetPointPosition = StereonetsController.instance.originTransform.position - dirNormal * 4.9f;

        SetLatestPointMeasurementAsStale();

        latestPoint = Instantiate(isOverturnedBedding ? specialPointPrefab : pointPrefab, stereonetPointPosition, Quaternion.identity, polePointsParentObj).transform;
        
        flag.stereonetPoint = latestPoint;
        polePoints.AddFirst(new PoleMeasurement(stereonetPointPosition, dirNormal, isOverturnedBedding, latestPoint.gameObject));

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.CreatePoint(-normal);
        }

        UpdatePolePlottingState();

        FitPlane();
    }

    private bool hasChanged = false;
    public override void ChangePoleData(Vector3 flagUp, Transform stereonetPoint)
    {
        hasChanged = true;
        var oldPos = stereonetPoint.position;

        var normal = flagUp;
        var isOverturnedBedding = normal.y > 0f;
        var dirNormal = isOverturnedBedding ? normal : -normal;
        var stereonetPointPosition = StereonetsController.instance.originTransform.position - dirNormal * 4.9f;

        stereonetPoint.position = stereonetPointPosition;
        
        polePoints.AddLast(new PoleMeasurement(stereonetPointPosition, dirNormal, isOverturnedBedding, stereonetPoint.gameObject));
        polePoints.Remove(new PoleMeasurement(oldPos, dirNormal, stereonetPoint.transform.up.z > 0f, stereonetPoint.gameObject)); // TODO

    }

    /// <summary>
    /// Note: is aligned with the points stack
    /// </summary>
    // Draws a point in the 3D stereonet
    public override void AddPole(Vector3 normal, float elevation, Flag flag)
    {
        AddPole(normal, flag);
        poleElevations.Add(elevation);
    }

    public override void AddPlanePointThreePoint(Transform point)
    {
        point.parent = pointPlanesParent;
        planePoints.AddFirst(point);
        
        point.transform.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

        if (planePoints.Count == 3)
        {
            // Create gameobject prefab
            var worldPlane = Instantiate(planeWorldPrefab, pointPlanesParent);
            worldPlane.transform.position = Vector3.zero;
            var stereonetPlane = Instantiate(planeLineRendererPrefab, stereonetPlanesParent); // Essentially the line renderer
            var piPlotPlane = stereonetPlane.GetComponent<PiPlotPlane3D>();

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

            worldPlane.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);
            
            worldPlanes.AddFirst(worldPlane.transform);
            stereonetPlanes.AddFirst(piPlotPlane);

            PiPlotPlaneButton.instance.UpdateButton();
            LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane.strike, piPlotPlane.dip);
        }
    }

    public override void AddPlanePointTwoPoint(Transform point)
    {
        point.parent = pointPlanesParent;
        planePoints.AddFirst(point);

        point.transform.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

        if (planePoints.Count == 2)
        {
            // Create gameobject prefab
            var worldPlaneParent = Instantiate(planeWorldTwoPointPrefab, pointPlanesParent); // Contains the plane and the two points
            var worldPlane = worldPlaneParent.transform.GetChild(0);
            worldPlane.position = Vector3.zero;
            var stereonetPlane = Instantiate(planeLineRendererPrefab, stereonetPlanesParent); // Essentially the line renderer
            PiPlotPlane3D piPlotPlane3D = stereonetPlane.GetComponent<PiPlotPlane3D>();

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

            piPlotPlane3D.SetForward(normal);

            // Update slider
            PlaneTwoPointerSlider.instance.UpdateValues(worldPlane, piPlotPlane3D);

            worldPlane.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

            worldPlanes.AddFirst(worldPlaneParent.transform);
            stereonetPlanes.AddFirst(piPlotPlane3D);

            PiPlotPlaneButton.instance.UpdateButton();
            LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane3D.strike, piPlotPlane3D.dip);
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
    public override void AddLinePoint(Transform point)
    {
        point.parent = stereonetLinearParent;
        worldLinePoints.AddFirst(point);
        
        point.transform.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

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
            //lineRenderer.startColor = stereonetColor;
            //lineRenderer.endColor = stereonetColor;
            lineRenderer.material.SetColor(_colorProperty, stereonetColor);

            Vector3 normal = (a.position - b.position).normalized;

            // Instaniate the prefab that will go into the stereonet
            var stereonetPoint = Instantiate(linearStereonetPointPrefab, stereonetLinearParent).GetComponent<PiPlotLine>();

            // Creating a point in the stereonet 
            RaycastHit hit;
            if (!Physics.Raycast(StereonetsController.instance.originTransform.position, -normal, out hit, 10f, ~stereonetLayer))
            {
                // Second raycast and setting the prefab as unique
                Physics.Raycast(StereonetsController.instance.originTransform.position, normal, out hit, 10f, ~stereonetLayer);
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
    public override void CalculateAverageLine()
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
        if (!Physics.Raycast(StereonetsController.instance.originTransform.position, -combinedNormal, out hit, 10f, ~stereonetLayer))
        {
            Debug.Log("Pole does not contact the stereonet conventionally - flipping pole");
            // Second raycast and setting the prefab as unique
            Physics.Raycast(StereonetsController.instance.originTransform.position, combinedNormal, out hit, 10f, ~stereonetLayer);
        }
        stereonetPoint.transform.position = hit.point;
        stereonetPoint.SetData(-hit.normal);
        stereonetLinearPoints.AddFirst(stereonetPoint);
        numCombinedLines++;
        
        StereonetCamera.instance.UpdateStereonet();
    }

    private int numCombinedPlanes = 0;
    private static readonly int _colorProperty = Shader.PropertyToID("_BaseColor");

    /// <summary>
    /// Creates a new stereonet line renderer, and adds it to the top of the stack
    /// so that it's the first one to be undone (nothing else changes) 
    /// </summary>
    public override void CalculateAveragePlane()
    {
        var combinedNormal = Vector3.zero;
        var numPlanes = worldPlanes.Count;

        var combinedLine = Instantiate(planeLineRendererPrefab, stereonetPlanesParent).transform;
        var piPlotPlane = combinedLine.GetComponent<PiPlotPlane3D>();
        piPlotPlane.ConvertToCombinedPlane();
        //piPlotPlane = (PiPlotPlaneCombined)piPlotPlane;

        // Calculates average normal
        // Lowers the importance of the line by desaturating its material
        while (worldPlanes.Count > 0)
        {
            var worldPlane = worldPlanes.First.Value;
            worldPlanes.RemoveFirst();

            var stereonetPlane = stereonetPlanes.First.Value as PiPlotPlane3D;
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
        
        StereonetCamera.instance.UpdateStereonet();
    }

    
    public override void ClearPoles() => StartCoroutine(ClearPolesCoroutine());
    IEnumerator ClearPolesCoroutine()
    {
        int numPoints = polePointsParentObj.transform.childCount;
        for (int i = 0; i < numPoints; i++)
        {
            UndoPole();
            yield return new WaitForEndOfFrame();
        }
    }

    public override void UndoPole()
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
        polePoints.RemoveFirst();
        avgStereonetPoleData.strikeDipPairs.RemoveFirst();
        avgStereonetPoleData.trendPlungePairs.RemoveFirst();
        FitPlane();

        PIPlotButton.instance.UpdateButton();

        UpdatePolePlottingState();
    }

    public override void UndoPlane()
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
                    ((PiPlotPlane3D) combinedLine).lineRenderer.colorGradient = normalStereonetPlaneLineGradient;
                    stereonetPlanes.AddFirst(combinedLine as PiPlotPlane3D);
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

    public override void UndoLine()
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

    public override void ClearUnfinishedPlanePoints() => StartCoroutine(ClearUnfinishedPlanePointsCoroutine());
    IEnumerator ClearUnfinishedPlanePointsCoroutine()
    {
        int numPoints = planePoints.Count;
        for (int i = 0; i < numPoints; i++)
        {
            UndoPlane();
            yield return new WaitForEndOfFrame();
        }
    }

    public override void AddStrikeAndDip(float strike, float dip)
    {
        avgStereonetPoleData.strikeDipPairs.AddFirst((strike, dip));
    }

    public override void AddTrendPlunge(float trend, float plunge)
    {
        avgStereonetPoleData.trendPlungePairs.AddFirst((trend, plunge));
    }
    

    /// <summary>
    /// Calculates the trend and plunge of the current list of normals.
    /// Note that this is unique from planes and lineations calculations because
    /// this requires fitting the final final plane again (inefficient).
    /// </summary>
    /// <returns></returns>
    public override AvgStereonetPoleData GetAvgStereonetPoleData()
    {
        if (polePoints.Count == 0)
        {
            avgStereonetPoleData.Clear();
            return avgStereonetPoleData;
        }

        // Bug fix: Since this can be called when switching cards in the dashboard, the finalPlane is actually not updated, so... call FitPlane()
        FitPlane();

        Vector3 planeVector = StereonetsController.instance.finalPlane.forward;
        StereonetUtils.CalculateTrendAndPlunge(planeVector, out avgStereonetPoleData.avgPoleTrend, out avgStereonetPoleData.avgPolePlunge);

        return avgStereonetPoleData;
    }

    // Returns the strike and dip data of all the pole measurements
    private void CalculatePoleStrikeAndDip(out float strike, out float dip)
    {
        strike = 0f;
        dip = 0f;
    }   

    public override void SetPoleLineRendererState(bool isEnabled)
    {
        lineRenderer.enabled = isEnabled;
    }

    // ha
    // returns the actual number of points
    private int GetInflatedNumPoints()
    {
        return polePoints.Count;
    }

    public override int GetNumPoints()
    {
        return polePoints.Count - NUM_INITIAL_POINTS;
    }

    /// <summary>
    /// Returns the number of un-combined planes
    /// </summary>
    /// <returns></returns>
    public override int GetNumPlanes()
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
    public override int GetNumLines()
    {
        return stereonetLinearPoints.Count - numCombinedLines;
    }
    
    public override void AddPoleFlag(Transform flag)
    {
        flag.GetComponent<Flag>().flagMeshRenderer.materials[1].SetColor(_colorProperty, stereonetColor);
        flagsList.Add(flag);
    }

    public override void AddPoleFlag(Transform flag, Transform hitTransform)
    {
        flag.SetParent(hitTransform.transform, true);
        flag.GetComponent<Flag>().flagMeshRenderer.materials[1].SetColor(_colorProperty, stereonetColor);
        flagsList.Add(flag);
    }
    
    public override void ChangeFlagsMaterial(Color color)
    {
        stereonetColor = color;

        // For pole measurements
        if (flagsList.Count != 0)
        {
            foreach (var poleFlag in flagsList)
            {
                poleFlag.GetComponent<Flag>().flagMeshRenderer.materials[1].SetColor(_colorProperty, color);
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
                childPoint.material.SetColor(_colorProperty, color);
            }

            var planeMeshRenderer = worldPlane.GetComponent<MeshRenderer>();
            if (planeMeshRenderer == null)
            {
                // This is a two-point plane
                planeMeshRenderer = worldPlane.GetChild(0).GetComponent<MeshRenderer>();
                planeMeshRenderer.material.SetColor(_colorProperty, color);
            }
            else
            {
                // This is a three-point plane
                planeMeshRenderer.material.SetColor(_colorProperty, color);
            }
        }
        foreach (var point in planePoints)
        {
            point.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, color);
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
                        mesh.material.SetColor(_colorProperty, color);
                    }
                }
            }
        }

        // For line measurements
        foreach (var worldLine in worldLines)
        {
            worldLine.GetComponent<LineRenderer>().material.SetColor(_colorProperty, color);

            // Change colors of their children
            for (int i = 0; i < worldLine.childCount; i++)
            {
                worldLine.GetChild(i).GetComponent<MeshRenderer>().material.SetColor(_colorProperty, color);
            }

        }
        foreach (var worldLinearPoint in worldLinePoints)
        {
            worldLinearPoint.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, color);
        }
        foreach (var stereonetPoint in stereonetLinearPoints)
        {
            var piPlotLine = stereonetPoint.GetComponent<PiPlotLine>();

            if (piPlotLine.isCombined)
            {
                foreach (var combinedLine in piPlotLine.combinedWorldLines)
                {
                    combinedLine.GetComponent<LineRenderer>().material.SetColor(_colorProperty, color);
                    var meshes = combinedLine.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mesh in meshes)
                    {
                        mesh.material.SetColor(_colorProperty, color);
                    }
                }
            }
        }
    }

    // Hid everything except for the flags in the scene
    public override void Hide()
    {
        polePointsParentObj.gameObject.SetActive(false);
        model.SetActive(false);
        SetPoleLineRendererState(false);
        finalPoint.gameObject.SetActive(false);

        stereonetPlanesParent.gameObject.SetActive(false);

        stereonetLinearParent.gameObject.SetActive(false);
    }

    public override void Show()
    {
        polePointsParentObj.gameObject.SetActive(true);
        model.SetActive(true);
        if (isPiPlotEnabled)
        {
            SetPoleLineRendererState(true);
            finalPoint.gameObject.SetActive(true);
        }
        stereonetPlanesParent.gameObject.SetActive(true);

        stereonetLinearParent.gameObject.SetActive(true);
    }

    public override void SetLatestPointMeasurementAsStale()
    {
        if (latestPoint && !polePoints.First.Value.IsOverturned)
        {
            latestPoint.GetComponent<MeshRenderer>().material = stalePointMaterial;
        }
    }
    
    public override void RotateModel()
    {
        throw new System.NotImplementedException();
    }
}