using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlockAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class BlockAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "BlockAnalyzer";
		internal static readonly string Title = "Roslyn Block Analyzer";
		internal static readonly string MessageFormat = "No 'God' Methods Allowed - Refactor '{0}' Method To 10 Statements Or Less";
		internal static readonly string Description = "";
		internal const string Category = "Roslyn Demo";

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterCodeBlockEndAction(AnalyzeBlock);
		}

		private static void AnalyzeBlock(CodeBlockEndAnalysisContext context)
		{
			SyntaxNode node = context.CodeBlock;

			//If the node isn't a Method Declaration - exit
			if (node.Kind() != SyntaxKind.MethodDeclaration) return;

			var block = node.ChildNodes().OfType<BlockSyntax>().FirstOrDefault();
			if (block == null) return;

			//If there are less than 10 statements - exit
			if (block.Statements.Count <= 10) return;

			//Get the identifier name
			var methodName = node.ChildTokens().FirstOrDefault(k => k.Kind() == SyntaxKind.IdentifierToken).Text;

			var diagnostic = Diagnostic.Create(Rule, block.GetLocation(), methodName);
			context.ReportDiagnostic(diagnostic);
		}
	}
}
