#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Camunda.Worker
{
    internal static class Guard
    {
        [ExcludeFromCodeCoverage]
        internal static T NotNull<T>(T parameterValue, string parameterName) where T : class
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return parameterValue;
        }

        [ExcludeFromCodeCoverage]
        internal static int GreaterThanOrEqual(int value, int minValue, string parameterName)
        {
            if (value < minValue)
            {
                throw new ArgumentException($"'{parameterName}' must be greater than or equal to {minValue}");
            }

            return value;
        }
    }
}
