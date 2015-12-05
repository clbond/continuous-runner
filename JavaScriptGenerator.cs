using System;
using System.Linq;
using System.Text;
using Jint.Parser.Ast;

namespace ContinuousRunner
{
    public class JavaScriptGenerator
    {
        #region Public methods

        public static string Generate(SyntaxNode syntaxNode)
        {
            var sb = new StringBuilder();

            if (syntaxNode == null)
            {
                return sb.ToString();
            }

            switch (syntaxNode.Type)
            {
                case SyntaxNodes.AssignmentExpression:
                    var assignment = syntaxNode.As<AssignmentExpression>();
                    sb.AppendFormat("{0} {1} {2}",
                        Generate(assignment.Left),
                        GetOperator(assignment.Operator),
                        Generate(assignment.Right));
                    break;
                case SyntaxNodes.ArrayExpression:
                    var serializedElements = syntaxNode.As<ArrayExpression>().Elements.Select(Generate).ToArray();
                    sb.AppendFormat("[{0}]", string.Join(", ", serializedElements));
                    break;
                case SyntaxNodes.BlockStatement:
                    var statement = syntaxNode.As<BlockStatement>();
                    sb.Append(string.Join(string.Empty, statement.Body.Select(Generate).ToArray()));
                    break;
                case SyntaxNodes.BinaryExpression:
                    var binaryExpr = syntaxNode.As<BinaryExpression>();
                    sb.AppendFormat("{0} {1} {2}",
                        Generate(binaryExpr.Left),
                        GetOperator(binaryExpr.Operator),
                        Generate(binaryExpr.Right));
                    break;
                case SyntaxNodes.BreakStatement:
                    sb.AppendLine("break;");
                    break;
                case SyntaxNodes.CallExpression:
                    var callExpr = syntaxNode.As<CallExpression>();
                    sb.Append(Generate(callExpr.Callee));

                    var arguments = callExpr.Arguments.Select(Generate).ToArray();
                    sb.Append(string.Format("({0})", string.Join(", ", arguments)));
                    break;
                case SyntaxNodes.CatchClause:
                    var catchClause = syntaxNode.As<CatchClause>();
                    sb.Append("catch ");
                    if (catchClause.Param != null)
                    {
                        sb.AppendFormat("({0}) ", Generate(catchClause.Param));
                    }
                    sb.AppendLine("{");
                    sb.Append(Generate(catchClause.Body));
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.ConditionalExpression:
                    var conditionalExpr = syntaxNode.As<ConditionalExpression>();
                    sb.AppendFormat("{0} ? {1} : {2}",
                        Generate(conditionalExpr.Test),
                        Generate(conditionalExpr.Consequent),
                        Generate(conditionalExpr.Alternate));
                    break;
                case SyntaxNodes.ContinueStatement:
                    sb.AppendLine("continue;");
                    break;
                case SyntaxNodes.DoWhileStatement:
                    var doWhile = syntaxNode.As<DoWhileStatement>();
                    sb.AppendLine("do {");
                    sb.Append(Generate(doWhile.Body));
                    sb.AppendLine(string.Format("}} while ({0});", Generate(doWhile.Test)));
                    break;
                case SyntaxNodes.DebuggerStatement:
                    sb.AppendLine("debugger;");
                    break;
                case SyntaxNodes.EmptyStatement:
                    break;
                case SyntaxNodes.ExpressionStatement:
                    var exprStatement = syntaxNode.As<ExpressionStatement>();
                    sb.AppendLine(string.Format("{0};", Generate(exprStatement.Expression)));
                    break;
                case SyntaxNodes.ForStatement:
                    var forStatement = syntaxNode.As<ForStatement>();
                    sb.AppendLine(string.Format("for ({0}; {1}; {2}) {{",
                        Generate(forStatement.Init),
                        Generate(forStatement.Test),
                        Generate(forStatement.Update)));
                    sb.Append(Generate(forStatement.Body));
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.ForInStatement:
                    var forInStatement = syntaxNode.As<ForInStatement>();
                    sb.AppendLine(string.Format("for {0}({1} in {2}) {{",
                        forInStatement.Each ? "each " : string.Empty,
                        Generate(forInStatement.Left),
                        Generate(forInStatement.Right)));
                    sb.Append(Generate(forInStatement.Body));
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.FunctionDeclaration:
                    var functionDeclaration = syntaxNode.As<FunctionDeclaration>();
                    sb.Append("function");
                    if (functionDeclaration.Id != null)
                    {
                        sb.Append(" ");
                        sb.Append(Generate(functionDeclaration.Id));
                    }
                    sb.Append("(");
                    var parameters = functionDeclaration.Parameters.Select(Generate).ToArray();
                    sb.AppendFormat(string.Join(", ", parameters));
                    sb.AppendLine(") {");
                    //var functionDecls = functionDeclaration.FunctionDeclarations.Select(Generate).ToArray();
                    //sb.AppendFormat(string.Join("\n\n", functionDecls));
                    //var varDecls = functionDeclaration.VariableDeclarations.Select(Generate).ToArray();
                    //sb.AppendFormat(string.Join("\n", varDecls));
                    sb.Append(Generate(functionDeclaration.Body));
                    sb.Append(Generate(functionDeclaration.Rest));
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.FunctionExpression:
                    var functionExpression = syntaxNode.As<FunctionExpression>();
                    sb.Append("function");
                    if (functionExpression.Id != null)
                    {
                        sb.Append(" ");
                        sb.Append(Generate(functionExpression.Id));
                    }
                    sb.Append("(");
                    var fparameters = functionExpression.Parameters.Select(Generate).ToArray();
                    sb.AppendFormat(string.Join(", ", fparameters));
                    sb.AppendLine(") {");
                    //var ffunctionDecls = functionExpression.FunctionDeclarations.Select(Generate).ToArray();
                    //sb.AppendFormat(string.Join("\n\n", ffunctionDecls));
                    //var fvarDecls = functionExpression.VariableDeclarations.Select(Generate).ToArray();
                    //sb.AppendFormat(string.Join("\n", fvarDecls));
                    sb.Append(Generate(functionExpression.Body));
                    sb.Append(Generate(functionExpression.Rest));
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.Identifier:
                    var identifier = syntaxNode.As<Identifier>();
                    if (identifier.Name == null)
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        sb.AppendFormat(identifier.Name);
                    }
                    break;
                case SyntaxNodes.IfStatement:
                    var ifStatement = syntaxNode.As<IfStatement>();
                    sb.AppendLine(string.Format("if ({0}) {{", Generate(ifStatement.Test)));
                    sb.Append(Generate(ifStatement.Consequent));
                    if (ifStatement.Alternate != null)
                    {
                        sb.AppendLine("} else {");
                        sb.Append(Generate(ifStatement.Alternate));
                    }
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.Literal:
                    var literal = syntaxNode.As<Literal>();
                    sb.Append(literal.Raw);
                    break;
                case SyntaxNodes.RegularExpressionLiteral:
                    var regexp = syntaxNode.As<Literal>();
                    sb.AppendFormat("/{0}/", regexp.Value);
                    break;
                case SyntaxNodes.LabeledStatement:
                    var labelled = syntaxNode.As<LabelledStatement>();
                    sb.AppendLine(string.Format("{0}:", Generate(labelled.Label)));
                    sb.Append(Generate(labelled.Body));
                    break;
                case SyntaxNodes.LogicalExpression:
                    var logical = syntaxNode.As<LogicalExpression>();
                    sb.AppendFormat("{0} {1} {2}",
                        Generate(logical.Left),
                        logical.Operator == LogicalOperator.LogicalOr ? "||" : "&&",
                        Generate(logical.Right));
                    break;
                case SyntaxNodes.MemberExpression:
                    var memberExpr = syntaxNode.As<MemberExpression>();
                    sb.AppendFormat(memberExpr.Computed ? "{0}[{1}]" : "{0}.{1}",
                        Generate(memberExpr.Object),
                        Generate(memberExpr.Property));
                    break;
                case SyntaxNodes.NewExpression:
                    var newExpr = syntaxNode.As<NewExpression>();
                    sb.AppendFormat("new {0}({1})",
                        Generate(newExpr.Callee),
                        string.Join(", ", newExpr.Arguments.Select(Generate).ToArray()));
                    break;
                case SyntaxNodes.ObjectExpression:
                    var objExpr = syntaxNode.As<ObjectExpression>();
                    sb.Append("{");
                    sb.Append(string.Join(", ", objExpr.Properties.Select(Generate).ToArray()));
                    sb.Append("}");
                    break;
                case SyntaxNodes.Program:
                    var program = syntaxNode.As<Program>();
                    foreach (var decl in program.FunctionDeclarations)
                    {
                        sb.AppendLine(Generate(decl));
                    }
                    foreach (var variable in program.VariableDeclarations)
                    {
                        sb.AppendLine(Generate(variable));
                    }
                    foreach (var stmt in program.Body)
                    {
                        sb.AppendLine(Generate(stmt));
                    }
                    break;
                case SyntaxNodes.Property:
                    var property = syntaxNode.As<Property>();
                    sb.Append(string.Format("{0}: {1}", GetProperty(property.Key.GetKey()), Generate(property.Value)));
                    break;
                case SyntaxNodes.ReturnStatement:
                    var returnStatement = syntaxNode.As<ReturnStatement>();
                    if (returnStatement.Argument != null)
                    {
                        sb.AppendLine(string.Format("return {0};", Generate(returnStatement.Argument)));
                    }
                    else
                    {
                        sb.AppendLine("return;");
                    }
                    break;
                case SyntaxNodes.SequenceExpression:
                    var sequence = syntaxNode.As<SequenceExpression>();
                    var exprs = sequence.Expressions.Select(Generate).ToArray();
                    sb.Append(string.Join(", ", exprs));
                    break;
                case SyntaxNodes.SwitchStatement:
                    var switchStatement = syntaxNode.As<SwitchStatement>();
                    sb.AppendLine(string.Format("switch ({0}) {{", Generate(switchStatement.Discriminant)));
                    foreach (var sc in switchStatement.Cases)
                    {
                        sb.Append(Generate(sc));
                    }
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.SwitchCase:
                    var caseStatement = syntaxNode.As<SwitchCase>();
                    if (caseStatement.Test != null)
                    {
                        sb.AppendLine(string.Format("case {0}:", Generate(caseStatement.Test)));
                    }
                    else
                    {
                        sb.AppendLine("default:");
                    }
                    foreach (var s in caseStatement.Consequent)
                    {
                        sb.Append(Generate(s));
                    }
                    break;
                case SyntaxNodes.ThisExpression:
                    sb.Append("this");
                    break;
                case SyntaxNodes.ThrowStatement:
                    var throwStatement = syntaxNode.As<ThrowStatement>();
                    if (throwStatement.Argument != null)
                    {
                        sb.AppendLine(string.Format("throw {0};", Generate(throwStatement.Argument)));
                    }
                    else
                    {
                        sb.AppendLine("throw;");
                    }
                    break;
                case SyntaxNodes.TryStatement:
                    var tryStatement = syntaxNode.As<TryStatement>();
                    sb.AppendLine("try {");
                    sb.AppendLine(Generate(tryStatement.Block));
                    sb.AppendLine("}");
                    sb.Append(string.Join(Environment.NewLine, tryStatement.Handlers.Select(Generate).ToArray()));
                    sb.AppendLine(Generate(tryStatement.Finalizer));
                    break;
                case SyntaxNodes.UnaryExpression:
                    var unaryExpression = syntaxNode.As<UnaryExpression>();
                    sb.Append(GetOperator(unaryExpression.Operator));
                    sb.Append(" ");
                    sb.Append(Generate(unaryExpression.Argument));
                    break;
                case SyntaxNodes.UpdateExpression:
                    break;
                case SyntaxNodes.VariableDeclaration:
                    var variableDecl = syntaxNode.As<VariableDeclaration>();
                    sb.Append(variableDecl.Kind);
                    sb.Append(" ");
                    sb.Append(string.Join(", ", variableDecl.Declarations.Select(Generate).ToArray()));
                    sb.AppendLine(";");
                    break;
                case SyntaxNodes.VariableDeclarator:
                    var declarator = syntaxNode.As<VariableDeclarator>();
                    sb.Append(Generate(declarator.Id));

                    if (declarator.Init != null)
                    {
                        sb.Append(" = ");
                        sb.Append(Generate(declarator.Init));
                    }
                    break;
                case SyntaxNodes.WhileStatement:
                    var whileStatement = syntaxNode.As<WhileStatement>();
                    sb.AppendLine(string.Format("while ({0}) {{", Generate(whileStatement.Test)));
                    sb.Append(Generate(whileStatement.Body));
                    sb.AppendLine("}");
                    break;
                case SyntaxNodes.WithStatement:
                    var withStatement = syntaxNode.As<WithStatement>();
                    sb.AppendLine(string.Format("with ({0}) {{", withStatement.Object));
                    sb.Append(Generate(withStatement.Body));
                    sb.AppendLine("}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(syntaxNode.Type.ToString());
            }

            return sb.ToString();
        }

