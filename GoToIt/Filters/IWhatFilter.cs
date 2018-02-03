using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Filters
{
    interface IWhatFilter
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IWhatFilter"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        bool Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether this filter accepted the specified Symbol.
        /// </summary>
        /// <param name="type">The Type Symbol.</param>
        /// <param name="namedType">The NamedType Symbol.</param>
        /// <returns></returns>
        bool Accepts(ITypeSymbol type, INamedTypeSymbol namedType);
    }

    class ImplementsInterfaceFilter : IWhatFilter
    {
        private readonly string _fullName;
        public bool Enabled { get; }

        public ImplementsInterfaceFilter(string fullName) {
            if (!string.IsNullOrWhiteSpace(fullName)) {
                _fullName = fullName;
                Enabled = true;
            }
        }

        public bool Accepts(ITypeSymbol type, INamedTypeSymbol namedType)
            => Enabled
            ? type.AllInterfaces.Any(i => i.Name == _fullName)
            : true;
    }

    class NameFilter : IWhatFilter
    {
        private readonly string _shortName;
        public bool Enabled { get; }

        public NameFilter(string shortName) {
            if (!string.IsNullOrWhiteSpace(shortName)) {
                _shortName = shortName;
                Enabled = true;
            }
        }

        public bool Accepts(ITypeSymbol type, INamedTypeSymbol namedType)
            => Enabled
            ? type.Name == _shortName
            : true;
    }

    class GenericFilter : IWhatFilter
    {
        private readonly bool _isGeneric;
        public bool Enabled { get; }

        public GenericFilter(bool? isGeneric) {
            if (isGeneric.HasValue) {
                _isGeneric = isGeneric.Value;
                Enabled = true;
            }
        }

        public bool Accepts(ITypeSymbol type, INamedTypeSymbol namedType)
            => Enabled
            ? namedType.IsGenericType == _isGeneric
            : true;
    }

    internal enum SymbolTypes
    {
        Class,
        Struct,
        Interface,
        Delegate
    }

    class TypeFilter : IWhatFilter
    {
        private static readonly TypeFilter NONE = new TypeFilter(null);

        private readonly SymbolTypes _type;
        public bool Enabled { get; }

        public TypeFilter(SymbolTypes? type) {
            if (type.HasValue) {
                Enabled = true;
                _type = type.Value;
            }
        }

        public bool Accepts(ITypeSymbol type, INamedTypeSymbol namedType) {
            if (!Enabled)
                return true;

            switch (_type) {
                case SymbolTypes.Class: return type.TypeKind == TypeKind.Class;
                case SymbolTypes.Struct: return type.TypeKind == TypeKind.Struct;
                case SymbolTypes.Interface: return type.TypeKind == TypeKind.Interface;
                case SymbolTypes.Delegate: return type.TypeKind == TypeKind.Delegate;
                default: return false;
            }
        }

        internal static TypeFilter From(ITypeSymbol type) {
            if (type == null) {
                return NONE;
            }

            switch (type.TypeKind) {
                case TypeKind.Class: return new TypeFilter(SymbolTypes.Class);
                case TypeKind.Delegate: return new TypeFilter(SymbolTypes.Delegate);
                case TypeKind.Struct: return new TypeFilter(SymbolTypes.Struct);
                case TypeKind.Interface: return new TypeFilter(SymbolTypes.Interface);
                case TypeKind.Unknown:
                case TypeKind.Array:
                case TypeKind.Dynamic:
                case TypeKind.Enum:
                case TypeKind.Error:
                case TypeKind.Module:
                case TypeKind.Pointer:
                case TypeKind.TypeParameter:
                case TypeKind.Submission:
                default: return NONE;
            }
        }
    }

    class AccessibilityFilter : IWhatFilter {
        private readonly Accessibility _accessibility;

        public bool Enabled { get; }

        public AccessibilityFilter(Accessibility? accessibility) {
            if (accessibility.HasValue) {
                Enabled = true;
                _accessibility = accessibility.Value;
            }
        }
    
        public bool Accepts(ITypeSymbol type, INamedTypeSymbol namedType)
            => Enabled
            ? type.DeclaredAccessibility == _accessibility
            : true;
    }
  
}
