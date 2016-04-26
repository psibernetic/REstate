﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace REstate.Web.Core {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Schemas {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Schemas() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("REstate.Web.Core.Schemas", typeof(Schemas).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///  &quot;id&quot;: &quot;{host}/machine&quot;,
        ///  &quot;type&quot;: &quot;object&quot;,
        ///  &quot;name&quot;: &quot;Machine&quot;,
        ///  &quot;title&quot;: &quot;Machine&quot;,
        ///  &quot;required&quot;: [
        ///    &quot;machineName&quot;,
        ///    &quot;triggers&quot;
        ///  ],
        ///  &quot;properties&quot;: {
        ///    &quot;machineName&quot;: {
        ///      &quot;id&quot;: &quot;{host}/machine/name&quot;,
        ///      &quot;type&quot;: &quot;string&quot;,
        ///      &quot;name&quot;: &quot;Name&quot;,
        ///      &quot;title&quot;: &quot;Name&quot;
        ///    },
        ///    &quot;stateConfigurations&quot;: {
        ///      &quot;id&quot;: &quot;{host}/machine/state-configurations&quot;,
        ///      &quot;type&quot;: &quot;array&quot;,
        ///      &quot;title&quot;: &quot;State Configurations&quot;, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Machine {
            get {
                return ResourceManager.GetString("Machine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///  &quot;id&quot;: &quot;{host}/state&quot;,
        ///  &quot;type&quot;: &quot;object&quot;,
        ///  &quot;properties&quot;: {
        ///    &quot;stateName&quot;: {
        ///      &quot;id&quot;: &quot;{host}/state/name&quot;,
        ///      &quot;type&quot;: &quot;string&quot;,
        ///      &quot;name&quot;: &quot;Name&quot;,
        ///      &quot;title&quot;: &quot;Name&quot;
        ///    },
        ///    &quot;stateDescription&quot;: {
        ///      &quot;id&quot;: &quot;{host}/state/description&quot;,
        ///      &quot;type&quot;: &quot;string&quot;,
        ///      &quot;name&quot;: &quot;Description&quot;,
        ///      &quot;title&quot;: &quot;Description&quot;
        ///    },
        ///    &quot;parentStateName&quot;: {
        ///      &quot;id&quot;: &quot;{host}/state/parent-state-name&quot;,
        ///      &quot;type&quot;: &quot;strin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string State {
            get {
                return ResourceManager.GetString("State", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///  &quot;id&quot;: &quot;{host}/trigger&quot;,
        ///  &quot;type&quot;: &quot;object&quot;,
        ///  &quot;additionalProperties&quot;: false,
        ///  &quot;name&quot;: &quot;Trigger&quot;,
        ///  &quot;title&quot;: &quot;Trigger&quot;,
        ///  &quot;properties&quot;: {
        ///    &quot;triggerName&quot;: {
        ///      &quot;id&quot;: &quot;{host}/trigger/name&quot;,
        ///      &quot;type&quot;: &quot;string&quot;,
        ///      &quot;default&quot;: null,
        ///      &quot;name&quot;: &quot;Name&quot;,
        ///      &quot;title&quot;: &quot;Name&quot;
        ///    },
        ///    &quot;triggerDescription&quot;: {
        ///      &quot;id&quot;: &quot;{host}/trigger/description&quot;,
        ///      &quot;type&quot;: &quot;string&quot;,
        ///      &quot;default&quot;: null,
        ///      &quot;name&quot;: &quot;Descripti [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Trigger {
            get {
                return ResourceManager.GetString("Trigger", resourceCulture);
            }
        }
    }
}
