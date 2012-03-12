using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;

namespace TCCC12.Mapper
{
    public class ExpressionMapper<TA, TB>
        where TA : new()
        where TB : new()
    {

        private readonly List<Action<TA, TB>> _fromAtoBActions =
            new List<Action<TA, TB>>();

        private readonly List<Action<TA, TB>> _fromBtoAActions =
            new List<Action<TA, TB>>();

        public ExpressionMapper<TA, TB> Map<TParameter>(
            Expression<Func<TA, TParameter>> aPropertyExpression,
            Expression<Func<TB, TParameter>> bPropertyExpression)
        {
            var aMemberInfo = GetMemberInfo(aPropertyExpression.Body);
            var bMemberInfo = GetMemberInfo(bPropertyExpression.Body);
            
            CreateFromAtoBAction(aMemberInfo, bMemberInfo);
            CreateFromBtoAAction(aMemberInfo, bMemberInfo);

            return this;
        }

        private MemberInfo GetMemberInfo(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentOutOfRangeException("expression", "expression is not a MemberExpression!");
            }

            return memberExpression.Member;
        }

        private void CreateFromAtoBAction(MemberInfo aMemberInfo, MemberInfo bMemberInfo)
        {
            var a = Expression.Parameter(typeof(TA), "a");
            var b = Expression.Parameter(typeof(TB), "b");
            var body = Expression.Assign(
                left: Expression.MakeMemberAccess(
                    expression: b,
                    member: bMemberInfo),
                right: Expression.MakeMemberAccess(
                    expression: a,
                    member: aMemberInfo));

            var fromAtoBActionExpression = Expression.Lambda<Action<TA, TB>>(body, a, b);
            _fromAtoBActions.Add((fromAtoBActionExpression.Compile()));
        }

        private void CreateFromBtoAAction(MemberInfo aMemberInfo, MemberInfo bMemberInfo)
        {
            var a = Expression.Parameter(typeof(TA), "a");
            var b = Expression.Parameter(typeof(TB), "b");
            var body = Expression.Assign(
                left: Expression.MakeMemberAccess(
                    expression: a,
                    member: aMemberInfo),
                right: Expression.MakeMemberAccess(
                    expression: b,
                    member: bMemberInfo));

            var fromBtoAActionExpression = Expression.Lambda<Action<TA, TB>>(body, a, b);
            _fromBtoAActions.Add((fromBtoAActionExpression.Compile()));
        }

        public TB MapAtoB(TA a)
        {
            TB b = new TB();
            foreach (var fromAtoBAction in _fromAtoBActions)
            {
                fromAtoBAction(a, b);
            }
            return b;
        }

        public TA MapBtoA(TB b)
        {
            TA a = new TA();
            foreach (var fromBtoAAction in _fromBtoAActions)
            {
                fromBtoAAction(a, b);
            }
            return a;
        }
    }
}
