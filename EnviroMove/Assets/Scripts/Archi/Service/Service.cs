using System;
using System.Collections.Generic;
using System.Reflection;
using Archi.Service.Interface;
using Attributes;
using UnityEngine;

namespace Archi.Service
{
    public abstract class Service : IService
    {
        protected bool currentServiceState = true;
        protected List<GameObject> gameObjectDependency;

        [InitializedOnCompose]
        protected abstract void Initialize();

        public virtual void SetServiceState(bool state)
        {
            currentServiceState = state;
            for (int i = 0; i < gameObjectDependency.Count; i++)
            {
                gameObjectDependency[i].SetActive(state);
            }
        }

        public virtual bool SwitchServiceState()
        {
            SetServiceState(!currentServiceState);
            return currentServiceState;
        }

        protected void SetObjectDependencies(object obj)
        {
            Debug.Log(obj);
            var fields = obj.GetType() .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                var dependenceFields = Attribute.GetCustomAttributes(field, typeof(ServiceDependency));
                if(dependenceFields.Length == 0)continue;
                foreach (var _ in dependenceFields)
                {
                    var varType = field.FieldType;
                    if (varType.IsInterface && typeof(IService).IsAssignableFrom(varType) &&
                        varType != typeof(IService))
                    {
                        var serviceFields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public |
                                                                BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        foreach (var serviceField in serviceFields)
                        {
                            if(serviceField.FieldType == varType) field.SetValue(obj,serviceField.GetValue(this));
                        }
                    }
                }
            }
        }
    }
}