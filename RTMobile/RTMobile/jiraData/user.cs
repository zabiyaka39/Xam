using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTMobile
{
    /// <summary>
    /// Роли в проекте
    /// </summary>
    public class ApplicationRoles
    {
        public int size { get; set; }
        public List<Item> items { get; set; }
    }
    /// <summary>
    /// Группы пользователей
    /// </summary>
    public class Groups
    {
        public int size { get; set; }
        public List<Item> items { get; set; }
    }
    /// <summary>
    /// Класс хранящий данные запроса для авторизации
    /// </summary>
    public class Authorization
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    /// <summary>
    ///Данные о сессии авторизации
    /// </summary>
    public class Session
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    /// <summary>
    /// Данные о авторизации
    /// </summary>
    public class LoginInfo
    {
        public int failedLoginCount { get; set; } //количество неудачных входов в систему
        public int loginCount { get; set; } //количество входов в систему
        private string _lastFailedLoginTime { get; set; } //дата последней неудавшейся авторизации
        public string lastFailedLoginTime
        {
            get
            {
                return _lastFailedLoginTime;
            }
            set
            {
                _lastFailedLoginTime = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        private string _previousLoginTime { get; set; } //дата последней удавшийся авторизации
        public string previousLoginTime
        {
            get
            {
                return _previousLoginTime;
            }
            set
            {
                _previousLoginTime = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
    }
    /// <summary>
    /// Данные о пользователе
    /// </summary>
    public class User
    {
        public string self { get; set; }
        public string key { get; set; }
        public string name { get; set; } = "Отсутствует";
        public string emailAddress { get; set; } = "Отсутствует";
        [JsonProperty("avatarUrls")]
        public Urls AvatarUrls { get; set; }
        public string displayName { get; set; } = "Отсутствует";
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string locale { get; set; }
        public Groups groups { get; set; }
        public ApplicationRoles applicationRoles { get; set; }
        public string expand { get; set; }
    }
}
