using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Made by Peter De Vroom - Student - 2021 Introduction to Games Development, University of Technology Sydney
public class KeyVis : MonoBehaviour
{
    private GameObject _keyDisplay;
    private Text _keyDisplayText;

    void Start()
    {
        // setup canvas
        GameObject canvas = new GameObject("Canvas");
        canvas.AddComponent<CanvasScaler>();
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.layer = 5;

        // Create Text
        _keyDisplay = new GameObject("keyDisplay");
        _keyDisplay.layer = 5;
        _keyDisplay.transform.parent = canvas.transform;
        _keyDisplayText = _keyDisplay.AddComponent<Text>();
        RectTransform rt = _keyDisplay.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(0, -300, 0);
        rt.sizeDelta = new Vector2(300, 100);
        _keyDisplayText.fontSize = 30;
        _keyDisplayText.alignment = TextAnchor.UpperCenter;
        _keyDisplayText.text = "Key: ";
        _keyDisplayText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //Debug.Log(_keyDisplayText.isActiveAndEnabled);
    }

    void Update()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                CancelInvoke();
                _keyDisplayText.text = $"Key: {key}";
                Invoke("Reset", 1f);
            }
        }
    }

    private void Reset()
    {
        _keyDisplayText.text = "Key: ";
    }
}
