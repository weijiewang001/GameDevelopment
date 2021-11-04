using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

public class ProgressEvaluator : MonoBehaviour
{
    private enum GradeBand { Deactivate, Z40, Pass50, Pass60, Credit70, Distinction80, HD90, HD100 };
    private Action[] evalMethods;
    [SerializeField] private uint studentNumber = 0;
    [SerializeField] private GradeBand bandReached = GradeBand.Deactivate;
    //[SerializeField] private bool showSuccessMessages = false;

    private string lastLog;
    private string currentLog;
    private GradeBand loadedBand = GradeBand.Deactivate;

    // Start is called before the first frame update
    void Start()
    {
        evalMethods = new Action[] { () => Z40Band(), () => Pass50Band(), () => Pass60Band(), 
            () => Credit70Band(), () => Distinction80Band(), () => HighDist90Band(), () => HighDist100Band() };
        loadedBand = EvalReader();

        // int i = 0;
        try
        {
            if (bandReached > GradeBand.Deactivate)
            {
                Debug.LogWarning("PROGRESS EVALUATOR: This week, the ProgressEvaluator will only check the " +
                    "band that you set it to, not the ones prior to it. Make sure you are running the " +
                    "ProgressEvaluator after every band!");
                evalMethods[(int)bandReached - 1]();
                if (bandReached <= GradeBand.Pass60)
                {
                    EvalPassMessage(bandReached.ToString());
                    if (bandReached > loadedBand)
                        EvalOutput(bandReached);
                }
                else
                    EvalInProgressMessage(bandReached.ToString());
            }

            /* while (i < (int)bandReached)
            {
                if (i == 0)
                    CheckStudentNum();
                GradeBand currentBand = (GradeBand)i + 1;
                evalMethods[i]();
                if (currentBand <= GradeBand.Pass60)
                {
                    EvalPassMessage(currentBand.ToString());
                    if (currentBand > loadedBand)
                        EvalOutput(currentBand);
                }
                else
                    EvalInProgressMessage(currentBand.ToString());
                i++;
            }
            */
        }
        catch (EvalFailedException e)
        {
            //EvalFailMessage(((GradeBand)i + 1).ToString(), e.Message);
            EvalFailMessage(bandReached.ToString(), e.Message);
        }
        catch (Exception e)
        {
            // EvalFailMessage(((GradeBand)i + 1).ToString()
            EvalFailMessage(bandReached.ToString(), "An unknown error occured during the progress evaluation. It is likely " +
                "that you have made a mistake that the ProgressEvaluator system has not been setup to handle. Go back over the " +
                "steps for this grade band and see if you can spot anything wrong or ask your tutor. The full error is below for reference: ");
            Debug.LogError(e.GetType() + ": " + e.Message);
            Debug.LogError(e.GetType() + ": Full Stack Trace: " + e.StackTrace);
        }
    }


    private void Z40Band()
    {
        // Test: ParentObj in Scene
        GameObject parentObj = GameObject.Find("ParentObj");
        if (parentObj == null)
            throw new EvalFailedException("Could not find GameObject in Scene named ParentObj.");
        if (parentObj.GetComponent<Renderer>())
            throw new EvalFailedException("ParentObj should be an Empty GameObject with only a " +
                "Transform and a PrintAndHide components");
        if (parentObj.transform.position != Vector3.zero ||
            parentObj.transform.rotation != Quaternion.identity ||
            parentObj.transform.localScale != Vector3.one)
            throw new EvalFailedException("ParentObj has not had its transform reset.");
        if (parentObj.GetComponent<PrintAndHide>() == null)
            throw new EvalFailedException("ParentObj does not have the PrintAndHide component attached");
        if (!parentObj.CompareTag("Red"))
            throw new EvalFailedException("The ParentObj's tag is not set to Red");
    }

