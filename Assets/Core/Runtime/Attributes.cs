using System;
using UnityEngine;

namespace NS.Core {
    /// <summary>
    /// Marks a field as read-only in the Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}