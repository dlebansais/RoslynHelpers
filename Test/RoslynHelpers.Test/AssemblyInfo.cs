using Microsoft.VisualStudio.TestTools.UnitTesting;

#if DO_NOT_PARALLELIZE_TEST_FOR_STRYKER
[assembly: DoNotParallelize]
#else
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
#endif
