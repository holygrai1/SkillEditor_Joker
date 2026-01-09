using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JKFrame;

public class UI_SkillSlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static UI_SkillSlotBase currentEnterSlot { get; private set; } // 当前鼠标进入的格子
    [SerializeField] protected GameObject selected;
    [SerializeField] protected Image iconImage;
    protected SkillConfig skillConfig;

    public void Init()
    {
        OnPointerExit(null);
    }
    protected void Show(SkillConfig skillConfig)
    {
        this.skillConfig = skillConfig;
        iconImage.gameObject.SetActive(skillConfig != null);
        if (skillConfig != null)
        {
            iconImage.sprite = skillConfig.skillIcon;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentEnterSlot = this;
        selected.gameObject.SetActive(true);
        if (skillConfig != null)
        {
            UISystem.Show<UI_SkillInfoPopupWindow>().Show(transform.position, ((RectTransform)transform).sizeDelta.y / 2, skillConfig);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentEnterSlot = null;
        selected.gameObject.SetActive(false);
        if (skillConfig != null)
        {
            UI_SkillInfoPopupWindow skillInfoPopupWindow = UISystem.GetWindow<UI_SkillInfoPopupWindow>();
            if (skillInfoPopupWindow != null && skillInfoPopupWindow.gameObject.activeInHierarchy)
            {
                UISystem.Close<UI_SkillInfoPopupWindow>();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillConfig == null) return;
        iconImage.transform.SetParent(UISystem.DragLayer);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillConfig == null) return;
        iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (skillConfig == null) return;
        // 如果格子是自己不需要处理额外逻辑
        if (currentEnterSlot != null)
        {
            OnDragToNewSlot(currentEnterSlot);
        }
        iconImage.transform.SetParent(transform);
        iconImage.transform.SetAsFirstSibling();
        iconImage.transform.localPosition = Vector3.zero;
    }

    protected virtual void OnDragToNewSlot(UI_SkillSlotBase other)
    {

    }
}
