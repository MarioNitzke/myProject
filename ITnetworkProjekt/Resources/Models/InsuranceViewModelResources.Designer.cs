﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ITnetworkProjekt.Resources.Models {
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
    public partial class InsuranceViewModelResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal InsuranceViewModelResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ITnetworkProjekt.Resources.Models.InsuranceViewModelResources", typeof(InsuranceViewModelResources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Datum vytvoření:.
        /// </summary>
        public static string CreatedDateLabel {
            get {
                return ResourceManager.GetString("CreatedDateLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Konec do:.
        /// </summary>
        public static string EndDateLabel {
            get {
                return ResourceManager.GetString("EndDateLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vyplňte datum konec do.
        /// </summary>
        public static string EndDateRequired {
            get {
                return ResourceManager.GetString("EndDateRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pojistitel:.
        /// </summary>
        public static string InsuredPersonIdLabel {
            get {
                return ResourceManager.GetString("InsuredPersonIdLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vyberte pojistitele.
        /// </summary>
        public static string InsuredPersonIdRequired {
            get {
                return ResourceManager.GetString("InsuredPersonIdRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Typ pojištění:.
        /// </summary>
        public static string PolicyTypeLabel {
            get {
                return ResourceManager.GetString("PolicyTypeLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vyplňte typ pojištění.
        /// </summary>
        public static string PolicyTypeRequired {
            get {
                return ResourceManager.GetString("PolicyTypeRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pojistná částka:.
        /// </summary>
        public static string PremiumAmountLabel {
            get {
                return ResourceManager.GetString("PremiumAmountLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zadejte hodnotu mezi 0 a 999999.99..
        /// </summary>
        public static string PremiumAmountRange {
            get {
                return ResourceManager.GetString("PremiumAmountRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vyplňte částku pojištění.
        /// </summary>
        public static string PremiumAmountRequired {
            get {
                return ResourceManager.GetString("PremiumAmountRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Začátek od:.
        /// </summary>
        public static string StartDateLabel {
            get {
                return ResourceManager.GetString("StartDateLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vyplňte datum začátek od:.
        /// </summary>
        public static string StartDateRequired {
            get {
                return ResourceManager.GetString("StartDateRequired", resourceCulture);
            }
        }
    }
}
