using System;
using NanoverImd.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace NanoverImd.UI
{
    public class PlayerColor : MonoBehaviour
    {
        private static string ColorKey = "nanover.player.color";

        private static Color DefaultColor = new Color(255, 102, 0);

        [SerializeField]
        private Image userIcon;

        [SerializeField]
        private GameObject presetButtonParent;
    
        public static Color GetPlayerColor()
        {
            if (PlayerPrefs.HasKey(ColorKey))
            {
                var value = PlayerPrefs.GetString(ColorKey);
                if (VisualiserFactory.TryParseColor(value, out var color))
                {
                    return color;
                }
            }
            return DefaultColor;
        }

        public static void SetPlayerColor(Color color)
        {
            PlayerPrefs.SetString(ColorKey, "#" + ColorUtility.ToHtmlStringRGB(color));
            PlayerColorChanged?.Invoke();
        }

        public static event Action PlayerColorChanged;
    
        private void Awake()
        {
            PlayerColorChanged += UpdateIconColor;
            UpdateIconColor();
            foreach (var childObj in presetButtonParent.transform)
            {
                var child = (childObj as Transform);
                var button = child.GetComponent<Button>();
                if (button == null)
                    continue;
                var color = child.Find("Icon").Find("Color").GetComponent<Image>().color;
                button.onClick.AddListener(() =>
                {
                    SetPlayerColor(color);
                });
            }
        }

        private void UpdateIconColor()
        {
            if (userIcon != null)
                userIcon.color = GetPlayerColor();
        }
    }
}