        private static string GetOperator(UnaryOperator @operator)
        {
            switch (@operator)
            {
                case UnaryOperator.Plus:
                    return "+";
                case UnaryOperator.Minus:
                    return "-";
                case UnaryOperator.BitwiseNot:
                    return "~";
                case UnaryOperator.LogicalNot:
                    return "!";
                case UnaryOperator.Delete:
                    return "delete";
                case UnaryOperator.Void:
                    return "void";
                case UnaryOperator.TypeOf:
                    return "typeof";
                case UnaryOperator.Increment:
                    return "++";
                case UnaryOperator.Decrement:
                    return "--";
                default:
                    throw new ArgumentOutOfRangeException("operator");
            }
        }

#endregion

        #region Private methods

        private static string GetOperator(AssignmentOperator @operator)
        {
            switch (@operator)
            {
                case AssignmentOperator.Assign:
                    return "=";
                case AssignmentOperator.PlusAssign:
                    return "+=";
                case AssignmentOperator.MinusAssign:
                    return "-=";
                case AssignmentOperator.TimesAssign:
                    return "*=";
                case AssignmentOperator.DivideAssign:
                    return "/=";
                case AssignmentOperator.ModuloAssign:
                    return "%=";
                case AssignmentOperator.BitwiseAndAssign:
                    return "&=";
                case AssignmentOperator.BitwiseOrAssign:
                    return "|=";
                case AssignmentOperator.BitwiseXOrAssign:
                    return "^=";
                case AssignmentOperator.LeftShiftAssign:
                    return "<<=";
                case AssignmentOperator.RightShiftAssign:
                    return ">>=";
                case AssignmentOperator.UnsignedRightShiftAssign:
                    return "<<<=";
                default:
                    throw new ArgumentOutOfRangeException("operator");
            }
        }

