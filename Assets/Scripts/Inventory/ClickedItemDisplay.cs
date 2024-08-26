using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickedItemDisplay : MonoBehaviour
{
    [SerializeField] GameObject _toolTip;
    [SerializeField] Image _itemImage;
    [SerializeField] TextMeshProUGUI _itemNameText, _itemValueText;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] float _toolTipDuration = 1f;

    float _timer = 0;

    void Start()
    {
        Item.OnAnyItemClicked += Item_OnAnyItemClicked;
        _toolTip.SetActive(false);
    }

    void OnDestroy()
    {
        Item.OnAnyItemClicked -= Item_OnAnyItemClicked;
    }

    void Update()
    {
        if(_toolTip.activeSelf)
        {
            _toolTip.transform.position = Input.mousePosition;
            _timer += Time.deltaTime;

            if(_timer > _toolTipDuration)
            {
                _toolTip.SetActive(false);
            }
        }
    }

    void Item_OnAnyItemClicked(Item item)
    {
        if(item.IsMoney)
        {
            _audioSource.Play();
        }
        else
        {
            _itemImage.sprite = item.ItemSO.ItemSprite;
            _itemNameText.text = item.ItemSO.ItemName;
            _itemValueText.text = $"Value {item.ItemSO.BaseValue}₡";
            _toolTip.SetActive(true);
            _timer = 0;
        }
    }
}
