#define HD

using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressEvaluator : MonoBehaviour
{
    private enum GradeBand { Deactivate, Pass, Credit, Distinction, HighDist};
    private Action[] evalMethods; 
    [SerializeField] private uint studentNumber = 0;
    [SerializeField] private GradeBand bandReached = GradeBand.Deactivate;
    //[SerializeField] private bool showSuccessMessages = false;

    private String lastLog;

    // Start is called before the first frame update
    void Start()
    {
        evalMethods = new Action[] { () => PassBand(), () => CreditBand(), () => DistinctionBand(), () => HighDistBand() };
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
                if (currentBand <= GradeBand.Distinction)
                {
                    EvalPassMessage(currentBand.ToString());
                    if (currentBand > loadedBand)
                        EvalOutput(currentBand);
                }
                else
                    EvalInProgressMessage(currentBand.ToString());
                i++;
            }            
        } catch (EvalFailedException e)
        {
            EvalFailMessage(((GradeBand)i+1).ToString(), e.Message);
        } catch (Exception e)
        {
            EvalFailMessage(((GradeBand)i + 1).ToString(), "An unknown error occured during the progress evaluation. It is likely " +
                "that you have made a mistake that the ProgressEvaluator system has not been setup to handle. Go back over the " +
                "steps for this grade band and see if you can spot anything wrong or ask your tutor. The full error is below for reference:");
            Debug.LogError(e.GetType() + ": " + e.Message);
        }        
    }      


    private void PassBand()
    {
        // Test: Scene name
        if (!SceneManager.GetActiveScene().name.Equals("Week1Scene"))
            throw new EvalFailedException("The scene name is not correct. Rename the Scene to Week1Scene");    
    }

    private void CreditBand()
    {
        // Test: Empty1 name
        GameObject empty1 = GameObject.Find("Empty1");
        if (empty1 == null)
            throw new EvalFailedException("No GameObject named Empty1 was found.");
        // Test: Empty1 position
        if (Vector3.Distance(empty1.transform.position, new Vector3(5.2525f, 3, 0)) > 0.1f)
            throw new EvalFailedException("Empty1 is not located at position (5.2525, 3, 0).");
        //Test: Empty2 name
        GameObject empty2 = GameObject.Find("Empty2");
        if (empty2 == null)
            throw new EvalFailedException("No GameObject named Empty2 was found.");
        //Test: Empty2 position
        if (Vector3.Distance(empty2.transform.position, new Vector3(-5, -1.31f, 0)) > 0.1f)
            throw new EvalFailedException("Empty2 is not located at position (-5, -1.31f, 0).");
        //Test: Empty1 rotation
        if (empty1.transform.eulerAngles.y < -90.0f || empty1.transform.eulerAngles.y > -70.0f)
        {
            if (empty1.transform.eulerAngles.y < 270.0f || empty1.transform.eulerAngles.y > 290.0f)
            throw new EvalFailedException("Empty1 is not rotated to (0, -80, 0).");
        }            
    }

    private void DistinctionBand()
    {
        GameObject empty1 = GameObject.Find("Empty1");
        //Tests: empty1 has various components
        if (empty1.GetComponent<AudioSource>() == null)
            throw new EvalFailedException("Empty1 does not have an AudioSource component");
        if (empty1.GetComponent<SkinnedMeshRenderer>() == null)
            throw new EvalFailedException("Empty1 does not have an SkinnedMeshRenderer component");
        if (empty1.GetComponent<BoxCollider>() == null)
            throw new EvalFailedException("Empty1 does not have an BoxCollider component");

        //Tests: FirstPrefab exists and has components
        using PrefabUtility.EditPrefabContentsScope prefabEdit = new PrefabUtility.EditPrefabContentsScope("Assets/FirstPrefab.prefab");
        GameObject prefabRoot = prefabEdit.prefabContentsRoot;
        if (prefabRoot.GetComponent<AudioSource>() == null)
            throw new EvalFailedException("FirstPrefab does not have an AudioSource component");
        if (prefabRoot.GetComponent<SkinnedMeshRenderer>() == null)
            throw new EvalFailedException("FirstPrefab does not have an SkinnedMeshRenderer component");

        //Tests: Empty2 has components
        GameObject empty2 = GameObject.Find("Empty2");
        if (empty2.GetComponent<BoxCollider>() == null)
            throw new EvalFailedException("Empty2 does not have an BoxCollider component");
    }
    
    
    private void HighDistBand()
    {
        StartCoroutine(HighDistCoroutine());
    }

    private IEnumerator HighDistCoroutine()
    {
#if HD
        //Test: Empty2 has a HelloWorld script
        GameObject empty2 = GameObject.Find("Empty2");
        if (empty2.GetComponent<HelloWorld>() == null)
            throw new EvalFailedException("Empty2 does not have the HelloWorld component attached.");

        Application.logMessageReceived += HandleLog;
        yield return null;
        yield return null;
        int countNum;
        if (!int.TryParse(lastLog, out countNum))
            throw new EvalFailedException("The Console is printing something other than a number. " +
                "Your console should only be showing a single number per line, which increases by 1 each line.");
        if (countNum == 0)
            throw new EvalFailedException("Something is wrong with your counting in your HelloWorld script. " +
                "Likely your counting in HelloWorld is not increasing by a value of 1 in each frame");
        if (countNum != 2)
            throw new EvalFailedException("Something is wrong with your counting in your HelloWorld script. " +
                "Likely your counting in HelloWorld has not started at 0 in the first frame");

        EvalPassMessage(GradeBand.HighDist.ToString());
        EvalOutput(GradeBand.HighDist);
#elif (!HD)
        yield return null;
        throw new EvalFailedException("Open ProgressEvaluator.cs and uncomment the \"#define HD\" at the top to run the High Distinction tests.");
#endif
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
}

public class EvalFailedException : Exception
{
    public EvalFailedException(string message) : base(message) { }
}
