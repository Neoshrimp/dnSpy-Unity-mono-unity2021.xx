using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMonoDllSourceCodePatcher.V40 {
	internal class CoreFxPatcher {

		
		SolutionOptions solutionOptions;
		

		public CoreFxPatcher(SolutionOptions solutionOptions) {
			this.solutionOptions = solutionOptions;
		}

		public void Patch() {
			PatchCoreFxTargets();
			PatchCoreFxFilters();
		}

		void PatchCoreFxTargets() {
			var textFilePatcher = new TextFilePatcher(Path.Combine(solutionOptions.UnityVersionDir, "msvc", "clrcompression.targets"));
			int indexC = textFilePatcher.GetIndexOfLine(line => line.Text.Contains(@"</ClCompile>"));
			var indentC = textFilePatcher.GetLeadingWhitespace(indexC);

			var missingIncludes = @"
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\command.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories >$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;% (AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\fast_log.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories>$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\encoder_dict.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories>$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\constants.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories>$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\context.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories>$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\transform.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories>$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\platform.c"">
	<CompileAs>CompileAsC</CompileAs>
	<AdditionalIncludeDirectories>$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>
	<PreprocessorDefinitions>%(PreprocessorDefinitions);DLLEXPORT=__declspec(dllexport);BROTLI_SHARED_COMPILATION</PreprocessorDefinitions>
</ClCompile>
";

			missingIncludes = AppendIndent(missingIncludes, indentC, textFilePatcher.NewLine);
			textFilePatcher.Insert(indexC + 1, missingIncludes);

			int indexH = textFilePatcher.GetIndexOfLine(line => line.Text.Contains(@"<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include\brotli\types.h""/>"));
			var indentH = textFilePatcher.GetLeadingWhitespace(indexH);

			var missingHeaders = @"
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\context.h""/>
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\transform.h""/>
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\platform.h""/>
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\encoder_dict.h""/>						
";

			missingIncludes = AppendIndent(missingHeaders, indentH, textFilePatcher.NewLine);
			textFilePatcher.Insert(indexH + 1, missingHeaders);

			textFilePatcher.Write();
		}


		public void PatchCoreFxFilters() {
			var textFilePatcher = new TextFilePatcher(Path.Combine(solutionOptions.UnityVersionDir, "msvc", "clrcompression.targets.filters"));

			int index = textFilePatcher.GetIndexOfLine(line => line.Text.Contains(@"</ClCompile>"));
			var indent = textFilePatcher.GetLeadingWhitespace(index);
			
			var missingFilters = @"
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\command.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\fast_log.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\enc\encoder_dict.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\constants.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\context.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\transform.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClCompile Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\platform.c"">
	<Filter>Source Files$(ClrCompressionFilterSubFolder)</Filter>
</ClCompile>
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\context.h"">
	<Filter>Header Files$(ClrCompressionFilterSubFolder)</Filter>
</ClInclude>
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\transform.h"">
	<Filter>Header Files$(ClrCompressionFilterSubFolder)</Filter>
</ClInclude>
<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\common\platform.h"">
	<Filter>Header Files$(ClrCompressionFilterSubFolder)</Filter>
</ClInclude>
	<ClInclude Include=""$(MonoSourceLocation)\external\corefx\src\Native\AnyOS\brotli\include\brotli\types.h"">
	<Filter>Header Files$(ClrCompressionFilterSubFolder)</Filter>
</ClInclude>
";

			missingFilters = AppendIndent(missingFilters, indent, textFilePatcher.NewLine);
			textFilePatcher.Insert(index+1, missingFilters);
			textFilePatcher.Write();
		}

		static string AppendIndent(string s, string indent, string newlineSymbol) {
			return s.Split('\n').Select(l => indent + l + newlineSymbol).Aggregate((l1, l2) => l1 + l2);
		}

	}
}