        private static string GetOperator(BinaryOperator @operator)
        {
            switch (@operator)
            {
                case BinaryOperator.Plus:
                    return "+";
                case BinaryOperator.Minus:
                    return "-";
                case BinaryOperator.Times:
                    return "*";
                case BinaryOperator.Divide:
                    return "/";
                case BinaryOperator.Modulo:
                    return "%";
                case BinaryOperator.Equal:
                    return "==";
                case BinaryOperator.NotEqual:
                    return "!=";
                case BinaryOperator.Greater:
                    return ">";
                case BinaryOperator.GreaterOrEqual:
                    return ">=";
                case BinaryOperator.Less:
                    return "<";
                case BinaryOperator.LessOrEqual:
                    return "<=";
                case BinaryOperator.StrictlyEqual:
                    return "===";
                case BinaryOperator.StricltyNotEqual:
                    return "!==";
                case BinaryOperator.BitwiseAnd:
                    return "&";
                case BinaryOperator.BitwiseOr:
                    return "|";
                case BinaryOperator.BitwiseXOr:
                    return "^";
                case BinaryOperator.LeftShift:
                    return "<<";
                case BinaryOperator.RightShift:
                    return ">>";
                case BinaryOperator.UnsignedRightShift:
                    return ">>>";
                case BinaryOperator.InstanceOf:
                    return "instanceof";
                case BinaryOperator.In:
                    return "in";
                default:
                    throw new ArgumentOutOfRangeException("operator");
            }
        }
        private static string GetProperty(string property)
        {
            var validChars = true;

            for (var i = 0; i < property.Length; i++)
            {
                var c = property[i];

                if (char.IsLetter(c) || c == '_')
                {
                    continue;
                }

                if (char.IsDigit(c) && i > 0)
                {
                    continue;
                }

                validChars = false;
            }

            if (validChars)
            {
                return property;
            }
            else
            {
                return string.Format("\"{0}\"", property);
            }
        }

        #endregion
    }
}
