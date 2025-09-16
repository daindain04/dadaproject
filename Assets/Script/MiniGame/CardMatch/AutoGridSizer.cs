using UnityEngine;
using UnityEngine.UI;

public class AutoGridSizer : MonoBehaviour
{
    [Header("�׸��� ����")]
    public Vector2 preferredCellSize = new Vector2(100, 150); // ��ȣ�ϴ� ī�� ũ��
    public Vector2 spacing = new Vector2(10, 10);
    public int maxColumns = 6; // �ִ� �� ����

    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// ī�� ������ ���� �׸��带 �ڵ� �����մϴ�.
    /// </summary>
    public void AdjustGrid(int totalCards)
    {
        if (gridLayout == null) return;

        // ������ ��/�� ���� ���
        int columns = CalculateOptimalColumns(totalCards);
        int rows = Mathf.CeilToInt((float)totalCards / columns);

        // ��� ������ ���� ũ��
        Vector2 availableSize = rectTransform.rect.size;

        // �����̽��� ����� ���� ��� ������ ũ��
        float usableWidth = availableSize.x - (columns - 1) * spacing.x;
        float usableHeight = availableSize.y - (rows - 1) * spacing.y;

        // ī�� ũ�� ��� (���� ����)
        float cellWidth = usableWidth / columns;
        float cellHeight = usableHeight / rows;

        // ���ϴ� ���� ���� (ī��� ���� ���ΰ� �� ��)
        float preferredRatio = preferredCellSize.y / preferredCellSize.x;

        // �� �������� ���� ����
        float calculatedHeight = cellWidth * preferredRatio;

        // ���̰� ��� ������ ������ �ʰ��ϸ� ���� �������� �� ����
        if (calculatedHeight > cellHeight)
        {
            cellHeight = usableHeight / rows;
            cellWidth = cellHeight / preferredRatio;
        }
        else
        {
            cellHeight = calculatedHeight;
        }

        // �׸��� ���̾ƿ� ���� ����
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        Debug.Log($"Cards: {totalCards}, Grid: {columns}x{rows}, Cell Size: {cellWidth:F1}x{cellHeight:F1}");
    }

    /// <summary>
    /// ī�� ������ ���� ������ �� ���� ���
    /// </summary>
    int CalculateOptimalColumns(int totalCards)
    {
        // ī�� ������ ����ȭ�� ���̾ƿ�
        switch (totalCards)
        {
            case <= 6:
                return Mathf.Min(totalCards, 3); // 1~6��: �ִ� 3��
            case <= 12:
                return 4; // 7~12��: 4�� (3x4 �Ǵ� 4x3)
            case <= 20:
                return 5; // 13~20��: 5��
            case <= 30:
                return 6; // 21~30��: 6��
            default:
                return maxColumns; // �� �̻�: �ִ� �� ����
        }
    }
}