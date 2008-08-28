using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using System.Collections;

namespace ContinuousLinq
{
    internal class SubscriptionNode : IWeakEventListener
    {
        private INotifyPropertyChanged _subject;

        internal PropertyAccessTreeNode AccessNode { get; set; }

        internal PropertyAccessNode PropertyAccessNode
        {
            get { return (PropertyAccessNode)this.AccessNode; }
        }

        internal List<SubscriptionNode> Children { get; set; }

        public INotifyPropertyChanged Subject
        {
            get { return _subject; }
            set
            {
                Unsubscribe();

                _subject = value;

                Subscribe();
            }
        }
        
        public event Action PropertyChanged;

        private void Subscribe()
        {
            INotifyPropertyChanged subject = this.Subject;
            if (subject == null)
                return;

            for (int i = 0; i < this.AccessNode.Children.Count; i++)
            {
                PropertyAccessNode propertyNode = (PropertyAccessNode)this.AccessNode.Children[i];
                PropertyChangedEventManager.AddListener(subject, this, propertyNode.Property.Name);
            }

            if (this.Children != null)
            {
                for (int i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].UpdateSubject(subject);
                }
            }
        }
        
        private void UpdateSubject(INotifyPropertyChanged parentSubject)
        {
            if (parentSubject == null)
                this.Subject = null;

            this.Subject = (INotifyPropertyChanged)this.PropertyAccessNode.Property.GetValue(parentSubject, null);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (this.Children != null)
            {
                var nodeMatchingPropertyName = this.Children.FirstOrDefault(node => node.PropertyAccessNode.Property.Name == args.PropertyName);
                
                if (nodeMatchingPropertyName == null)
                    return;

                nodeMatchingPropertyName.UpdateSubject(this.Subject);
            }

            if (PropertyChanged == null)
                return;

            PropertyChanged();
        }

        private void Unsubscribe()
        {
            INotifyPropertyChanged subject = this.Subject;
            if (subject == null)
                return;

            for (int i = 0; i < this.AccessNode.Children.Count; i++)
            {
                PropertyAccessNode propertyNode = (PropertyAccessNode)this.AccessNode.Children[i];
                PropertyChangedEventManager.RemoveListener(subject, this, propertyNode.Property.Name);
            }
            
            if (this.Children != null)
            {
                foreach (SubscriptionNode child in this.Children)
                {
                    child.Unsubscribe();
                }
            }
        }

        #region IWeakEventListener Members

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs args)
        {
            OnPropertyChanged(sender, (PropertyChangedEventArgs)args);
            return true;
        }

        #endregion
    }
}
