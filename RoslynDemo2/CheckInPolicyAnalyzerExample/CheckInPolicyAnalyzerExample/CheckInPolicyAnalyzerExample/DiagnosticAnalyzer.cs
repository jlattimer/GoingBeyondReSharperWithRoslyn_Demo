using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CheckInPolicyAnalyzerExample
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CheckInPolicyAnalyzerExampleAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "CheckInPolicyAnalyzerExample";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		internal const string Category = "Demo Check-in Policy";

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxTreeAction(AnalyzeTree);
		}

		private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
		{
			var root = context.Tree.GetRoot();
			var allTrivia = root.DescendantTrivia();
			var commentTriva = allTrivia.Where(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
				x.IsKind(SyntaxKind.MultiLineCommentTrivia)).ToList();

			foreach (var trivia in commentTriva)
			{
				//Only continue if the comment contains "sh!t"
				if (!trivia.ToString().Contains("sh!t")) continue;

				var diagnostic = Diagnostic.Create(Rule, trivia.GetLocation(), trivia.ToString());
				
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
