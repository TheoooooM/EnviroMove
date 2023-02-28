using System;
using System.Collections;
using System.IO;
using System.Net;
using Firebase.Database;
using Google.MiniJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BDD
{
    public class DatabaseManager : MonoBehaviour
    {
        public TMP_InputField name;
        public TMP_InputField gold;

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI goldText;

        public string userID;
        private DatabaseReference dbReference;
        
        private void Start()
        {
            //userID = SystemInfo.deviceUniqueIdentifier;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))dbReference.Child("user").GetValueAsync();
        }

        public void CreateUser()
        {
            User newUser = new(name.text, int.Parse(gold.text));
            string json = JsonUtility.ToJson(newUser);


            dbReference.Child("user").Child(userID).SetRawJsonValueAsync(json);
        }

        public void UpdateUserInfo()
        {
            UpdateName();
            UpdateGold();
        }

        private void UpdateName()=>dbReference.Child("user").Child(userID).Child("name").SetValueAsync(name.text);
        private void UpdateGold()=>dbReference.Child("user").Child(userID).Child("gold").SetValueAsync(gold.text);
        
        public void GetUserInfo()
        {
            StartCoroutine(GetName(name => nameText.text = $"Name: {name}"));
            StartCoroutine(GetGold(gold => goldText.text = $"Gold: {gold}"));
        }


        IEnumerator GetName(Action<string> onCallback)
        {
            var userNameData = dbReference.Child("user").Child(userID).Child("name").GetValueAsync();
            
            yield return new WaitUntil(predicate: () => userNameData.IsCompleted);
            if (userNameData != null)
            {
                DataSnapshot snapshot = userNameData.Result;
                
                onCallback.Invoke(snapshot.Value.ToString());
            }
        }
        IEnumerator GetGold(Action<string> onCallback)
        {
            var userGoldData = dbReference.Child("user").Child(userID).Child("gold").GetValueAsync();

            yield return new WaitUntil(predicate: () => userGoldData.IsCompleted);
            if (userGoldData != null)
            {
                DataSnapshot snapshot = userGoldData.Result;
                
                onCallback.Invoke($"{snapshot.Value}");
            }
        }

        
    }
}
