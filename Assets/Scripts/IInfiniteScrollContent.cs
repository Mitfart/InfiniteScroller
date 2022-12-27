using System;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteScroller {
  public interface IInfiniteScrollContent {
    ScrollRect ScrollRect {
      get;
    }
    bool Infinite {
      get;
    }

    int TotalCount {
      get;
    }

    int First {
      get;
    }
    int Last {
      get;
    }
    int CurrentFirst {
      get;
    }
    int CurrentLast {
      get;
    }

    float ContentWidth {
      get;
    }
    float ContentHeight {
      get;
    }

    float ViewportLength {
      get;
    }

    RectTransform Content {
      get;
    }
    HorizontalOrVerticalLayoutGroup LayoutGroup {
      get;
    }
    RectTransform Viewport {
      get;
    }

    void Generate(RectTransform newCell, int count, Action < int, ICell > setCellData = null);
    void Clear();
  }
}