using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Gradient = UnityEngine.Gradient;


public class Stereonet2D : Stereonet
{
    public static readonly float STEREONET_IMAGE_RADIUS_PERCENTAGE = 0.955f; // Hard coding the % of the stereonet image we're using is the actual stereonet and not white space
    public static readonly float STEREONET_IMAGE_RADIUS = 200f * 0.955f; // Hard coding the radius of the stereonet image we're using
    public static readonly int NUM_CURVE_POINTS = 30;
    
    public Transform measurementsParent; // Transform that has all the measurements parented to it

    [Header("2D Stereonet Rendering")] 
    [SerializeField] public Transform StereonetUIContainer; // The container transform of all of the stereonet's poles and planes 
    [SerializeField] private GameObject PoleImagePrefab;
    [SerializeField] private GameObject lineRendererPrefab;

    // Pole plotting
    [Header("Pole")]
    private RectTransform latestPolePoint;
    [SerializeField] private RectTransform foldAxisPolePoint;
    [SerializeField] private UILineRenderer polePlotLineLineRenderer;
    [SerializeField] private Color poleBeddingColor;
    [SerializeField] private Color latestPoleColor;
    [SerializeField] private Color overturnedPoleColor;

    // Line plotting
    [Header("Lineation")]
    [SerializeField] private Color lineationPoleColor;
    [SerializeField] private Color groupedLinePoleColor;

    // Plane plotting
    [Header("Plane")]
    [SerializeField] private Color planeLineColor;
    [SerializeField] private Color groupedPlaneLineColor;


    [Header("3D Stereonet Rendering")] 
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform measurementsTransform3D;
    [SerializeField] private Transform foldAxisPlane3D;
    [SerializeField] private StereonetPole3D foldAxisPole3D;
    [SerializeField] private Color poleColor3D;
    [SerializeField] private Color lineationPoleColor3D;
    [SerializeField] private Color planeColor3D;
    [SerializeField] private Color groupedLineMeasurementsColor;
    [SerializeField] private Color groupedPlaneMeasurementsColor;
    [SerializeField] private GameObject polePrefab3D;
    [SerializeField] private GameObject planePrefab3D;

    private LinkedList<StereonetPole3D> poles3D;
    private LinkedList<StereonetPole3D> lineationPoles3D;
    private LinkedList<StereonetPlane3D> planes3D;

    protected override void Start()
    {
        base.Start();
        
        // Move the UI elements to the stereonet dashboard
        MoveStereonetUI(StereonetCanvas.Instance.Stereonet2DContainer.transform);
        
        poles3D = new LinkedList<StereonetPole3D>();
        lineationPoles3D = new LinkedList<StereonetPole3D>();
        planes3D = new LinkedList<StereonetPlane3D>();
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
        
        Destroy(StereonetUIContainer);
    }
    
    private void UpdatePolePlottingState()
    {
        if (GetNumPoints() < 3)
        {
            foldAxisPolePoint.gameObject.SetActive(false);
            polePlotLineLineRenderer.enabled = false;
            foldAxisPlane3D.gameObject.SetActive(false);
            foldAxisPole3D.gameObject.SetActive(false);
            isPiPlotEnabled = false;
            PIPlotButton.instance.isToggled = false;
        }
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
        
        // 2D
        foldAxisPolePoint.anchoredPosition = TwoDimensionalStereonetUtils.GetPolePosition(STEREONET_IMAGE_RADIUS, normal);
        var linePoints = TwoDimensionalStereonetUtils.GetPlaneLinePoints(STEREONET_IMAGE_RADIUS, normal, NUM_CURVE_POINTS);
        polePlotLineLineRenderer.Points = linePoints;
        
        // 3D
        var normal3d = modelTransform.TransformDirection(normal);
        foldAxisPlane3D.rotation = Quaternion.LookRotation(normal3d, modelTransform.up);
        foldAxisPole3D.SetNormal(normal.y > 0f ? normal : -normal);
    }
    
