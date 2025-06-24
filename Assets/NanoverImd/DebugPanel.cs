using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace NanoverImd
{
    public class DebugPanel : MonoBehaviour
    {
        public static DebugPanel Instance;

        [SerializeField]
        private Text text;

        private void Start()
        {
            Instance = this;
        }

        public void ClearText()
        {
            text.gameObject.SetActive(false);
            text.text = "";
        }

        public void AddText(string value)
        {
            text.gameObject.SetActive(true);
            text.text += value;
        }
    }
}