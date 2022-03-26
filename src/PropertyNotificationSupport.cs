using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PropertyNotificationSupportPackage
{
    public abstract class PropertyNotificationSupport : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;
        
        private readonly Dictionary<string, HashSet<string>> _affectedBy;

        private readonly Dictionary<string, Expression<Func<object>>> _providers;

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyNotificationSupport()
        {
            _affectedBy = new Dictionary<string, HashSet<string>>();
            _providers = new Dictionary<string, Expression<Func<object>>>();
        }
        
        protected virtual void OnPropertyChanged
            ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            foreach (var affected in _affectedBy.Keys)
            {
                if (_affectedBy[affected].Contains(propertyName))
                {
                    var newValue = _providers[affected].Compile().Invoke();
                    GetType()?.GetProperty(affected)?.SetValue(this, newValue);
                }
            }
        }
        
        /// <summary>
        /// Set dependency between properties for auto changing
        /// </summary>
        /// <param name="propertyName">That depends to another properties</param>
        /// <param name="provider">Property how provides by dependencies</param>
        protected void AddProvider(string propertyName, Expression<Func<object>> provider)
        {
            // extract property names from expression
            var visitor = new MemberAccessVisitor(GetType());
            visitor.Visit(provider);

            // add property names to affectedBy list
            if (visitor.PropertyNames.Any())
            {
                if (!_affectedBy.ContainsKey(propertyName))
                {
                    _affectedBy.Add(propertyName, new HashSet<string>());
                }

                foreach (var propName in visitor.PropertyNames)
                    if (propName != propertyName)
                        _affectedBy[propertyName].Add(propName);
            }

            // add function to providers
            _providers.Add(propertyName, provider);
        }
        
        /// <summary>
        /// Bind a property to property of another object (target)
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="sourcePropertyExpr">Get property of source</param>
        /// <param name="destinationPropertyExpr">Get property of destination (target)</param>
        public void BindProperty(INotifyPropertyChanged target,
            Expression<Func<object>> sourcePropertyExpr,
            Expression<Func<object>> destinationPropertyExpr)
        {
            MemberExpression sourceExpr = null;
            MemberExpression destinationExpr = null;
            
            if (sourcePropertyExpr.Body is MemberExpression sourceMemberExpr
                && destinationPropertyExpr.Body is MemberExpression destinationMemberExpr)
            {
                sourceExpr = sourceMemberExpr;
                destinationExpr = destinationMemberExpr;   
            }
            else if (sourcePropertyExpr.Body is UnaryExpression sourceUnaryExpr
                     && destinationPropertyExpr.Body is UnaryExpression destinationUnaryExpr)
            {
                sourceExpr = sourceUnaryExpr.Operand as MemberExpression;
                destinationExpr = destinationUnaryExpr.Operand as MemberExpression;
            }
            
            if (sourceExpr?.Member is PropertyInfo sourceProp
                && destinationExpr?.Member is PropertyInfo destinationProp)
            {
                // source property -> destination property
                this.PropertyChanged += (sender, args) =>
                {
                    if (!_disposed)
                    {
                        destinationProp.SetValue(target, sourceProp.GetValue(this));
                    }
                };
        
                // destination property -> source property
                target.PropertyChanged += (sender, args) =>
                {
                    if (!_disposed)
                    {
                        sourceProp.SetValue(this, destinationProp.GetValue(target));
                    }
                };
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    
                }
            }

            _disposed = true;
        }
    }
}