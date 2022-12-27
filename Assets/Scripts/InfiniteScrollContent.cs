using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteScroller {
  public abstract class InfiniteScrollContent: MonoBehaviour, IInfiniteScrollContent {
    protected RectTransform CellPrefab;

    protected Vector2 CellSize;
    protected float Gap;

    protected bool Generated;
    protected Padding Padding;

    protected Action < int, ICell > SetCellData;
    protected List < DataCell > VisibleCells;
    [field: SerializeField] public ScrollRect ScrollRect {
      get;
      protected set;
    }
    [field: SerializeField] public bool Infinite {
      get;
      protected set;
    } = true;

    public int TotalCount {
      get;
      protected set;
    }

    public abstract int First {
      get;
    }
    public abstract int Last {
      get;
    }
    public int CurrentFirst {
      get;
      protected set;
    }
    public int CurrentLast {
      get;
      protected set;
    }

    public float ContentHeight {
      get => Content.rect.height;
      protected set => Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
    }

    public float ContentWidth {
      get => Content.rect.width;
      protected set => Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
    }

    public abstract float ViewportLength {
      get;
    }

    public RectTransform Content {
      get;
      protected set;
    }
    public HorizontalOrVerticalLayoutGroup LayoutGroup {
      get;
      protected set;
    }
    public RectTransform Viewport {
      get;
      protected set;
    }

    public virtual void Generate(RectTransform newCell, int count, Action < int, ICell > setCellData = null) {
      if (Generated) return;

      Generated = true;

      InitViewport();
      InitCells();
      InitContentContainer();
      InitScrollRect();

      if (!newCell.TryGetComponent < ICell > (out ICell _))
        throw new Exception($"Cell prefab must contain <{nameof(ICell)}> component!");

      if (TotalCount <= 0) return;

      CreateAllCells();

      void InitViewport() {
        LayoutGroup = GetComponent < HorizontalOrVerticalLayoutGroup > ();
        Viewport = ScrollRect.viewport;
      }

      void InitCells() {
        CellPrefab = newCell;
        TotalCount = count;
        SetCellData = setCellData;
        VisibleCells = new List < DataCell > ();

        Rect rect = CellPrefab.GetComponent < RectTransform > ().rect;
        CellSize.x = rect.width;
        CellSize.y = rect.height;
      }

      void InitContentContainer() {
        Content = GetComponent < RectTransform > ();
        Gap = LayoutGroup.spacing;
        Padding = new Padding(LayoutGroup.padding);

        Content.anchoredPosition = Vector2.zero;
        Content.anchorMin = Vector2.up;
        Content.anchorMax = Vector2.up;
      }

      void InitScrollRect() {
        if (Infinite) {
          ScrollRect.horizontalScrollbar.gameObject.SetActive(false);
          ScrollRect.verticalScrollbar.gameObject.SetActive(false);

          ScrollRect.horizontalScrollbar = null;
          ScrollRect.verticalScrollbar = null;

          ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }

        ScrollRect.onValueChanged.AddListener(OnScroll);
      }
    }
    public virtual void Clear() {
      if (!Generated) return;

      DestroyAllCells();
      Generated = false;

      ContentHeight = 0f;
      ContentWidth = 0f;

      LayoutGroup.padding.top = Padding.Top;
      LayoutGroup.padding.bottom = Padding.Bottom;
      LayoutGroup.padding.left = Padding.Left;
      LayoutGroup.padding.right = Padding.Right;
    }

    public virtual void CreateCell(int index, ScrollerPanelSide panelSide) {
      RectTransform cell = Instantiate(CellPrefab, Content);
      int order = GetOrder(index);
      int dataIndex = GetDataIndex(index);

      cell.name = $"{CellPrefab.name}_{dataIndex}";
      cell.SetSiblingIndex(order);

      VisibleCells.Insert(order, new DataCell(index, cell.gameObject));
      SetCellData?.Invoke(dataIndex, cell.gameObject.GetComponent < ICell > ());

      OnCreateCell(index, panelSide);
    }
    public virtual void DestroyCell(int index, ScrollerPanelSide panelSide) {
      int order = GetOrder(index - 1);
      DataCell cell = VisibleCells[order];

      VisibleCells.RemoveAt(order);
      Destroy(cell.Go);

      OnDestroyCell(index, panelSide);
    }

    protected int GetDataIndex(int index) {
      if (!Infinite) return Mathf.Clamp(index, 0, TotalCount - 1);

      index %= TotalCount;
      if (index < 0) index += TotalCount;

      return Mathf.Clamp(index, 0, TotalCount - 1);
    }
    protected int GetOrder(int index) {
      int maxOrder = VisibleCells.Count;

      for (int i = 0; i < maxOrder; i++)
        if (index < VisibleCells[i].Index)
          return i;

      return maxOrder;
    }

    protected virtual void OnScroll(Vector2 position) {
      if (!Generated || TotalCount <= 0) return;

      if (Last < CurrentFirst || First > CurrentLast) {
        DestroyAllCells();
        CreateAllCells();
        return;
      }

      if (CurrentFirst > First) {
        for (int i = CurrentFirst - 1; i >= First; i--) CreateCell(i, ScrollerPanelSide.Start);

        CurrentFirst = First;
      }

      if (CurrentLast < Last) {
        for (int i = CurrentLast + 1; i <= Last; i++) CreateCell(i, ScrollerPanelSide.End);

        CurrentLast = Last;
      }

      if (CurrentFirst < First) {
        for (int i = CurrentFirst; i < First; i++) DestroyCell(i, ScrollerPanelSide.Start);

        CurrentFirst = First;
      }

      if (CurrentLast > Last) {
        for (int i = CurrentLast; i > Last; i--) DestroyCell(i, ScrollerPanelSide.End);

        CurrentLast = Last;
      }
    }

    public abstract void OnCreateCell(int index, ScrollerPanelSide panelSide);
    public abstract void OnDestroyCell(int index, ScrollerPanelSide panelSide);

    protected abstract void CreateAllCells();
    protected abstract void DestroyAllCells();

    protected abstract float GetCellsLength(int count);
  }
}