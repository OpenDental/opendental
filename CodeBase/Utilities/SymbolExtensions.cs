using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeBase {

	///<summary>Extensions to find methodinfo from method calls. 
	///Source: http://blog.functionalfun.net/2009/10/getting-methodinfo-of-generic-method.html </summary>
	public static class SymbolExtensions {

		///<summary>Given a lambda expression that calls a method, returns the method info.</summary>
		public static MethodInfo GetMethodInfo(Expression<Action> expression) {
			return GetMethodInfo((LambdaExpression)expression);
		}

		///<summary>Given a lambda expression that calls a method, returns the method info.</summary>
		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression) {
			return GetMethodInfo((LambdaExpression)expression);
		}

		///<summary>Given a lambda expression that calls a method, returns the method info.</summary>
		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression) {
			return GetMethodInfo((LambdaExpression)expression);
		}

		///<summary>Given a lambda expression that calls a method, returns the method info.  Throws an exception if no method found.</summary>
		public static MethodInfo GetMethodInfo(LambdaExpression expression) {
			MethodCallExpression outermostExpression=expression.Body as MethodCallExpression;
			if(outermostExpression==null) {
				throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
			}
			return outermostExpression.Method;
		}

	}

}
