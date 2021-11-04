#define C70

using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        DontDestroyOnLoad(this);

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
        if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
            throw new EvalFailedException("Camera Clear Flag not set to Solid Color");
        if (!Camera.main.backgroundColor.Equals(new Color(0,0,0,0)))
            throw new EvalFailedException("Camera Background not set to complete black color");
        if (RenderSettings.skybox != null)
            throw new EvalFailedException("The Skybox Material in the Lighting Settings should be set to None");
        if (FindObjectOfType<Canvas>() == null)
            throw new EvalFailedException("No Canvas component found in the scene");

        // Eval Finished
        EvalPassMessage(bandReached);
    }

    private void Pass50Band()
    {
        GameObject startButton = GameObject.Find("RoundButton");
        if (startButton == null)
            throw new EvalFailedException("RoundButton not found in the scene");

        RectTransform startTrans = (RectTransform) startButton.transform;
        if (!Mathf.Approximately(startTrans.anchoredPosition.x, 0.0f) 
                || !Mathf.Approximately(startTrans.anchoredPosition.y, 0.0f)
                || !Mathf.Approximately(startTrans.sizeDelta.x, 110.0f)
                || !Mathf.Approximately(startTrans.sizeDelta.y, 110.0f))
            throw new EvalFailedException("The RoundButton's RectTransform is not in the correct position OR is the wrong size.");

        if (!startButton.GetComponent<Image>().sprite.texture.name.Equals("button_theme17_round0"))
            throw new EvalFailedException("Incorrect Image source for the RoundButton");

        Text startText = startButton.GetComponentInChildren<Text>();
        if (!startText.text.Equals("Start Game"))
            throw new EvalFailedException("RoundButton does not have the text 'Start Game' in it.");
        if (!startText.font.material.color.Equals(Color.white))
            throw new EvalFailedException("Incorrect font color on the Text GameObject");

        if (!Mathf.Approximately(((RectTransform)startText.transform).sizeDelta.y, -10.0f))
            throw new EvalFailedException("The RectTransform of the Text GameObject is not correct");

        Shadow startShadow = startText.GetComponent<Shadow>();
        if (startShadow == null)
            throw new EvalFailedException("Missing Shadow component on the Text GameObject");
        if (!Mathf.Approximately(startShadow.effectDistance.x, -1.5f) || !Mathf.Approximately(startShadow.effectDistance.y, -1.5f))
            throw new EvalFailedException("The Shadow effect distance is not correct");

        // Eval Finished
        EvalPassMessage(bandReached);
    }


    private void Pass60Band()
    {
        RectTransform border = (RectTransform) GameObject.Find("RoundButton").transform.Find("Border");
        if (border == null)
            throw new EvalFailedException("Could not find Border as a child of RoundButton.");

        Image borderImg = border.GetComponent<Image>();
        if (!borderImg.sprite.texture.name.Equals("circle_border_md"))
            throw new EvalFailedException("Incorrect Image source for the Border");
        if (!borderImg.color.Equals(new Color32(255, 255, 128, 255)))
            throw new EvalFailedException("The Image color for the Border GameObject is incorrect");

        if (!Mathf.Approximately(border.anchoredPosition.x, 0.0f)
                || !Mathf.Approximately(border.anchoredPosition.y, 3.0f)
                || !Mathf.Approximately(border.sizeDelta.x, 105.0f)
                || !Mathf.Approximately(border.sizeDelta.y, 105.0f))
            throw new EvalFailedException("The Border's RectTransform is not in the correct position OR is the wrong size.");

        RectTransform background = (RectTransform)FindObjectOfType<Canvas>().transform.GetChild(0);
        Debug.Log(background.gameObject.name);
        if (!background.gameObject.name.Equals("Background"))
            throw new EvalFailedException("Could not find Background GameObject as the first child of the Canvas");

        Debug.Log(background.anchorMax);
        if (!VectApprox(background.anchoredPosition, Vector2.zero)
                || !VectApprox(background.anchorMin, Vector2.zero)
                || !VectApprox(background.anchorMax, Vector2.one))
            throw new EvalFailedException("The Background's RecTransform isn't setup correctly. It should have a centered position and stretched anchors");

        //if (background.gameObject.GetComponent<Gradient>() != null)
            //throw new EvalFailedException("Could not find the Gradient component on the Background GameObject");

        // Eval Finished
        EvalPassMessage(bandReached);
    }


    private void Credit70Band()
    {
        Button startButton = GameObject.Find("RoundButton").GetComponent<Button>();
        if (!startButton.onClick.GetPersistentMethodName(0).Equals("LoadFirstLevel"))
            throw new EvalFailedException("LoadFirstLevel is not registered with the OnClick event of the RoundButton's Button component");
        
        EvalInProgressMessage(bandReached, "Press the Start Game button to load the next scene and continue the tests.");
        StartCoroutine(Credit70Coroutine());
    }

    private IEnumerator Credit70Coroutine()
    {        
#if C70
        while (!SceneManager.GetActiveScene().name.Equals("WalkingScene"))
            yield return null;

        yield return null;
        if (GameObject.Find("Managers").GetComponent<UIManager>() == null)
            throw new EvalFailedException("The Managers GameObject with the UIManagers component should be carried across from StartScene to Walking Scene." +
                " See DontDestroyOnLoad()");

        // Eval Finished
        EvalPassMessage(bandReached);
#else
        throw new EvalFailedException("Open the ProgressEvaluator script and uncomment the #define C70 line at the top");
#endif

    }


    private void Distinction80Band()
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + bandReached.ToString() + ": No automated checks in this band due to a very specific limitation in Unity. " +
            "Press Start Game and then the Quit button and make sure that your game exits (stops Play) correctly.");

        // Eval Finished
        EvalPassMessage(bandReached);
    }

    private void HighDist90Band()
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + bandReached.ToString() + ": No automated checks in this band. " +
            "See the Status-100Percent video and executables and observe how the player health bar looks and functions.");

        // Eval Finished
        EvalPassMessage(bandReached);
    }

    private void HighDist100Band()
    {
        Debug.LogWarning("PROGRESS EVALUATOR: " + bandReached.ToString() + ": No automated checks in this band. " +
            "See the Status-100Percent video and executables and observe how the loading screen lerps in and out of view between scenes.");

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

    /*private bool RegexCheck(string fileName, string regExp) {
        string[] laContents = Directory.GetFiles("./", fileName, SearchOption.AllDirectories);
        if (laContents.Length == 0)
            throw new EvalFailedException("Can't find the " + fileName + " script in your Assets folder. ");
        string laContent = File.ReadAllText(laContents[0]);
        Regex rx = new Regex(regExp);
        return rx.Match(laContent).Success;
    }*/
}

public class EvalFailedException : Exception
{
    public EvalFailedException(string message) : base(message) { }
}
