using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoGridSizer : MonoBehaviour
{
    [Tooltip("�� �ٿ� ���� ī�� ���� (�� ��).")]
    public int columns;
    [Tooltip("�� ī�� ���� (pairsCount * 2).")]
    public int totalCards;

    GridLayoutGroup grid;
    RectTransform rt;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rt = GetComponent<RectTransform>();
    }

    // �г��� Ȱ��ȭ�� ������ ȣ��
    void OnEnable()
    {
        ResizeCells();
        // ������ ���̾ƿ� ������
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
