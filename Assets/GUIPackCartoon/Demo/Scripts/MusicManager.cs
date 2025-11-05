// Copyright (C) 2015 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
    // This class handles updating the music UI widgets depending on the player's selection.
    // Updated to work with SoundController
    public class MusicManager : MonoBehaviour
    {
        private Slider m_musicSlider;
        private GameObject m_musicButton;

        private void Start()
        {
            m_musicSlider = GetComponent<Slider>();
            
            // Load từ SoundController's PlayerPrefs key (0 = ON, 1 = OFF)
            int musicState = PlayerPrefs.GetInt("musicState", 0);
            m_musicSlider.value = musicState == 0 ? 1 : 0; // Invert vì slider: 1 = ON, 0 = OFF
            
            m_musicButton = GameObject.Find("MusicButton/Button");
            
            Debug.Log($"[MusicManager] Music state loaded: {musicState} (slider value: {m_musicSlider.value})");
        }

        public void SwitchMusic()
        {
            // Lấy SoundController
            var soundController = SoundController.GetInstance();
            if (soundController != null)
            {
                // slider.value = 1 nghĩa là ON, 0 nghĩa là OFF
                bool desiredState = m_musicSlider.value >= 0.5f;
                
                Debug.Log($"[MusicManager] Slider value: {m_musicSlider.value}, Setting music to: {(desiredState ? "ON" : "OFF")}");
                
                // Set state trực tiếp (không toggle)
                soundController.SetMusicState(desiredState);
                
                if (m_musicButton != null)
                    m_musicButton.GetComponent<MusicButton>().ToggleSprite();
            }
            else
            {
                Debug.LogWarning("[MusicManager] SoundController not found!");
            }
        }
    }
}
