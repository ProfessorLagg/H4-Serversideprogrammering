﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("API.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to 
        ///&lt;H3&gt;Please Log In&lt;/H3&gt;
        ///&lt;form&gt;
        ///    &lt;label for=&quot;username&quot;&gt;Username: &lt;/label&gt;
        ///    &lt;input name=&quot;username&quot; type=&quot;text&quot; /&gt;
        ///
        ///    &lt;label for=&quot;password&quot;&gt;Password: &lt;/label&gt;
        ///    &lt;input name=&quot;password&quot; type=&quot;password&quot; /&gt;
        ///&lt;/form&gt;
        ///.
        /// </summary>
        internal static string PageHome {
            get {
                return ResourceManager.GetString("PageHome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html lang=&quot;en&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;UTF-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1.0&quot;&gt;
        ///    &lt;title&gt;Login&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;H3&gt;Please Log In&lt;/H3&gt;
        ///&lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string PageLogin {
            get {
                return ResourceManager.GetString("PageLogin", resourceCulture);
            }
        }
    }
}