    private void Pass50Band()
    {
        //Test: RedPrefab setup
        string[] prefabFiles = Directory.GetFiles("./", "RedPrefab.prefab", SearchOption.AllDirectories);
        if (prefabFiles.Length == 0)
            throw new EvalFailedException("No RedPrefab found in the Assets folder.");
        GameObject prefabEdit = PrefabUtility.LoadPrefabContents(prefabFiles[0]);
        try
        {
            if (prefabEdit.transform.childCount == 0)
                throw new EvalFailedException("RedPrefab has no child GameObject on it.");            
            Transform child = prefabEdit.transform.GetChild(0); 
            
            Material mat = child.GetComponent<Renderer>().material;
            if (mat.color.r < mat.color.g || mat.color.r < mat.color.b)
                throw new EvalFailedException("The red material on the child of RedPrefab doesn't seem to be red");
            if (!VectApprox(child.localScale, new Vector3(1.5f, 0.75f, 1.0f)))
                throw new EvalFailedException("The child of RedPrefab does not have the correct scale");
            if (!VectApprox(child.localPosition, Vector3.zero))
                throw new EvalFailedException("The child of RedPrefab is not positioned at the center of its parent object");
            if (prefabEdit.GetComponent<PrintAndHide>().rend != child.GetComponent<Renderer>())
                throw new EvalFailedException("The PrintAndHide.rend variable on the RedPrefab is not set to the Renderer " +
                    "component of its child game object");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabEdit);
        }