    // Draws a point in the 3D stereonet
    public override void AddPole(Vector3 normal, Flag flag)
    {
        normal = -normal;

        var isOverturnedBedding = normal.y > 0f;
        var dirNormal = isOverturnedBedding ? normal : -normal;

        SetLatestPointMeasurementAsStale();

        latestPolePoint = Instantiate(PoleImagePrefab, StereonetUIContainer).GetComponent<RectTransform>();
        latestPolePoint.GetComponent<RawImage>().color = isOverturnedBedding ? overturnedPoleColor : latestPoleColor;
        latestPolePoint.anchoredPosition = TwoDimensionalStereonetUtils.GetPolePosition(STEREONET_IMAGE_RADIUS, dirNormal);
        
        flag.stereonetPoint = latestPolePoint;
        var stereonet3DPointPosition = StereonetsController.instance.originTransform.position - dirNormal * 4.9f;
        polePoints.AddFirst(new PoleMeasurement(stereonet3DPointPosition, dirNormal, isOverturnedBedding, latestPolePoint.gameObject));

        if (StereonetCameraStack.instance)
        {
            StereonetCameraStack.instance.CreatePoint(-normal);
        }

        // 3D
        var normal3d = modelTransform.TransformDirection(dirNormal);
        var pole3D = Instantiate(polePrefab3D, Vector3.zero, Quaternion.identity, measurementsTransform3D).GetComponent<StereonetPole3D>();
        pole3D.SetNormal(normal3d);
        pole3D.SetColor(isOverturnedBedding ? overturnedPoleColor : poleColor3D);
        poles3D.AddFirst(pole3D);
        
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
        var stereonetPointPosition = TwoDimensionalStereonetUtils.GetPolePosition(STEREONET_IMAGE_RADIUS, dirNormal);

        stereonetPoint.position = stereonetPointPosition;
        
        polePoints.AddLast(new PoleMeasurement(stereonetPointPosition, dirNormal, isOverturnedBedding, stereonetPoint.gameObject));
        polePoints.Remove(new PoleMeasurement(oldPos, dirNormal, stereonetPoint.transform.up.z > 0f, stereonetPoint.gameObject)); // TODO
        
        
        // TODO not work done for 3D stereoent

    }

    /// <summary>
    /// Draws a point in the 3D stereonet
    /// Note: is aligned with the points stack
    /// </summary>
    public override void AddPole(Vector3 normal, float elevation, Flag flag)
    {
        AddPole(normal, flag);
        poleElevations.Add(elevation);
    }

