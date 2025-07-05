using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    /** --------- 位置相关（新增） --------- */
    [Header("Position Presets")]
    public Vector3 ResetPosition;          // Reset 时的目标位置
    public Vector3 SmallPosition;          // ToSmall 时的目标位置
    public Vector3 BigPosition;            // ToBig 时的目标位置
    public float   MoveSpeed = 2f;         // 位置插值速度

    /** --------- 现有字段 --------- */
    public Vector3 FinalPosition = Vector3.zero;  // 仍保留，外部若依赖不会报错

    [Header("Scale Presets")]
    public float SmallScale = 1f;
    public float BigScale   = 3f;
    public float ScaleSpeed = 2f;
    public bool  canScale   = false;

    [Header("Scale Limits")]
    public float MaxScale = 4f;
    public float MinScale = 0.5f;

    public Transform PresetTransform;
    public Vector3   _presetPosition;
    public Vector3   _presetLocalScale;

    private float _targetScale  = 1f;
    private float _currentScale = 1f;

    /** --------- 私有位置插值值（新增） --------- */
    private Vector3 _targetPosition;
    private Vector3 _currentPosition;

    /* ===========================  MonoBehaviour  =========================== */

    void Start()
    {
        // 记录初始姿态
        _presetPosition    = PresetTransform.localPosition;
        _presetLocalScale  = PresetTransform.localScale;

        // 如果 ResetPosition 没手动设，把预设位置当作 ResetPosition
        if (ResetPosition == Vector3.zero)  ResetPosition = _presetPosition;

        /* ------ 初始化缩放 ------ */
        _currentScale = 0f;
        _targetScale  = 0f;
        transform.localScale = Vector3.zero;

        /* ------ 初始化位置 ------ */
        _currentPosition = ResetPosition;
        _targetPosition  = ResetPosition;
        transform.localPosition = _currentPosition;

        canScale = false;
        ActivateMushroomByDifficulty();
    }
    
    void ActivateMushroomByDifficulty()
    {
        Transform blue = transform.Find("BlueMushroom");
        Transform red = transform.Find("RedMushroom");
        Transform yellow = transform.Find("YellowMushroom");

        // 先全部禁用
        blue.gameObject.SetActive(false);
        red.gameObject.SetActive(false);
        yellow.gameObject.SetActive(false);

        switch (GameStateHolder.Instance.SelectedDifficulty)
        {
            case 5:
                Debug.Log("难度5");
                blue.gameObject.SetActive(true);
                break;
            case 4:
                Debug.Log("难度4");
                red.gameObject.SetActive(true);
                break;
            case 3:
                Debug.Log("难度3");
                yellow.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("未识别的难度等级");
                break;
        }
    }

    void Update()
    {
        /* ------ Scale 缓动 ------ */
        _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * ScaleSpeed);
        if (Mathf.Abs(_currentScale - _targetScale) < 0.01f)
            _currentScale = _targetScale;

        transform.localScale = _presetLocalScale * _currentScale;

        /* ------ Position 缓动（新增） ------ */
        _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, Time.deltaTime * MoveSpeed);
        if (Vector3.Distance(_currentPosition, _targetPosition) < 0.01f)
            _currentPosition = _targetPosition;

        transform.localPosition = _currentPosition;
    }

    /* ===========================  外部接口（全部保留）  =========================== */

    public void Reset()
    {
        // 缩放即时复位，与原逻辑一致
        transform.localScale = _presetLocalScale;
        _currentScale        = 1f;
        _targetScale         = 1f;

        // 位置改为缓动到 ResetPosition
        _targetPosition = ResetPosition;

        canScale = false;
    }

    public void ToSmall()
    {
        SetTargetScale(SmallScale);
        _targetPosition = SmallPosition;          // --- NEW
    }

    public void ToBig()
    {
        SetTargetScale(BigScale);
        _targetPosition = BigPosition;            // --- NEW
    }

    public void SetTargetScale(float targetScale)
    {
        _targetScale = Mathf.Clamp(targetScale, MinScale, MaxScale);
    }

    public void MultiplyBy(float multiplier)
    {
        if (canScale)
        {
            float scale = GetTargetScale();
            SetTargetScale(scale * multiplier);
        }
    }

    public void AddBy(float addAmount)
    {
        if (canScale)
        {
            float scale = GetTargetScale();
            SetTargetScale(scale + addAmount);
        }
    }

    public float GetTargetScale()
    {
        return _targetScale;
    }

    public void SetCanScale(bool canScale)
    {
        this.canScale = canScale;
    }
}
