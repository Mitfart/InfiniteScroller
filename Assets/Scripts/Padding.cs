using UnityEngine;

namespace InfiniteScroller {
  public struct Padding {
    public int Top, Bottom, Left, Right;

    public Padding(RectOffset offset) {
      Top = offset.top;
      Bottom = offset.bottom;
      Left = offset.left;
      Right = offset.right;
    }
  }
}