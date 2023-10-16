# Roslyn Helpers

A set of helpers to assist Roslyn analyzers.

## OperatorOverloadHelper

Extension | Method | Purpose
----------|--------|--------
ExpressionSyntax | IsEqualsOperatorOverloadedInType | Checks whether the type of an expression is overloading the `==` operator
ITypeSymbol | IsOverloadingEqualsOperator | Checks whether a type is overloading the `==` operator
ExpressionSyntax | IsExclamationEqualsOperatorOverloadedInType | Checks whether the type of an expression is overloading the `!=` operator
ITypeSymbol | IsOverloadingExclamationEqualsOperator | Checks whether a type is overloading the `==` operator
