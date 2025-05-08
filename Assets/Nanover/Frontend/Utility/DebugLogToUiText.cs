using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace Nanover.Frontend.Utility
{
    /// <summary>
    /// Component that listens to Unity's log output and outputs it to a
    /// linked TextMeshPro <see cref="Text" /> component.
    /// </summary>
    public class DebugLogToUiText : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Text outputTextField;

        [SerializeField]
        private int lineLimit = 32;
#pragma warning restore 0649

        private void Awake()
        {
            Application.logMessageReceived += HandleLog;
        }

        private List<string> lines = new List<string>();

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            lines.Add(condition);
            lines.AddRange(stackTrace.Split('\n'));

            lines = lines.Reverse<string>().Take(lineLimit).Reverse().ToList();

            outputTextField.text = string.Join("\n", lines);
        }
    }
}