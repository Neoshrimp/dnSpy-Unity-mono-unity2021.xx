/*
    Copyright (C) 2018 de4dot@gmail.com

    This file is part of umpatcher

    umpatcher is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    umpatcher is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with umpatcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Linq;

namespace UnityMonoDllSourceCodePatcher.V40 {
	sealed class LibmonoDynamicProjectPatcher : ProjectPatcherV40 {
	

		readonly ProjectInfo? libgc;
		readonly ProjectInfo? libgcbdwgcProject;

		public LibmonoDynamicProjectPatcher(SolutionOptionsV40? solutionOptions)
			: base(solutionOptions, solutionOptions?.LibmonoDynamicProject) {
			libgcbdwgcProject = solutionOptions!.LibgcbdwgcProject;
			libgc = solutionOptions!.LibgcProject;
			if ((libgc == null && libgcbdwgcProject == null) || (libgc != null && libgcbdwgcProject != null))
				throw new InvalidOperationException();
		}

		protected override void PatchCore() {
			PatchOutDirs();
			PatchDebugInformationFormats(ConstantsV40.ReleaseConfigsWithNoPdb);
			PatchGenerateDebugInformationTags(ConstantsV40.ReleaseConfigsWithNoPdb);
			AddSourceFiles();
			if (libgc != null) {

				// not exactly a sound assumption
				if (File.Exists(Path.Combine(solutionOptions.UnityVersionDir, "external/corefx/src/Native/AnyOS/brotli/common/constants.c"))) {
					var coreFxPatcher = new CoreFxPatcher(solutionOptions);
					coreFxPatcher.Patch();
				}

				// change .o files location/name since corefx introdces a duplicate
				textFilePatcher.GetIndexesOfLine(l => l.Text.Contains(@"<WarningLevel>Level3</WarningLevel>")).ToList()
					.ForEach(i => UpdateOrCreateTag(i, i, "ObjectFileName", "$(IntDir)%(RelativeDir)"));

				textFilePatcher.Insert(
					textFilePatcher.GetIndexOfLine(l => l.Text.Contains("<Import Project=\"clrcompression.targets\" />")),
					@"
<ItemGroup>
  <ClInclude Include=""dnSpy.h"" />
</ItemGroup>
"
					);
	
			}
			if (libgcbdwgcProject != null) {
				AddProjectReference(libgcbdwgcProject);
				RemoveProjectReference("libgc.vcxproj");
			}
			PatchSolutionDir();
		}

		void AddSourceFiles() {
			var textFilePatcher = new TextFilePatcher(Path.Combine(solutionOptions.UnityVersionDir, "msvc", "libmini-common.targets"));
			int index = textFilePatcher.GetIndexOfLine(line => line.Text.Contains(@"<ClCompile Include=""$(MonoSourceLocation)\mono\mini\debugger-agent.c"""));
			var indent = textFilePatcher.GetLeadingWhitespace(index);
			textFilePatcher.Insert(index + 1, indent + @"<ClCompile Include=""$(MonoSourceLocation)\mono\mini\dnSpy.c"" />");
			textFilePatcher.Write();
		}

		
	}
}
