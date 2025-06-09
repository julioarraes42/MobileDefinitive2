using UnityEngine;
using UnityEngine.UI;

public class VelocityController : MonoBehaviour
{
    public Slider speedSlider;
    public float baseSpeed = 30f;

    private void Start()
    {
        // Configuração dos valores max e min
        speedSlider.minValue = 0.1f;
        speedSlider.maxValue = 3f;
        speedSlider.value = 1f;
    }

    public float GetSpeedMultiplier()
    {
        return speedSlider.value;
    }
}