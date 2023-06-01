using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Archi.Service;
using Archi.Service.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Attributes;
using Levels;
using UI;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Compositor : MonoBehaviour {
    [SerializeField] private LevelSO tutorial = null;
    protected struct DependenceInfos
    {
        public object obj;
        public FieldInfo field;
    }

    protected Dictionary<Type, IService> servicesDictionnary = new();
    protected Dictionary<Type, List<DependenceInfos>> fieldDependenciesDictionnary = new();
    private LevelData dataToTest;
    
    private void Awake()
    {
       InitCompositor().Forget();
       Addressables.CheckForCatalogUpdates();
    }

    private async UniTaskVoid InitCompositor()
    {
        //Debug.Log("Init");
        if(!Compose()) Debug.LogError("Can't Compose");
    }

    bool Compose() {
        PlayerPrefs.DeleteKey("HasMadeTutorial");

        CreateServices();
        ResoleDepencencies();
        InitializeServices();
        SceneManager.LoadScene("MainMenu");
        return true;
    }

    void CreateServices()
    {
        //Debug.Log("Create Service");
        AddService<IGameService>(new GameService());
        AddService<ILevelService>(new LevelService());
        AddService<IToolService>(new ToolService());
        AddService<IDataBaseService>(new DataBaseService());
        AddService<ITickService>(new TickService());
        AddService<IInterfaceService>(new InterfaceService());
    }

    void AddService<T>(T service) where  T : IService
    {
        if (servicesDictionnary.ContainsKey(typeof(T))) throw new DuplicateNameException("Double Services");
        servicesDictionnary.Add(typeof(T), service);

        CollectServiceDependencies(service);
        
    }

    void CollectServiceDependencies(object obj)
    {
        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
           var dependenceFields = Attribute.GetCustomAttributes(field, typeof(DependeOnService));
           if (dependenceFields.Length == 0) continue;
           foreach (var _ in dependenceFields)
           {
               if (field.FieldType.IsInterface && typeof(IService).IsAssignableFrom(field.FieldType) &&
                   field.FieldType != typeof(IService))
               {
                  var fieldList = fieldDependenciesDictionnary.ContainsKey(field.FieldType)? fieldDependenciesDictionnary[field.FieldType] : new List<DependenceInfos>();
                  fieldList.Add(new DependenceInfos
                  {
                      obj = obj,
                      field = field
                  });
                  fieldDependenciesDictionnary[field.FieldType] = fieldList;
               }
               else Debug.LogError("Can't Assign Field");
           }
        }
    }

    void ResoleDepencencies()
    {
        foreach (KeyValuePair<Type,List<DependenceInfos>> dependenceInfo in fieldDependenciesDictionnary)
        {
            Type type = dependenceInfo.Key;
            List<DependenceInfos> fieldInfos = dependenceInfo.Value;

            if (servicesDictionnary.ContainsKey(type))
            {
                var service = servicesDictionnary[type];
                foreach (var infos in fieldInfos)
                {
                    infos.field.SetValue(infos.obj, service);
                }
            }
            else throw new MissingMemberException($"Missing Service typeof({type})");
        }
    }

    void InitializeServices()
    {
        var services = new HashSet<IService>(servicesDictionnary.Values);

        foreach (var service in services)
        {
            Type serviceType = service.GetType();
            foreach (var methodInfos in serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                InitializedOnCompose[] initMethods =  (InitializedOnCompose[])Attribute.GetCustomAttributes(methodInfos, typeof(InitializedOnCompose));

                if (initMethods.Length == 0) continue;
                foreach (var _ in initMethods)
                {
                    if (methodInfos.GetParameters().Length > 0)
                        throw new TargetParameterCountException("InitializeOnCompose Method can't have param ");
                    if (methodInfos.ReturnType == typeof(void))
                    {
                        methodInfos.Invoke(service, null);
                    }
                    else throw new TypeAccessException("InitializeOnCompose Method must return void type");
                }
            }
        }
    }
}