    public override void AddPlanePointThreePoint(Transform point)
    {
        point.parent = measurementsParent;
        planePoints.AddFirst(point);
        
        point.transform.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

        if (planePoints.Count == 3)
        {
            // Create gameobject prefab
            var worldPlane = Instantiate(planeWorldPrefab, measurementsParent);
            worldPlane.transform.position = Vector3.zero;
            var stereonetPlane2D = Instantiate(lineRendererPrefab, StereonetUIContainer); // The 2D line renderer
            var piPlotPlane = stereonetPlane2D.AddComponent<PiPlotPlane2D>();
            piPlotPlane.LineRenderer.color = planeLineColor;
            
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
            bool isFlipped = Vector3.Dot(normal, point.up) < 0f;
            normal = isFlipped ? -normal : normal;
            
            piPlotPlane.SetForward(normal);

            // Create mesh
            Mesh planeMesh = worldPlane.GetComponent<MeshFilter>().mesh;

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
            
            // 3D
            var stereonetPlane3D = Instantiate(planePrefab3D, measurementsTransform3D).GetComponent<StereonetPlane3D>();
            stereonetPlane3D.SetNormal(normal, modelTransform);
            stereonetPlane3D.SetColor(planeColor3D);
            planes3D.AddFirst(stereonetPlane3D);
            
            PiPlotPlaneButton.instance.UpdateButton();
            LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane.strike, piPlotPlane.dip);
        }
    }

    public override void AddPlanePointTwoPoint(Transform point)
    {
        point.parent = measurementsParent;
        planePoints.AddFirst(point);

        point.transform.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

        if (planePoints.Count == 2)
        {
            // Create gameobject prefab
            var worldPlaneParent = Instantiate(planeWorldTwoPointPrefab, measurementsParent); // Contains the plane and the two points
            var worldPlane = worldPlaneParent.transform.GetChild(0);
            worldPlane.position = Vector3.zero;
            var stereonetPlane = Instantiate(lineRendererPrefab, StereonetUIContainer); // Essentially the line renderer
            var piPlotPlane = stereonetPlane.AddComponent<PiPlotPlane2D>();
            piPlotPlane.LineRenderer.color = planeLineColor;
            
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

            worldPlane.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

            worldPlanes.AddFirst(worldPlaneParent.transform);
            stereonetPlanes.AddFirst(piPlotPlane);
            
            // 3D
            var stereonetPlane3D = Instantiate(planePrefab3D, measurementsTransform3D).GetComponent<StereonetPlane3D>();
            stereonetPlane3D.SetNormal(normal, modelTransform);
            stereonetPlane3D.SetColor(planeColor3D);
            piPlotPlane.OnRotate.AddListener((forward) =>
            {
                stereonetPlane3D.SetNormal(forward, modelTransform);
            });
            planes3D.AddFirst(stereonetPlane3D);

            PiPlotPlaneButton.instance.UpdateButton();
            LatestMeasurementUI.instance.SetPlaneMeasurementInformation(piPlotPlane.strike, piPlotPlane.dip);
        }
    }

    // Draws a point in the stereonet (for the line measurements)
    public override void AddLinePoint(Transform point)
    {
        point.parent = measurementsParent;
        worldLinePoints.AddFirst(point);
        
        point.transform.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, stereonetColor);

        if (worldLinePoints.Count == 2)
        {
            // Create gameobject prefab, and set its line renderer to connect the two points
            var lineGameObject = Instantiate(linearLinePrefab, measurementsParent);

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
            lineRenderer.material.SetColor(_colorProperty, stereonetColor);

            Vector3 normal = (a.position - b.position).normalized;
            var isOverturned = normal.y > 0f;
            normal = isOverturned ? -normal : normal;

            // Instaniate the prefab that will go into the stereonet
            var stereonetPointObj = Instantiate(PoleImagePrefab, StereonetUIContainer);
            var stereonetPoint = stereonetPointObj.gameObject.AddComponent<PiPlotLine>();
            stereonetPointObj.GetComponent<RawImage>().color = lineationPoleColor;
            
            stereonetPoint.SetData(normal);
            stereonetPoint.GetComponent<RectTransform>().anchoredPosition = TwoDimensionalStereonetUtils.GetPolePosition(STEREONET_IMAGE_RADIUS, stereonetPoint.trend, stereonetPoint.plunge);

            stereonetLinearPoints.AddFirst(stereonetPoint);
            worldLines.AddFirst(lineGameObject.transform);
            
            // 3D
            var normal3d = modelTransform.TransformDirection(normal);
            var stereonetLineationPole3D = Instantiate(polePrefab3D, Vector3.zero, Quaternion.identity, measurementsTransform3D).GetComponent<StereonetPole3D>();
            stereonetLineationPole3D.SetNormal(-normal3d);
            stereonetLineationPole3D.SetColor(lineationPoleColor3D);
            lineationPoles3D.AddFirst(stereonetLineationPole3D);
            

            PiPlotLinearButton.instance.UpdateButton();
            UpdateLatestMeasurementUILinear(normal);
        }
    }

    void UpdateLatestMeasurementUILinear(Vector3 vector)
    {
        StereonetUtils.CalculateTrendAndPlunge(vector, out var trend, out var plunge);
        LatestMeasurementUI.instance.SetTrendPlungeInformation(trend, plunge);
    }

    private int numCombinedLines = 0;
    public override void CalculateAverageLine()
    {
        var combinedNormal = Vector3.zero;

        // Instantiate the prefab that will go into the stereonet
        var stereonetPointObj = Instantiate(PoleImagePrefab, StereonetUIContainer);
        var stereonetPoint = stereonetPointObj.AddComponent<PiPlotLine>();
        stereonetPoint.GetComponent<RawImage>().color = lineationPoleColor;
        stereonetPoint.ConvertToCombinedLine();

        while (worldLines.Count > 0)
        {
            var stereonetLinearPoint = stereonetLinearPoints.First.Value;
            stereonetLinearPoints.RemoveFirst();
            stereonetLinearPoint.GetComponent<RawImage>().color = groupedLinePoleColor;
            stereonetPoint.AddCombinedLinearLine(stereonetLinearPoint);

            var currWorldLine = worldLines.First.Value;
            worldLines.RemoveFirst();
            stereonetPoint.AddCombinedWorldLine(currWorldLine);

            var line = currWorldLine.GetComponent<LineRenderer>();
            combinedNormal += line.GetPosition(1) - line.GetPosition(0);
            
            // 3D 
            var linePole = lineationPoles3D.First.Value;
            lineationPoles3D.RemoveFirst();
            linePole.SetColor(groupedLineMeasurementsColor);
            stereonetPoint.combinedStereonetLines3D.Enqueue(linePole);
        }
        combinedNormal = combinedNormal.normalized;
        
        // Creating a point in the stereonet 
        stereonetPoint.GetComponent<RectTransform>().anchoredPosition = TwoDimensionalStereonetUtils.GetPolePosition(STEREONET_IMAGE_RADIUS, combinedNormal);
        stereonetLinearPoints.AddFirst(stereonetPoint);
        numCombinedLines++;
        
        // 3D
        var normal3d = modelTransform.TransformDirection(combinedNormal);
        var stereonetLineationPole3D = Instantiate(polePrefab3D, Vector3.zero, Quaternion.identity, measurementsTransform3D).GetComponent<StereonetPole3D>();
        stereonetLineationPole3D.SetNormal(normal3d);
        stereonetLineationPole3D.SetColor(lineationPoleColor3D);
        lineationPoles3D.AddFirst(stereonetLineationPole3D);

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

        var combinedLine = Instantiate(lineRendererPrefab, StereonetUIContainer);
        var piPlotPlane = combinedLine.AddComponent<PiPlotPlane2D>();
        piPlotPlane.ConvertToCombinedPlane();
        piPlotPlane.LineRenderer.color = planeLineColor;

        // Calculates average normal
        // Lowers the importance of the line by desaturating its material
        while (worldPlanes.Count > 0)
        {
            var worldPlane = worldPlanes.First.Value;
            worldPlanes.RemoveFirst();

            var stereonetPlane = stereonetPlanes.First.Value as PiPlotPlane2D;
            stereonetPlanes.RemoveFirst();
            stereonetPlane.LineRenderer.color = groupedPlaneLineColor;

            combinedNormal += stereonetPlane.forward.normalized;

            piPlotPlane.AddCombinedPlane(worldPlane);
            piPlotPlane.AddCombinedPLaneLine(stereonetPlane);
            
            // 3D
            var groupedStereonetPlane3D = planes3D.First.Value;
            planes3D.RemoveFirst();
            groupedStereonetPlane3D.SetColor(groupedPlaneMeasurementsColor);
            piPlotPlane.stereonetPlanes3D.Enqueue(groupedStereonetPlane3D);
        }
        combinedNormal /= numPlanes;

        piPlotPlane.SetForward(combinedNormal);
        stereonetPlanes.AddFirst(piPlotPlane);
        numCombinedPlanes++;
        
        // 3D
        var stereonetPlane3D = Instantiate(planePrefab3D, measurementsTransform3D).GetComponent<StereonetPlane3D>();
        stereonetPlane3D.SetNormal(combinedNormal, modelTransform);
        stereonetPlane3D.SetColor(planeColor3D);
        planes3D.AddFirst(stereonetPlane3D);

        StereonetCamera.instance.UpdateStereonet();
    }

    
    public override void ClearPoles() => StartCoroutine(ClearPolesCoroutine());
    IEnumerator ClearPolesCoroutine()
    {
        int numPoints = polePoints.Count;
        for (int i = 0; i < numPoints; i++)
        {
            UndoPole();
            yield return new WaitForEndOfFrame();
        }
    }

    public override void UndoPole()
    {
        if (polePoints.Count <= 0)
        {
            return;
        }

        if (poleElevations.Count > 0)
        {
            poleElevations.RemoveAt(poleElevations.Count - 1);
        }

        var point = polePoints.First.Value;

        Destroy(point.StereonetPointObj);
        Destroy(flagsList[flagsList.Count - 1].gameObject);
        polePoints.RemoveFirst();
        flagsList.RemoveAt(flagsList.Count - 1);
        avgStereonetPoleData.strikeDipPairs.RemoveFirst();
        avgStereonetPoleData.trendPlungePairs.RemoveFirst();
        FitPlane();

        PIPlotButton.instance.UpdateButton();
        
        // 3D
        var pole3D = poles3D.First.Value;
        Destroy(pole3D.gameObject);
        poles3D.RemoveFirst();

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
            var stereonetPlane = stereonetPlanes.First.Value as PiPlotPlane2D;
            stereonetPlanes.RemoveFirst();
            
            if (stereonetPlane.isCombined)
            {
                // Deleting a combined plane means we need to bring back all of the previous planes
                foreach (var plane in stereonetPlane.combinedPlanes)
                {
                    worldPlanes.AddFirst(plane);
                }
                foreach (var combinedPlane in stereonetPlane.combinedPlaneLines)
                {
                    var line2D = combinedPlane as PiPlotPlane2D;
                    line2D.LineRenderer.color = planeLineColor;
                    stereonetPlanes.AddFirst(combinedPlane);
                }
                
                // 3D
                Destroy(planes3D.First.Value.gameObject);
                planes3D.RemoveFirst();
                foreach (var combinedPlane3D in stereonetPlane.stereonetPlanes3D)
                {
                    combinedPlane3D.SetColor(planeColor3D);
                    planes3D.AddFirst(combinedPlane3D);
                }
                numCombinedPlanes--;
            } else
            {
                Destroy(worldPlanes.First.Value.gameObject);
                worldPlanes.RemoveFirst();
                
                // 3D
                Destroy(planes3D.First.Value.gameObject);
                planes3D.RemoveFirst();
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
                // Deleting a combined measurement means we need to bring back all of the grouped measurements back
                foreach (var combinedWorldLine in piPlotLine.combinedWorldLines)
                {
                    worldLines.AddFirst(combinedWorldLine);
                }
                foreach (var combinedStereonetLinearPoint in piPlotLine.combinedStereonetPoints)
                {
                    combinedStereonetLinearPoint.GetComponent<RawImage>().color = lineationPoleColor;
                    stereonetLinearPoints.AddFirst(combinedStereonetLinearPoint.GetComponent<PiPlotLine>());
                }
                
                // 3D
                Destroy(lineationPoles3D.First.Value.gameObject);
                lineationPoles3D.RemoveFirst();
                foreach (var combinedStereonetLine3D in piPlotLine.combinedStereonetLines3D)
                {
                    combinedStereonetLine3D.SetColor(lineationPoleColor3D);
                    lineationPoles3D.AddFirst(combinedStereonetLine3D);
                }
                numCombinedLines--;
            }
            else
            {
                Destroy(worldLines.First.Value.gameObject);
                worldLines.RemoveFirst();
                
                // 3D
                Destroy(lineationPoles3D.First.Value.gameObject);
                lineationPoles3D.RemoveFirst();
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
        
        Vector3 planeVector = StereonetsController.instance.finalPlane.forward;
        StereonetUtils.CalculateTrendAndPlunge(planeVector, out avgStereonetPoleData.avgPoleTrend, out avgStereonetPoleData.avgPolePlunge);

        return avgStereonetPoleData;
    }
    
    public override void SetPoleLineRendererState(bool isEnabled)
    {
        foldAxisPolePoint.gameObject.SetActive(isEnabled);
        polePlotLineLineRenderer.enabled = isEnabled;
        
        foldAxisPlane3D.gameObject.SetActive(isEnabled);
        foldAxisPole3D.gameObject.SetActive(isEnabled);
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
        foreach (var stereonetPlane in stereonetPlanes)
        {
            var piPlotPlane2D = stereonetPlane as PiPlotPlane2D;
            if (piPlotPlane2D.combinedPlaneLines != null)
            {
                foreach (var combinedPlane in stereonetPlane.combinedPlanes)
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

    public override void Hide()
    {
        modelTransform.gameObject.SetActive(false);
    }

    public override void Show()
    {
        modelTransform.gameObject.SetActive(true);
    }

    public override void SetLatestPointMeasurementAsStale()
    {
        if (latestPolePoint && !polePoints.First.Value.IsOverturned)
        {
            latestPolePoint.GetComponent<RawImage>().color = poleBeddingColor;
        }
    }

    public void MoveStereonetUI(Transform newParent)
    {
        var parentRect = newParent.GetComponent<RectTransform>().rect;
        Assert.AreApproximatelyEqual(parentRect.width, parentRect.height); // The stereonet container should be square
        
        StereonetUIContainer.SetParent(newParent);
        StereonetUIContainer.transform.localScale = Vector3.one * parentRect.width / 400f; // Scale accordingly to the new container
        StereonetUIContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }



    /// <summary>
    /// Rotates the 3D stereonet model
    /// </summary>
    public override void RotateModel()
    {
        var delta = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * 10f;

        modelTransform.Rotate(Vector3.right, delta.x, Space.World);
        modelTransform.Rotate(Vector3.up, -delta.y);
    }
}