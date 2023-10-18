# Roslyn Helpers

A set of helpers to assist Roslyn analyzers.

## OperatorOverloadHelper

Extension | Method | Purpose
----------|--------|--------
ITypeSymbol | IsOverloadingEqualsOperator | Checks whether a type is overloading the `==` operator
ITypeSymbol | IsOverloadingExclamationEqualsOperator | Checks whether a type is overloading the `!=` operator

## TypeHelper

Extension | Method | Purpose
----------|--------|--------
ExpressionSyntax | GetExpressionValidType | Returns the type of an expression as a valid type, or null.
