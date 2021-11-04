#define Z40
#define P50
#define P60

using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;

public class ProgressEvaluator : MonoBehaviour
{
    private enum GradeBand { Deactivate, Z40, Pass50, Pass60, Credit70, Distinction80, HD90, HD100 };
    private Action[] evalMethods;
    [SerializeField] private uint studentNumber = 0;
    [SerializeField] private GradeBand bandReached = GradeBand.Deactivate;

    private string lastLog;
    private string currentLog;
    private GradeBand loadedBand = GradeBand.Deactivate;

    // Start is called before the first frame update
    void Start()
    {
        evalMethods = new Action[] { () => Z40Band(), () => Pass50Band(), () => Pass60Band(), 
            () => Credit70Band(), () => Distinction80Band(), () => HighDist90Band(), () => HighDist100Band() };
        loadedBand = EvalReader();

        try
        {
            if (bandReached > GradeBand.Deactivate)
                evalMethods[(int)bandReached - 1]();
        }
        catch (EvalFailedException e)
        {
            EvalFailMessage(bandReached, e.ToString());
        }
        catch (Exception e)
        {
            EvalFailMessage(bandReached, "An unknown error occured during the progress evaluation. It is likely " +
                "that you have made a mistake that the ProgressEvaluator system has not been setup to handle. Go back over the " +
                "steps for this grade band and see if you can spot anything wrong or ask your tutor. The full error is below for reference: ");
            Debug.LogError(e.GetType() + ": " + e.ToString());
        }
    }


    private void Z40Band()
    {
#if Z40
        GameObject tempGO = GameObject.Find("Floor");
        if (tempGO == null)
            throw new EvalFailedException("Couldn't find GameObject called Floor");
        if (!VectApprox(tempGO.transform.position, Vector3.zero))
            throw new EvalFailedException("Floor is not in the correct position");
        if (!VectApprox(tempGO.transform.localScale, new Vector3(5,0.5f,5)))
            throw new EvalFailedException("Floor does not have the correct scale");
        if (!tempGO.GetComponent<Renderer>().material.color.Equals(new Color32(204, 0, 204, 255)))
            throw new EvalFailedException("Floor is not the correct color");

        tempGO = GameObject.Find("Box");
        if (tempGO == null)
            throw new EvalFailedException("Couldn't find GameObject called Box");
        if (!VectApprox(tempGO.transform.position, new Vector3(0, 0.5f, 0)))
            throw new EvalFailedException("Box is not in the correct position");
        if (!VectApprox(tempGO.transform.localScale, new Vector3(0.5f, 0.5f, 0.5f)))
            throw new EvalFailedException("Box does not have the correct scale");
        if (!tempGO.GetComponent<Renderer>().material.color.Equals(new Color32(255, 255, 0, 255)))
            throw new EvalFailedException("Box is not the correct color");

        if (!VectApprox(Camera.main.transform.position, new Vector3(0,3,-4)) 
            || !VectApprox(Camera.main.transform.rotation.eulerAngles, new Vector3(45,0,0)))
            throw new EvalFailedException("The camera is either in the wrong position or has the wrong rotation");

        tempGO = GameObject.Find("TweenController");
        if (tempGO == null)
            throw new EvalFailedException("Couldn't find GameObject called TweenController");
        if (tempGO.GetComponent<Tweener>() == null || tempGO.GetComponent<InputManager>() == null)
            throw new EvalFailedException("The TweenController is missing either the Tweener or InputManager components");

        // Eval Finished
        EvalPassMessage(bandReached);
#else
        throw new EvalFailedException("Open the ProgressEvaluator.cs file and uncomment (i.e. remove the // symbol) the line at the top that says #define Z40");
#endif
    }

