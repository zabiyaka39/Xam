using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMobile
{
    /// <summary>
    /// Параметры поиска задачи
    /// </summary>
    public class IssueJSONSearch
    {
        public string jql { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public string properties { get; set; }
        public bool updateHistory { get; set; }
    }

    /// <summary>
    /// Решение
    /// </summary>
    public class Resolution
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    /// <summary>
    /// Исполнитель
    /// </summary>
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

    /// <summary>
    /// Проголосовавшие за задачу
    /// </summary>
    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    /// <summary>
    /// Тип задачи
    /// </summary>
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

    /// <summary>
    /// Данные по проекту
    /// </summary>
    public class Project
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string projectTypeKey { get; set; }
    }

    /// <summary>
    /// Наблюдатели
    /// </summary>
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

    /// <summary>
    /// Статус задачи
    /// </summary>
    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public StatusCategory statusCategory { get; set; }
    }

    /// <summary>
    /// Создатель задачи
    /// </summary>
    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    /// <summary>
    /// Ссылки на связанные задачи
    /// </summary>
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

    /// <summary>
    /// Поля задачи
    /// </summary>
    public class Fields
    {
        public Resolution resolution { get; set; }
        public Assignee assignee { get; set; }
        public List<object> subtasks { get; set; }
        public Reporter reporter { get; set; }
        public Votes votes { get; set; }
        public Issuetype issuetype { get; set; }
        public Project project { get; set; }
        public DateTime resolutiondate { get; set; }
        public Watches watches { get; set; }
        public DateTime updated { get; set; }
        public string description { get; set; }
        public string summary { get; set; }
        public object duedate { get; set; }
        public Status status { get; set; }
        public Creator creator { get; set; }
        public DateTime created { get; set; }
        public List<Issuelink> issuelinks { get; set; }
    }
    /// <summary>
    /// Основные параметры задачи и ее поля
    /// </summary>
    public class Issue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
    }

    /// <summary>
    /// Корневой каталог результата поиска
    /// </summary>
    public class RootObject
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<Issue> issues { get; set; }
    }
}

