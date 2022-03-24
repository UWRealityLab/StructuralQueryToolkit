using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Gradient = UnityEngine.Gradient;

public abstract class Stereonet : MonoBehaviour
{
    public int id;
    
    protected Color stereonetColor;

    protected const int NUM_INITIAL_POINTS = 0; // Points to add to the middle of the stereonet to have better line fitting (for pole plotting)

    // Used to store the default measurement groups' names 
    [HideInInspector] public Measurement defaultPlaneMeasurement;
    [HideInInspector] public Measurement defaultLineationMeasurement;

    public abstract void Show();
    public abstract void Hide();

    // Pole
    protected LinkedList<PoleMeasurement> polePoints;
    public List<float> poleElevations { get; private set; }
    protected List<Transform> flagsList;
    protected AvgStereonetPoleData avgStereonetPoleData;
    [HideInInspector] public bool isPiPlotEnabled;
    public abstract void SetLatestPointMeasurementAsStale();
    public abstract int GetNumPoints();
    public abstract void SetPoleLineRendererState(bool isEnabled);
    public abstract void UndoPole();
    public abstract void ClearPoles();
    public abstract void AddStrikeAndDip(float strike, float dip);
    public abstract void AddTrendPlunge(float trend, float plunge);
    public abstract void AddPole(Vector3 meanVector, float elevation, Flag flagComponent);
    public abstract void AddPole(Vector3 meanVector, Flag flagComponent);
    public abstract void AddPoleFlag(Transform flag);
    public abstract void AddPoleFlag(Transform flag, Transform hitTransform);
    public abstract void ChangeFlagsMaterial(Color colorButtonColor);
    public abstract AvgStereonetPoleData GetAvgStereonetPoleData();
    public abstract void ChangePoleData(Flag flag);


    // Line
    [SerializeField] protected GameObject linearLinePrefab;
    
    public LinkedList<PiPlotLine> stereonetLinearPoints;
    public abstract void AddLinePoint(Transform linePoint);
    public abstract void UndoLine();

    // Plane
    [SerializeField] protected GameObject planeWorldPrefab;
    [SerializeField] protected GameObject planeWorldTwoPointPrefab; // Two point variant (it's a square plane)

    public LinkedList<PiPlotPlane> stereonetPlanes;
    protected LinkedList<Transform> planePoints;
    protected LinkedList<Transform> worldPlanes;
    protected LinkedList<Transform> worldLinePoints; // In reality, this will only be 1 or 2 points 
    protected LinkedList<Transform> worldLines;
    public abstract void AddPlanePointTwoPoint(Transform planePoint);
    public abstract void AddPlanePointThreePoint(Transform planePoint);
    public abstract void ClearUnfinishedPlanePoints();
    public abstract void UndoPlane();
    public abstract void CalculateAverageLine();
    public abstract int GetNumLines();
    public abstract void CalculateAveragePlane();
    public abstract int GetNumPlanes();
    
    
    // 3D representation
    public abstract void RotateModel();
    
    // VR 
    public abstract void RenderCamera();

    protected virtual void Awake()
    {
        stereonetColor = new Color32(252, 141, 82, 255);

        polePoints = new LinkedList<PoleMeasurement>();
        poleElevations = new List<float>();
        flagsList = new List<Transform>();
        
        stereonetPlanes = new LinkedList<PiPlotPlane>();
        planePoints = new LinkedList<Transform>();
        worldPlanes = new LinkedList<Transform>();

        worldLinePoints = new LinkedList<Transform>();
        worldLines = new LinkedList<Transform>();
        stereonetLinearPoints = new LinkedList<PiPlotLine>();

        for (int i = 0; i < NUM_INITIAL_POINTS; i++)
        {
            polePoints.AddFirst(new PoleMeasurement(Vector3.zero, false, null));
        }

        defaultPlaneMeasurement = new PiPlotPlane3D();
        defaultLineationMeasurement = new PiPlotLine();
        
        avgStereonetPoleData = new AvgStereonetPoleData();
        avgStereonetPoleData.strikeDipPairs = new LinkedList<(float, float)>();
        avgStereonetPoleData.trendPlungePairs = new LinkedList<(float, float)>();
    }
    
    protected virtual void Start()
    {
        
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