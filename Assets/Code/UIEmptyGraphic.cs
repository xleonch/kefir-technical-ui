using UnityEngine;
using UnityEngine.UI;

namespace Code {

	[RequireComponent(typeof(CanvasRenderer))]
	public class UIEmptyGraphic : Graphic {

		protected override void OnPopulateMesh(VertexHelper vh) {
			vh.Clear();
		}

	}

}