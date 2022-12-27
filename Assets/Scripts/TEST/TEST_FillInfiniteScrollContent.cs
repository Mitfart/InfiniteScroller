using System.Collections;
using UnityEngine;

namespace InfiniteScroller{
   public class TEST_FillInfiniteScrollContent : MonoBehaviour{
      public NumberedCell cellPrefab;
      public bool         autoGenerate;
      public int          count;

      InfiniteScrollContent _infiniteScroll;

      void Start(){
         _infiniteScroll = GetComponent<InfiniteScrollContent>();
         if (autoGenerate) StartCoroutine(DelayGenerate());
      }

      [ContextMenu("Generate")]
      void Generate(){
         _infiniteScroll.Generate(
            cellPrefab.GetComponent<RectTransform>(),
            count,
            (index, cell) => {
               if (cell is NumberedCell numberedCell)
                  numberedCell.SetNumber(index);
               else
                  Debug.Log("???");
            });
      }
      IEnumerator DelayGenerate(){
         yield return new WaitForEndOfFrame();

         Generate();
      }
   }
}
