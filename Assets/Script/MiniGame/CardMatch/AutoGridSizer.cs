using UnityEngine;
using UnityEngine.UI;

public class AutoGridSizer : MonoBehaviour
{
    [Header("그리드 설정")]
    public Vector2 preferredCellSize = new Vector2(100, 150); // 선호하는 카드 크기
    public Vector2 spacing = new Vector2(10, 10);
    public int maxColumns = 6; // 최대 열 개수

    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 카드 개수에 따라 그리드를 자동 조정합니다.
    /// </summary>
    public void AdjustGrid(int totalCards)
    {
        if (gridLayout == null) return;

        // 최적의 열/행 개수 계산
        int columns = CalculateOptimalColumns(totalCards);
        int rows = Mathf.CeilToInt((float)totalCards / columns);

        // 사용 가능한 영역 크기
        Vector2 availableSize = rectTransform.rect.size;

        // 스페이싱을 고려한 실제 사용 가능한 크기
        float usableWidth = availableSize.x - (columns - 1) * spacing.x;
        float usableHeight = availableSize.y - (rows - 1) * spacing.y;

        // 카드 크기 계산 (비율 유지)
        float cellWidth = usableWidth / columns;
        float cellHeight = usableHeight / rows;

        // 원하는 비율 유지 (카드는 보통 세로가 더 김)
        float preferredRatio = preferredCellSize.y / preferredCellSize.x;

        // 폭 기준으로 높이 조정
        float calculatedHeight = cellWidth * preferredRatio;

        // 높이가 사용 가능한 공간을 초과하면 높이 기준으로 폭 조정
        if (calculatedHeight > cellHeight)
        {
            cellHeight = usableHeight / rows;
            cellWidth = cellHeight / preferredRatio;
        }
        else
        {
            cellHeight = calculatedHeight;
        }

        // 그리드 레이아웃 설정 적용
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        Debug.Log($"Cards: {totalCards}, Grid: {columns}x{rows}, Cell Size: {cellWidth:F1}x{cellHeight:F1}");
    }

    /// <summary>
    /// 카드 개수에 따른 최적의 열 개수 계산
    /// </summary>
    int CalculateOptimalColumns(int totalCards)
    {
        // 카드 개수별 최적화된 레이아웃
        switch (totalCards)
        {
            case <= 6:
                return Mathf.Min(totalCards, 3); // 1~6장: 최대 3열
            case <= 12:
                return 4; // 7~12장: 4열 (3x4 또는 4x3)
            case <= 20:
                return 5; // 13~20장: 5열
            case <= 30:
                return 6; // 21~30장: 6열
            default:
                return maxColumns; // 그 이상: 최대 열 개수
        }
    }
}