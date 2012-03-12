using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace TCCC12.Javascript
{
    public class JavascriptFunction : ExpressionVisitor        
    {
        public static string CreateFunction(string functionName, LambdaExpression expression)
        {
            var javascriptFunction = new JavascriptFunction(functionName, expression);
            return javascriptFunction._javascript.ToString();
        }

        private StringBuilder _javascript = new StringBuilder();
        
        private JavascriptFunction(string functionName, LambdaExpression expression)
        {            
            StartFunction(functionName, expression.Parameters);

            if (expression.ReturnType.Name != "Void")
            {
                _javascript.Append("return ");
            }

            var simplifiedBody = Evaluator.PartialEval(expression.Body);

            Visit(simplifiedBody);

            EndFunction();
        }

        private void StartFunction(string functionName, IEnumerable<ParameterExpression> parameters)
        {
            _javascript = new StringBuilder("function ");
            _javascript.Append(functionName);
            _javascript.Append("(");
            
            var parameterNames = parameters.Select(x => x.Name);
            _javascript.Append(string.Join(", ", parameterNames));

            _javascript.Append("){ ");
        }

        private void EndFunction()
        {
            _javascript.Append(" }");
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object == null)
            {
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
            }

            if (m.Object.Type == typeof(string) && m.Method.Name == "Substring")
            {
                CreateMethodCall(m.Object, "substring", m.Arguments);
            }
            else
            {
                CreateMethodCall(m.Object, m.Method.Name, m.Arguments);
            }

            return m;
        }

        private void CreateMethodCall(Expression obj, string methodName, ReadOnlyCollection<Expression> args)
        {
            this.Visit(obj);
            _javascript.Append(".");
            _javascript.Append(methodName);
            _javascript.Append("(");
            for (int i = 0; i < args.Count; i++)
            {
                if (i > 0)
                {
                    _javascript.Append(", ");
                }
                this.Visit(args[i]);
            }
            _javascript.Append(")");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _javascript.Append(" !");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _javascript.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    _javascript.Append(" & ");
                    break;
                case ExpressionType.Or:
                    _javascript.Append(" | ");
                    break;
                case ExpressionType.Equal:
                    _javascript.Append(" == ");
                    break;
                case ExpressionType.NotEqual:
                    _javascript.Append(" != ");
                    break;
                case ExpressionType.LessThan:
                    _javascript.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _javascript.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    _javascript.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _javascript.Append(" >= ");
                    break;
                case ExpressionType.AndAlso:
                    _javascript.Append(" && ");
                    break;
                case ExpressionType.OrElse:
                    _javascript.Append(" || ");
                    break;
                case ExpressionType.Add:
                    _javascript.Append(" + ");
                    break;
                case ExpressionType.Subtract:
                    _javascript.Append(" - ");
                    break;
                case ExpressionType.Multiply:
                    _javascript.Append(" * ");
                    break;
                case ExpressionType.Divide:
                    _javascript.Append(" / ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            _javascript.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                _javascript.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _javascript.Append(((bool)c.Value) ? "true" : "false");
                        break;
                    case TypeCode.String:
                        _javascript.Append("\"");
                        _javascript.Append(c.Value);
                        _javascript.Append("\"");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                    default:
                        _javascript.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null)
            {
                this.Visit(m.Expression);
                _javascript.Append(".");
                _javascript.Append(m.Member.Name);                
                return m;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            _javascript.Append(p.Name);
            return p;
        }
    }
}
