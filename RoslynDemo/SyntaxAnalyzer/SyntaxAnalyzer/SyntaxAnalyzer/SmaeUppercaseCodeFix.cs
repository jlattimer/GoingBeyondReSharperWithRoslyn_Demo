﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SyntaxAnalyzer
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SmaeUppercaseCodeFix))]
	public class SmaeUppercaseCodeFix : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SmaeUppercaseAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			// Find the type declaration identified by the diagnostic.
			var declaration = root.FindToken(diagnosticSpan.Start).Parent;

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create("Make lowercase", c => MakeLowercaseAsync(context.Document, declaration, c)),
				diagnostic);
		}

		private async Task<Document> MakeLowercaseAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
		{
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			LiteralExpressionSyntax literal = (LiteralExpressionSyntax)node;

			var newFieldName = literal.Token.Value.ToString().ToLowerInvariant();

			var newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(newFieldName));

			var newRoot = root.ReplaceNode(node, newNode);

			return document.WithSyntaxRoot(newRoot);
		}
	}
}