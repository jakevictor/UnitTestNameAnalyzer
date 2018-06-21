# Unit Test Name Analyzer

A Roslyn analyzer that validates C# unit test names against standards used by my team. This is largely an excuse for me to learn more about Roslyn.

Rules include:
* Test class names should be the name of the system under test followed by "Tests"
* Test method names should be two or three parts separated by an underscore
* The first part of a test method name should be the name of the method being called on the system under test (e.g. "Initialize_DoesCoolThing")
* If a test method name has three parts, the second part should begin with "When" (e.g. "Initialize_WhenFooIsNull_DoesOtherCoolThing")

[![Build status](https://ci.appveyor.com/api/projects/status/4q85r35yyghotsut?svg=true)](https://ci.appveyor.com/project/jakevictor/unittestnameanalyzer)

[![codecov](https://codecov.io/gh/jakevictor/UnitTestNameAnalyzer/branch/master/graph/badge.svg)](https://codecov.io/gh/jakevictor/UnitTestNameAnalyzer)