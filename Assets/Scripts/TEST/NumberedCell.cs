using TMPro;
using UnityEngine;

namespace InfiniteScroller{
   [RequireComponent(typeof(RectTransform))]
   public class NumberedCell : MonoBehaviour, ICell{
      public TextMeshProUGUI textMesh;

      public void SetNumber(int index){
         textMesh.text = index.ToString();
      }
   }
}
