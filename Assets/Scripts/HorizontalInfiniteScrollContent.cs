using System;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteScroller {
  [RequireComponent(typeof (HorizontalLayoutGroup))]
  public class HorizontalInfiniteScrollContent: InfiniteScrollContent {
    [field: SerializeField] public int CellsBefore {
      get;
      private set;
    }
    [field: SerializeField] public int CellsAfter {
      get;
      private set;
    }

    public float LengthBefore => GetCellsLength(CellsBefore);
    public float LengthAfter => GetCellsLength(CellsAfter);

    public override int First {
      get {
        int i = (int)((-Content.anchoredPosition.x - Padding.Left - LengthBefore) / (CellSize.x + Gap));
        return !Infinite ? GetDataIndex(i) : i;
      }
    }

    public override int Last {
      get {
        int i = (int)((-Content.anchoredPosition.x - Padding.Left + ViewportLength + LengthAfter) /
          (CellSize.x + Gap));
        return !Infinite ? GetDataIndex(i) : i;
      }
    }

    public override float ViewportLength => Viewport.rect.width;

    public override void Generate(RectTransform newCell, int count, Action < int, ICell > setCellData = null) {
      base.Generate(newCell, count, setCellData);

      ScrollRect.vertical = false;
      ScrollRect.horizontal = true;

      ContentWidth = GetCellsLength(TotalCount) + Padding.Left + Padding.Right;
    }

    public override void OnCreateCell(int index, ScrollerPanelSide panelSide) {
      switch (panelSide) {
      case ScrollerPanelSide.Start:
        LayoutGroup.padding.left -= (int)(CellSize.x + Gap);
        break;
      case ScrollerPanelSide.End:
        LayoutGroup.padding.right -= (int)(CellSize.x + Gap);
        break;
      case ScrollerPanelSide.NoSide:
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(panelSide), panelSide, null);
      }
    }
    public override void OnDestroyCell(int index, ScrollerPanelSide panelSide) {
      switch (panelSide) {
      case ScrollerPanelSide.Start:
        LayoutGroup.padding.left += (int)(CellSize.x + Gap);
        break;
      case ScrollerPanelSide.End:
        LayoutGroup.padding.right += (int)(CellSize.x + Gap);
        break;
      case ScrollerPanelSide.NoSide:
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(panelSide), panelSide, null);
      }
    }

    protected override void DestroyAllCells() {
      foreach(DataCell visibleCell in VisibleCells) Destroy(visibleCell.Go);
      VisibleCells.Clear();
    }
    protected override void CreateAllCells() {
      CurrentFirst = First;
      CurrentLast = Last;

      LayoutGroup.padding.left =
        Padding.Left + (int)(CurrentFirst == 0 ? 0 : CurrentFirst * (CellSize.x + Gap) - Gap);
      LayoutGroup.padding.right = Padding.Right + (int)((TotalCount - Last - 1) * (CellSize.x + Gap));

      for (int i = CurrentFirst; i <= CurrentLast; i++) {
        if (i >= TotalCount) continue;

        CreateCell(i, ScrollerPanelSide.NoSide);
      }
    }

    protected override float GetCellsLength(int count) {
      return count * (CellSize.x + Gap) - Gap;
    }
  }
}