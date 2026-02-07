namespace Transversales.Shared.Extensions
{
    using System;
    using System.Linq.Expressions;

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var left = Expression.Invoke(expr1, parameter);
            var right = Expression.Invoke(expr2, parameter);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right),
                parameter
            );
        }
    }
}
