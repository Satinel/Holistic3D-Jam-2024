using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickedItemDisplay : MonoBehaviour
{
    [SerializeField] GameObject _toolTip;
    [SerializeField] RectTransform _tipRectTransform, _tipPositioner;
    [SerializeField] TextMeshProUGUI _itemNameText, _itemValueText;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] float _toolTipDuration = 1f;
    [SerializeField] Vector2 _tipPositionerDefaults;

    float _timer = 0;
    bool _isDragging;

    void OnEnable()
    {
        Item.OnCoinClicked += Item_OnCoinClicked;
        Item.OnItemPointedAt += Item_OnItemPointedAt;
        Item.OnItemDrag += Item_OnItemDrag;
    }

    void OnDisable()
    {
        Item.OnCoinClicked -= Item_OnCoinClicked;
        Item.OnItemPointedAt -= Item_OnItemPointedAt;
        Item.OnItemDrag -= Item_OnItemDrag;
    }

    void Update()
    {
        if(_toolTip.activeSelf)
        {
            _toolTip.transform.position = Input.mousePosition;

            if(_tipRectTransform.anchoredPosition.x > 0)
            {
                if(_tipRectTransform.anchoredPosition.y > 0)
                {
                    _tipPositioner.anchoredPosition = new Vector2(-_tipPositionerDefaults.x, -_tipPositionerDefaults.y);
                }
                else
                {
                    _tipPositioner.anchoredPosition = new Vector2(-_tipPositionerDefaults.x, _tipPositionerDefaults.y);
                }
            }
            else
            {
                if(_tipRectTransform.anchoredPosition.y > 0)
                {
                    _tipPositioner.anchoredPosition = new Vector2(_tipPositionerDefaults.x, -_tipPositionerDefaults.y);
                }
                else
                {
                    _tipPositioner.anchoredPosition = new Vector2(_tipPositionerDefaults.x, _tipPositionerDefaults.y);
                }
            }
            
            _timer += Time.deltaTime;
            
            if(_isDragging) { return; }

            if(_timer > _toolTipDuration)
            {
                _toolTip.SetActive(false);
            }
        }
    }

    void Item_OnCoinClicked()
    {
        _audioSource.Play();
    }

    void Item_OnItemPointedAt(Item item)
    {
        if(_isDragging) { return; }

        // _itemImage.sprite = item.ItemSO.ItemSprite;
        _itemNameText.text = item.ItemSO.ItemName;
        _itemValueText.text = $"Value {item.ItemSO.BaseValue}₡";
        _toolTip.SetActive(true);
        _timer = 0;
    }

    void Item_OnItemDrag(bool isDragging)
    {
        _isDragging = isDragging;
    }
}
