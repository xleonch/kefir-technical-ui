using UnityEngine;
using UnityEngine.UI;

namespace Client.Core.UI.Controls {

	[RequireComponent(typeof(CanvasRenderer))]
	public class UIEmptyGraphic : Graphic {

		protected override void OnPopulateMesh(VertexHelper vh) {
			vh.Clear();
		}

	}

}