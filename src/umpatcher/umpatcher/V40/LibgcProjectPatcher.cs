using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMonoDllSourceCodePatcher.V40 {
	sealed class LibgcProjectPatcher : ProjectPatcherV40 {
		public LibgcProjectPatcher(SolutionOptionsV40? solutionOptions)
			: base(solutionOptions, solutionOptions?.LibgcProject) {
		}

		protected override void PatchCore() { }
	}
}
