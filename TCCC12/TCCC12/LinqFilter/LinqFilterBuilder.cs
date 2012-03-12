using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace TCCC12.LinqFilter
{
    public static class LinqFilterBuilder
    {
        public static Expression<Func<T, bool>> CreateTrueLambda<T>()
        {
            return x => true;
        }
        
        public static Expression<Func<T, bool>> CreateFalseLambda<T>()
        {
            return x => false;
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> baseExpression,
            Expression<Func<T, bool>> newExpression)
        {
            return Expression.Lambda<Func<T, bool>>(
                body: Expression.AndAlso(
                    left: baseExpression.Body,
                    right: Expression.Invoke(
                        expression: newExpression,
                        arguments: baseExpression.Parameters)),
                parameters: baseExpression.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> baseExpression,
            Expression<Func<T, bool>> newExpression)
        {
            return Expression.Lambda<Func<T, bool>>(
                body: Expression.OrElse(
                    left: baseExpression.Body,
                    right: Expression.Invoke(
                        expression: newExpression,
                        arguments: baseExpression.Parameters)),
                parameters: baseExpression.Parameters);
        }
    }
}
