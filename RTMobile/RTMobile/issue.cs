﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RTMobile
{
   
    /// <summary>
    /// Класс хранящий JSON запрос для поиска задач
    /// </summary>
    public class IssueJSONSearch
    {
        public string jql { get; set; }
        public string fields { get; set; }
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public bool validateQuery { get; set; } = true;
    }
    /// <summary>
    /// Класс хранящий JSON запрос для поиска комментариев
    /// </summary>
    public class CommentJSONSearch
    {
        public string orderBy { get; set; }
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
    }
    /// <summary>
    /// Класс хранящий данные по истории (общие данные и список событий задачи)  задачи
    /// </summary>
    public class Changelog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<History> histories { get; set; }
    }
    /// <summary>
    /// Метаданные истории
    /// </summary>
    public class HistoryMetadata
    {
    }
    /// <summary>
    /// События по задаче
    /// </summary>
    public class History
    {
        public string id { get; set; }
        public Author author { get; set; }
        private string _created { get; set; }
        public string сreated
        {
            get { return _created; }
            set
            {
                _created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        public List<Item> items { get; set; }
        public HistoryMetadata historyMetadata { get; set; }
    }
    /// <summary>
    /// Изменения по задачи, какие поля были затронуты, значения до и после
    /// </summary>
    public class Item
    {
        public string name { get; set; }
        public string field { get; set; }
        public string fieldtype { get; set; }
        public string from { get; set; }
        public string fromString { get; set; }
        public string to { get; set; }
        public string toString { get; set; }
    }
    public class Resolution
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }
    public class Assignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }
    public class Reporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }
    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Issuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }
    public class Project
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string expand { get; set; }
        public int recent { get; set; }
        public bool includeArchived { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string projectTypeKey { get; set; }
        public List<Issuetype> issuetypes { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class StatusCategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public StatusCategory statusCategory { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public AvatarUrls avatarUrls { get; set; }
    }
    /// <summary>
    /// Значение внешний или внутренний будет комментарий
    /// </summary>
    public class Value
    {
        public bool Internal { get; set; }
    }
    /// <summary>
    /// Класс настроек комментариев
    /// </summary>
    public class Property
    {
        public string key { get; set; }
        public Value value { get; set; }
    }
    /// <summary>
    /// Класс комментариев задачи
    /// </summary>
    public class Comment
    {
        public string self { get; set; }
        public string id { get; set; }
        public Author author { get; set; }
        public string body { get; set; }
        public UpdateAuthor updateAuthor { get; set; }
        private string _created { get; set; }
        public string сreated
        {
            get { return _created; }
            set
            {
                _created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        private string _updated { get; set; }
        public string updated
        {
            get { return _updated; }
            set
            {
                _updated = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        public List<Property> properties { get; set; }
    }
    public class UpdateAuthor
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }
    public class AvatarUrls
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }
    public class Worklog
    {
        public string self { get; set; }
        public Author author { get; set; }
        public UpdateAuthor updateAuthor { get; set; }
        public string comment { get; set; }
        private string _created { get; set; }
        public string created
        {
            get { return _created; }
            set
            {
                _created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        private string _updated { get; set; }
        public string updated
        {
            get { return _updated; }
            set
            {
                _updated = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        private string _started { get; set; }
        public string started
        {
            get { return _started; }
            set
            {
                _started = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        public string timeSpent { get; set; }
        public int timeSpentSeconds { get; set; }
        public string id { get; set; }
        public string issueId { get; set; }
    }
    public class Author
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Issuelink
    {
        public string id { get; set; }
        public string self { get; set; }
        public Type type { get; set; }
        public OutwardIssue outwardIssue { get; set; }
    }
    public class OutwardIssue
    {
        public string id { get; set; }
        public string key { get; set; }
        public string self { get; set; }
        public Fields fields { get; set; }
    }
    public class Watcher
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }
    public class Fields
    {
        public Resolution resolution { get; set; }
        public Assignee assignee { get; set; }
        public List<object> subtasks { get; set; }
        public Reporter reporter { get; set; }
        public Votes votes { get; set; }
        public Issuetype issuetype { get; set; }
        public Project project { get; set; }
        public string resolutiondate { get; set; }
        public Watches watches { get; set; }

        private string _updated;
        public string updated
        {
            get { return _updated; }
            set
            {
                _updated = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        public string description { get; set; }
        public string summary { get; set; }
        public object duedate { get; set; }
        public Status status { get; set; }
        public Creator creator { get; set; }

        private string _created;
        public string created
        {
            get { return _created; }
            set
            {
                _created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
            }
        }
        public List<Issuelink> issuelinks { get; set; }
    }
    public class Issue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
    }

    public class RootObject
    {
        public List<string> errorMessages { get; set; }
        public List<Watcher> watchers { get; set; }
        public List<Worklog> worklogs { get; set; }
        public List<Project> projects { get; set; }
        public List<Issue> issues { get; set; }
        public List<Comment> comments { get; set; }
        public Changelog changelog { get; set; }
        public Session session { get; set; }
        public LoginInfo loginInfo { get; set; }
        public Groups groups { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public ApplicationRoles applicationRoles { get; set; }
        public int id { get; set; }
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public bool isWatching { get; set; }
        public string watchCount { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string emailAddress { get; set; }       
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string locale { get; set; }
       
        
    }
}
