using TMPro;
using UnityEngine;

public class SizeBar : MonoBehaviour
{
    [SerializeField] private Transform SizeBarImg;
    [SerializeField] private TMP_Text SizeBarText;
    [SerializeField] private int MaxSize;

    public void SetSizeBar(float size)
    {
        Vector3 currentScale = SizeBarImg.localScale;
        float scale = Mathf.Clamp(size / MaxSize, 0f, 1f);

        SizeBarImg.localScale = new Vector3(scale, currentScale.y, currentScale.z);

        SizeBarText.SetText(Mathf.Round(size-7).ToString());
    }
}
