using UnityEngine;

namespace InfiniteScroller {
  public struct DataCell {
    public GameObject Go {
      get;
    }
    public int Index {
      get;
    }

    public DataCell(int index, GameObject go) {
      Index = index;
      Go = go;
    }
  }
}