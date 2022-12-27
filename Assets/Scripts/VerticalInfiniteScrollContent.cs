using System;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteScroller {
  [RequireComponent(typeof (VerticalLayoutGroup))]
  public class VerticalInfiniteScrollContent: InfiniteScrollContent {
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
        int i = (int)((Content.anchoredPosition.y - Padding.Top - LengthBefore) / (CellSize.y + Gap));
        return !Infinite ? GetDataIndex(i) : i;
      }
    }

    public override int Last {
      get {
        int i =
          (int)((Content.anchoredPosition.y - Padding.Top + ViewportLength + LengthAfter) / (CellSize.y + Gap));
        return !Infinite ? GetDataIndex(i) : i;
      }
    }

    public override float ViewportLength => Viewport.rect.height;

    public override void Generate(RectTransform newCell, int count, Action < int, ICell > setCellData = null) {
      base.Generate(newCell, count, setCellData);

      ScrollRect.vertical = true;
      ScrollRect.horizontal = false;

      ContentHeight = GetCellsLength(TotalCount) + Padding.Top + Padding.Bottom;
    }

    public override void OnCreateCell(int index, ScrollerPanelSide panelSide) {
      switch (panelSide) {
      case ScrollerPanelSide.Start:
        LayoutGroup.padding.top -= (int)(CellSize.y + Gap);
        break;
      case ScrollerPanelSide.End:
        LayoutGroup.padding.bottom -= (int)(CellSize.y + Gap);
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
        LayoutGroup.padding.top += (int)(CellSize.y + Gap);
        break;
      case ScrollerPanelSide.End:
        LayoutGroup.padding.bottom += (int)(CellSize.y + Gap);
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

      LayoutGroup.padding.top =
        Padding.Top + (int)(CurrentFirst == 0 ? 0 : CurrentFirst * (CellSize.y + Gap) - Gap);
      LayoutGroup.padding.bottom = Padding.Bottom + (int)((TotalCount - Last - 1) * (CellSize.y + Gap));

      for (int i = CurrentFirst; i <= CurrentLast; i++) {
        if (i >= TotalCount) continue;

        CreateCell(i, ScrollerPanelSide.NoSide);
      }
    }

    protected override float GetCellsLength(int count) {
      return count * (CellSize.y + Gap) - Gap;
    }
  }
}