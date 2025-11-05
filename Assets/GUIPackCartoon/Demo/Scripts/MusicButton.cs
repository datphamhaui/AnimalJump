// Copyright (C) 2015 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace Ricimi
{
    // This class represents the music button that is used in several places in the demo.
    // It handles the logic to enable and disable the demo's music and store the player
    // selection to PlayerPrefs.
    // Updated to work with SoundController
    public class MusicButton : MonoBehaviour
    {
        private SpriteSwapper m_spriteSwapper;
        private bool m_on;

        private void Start()
        {
            m_spriteSwapper = GetComponent<SpriteSwapper>();
            
            // Load từ SoundController's PlayerPrefs key (0 = ON, 1 = OFF)
            m_on = PlayerPrefs.GetInt("musicState", 0) == 0;
            if (!m_on)
                m_spriteSwapper.SwapSprite();
                
            Debug.Log($"[MusicButton] Initial state: {(m_on ? "ON" : "OFF")}");
        }

        public void Toggle()
        {
            var soundController = SoundController.GetInstance();
            if (soundController != null)
            {
                soundController.ToggleMusic(ref m_on);
                m_spriteSwapper.SwapSprite();
                Debug.Log($"[MusicButton] 🎵 Music toggled: {(m_on ? "ON" : "OFF")}");
            }
            else
            {
                Debug.LogWarning("[MusicButton] SoundController not found!");
            }
        }

        public void ToggleSprite()
        {
            m_on = !m_on;
            m_spriteSwapper.SwapSprite();
        }
    }
}
