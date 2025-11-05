using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSelectionManager : MonoBehaviour
{
    private const string SELECTED_ANIMAL_KEY = "SelectedAnimal";

    public static void SaveSelectedAnimal(int animalIndex)
    {
        PlayerPrefs.SetInt(SELECTED_ANIMAL_KEY, animalIndex);
        PlayerPrefs.Save();
    }

    public static int GetSelectedAnimalIndex()
    {
        return PlayerPrefs.GetInt(SELECTED_ANIMAL_KEY, 1); // Default: 1 (Gorilla)
    }

    public static AnimalType GetSelectedAnimal()
    {
        int index = GetSelectedAnimalIndex();
        return (AnimalType)(index - 1); // Convert panel index (1-4) to enum (0-3)
    }
}