    private void Pass50Band()
    {
#if (Z40 && P50)
        if (RegexCheck("Tween.cs", "MonoBehaviour"))
            throw new EvalFailedException("You skipped the first instruction in 50% band");
        if (!RegexCheck("Tweener.cs", "private\\s+Tween\\s+activeTween"))
            throw new EvalFailedException("No private variable named activeTween found");
        
        PropertyInfo[] propInfo = Type.GetType("Tween").GetProperties();
        if (propInfo.Length == 0)
            throw new EvalFailedException("There are no PROPERTIES in the Tween class. " +
                "Make sure you lookup C# properties on the Microsoft Docs site.");
        if (propInfo.Length < 5)
            throw new EvalFailedException("There should be 5 properties in the Tween class. You have " + propInfo.Length);
        for (int i = 0; i < propInfo.Length; i++)
        {
            if (propInfo[i].GetGetMethod() == null)
                throw new EvalFailedException("Property named " + propInfo[i].Name + " doesn't have a public getter");
            if (propInfo[i].GetSetMethod() != null)
                throw new EvalFailedException("Property named " + propInfo[i].Name + " doesn't have a private setter");
        }

        // Eval Finished
        EvalPassMessage(bandReached);

#else
        throw new EvalFailedException("Open the ProgressEvaluator.cs file and uncomment (i.e. remove the // symbol) the line at the top that says #define P50 (and all lines before it)");
#endif
    }


    private void Pass60Band()
    {
#if (Z40 && P50 && P60)
        /* TO STUDENTS: If you are getting a compile error on the below line, its because your Tween constructor is wrong.
         * Comment out the #define P60 line and try again */
        Tween tween = new Tween(transform, Vector3.zero, Vector3.one, 0.0f, 1.0f);
        if (tween.Target != transform)
            throw new EvalFailedException("The Tween constructor did not properly set the Target property");
        if (tween.StartPos != Vector3.zero)
            throw new EvalFailedException("The Tween constructor did not properly set the StartPos property");
        if (tween.EndPos != Vector3.one)
            throw new EvalFailedException("The Tween constructor did not properly set the EndPos property");
        if (!Mathf.Approximately(tween.StartTime, 0.0f))
            throw new EvalFailedException("The Tween constructor did not properly set the StartTime property");
        if (!Mathf.Approximately(tween.Duration, 1.0f))
            throw new EvalFailedException("The Tween constructor did not properly set the Duration property");

        StartCoroutine(Pass60Coroutine());
#else
        throw new EvalFailedException("Open the ProgressEvaluator.cs file and uncomment (i.e. remove the // symbol) the line at the top that says #define P60 (and all lines before it)");
#endif
    }

    private IEnumerator Pass60Coroutine()
    {
        yield return null;
#if (Z40 && P50 && P60)
        Tweener tweener = GameObject.Find("TweenController").GetComponent<Tweener>();
        /* TO STUDENTS: If you are getting a compile error on the below line, its because your AddTween method declaration is wrong.
        * Comment out the #define P60 line and try again */
        tweener.AddTween(transform, Vector3.zero, Vector3.one, 1.0f);

        // Eval Finished
        EvalPassMessage(bandReached);
#else
        throw new EvalFailedException("Open the ProgressEvaluator.cs file and uncomment (i.e. remove the // symbol) the line at the top that says #define P60 (and all lines before it)");
#endif
    }

