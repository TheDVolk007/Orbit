using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Assets.Scripts.Interface
{
    public class DebugText : MonoBehaviour
    {
        private static string textToDisplay;
        private static bool visible = true;

        private Vector3 mousePos;

        private void Start () {
            Show(visible);
        }

        private const int SampleFrames = 30;
        private readonly List<float> frameTimes = new List<float>();
        private float fps;

        private void Update () {
            frameTimes.Add(Time.unscaledDeltaTime);

            if (frameTimes.Count == (SampleFrames - 1))
            {
                fps = 1 / frameTimes.Average();
                frameTimes.Clear();
            }
            textToDisplay = "FPS = " + Mathf.RoundToInt(fps);
            textToDisplay += "\r\nParticles: " + SceneManager.cachedParticlesData.Count;
            //textToDisplay += "\r\nBiggest velocity: " + SceneManager.BiggestVelocitySoFar;
            //textToDisplay += "\r\nBiggest velocity magnitude: " + SceneManager.BiggestVelocitySoFar.magnitude;
            //textToDisplay += "\r\nBiggest velocity in this cycle: " + SceneManager.BiggestVelocityThisCycle;
            //textToDisplay += "\r\nBiggest velocity in this cycle magnitude: " + SceneManager.BiggestVelocityThisCycle.magnitude;

            gameObject.GetComponent<GUIText>().text = textToDisplay;
            
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) &&
                mousePos.x < -Camera.main.orthographicSize * Camera.main.aspect * (1f - (0.30f * 2f)) && // 0.30 - 30% ширины экрана (энкоры)
                mousePos.y > Camera.main.orthographicSize * (1f - (0.15f * 2f)))
            {
                Show(!visible);
            }
        }

        private void Show(bool visible)
        {
            DebugText.visible = visible;
            gameObject.GetComponent<GUIText>().enabled = DebugText.visible;
        }
    }
}
