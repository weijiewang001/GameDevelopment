using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProgressEvaluator : MonoBehaviour
{
    private enum GradeBand { Deactivate, Pass, Credit, Distinction, HighDist90, HighDist100};
    private Action[] evalMethods; 
    [SerializeField] private uint studentNumber = 0;
    [SerializeField] private GradeBand bandReached = GradeBand.Deactivate;
    //[SerializeField] private bool showSuccessMessages = false;

    private String lastLog;

    // Start is called before the first frame update
    void Start()
    {
        evalMethods = new Action[] { () => PassBand(), () => CreditBand(), () => DistinctionBand(), () => HighDist90Band(), () => HighDist100Band() };
        GradeBand loadedBand = EvalReader();
        //Debug.LogWarning("LOADED BAND == " + loadedBand);

        int i = 0;
        try
        {         
            while (i < (int)bandReached)
            {
                if (i == 0)
                    CheckStudentNum();
                GradeBand currentBand = (GradeBand)i + 1;
                evalMethods[i]();
                EvalPassMessage(currentBand.ToString());
                if (currentBand > loadedBand)
                    EvalOutput(currentBand);
                i++;
            }            
        } catch (EvalFailedException e)
        {
            EvalFailMessage(((GradeBand)i+1).ToString(), e.Message);
        } catch (Exception e)
        {
            EvalFailMessage(((GradeBand)i + 1).ToString(), "An unknown error occured during the progress evaluation. It is likely " +
                "that you have made a mistake that the ProgressEvaluator system has not been setup to handle. Go back over the " +
                "steps for this grade band and see if you can spot anything wrong or ask your tutor. The full error is below for reference: ");
            Debug.LogError(e.GetType() + ": " + e.Message);
        }        
    }      


    private void PassBand()
    {
        // Test: RedSphere transform
        GameObject redSphere = GameObject.Find("RedSphere");
        if (redSphere == null)
            throw new EvalFailedException("No GameObject named RedSphere was found.");
        if (!VectApprox(redSphere.transform.position, new Vector3(2, 0, 5)))
            throw new EvalFailedException("RedSphere is not located at position (2, 0, 5).");
        if (!VectApprox(redSphere.transform.localScale, new Vector3(2,2,2)))
            throw new EvalFailedException("RedSphere is not scaled to (2, 2, 2).");

        // Test: RedSphere material
        Color redColor = redSphere.GetComponent<MeshRenderer>().material.GetColor("_Color");
        if (redColor.r < 0.999f)
            throw new EvalFailedException("The red value for the RedSphere's Albedo is too low.");
        else if (redColor.r > 1.001f)
            throw new EvalFailedException("The red value for the RedSphere's Albedo is too high.");
        if (!redColor.Equals(Color.red))
            throw new EvalFailedException("The color of the RedSphere's Albedo is incorrect. It should be pure red.");

        //Test: RedSphere Prefab
        using PrefabUtility.EditPrefabContentsScope prefabEdit = new PrefabUtility.EditPrefabContentsScope("Assets/Prefabs/RedPrefab.prefab");
    }

    private void CreditBand()
    {
        // Test: RedSphere transform
        GameObject woodSphere = GameObject.Find("WoodSphere");
        if (woodSphere == null)
            throw new EvalFailedException("No GameObject named WoodSphere was found.");
        if (!VectApprox(woodSphere.transform.position, new Vector3(-2, 0, 5)))
            throw new EvalFailedException("WoodSphere is not located at position (-2, 0, 5).");
        if (!VectApprox(woodSphere.transform.localScale, new Vector3(2, 2, 2)))
            throw new EvalFailedException("WoodSphere is not scaled to (2, 2, 2).");

        // Test: WoodSphere material
        if (!File.Exists("Assets/Textures/Wood_Wicker_004_basecolor.jpg"))
            throw new EvalFailedException("No Wood_Wicker_004_basecolor.png file was not found. " +
                " Have you imported it and placed it in a folder name Textures?");
        Material woodMat = woodSphere.GetComponent<MeshRenderer>().material;
        if(!woodMat.name.Equals("WoodMaterial (Instance)"))
            throw new EvalFailedException("The WoodSphere GameObject should have a single Material named WoodMaterial.");
        if (!woodMat.GetTexture("_MainTex").name.Equals("Wood_Wicker_004_basecolor"))
            throw new EvalFailedException("The texture placed in the main Albedo property slot of the WoodMaterial is incorrect." +
                " Carefuly consider which texture should go in each property of the WoodMaterial.");
        if (!woodMat.GetTexture("_BumpMap").name.Equals("Wood_Wicker_004_normal")) 
            throw new EvalFailedException("The texture placed in the Normal Map property slot of the WoodMaterial is incorrect." +
                " Carefuly consider which texture should go in each property of the WoodMaterial.");
        if (!woodMat.GetTexture("_ParallaxMap").name.Equals("Wood_Wicker_004_height"))
            throw new EvalFailedException("The texture placed in the Height Map property slot of the WoodMaterial is incorrect." +
                " Carefuly consider which texture should go in each property of the WoodMaterial.");
        if (!woodMat.GetTexture("_OcclusionMap").name.Equals("Wood_Wicker_004_ambientOcclusion"))
            throw new EvalFailedException("The texture placed in the Occlusion Map property slot of the WoodMaterial is incorrect." +
                " Carefuly consider which texture should go in each property of the WoodMaterial.");
    }

    private void DistinctionBand()
    {
        // Test: Directional light rotation and intensity
        GameObject dirLight = GameObject.Find("Directional Light");
        if (dirLight == null)
            throw new EvalFailedException("No GameObject named Direction Light was found in the scene.");
        if (!Mathf.Approximately(dirLight.transform.rotation.eulerAngles.y, 90))
            throw new EvalFailedException("The Directional Light is not pointing along the global x-axis. Check its Y rotation.");
        if (!Mathf.Approximately(dirLight.transform.rotation.eulerAngles.x, 45))
            throw new EvalFailedException("The Directional Light is not pointing at a 45 degree angle downwards. Check it X rotation.");
        
        Light lightComp = dirLight.GetComponent<Light>();
        if (lightComp.type != LightType.Directional)
            throw new EvalFailedException("The GameObject named Directional Light is not of a directional light type. Check the Light component on this GameObject");
        if (!Mathf.Approximately(lightComp.intensity, 2.0f))
            throw new EvalFailedException("The Directional Light has the wrong Intensity value. It should be set to 2.0f .");

        // Test: Spot Light transform and properties
        GameObject spotLight = GameObject.Find("Spot Light");
        if (spotLight == null)
            throw new EvalFailedException("No GameObject named Spot Light was found in the scene.");
        if (!VectApprox(spotLight.transform.position, new Vector3(3, -3, 3)))
            throw new EvalFailedException("The Spot Light needs to be positioned (1,-3,-2) units away from the RedSphere.");
        if (!VectApprox(spotLight.transform.rotation.eulerAngles, new Vector3(315, 315, 0)))
            throw new EvalFailedException("The Spot Light needs to be rotated to (-45,-45,-0) units away from the RedSphere.");
        
        lightComp = spotLight.GetComponent<Light>();        
        if (lightComp.type != LightType.Spot)
            throw new EvalFailedException("The GameObject named Spot Light is not of a spot light type. Check the Light component on this GameObject");
        if (!lightComp.color.Equals(Color.green))
            throw new EvalFailedException("The Spot Light must have a pure green color.");
        if (!Mathf.Approximately(lightComp.spotAngle, 80.0f))
            throw new EvalFailedException("The Spot Angle porperty of the Spot Light must be 80.");
        if (!Mathf.Approximately(lightComp.range, 50.0f))
            throw new EvalFailedException("The Range porperty of the Spot Light must be 50.");

        // Test: Red Material sharpness and metallic
        GameObject redSphere = GameObject.Find("RedSphere");
        Material redMat = redSphere.GetComponent<MeshRenderer>().material;
        if (!Mathf.Approximately(redMat.GetFloat("_Metallic"), 0.0f))
            throw new EvalFailedException("The Metallic property of the RedMaterial must be 0");
        if (!Mathf.Approximately(redMat.GetFloat("_Glossiness"), 0.7f))
            throw new EvalFailedException("The Smoothness property of the RedMaterial must be 0.7f");
    }


    private void HighDist90Band()
    {
        if (!VectApprox(Camera.main.transform.position, new Vector3(0, 1, 1)))
            throw new EvalFailedException("The Main Camera is in the wrong position.");
        if (!VectApprox(Camera.main.transform.rotation.eulerAngles, new Vector3(15.0f, 0, 0)))
            throw new EvalFailedException("The Main Camera has the wrong rotation.");
        if (!Color.Equals(Camera.main.backgroundColor, Color.black))
            throw new EvalFailedException("The Main Camera must have a black background color with no transparency (e.g. alpha value = 255).");
        if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
            throw new EvalFailedException("The Main Camera must have a solid color background.");
        if (!Camera.main.orthographic)
            throw new EvalFailedException("The Main Camera must have an orthographic perspective.");
        if (!Mathf.Approximately(Camera.main.orthographicSize, 3.0f))
            throw new EvalFailedException("The Main Camera must have an orthographic size of 3.");
    }

    private void HighDist100Band()
    {
        GameObject gObj = GameObject.Find("Particle System");
        if (gObj == null)
            throw new EvalFailedException("No GameObject named Particle System was found in the scene.");
        ParticleSystem ps = gObj.GetComponent<ParticleSystem>();
        if (!VectApprox(gObj.transform.rotation.eulerAngles, new Vector3(270.0f, 180.0f, 0.0f)))
            throw new EvalFailedException("The particle system does not have the correct roation.");
        if (!ps.GetComponent<ParticleSystemRenderer>().material.name.Equals("Star_Particle_Material (Instance)"))
            throw new EvalFailedException("The particle system doesn't have the Star Particle Material applied");
        if (ps.shape.shapeType != ParticleSystemShapeType.Cone)
            throw new EvalFailedException("The particle system does not have the correct shape.");
        if (!Mathf.Approximately(ps.shape.arc, 180.0f))
            throw new EvalFailedException("The particle system does not have the correct shape arc.");
        if (ps.shape.arcMode != ParticleSystemShapeMultiModeValue.PingPong)
            throw new EvalFailedException("The particle system is not doing a ping-pong motion.");
        if (!ps.colorOverLifetime.enabled)
            throw new EvalFailedException("Your particle system should change color over time");
        if (!ps.sizeOverLifetime.enabled)
            throw new EvalFailedException("Your particle system should change size over time");
        if (!ps.trails.enabled)
            throw new EvalFailedException("Your particle system should use the Trails property");
        if (Mathf.Approximately(ps.main.gravityModifier.constant, 0.0f))
            throw new EvalFailedException("Your particle system should have gravity");

        Debug.LogWarning("PROGRESS EVALUATOR: USE YOUR EYES! There are so many ways to " +
            "get this particle system wrong and the PrgressEvaluator can't anticipate them all.");
        Debug.LogWarning("Use the Status-100Percent executables (Windows or Mac) to compare " +
            "your particle system to the reference one.");

    }


    void HandleLog(string logString, string stackTrace, LogType type)
    {
        lastLog = logString;
    }

    private void CheckStudentNum()
    {
        if (studentNumber < 10000000)
            throw new EvalFailedException("Student Number Check: Invalid student number. This must be an 8 digit student ID.");
    }

    private static void EvalFailMessage(String band, String message)
    {
        Debug.LogError("PROGRESS EVALUATOR: " + band + ": " + message);
    }

    private static void EvalPassMessage(String band)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + band + " Band: No common mistakes found. -------------");
    }

    private static void EvalInProgressMessage(String band)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + band + " Band: This band is doing tests over multiple frames, please wait. -------------");
    }


    private void EvalOutput(GradeBand evalGrade)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: Remember to also check your progress against the Status-xPercent.png files and " +
            "Status-100Percent-Windows or Status-100Percent-Mac.app executables.");
        using BinaryWriter writer = new BinaryWriter(File.Open("ProgEv", FileMode.Create));
        writer.Write(evalGrade.ToString());
        writer.Write(studentNumber);
        writer.Write(DateTime.Now.Year.ToString());
    }

    private GradeBand EvalReader()
    {
        try
        {
            using BinaryReader reader = new BinaryReader(File.Open("ProgEv", FileMode.Open));
            return (GradeBand)Enum.Parse(typeof(GradeBand), reader.ReadString());
        } catch (Exception e)
        {
            lastLog = e.Message;
            return GradeBand.Deactivate;
        }
    }

    private bool VectApprox(Vector3 a, Vector3 b)
    {
        if (Vector3.Distance(a, b) < 0.01f)
            return true;
        else
            return false;
    }
}

public class EvalFailedException : Exception
{
    public EvalFailedException(string message) : base(message) { }
}
