using JKFrame;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuffSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text layerCountText;
    [SerializeField] private Image maskImage;

    public void Init(BuffConfig config)
    {
        iconImage.sprite = config.icon;
        layerCountText.gameObject.SetActive(config.maxLayer > 1);
    }

    public void UpdateLayer(int layer)
    {
        layerCountText.text = layer.ToString();
    }

    public void UpdateMask(float maskFillAmount)
    {
        maskImage.fillAmount = maskFillAmount;
    }

    public void Destroy()
    {
        iconImage.sprite = null;
        this.GameObjectPushPool();
    }

}