    private void Credit70Band()
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + bandReached.ToString() + ": No automated checks in this band. " +
            "Make sure you have carefuly read and implemented the AddTween functionality, otherwise you will have issues later on.");
        // Eval Finished
        EvalPassMessage(bandReached);
    }

    private void Distinction80Band()
    {
        StartCoroutine(Dist80Coroutine());
        EvalInProgressMessage(bandReached, "When the game starts, press the A key. " +
            "Once the box stops moving, press the D key and wait for it to stop again");
    }


    private IEnumerator Dist80Coroutine()
    {
        Transform box = GameObject.Find("Box").transform;

        while (!Input.GetKeyDown(KeyCode.A))
            yield return null;

        yield return new WaitForSeconds(0.75f);
        if (box.position.x < -1.15f || box.position.x > -0.85f)
            throw new EvalFailedException("The Box is not in the correct position half-way through the linear interpolation after pressing A");

        yield return new WaitForSeconds(0.76f);
        if (!VectApprox(box.position, new Vector3(-2.0f, 0.5f, 0.0f)))
            throw new EvalFailedException("The box is not in the correct position at the end of the linear interpolation after pressing the A key");


        while (!Input.GetKeyDown(KeyCode.D))
            yield return null;

        yield return new WaitForSeconds(0.75f);
        if (box.position.x < -0.15f || box.position.x > 0.15f)
            throw new EvalFailedException("The Box is not in the correct position half-way through the linear interpolation after pressing D");

        yield return new WaitForSeconds(0.76f);
        if (!VectApprox(box.position, new Vector3(2.0f, 0.5f, 0.0f)))
            throw new EvalFailedException("The box is not in the correct position at the end of the linear interpolation after pressing the D key");


        // Eval Finished
        EvalPassMessage(bandReached);
    }


    private void HighDist90Band()
    {
        StartCoroutine(HD90Coroutine());
        EvalInProgressMessage(bandReached, "When the game starts, press the A key. " +
            "Once the box stops moving, press the D key and wait for it to stop again");
    }


    private IEnumerator HD90Coroutine()
    {
        Transform box = GameObject.Find("Box").transform;       

        while (!Input.GetKeyDown(KeyCode.A))
            yield return null;

        yield return new WaitForSeconds(0.75f);
        if (box.position.x < -0.3f || box.position.x > -0.2f)
            throw new EvalFailedException("The Box is not in the correct position half-way through the cubic interpolation after pressing A");
        
        yield return new WaitForSeconds(0.45f);        
        if (box.position.x < -1.2f || box.position.x > -0.85f)
            throw new EvalFailedException("The Box is not in the correct position at 4/5ths of the way through the cubic interpolation after pressing A");

        yield return new WaitForSeconds(0.3f);
        if (!VectApprox(box.position, new Vector3(-2.0f, 0.5f, 0.0f)))
            throw new EvalFailedException("The box is not in the correct position at the end of the cubic interpolation after pressing the A key");


        while (!Input.GetKeyDown(KeyCode.D))
            yield return null;

        yield return new WaitForSeconds(0.75f);        
        if (box.position.x < -1.7f || box.position.x > -1.3f)
            throw new EvalFailedException("The Box is not in the correct position half-way through the cubic interpolation after pressing A");

        yield return new WaitForSeconds(0.45f);
        if (box.position.x < -0.1f || box.position.x > 0.45f)
            throw new EvalFailedException("The Box is not in the correct position at 4/5ths of the way through the cubic interpolation after pressing A");

        yield return new WaitForSeconds(0.3f);
        if (!VectApprox(box.position, new Vector3(2.0f, 0.5f, 0.0f)))
            throw new EvalFailedException("The box is not in the correct position at the end of the cubic interpolation after pressing the A key");


        // Eval Finished
        EvalPassMessage(bandReached);
    }


    private void HighDist100Band()
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + bandReached.ToString() + ": No automated checks in this band. " +
            "Look at the Status-100Percent video and the executables");
        // Eval Finished
        EvalPassMessage(bandReached);
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

    private void EvalFailMessage(GradeBand band, string message)
    {
        Debug.LogError("PROGRESS EVALUATOR: " + band.ToString() + ": " + message);
    }

    private void EvalPassMessage(GradeBand band)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + band.ToString() + " Band: No common mistakes found. -------------");
        if (band > loadedBand)
            EvalOutput(band);
    }

    private void EvalInProgressMessage(GradeBand band, string instructions)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + band.ToString() + " Band: This band is doing checks over multiple frames. -------------");
        if (!instructions.Equals(""))
            Debug.LogWarning("PROGRESS EVALUATOR: " + band.ToString() + " Band: Instructions: " + instructions);
    }


    private void EvalOutput(GradeBand evalGrade)
    {
        Debug.LogWarning("PROGRESS EVALUATOR: Remember to also check your progress against the Status-xPercent.png files and " +
            "Status-100Percent-Windows or Status-100Percent-Mac.app executables.");
        using BinaryWriter writer = new BinaryWriter(File.Open("ProgEv", FileMode.Create));
        writer.Write(evalGrade.ToString());
        writer.Write(studentNumber);
        writer.Write(DateTime.Now.ToString("MM/dd/yyyy"));
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

    private bool RegexCheck(string fileName, string regExp) {
        string[] laContents = Directory.GetFiles("./", fileName, SearchOption.AllDirectories);
        if (laContents.Length == 0)
            throw new EvalFailedException("Can't find the " + fileName + " script in your Assets folder. ");
        string laContent = File.ReadAllText(laContents[0]);
        Regex rx = new Regex(regExp);
        return rx.Match(laContent).Success;
    }
}

public class EvalFailedException : Exception
{
    public EvalFailedException(string message) : base(message) { }
}
