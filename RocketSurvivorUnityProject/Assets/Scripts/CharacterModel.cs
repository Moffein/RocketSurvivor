using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2 {

	public class CharacterModel : MonoBehaviour {
		public CharacterModel.RendererInfo[] baseRendererInfos = Array.Empty<CharacterModel.RendererInfo>();

		[System.Serializable]
		public struct RendererInfo {
			// Token: 0x06001ECB RID: 7883 RVA: 0x000853F8 File Offset: 0x000835F8
			public bool Equals(CharacterModel.RendererInfo other) {
				return this.renderer == other.renderer && this.defaultMaterial == other.defaultMaterial && object.Equals(this.defaultShadowCastingMode, other.defaultShadowCastingMode) && object.Equals(this.ignoreOverlays, other.ignoreOverlays) && object.Equals(this.hideOnDeath, other.hideOnDeath);
			}

			// Token: 0x04002485 RID: 9349
			[PrefabReference]
			public Renderer renderer;

			// Token: 0x04002486 RID: 9350
			public Material defaultMaterial;

			// Token: 0x04002487 RID: 9351
			public ShadowCastingMode defaultShadowCastingMode;

			// Token: 0x04002488 RID: 9352
			public bool ignoreOverlays;

			// Token: 0x04002489 RID: 9353
			public bool hideOnDeath;
		}
	}

	// Token: 0x020009EE RID: 2542
	public class PrefabReferenceAttribute : PropertyAttribute {
	}
}
