# Roslyn Helpers

A set of helpers to assist Roslyn analyzers.

[![Build status](https://ci.appveyor.com/api/projects/status/2w6qi706m1w23y4g?svg=true)](https://ci.appveyor.com/project/dlebansais/roslynhelpers)
[![codecov](https://codecov.io/gh/dlebansais/RoslynHelpers/graph/badge.svg?token=hdBnfw1WLJ)](https://codecov.io/gh/dlebansais/RoslynHelpers)
[![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/roslynhelpers/badge)](https://www.codefactor.io/repository/github/dlebansais/roslynhelpers)
[![NuGet](https://img.shields.io/nuget/v/dlebansais.RoslynHelpers.svg)](https://www.nuget.org/packages/dlebansais.RoslynHelpers)

## OperatorOverloadHelper

Extension | Method | Purpose
----------|--------|--------
ITypeSymbol | IsOverloadingEqualsOperator | Checks whether a type is overloading the `==` operator
ITypeSymbol | IsOverloadingExclamationEqualsOperator | Checks whether a type is overloading the `!=` operator

## TypeHelper

Extension | Method | Purpose
----------|--------|--------
ExpressionSyntax | GetExpressionValidType | Returns the type of an expression syntax as a valid type, or null.
TypeSyntax | GetTypeValidType | Returns the type of a type syntax as a valid type, or null.