        //Test: BluePrefab setup
        prefabFiles = Directory.GetFiles("./", "BluePrefab.prefab", SearchOption.AllDirectories);
        if (prefabFiles.Length == 0)
            throw new EvalFailedException("No BluePrefab found in the Assets folder.");
        GameObject prefabEdit2 = PrefabUtility.LoadPrefabContents(prefabFiles[0]);
        try
        {
            if (prefabEdit2.transform.childCount == 0)
                throw new EvalFailedException("BluePrefab has no child GameObject on it.");
            Transform child = prefabEdit2.transform.GetChild(0);

            Material mat = child.GetComponent<Renderer>().material;
            if (mat.color.b < mat.color.g || mat.color.b < mat.color.r)
                throw new EvalFailedException("The blue material on the child of BluePrefab doesn't seem to be blue");
            if (!VectApprox(child.localScale, new Vector3(1.5f, 0.75f, 1.0f)))
                throw new EvalFailedException("The child of BluePrefab does not have the correct scale");
            if (!VectApprox(child.localPosition, Vector3.zero))
                throw new EvalFailedException("The child of BluePrefab is not positioned at the center of its parent object");
            if (prefabEdit2.GetComponent<PrintAndHide>().rend != child.GetComponent<Renderer>())
                throw new EvalFailedException("The PrintAndHide.rend variable on the BedPrefab is not set to the Renderer " +
                    "component of its child game object");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabEdit2);
        }
    }


    private void Pass60Band()
    {
        // Test: Object existence
        GameObject redObj = GameObject.FindWithTag("Red");
        if (redObj == null)
            throw new EvalFailedException("No GameObject with the tag Red was found in the scene. " +
                "If you think you have this correct but are still getting this error, make sure you " +
                "red and follow the last line of the PDF instructions for this band.");
        GameObject blueObj = GameObject.FindWithTag("Blue");
        if (blueObj == null)
            throw new EvalFailedException("No GameObject with the tag Blue was found in the scene. " +
                "If you think you have this correct but are still getting this error, make sure you " +
                "red and follow the last line of the PDF instructions for this band.");

        // Test: Object position
        if (!VectApprox(redObj.transform.position, new Vector3(2, 1, 0)))
            throw new EvalFailedException("The red object is not in the correct position");
        if (!VectApprox(blueObj.transform.position, new Vector3(-2, -1, 0)))
            throw new EvalFailedException("The blue object is not in the correct position");
    }

    private void Credit70Band()
    {
        StartCoroutine(Cred70Coroutine());        
    }

    private IEnumerator Cred70Coroutine()
    {
        GameObject redObj = GameObject.FindWithTag("Red");
        GameObject blueObj = GameObject.FindWithTag("Blue");

        // Test: Check Rotation
        EvalInstructions(GradeBand.Credit70.ToString(), "Press the W key on your keyboard to complete this evaluation.");
        while (!Input.GetKeyDown(KeyCode.W))
            yield return null;

        yield return null;
        if (!VectApprox(redObj.transform.rotation.eulerAngles, new Vector3(0, 0, 45)))
            throw new EvalFailedException("The red object was not correctly rotated afer W was pressed");
        if (!VectApprox(blueObj.transform.rotation.eulerAngles, new Vector3(0, 0, 315)))
            throw new EvalFailedException("The blue object was not correctly rotated afer W was pressed");

        // Eval Finished
        EvalPassMessage(GradeBand.Credit70.ToString());
        if (GradeBand.Credit70 > loadedBand)
            EvalOutput(GradeBand.Credit70);
    }

    private void Distinction80Band()
    {
        string[] laContents = Directory.GetFiles("./", "InputManager.cs", SearchOption.AllDirectories);
        if (laContents.Length == 0)
            throw new EvalFailedException("Can't find the InputManager.cs script in your Assets folder. " +
                "You should not be seeing this error, let William Raffe know.");
        string laContent = File.ReadAllText(laContents[0]);
        Regex rx = new Regex("Input.GetButtonDown");
        if (!rx.Match(laContent).Success)
            throw new EvalFailedException("You are not using Input.GetButtonDown() in InputManager. " +
                "You should be using this method for detecting the Q button press");

        StartCoroutine(Dist80Coroutine());        
    }


    private IEnumerator Dist80Coroutine()
    {
        yield return null;

        Transform redTrans = GameObject.FindWithTag("Red").transform;
        Transform blueTrans = GameObject.FindWithTag("Blue").transform;

        EvalInstructions(bandReached.ToString(), "Press the Q key 4 times to complete this evaluation");
        int i = 0;
        while(i < 4)
        {   
            if (Input.GetKeyDown(KeyCode.Q))
            {
                yield return null;
                try
                {
                    i++;
                    if (i % 2 == 0)
                    {
                        if (!VectApprox(redTrans.position, new Vector3(2, 1, 0)) ||
                            !VectApprox(blueTrans.position, new Vector3(-2, -1, 0)))
                            throw new EvalFailedException("The red and blue objects did not switch position properly");                        
                    }
                    else
                    {
                        if (!VectApprox(redTrans.position, new Vector3(2, -1, 0)) ||
                            !VectApprox(blueTrans.position, new Vector3(-2, 1, 0)))
                            throw new EvalFailedException("The red and blue objects did not switch position properly");
                        
                    }
                }
                catch (EvalFailedException e)
                {
                    EvalFailMessage(bandReached.ToString(), e.Message);
                }
            }
            yield return null;
        }

        // Eval Finished
        if (i == 4)
        {
            EvalPassMessage(bandReached.ToString());
            if (bandReached > loadedBand)
                EvalOutput(bandReached);
        }
    }


    private void HighDist90Band()
    {
        StartCoroutine(HD90Coroutine());
    }


    private IEnumerator HD90Coroutine()
    {
        yield return null;
        EvalInstructions(bandReached.ToString(), "Press the Q key 1 time to complete this evaluation");

        GameObject red = GameObject.FindWithTag("Red");
        GameObject blue = GameObject.FindWithTag("Blue"); 
        while (!Input.GetKeyUp(KeyCode.Q))
            yield return null;

        yield return null;
        try
        {

            Color redCol = red.GetComponent<PrintAndHide>().rend.material.color;
            Color blueCol = blue.GetComponent<PrintAndHide>().rend.material.color;
            if (Mathf.Approximately(redCol.r, 1.0f) || Mathf.Approximately(blueCol.b, 1.0f))
                throw new EvalFailedException("Either the red object or blue object have an incorect color value. " +
                    "This may have been a fluke. Press play two or three more times, if you get the same error again, then you have an issue. " +
                    "Note that the Color class constructor takes values between 0 and 1 " +
                    "(instead of 0 and 255 as seen in the pop-up color selector windows)");

            // Eval Finished
            EvalPassMessage(bandReached.ToString());
            if (bandReached > loadedBand)
                EvalOutput(bandReached);
        } catch (EvalFailedException e)
        {
            EvalFailMessage(bandReached.ToString(), e.Message);
        }        
    }


    private void HighDist100Band()
    {
        StartCoroutine(HD100Coroutine());
    }

    private IEnumerator HD100Coroutine()
    {
        yield return null;
        EvalInstructions(bandReached.ToString(), "Press the E key 2 times to complete this evaluation");
        
        GameObject red = GameObject.FindWithTag("Red");
        GameObject blue = GameObject.FindWithTag("Blue");

        // Note: if you are reading the ProgressEvaluator for hints, the below is NOT how you should be doing your code
        // Your code needs to work with an indefinite number of E key presses
        int i = 0;
        while (i<2)
        {
            
            bool trigger = false;
            if (Input.GetKeyDown(KeyCode.E))
            {
                i++;
                trigger = true;
                yield return null;
            }
            try { 
                if (i == 1 && trigger)
                    if (red.GetComponent<PrintAndHide>() != null ||
                        blue.GetComponent<PrintAndHide>() != null)
                        throw new EvalFailedException("You have not correctly removed the PrintAndHide Component from either the red or blue objects");
                if (i == 2 && trigger)
                {
                    if (red.GetComponent<PrintAndHide>() == null ||
                        blue.GetComponent<PrintAndHide>() == null)
                        throw new EvalFailedException("You have not correctly add the PrintAndHide Component back onto either the red or blue objects");

                    if (!red.activeSelf || !blue.activeSelf)
                        throw new EvalFailedException("Either the red or blue GameObjects are not active. They should be re-activated after pressing E the second time");
                    // Eval Finished
                    EvalPassMessage(bandReached.ToString());
                    if (bandReached > loadedBand)
                        EvalOutput(bandReached);
                }
            } catch (EvalFailedException e)
            {
                EvalFailMessage(bandReached.ToString(), e.Message);
                break;
            }

            yield return null;
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        lastLog = currentLog;
        currentLog = logString;
    }

    private void CheckStudentNum()
    {
        if (studentNumber < 10000000)
            throw new EvalFailedException("Student Number Check: Invalid student number. This must be an 8 digit student ID.");
    }

    private static void EvalFailMessage(string band, string message)
    {
        Debug.LogError("PROGRESS EVALUATOR: " + band + ": " + message);
    }

    private static void EvalPassMessage(string band)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + band + " Band: No common mistakes found. -------------");
    }

    private static void EvalInProgressMessage(string band)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + band + " Band: This band is doing checks over multiple frames. Press keys identified in this band (if any) in the PDF instructions -------------");
    }

    private static void EvalInstructions(string band, string message)
    {
        Debug.LogWarning("----PROGRESS EVALUATOR: " + band + " Instructions: " + message);
    }


    private void EvalOutput(GradeBand evalGrade)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: Remember to also check your progress against the Status-xPercent.png files and " +
            "Status-100Percent-Windows or Status-100Percent-Mac.app executables.");
        using BinaryWriter writer = new BinaryWriter(File.Open("ProgEv", FileMode.Create));
        writer.Write(evalGrade.ToString());
        writer.Write(studentNumber);
        writer.Write(DateTime.Now.ToString("MM/dd/yyyy"));
        //writer.Write(DateTime.Now.Year.ToString());
    }

    private GradeBand EvalReader()
    {
        try
        {
            using BinaryReader reader = new BinaryReader(File.Open("ProgEv", FileMode.Open));
            return (GradeBand)Enum.Parse(typeof(GradeBand), reader.ReadString());
        }
        catch (Exception e)
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
