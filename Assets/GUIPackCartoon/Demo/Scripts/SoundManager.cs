// Copyright (C) 2015 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
    // This class handles updating the sound UI widgets depending on the player's selection.
    // Updated to work with SoundController
    public class SoundManager : MonoBehaviour
    {
        private Slider m_soundSlider;
        private GameObject m_soundButton;

        private void Start()
        {
            m_soundSlider = GetComponent<Slider>();
            
            // Load từ SoundController's PlayerPrefs key (0 = ON, 1 = OFF)
            int sfxState = PlayerPrefs.GetInt("sfxState", 0);
            m_soundSlider.value = sfxState == 0 ? 1 : 0; // Invert vì slider: 1 = ON, 0 = OFF
            
            m_soundButton = GameObject.Find("SoundButton/Button");
            
            Debug.Log($"[SoundManager] SFX state loaded: {sfxState} (slider value: {m_soundSlider.value})");
        }

        public void SwitchSound()
        {
            // Lấy SoundController
            var soundController = SoundController.GetInstance();
            if (soundController != null)
            {
                // slider.value = 1 nghĩa là ON, 0 nghĩa là OFF
                bool desiredState = m_soundSlider.value >= 0.5f;
                
                Debug.Log($"[SoundManager] Slider value: {m_soundSlider.value}, Setting sound FX to: {(desiredState ? "ON" : "OFF")}");
                
                // Set state trực tiếp (không toggle)
                soundController.SetFXState(desiredState);
                
                if (m_soundButton != null)
                {
                    m_soundButton.GetComponent<SoundButton>().ToggleSprite();
                }
            }
            else
            {
                Debug.LogWarning("[SoundManager] SoundController not found!");
            }
        }
    }
}
