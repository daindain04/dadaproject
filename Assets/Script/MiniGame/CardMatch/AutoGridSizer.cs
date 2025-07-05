using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoGridSizer : MonoBehaviour
{
    [Tooltip("한 줄에 놓일 카드 개수 (열 수).")]
    public int columns;
    [Tooltip("총 카드 개수 (pairsCount * 2).")]
    public int totalCards;

    GridLayoutGroup grid;
    RectTransform rt;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rt = GetComponent<RectTransform>();
    }

    // 패널이 활성화될 때마다 호출
    void OnEnable()
    {
        ResizeCells();
        // 강제로 레이아웃 리빌드
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    void ResizeCells()
    {
        float W = rt.rect.width;
        float H = rt.rect.height;
        float sx = grid.spacing.x;
        float sy = grid.spacing.y;
        int rows = Mathf.CeilToInt((float)totalCards / columns);

        float cellW = (W - sx * (columns - 1)) / columns;
        float cellH = (H - sy * (rows - 1)) / rows;

        grid.cellSize = new Vector2(cellW, cellH);
    }
}
