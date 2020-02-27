using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using Xamarin.Forms;

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
		public ObservableCollection<History> histories { get; set; }
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
		public string created
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
	/// Изменения по задачи, какие поля были затронуты, значения до и после, данные по пользователю (группы)
	/// </summary>
	public class Item
	{
		public string key { get; set; }
		public string name { get; set; }
		public string field { get; set; }
		public string fieldtype { get; set; }
		public string from { get; set; }
		public string fromString { get; set; } = "Отсутствует";
		public string to { get; set; }
		public string toString { get; set; } = "Отсутствует";
		public string self { get; set; }
	}
	public class Resolution
	{
		public string self { get; set; }
		public string id { get; set; }
		public string description { get; set; }
		public string name { get; set; }
		public bool required { get; set; }
		public Schema schema { get; set; }
		public List<string> operations { get; set; }
		public List<AllowedValue> allowedValues { get; set; }
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
		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
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
		public ImageSource icon
		{
			get
			{
				Image image = new Image();
				Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
				//выбираем изображение с максимальным разрешением, при его наличии
				if (iconUrl != null)
				{
					uri = iconUrl;
				}

				WebClient webClient = new WebClient();
				string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
				var byteArray = webClient.DownloadData(uri);
				image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
				return image.Source;
			}
			set
			{
				icon = value;
			}
		}
		public string self { get; set; }
		public string id { get; set; }
		public string description { get; set; }
		public Uri iconUrl { get; set; }
		public string name { get; set; }
		public bool subtask { get; set; }
		public long avatarId { get; set; }
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

		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
		public string projectTypeKey { get; set; }
		public List<Issuetype> issuetypes { get; set; }
	}
	/// <summary>
	/// Адреса изображений и изображение
	/// </summary>
	public partial class Urls
	{
		//изображение для выгрузки
		public ImageSource image
		{
			get
			{
				Image img = new Image();
				Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
				//выбираем изображение с максимальным разрешением, при его наличии
				if (The48X48 != null)
				{
					uri = The48X48;
				}
				else
				{
					if (The32X32 != null)
					{
						uri = The32X32;
					}
					else
					{
						if (The24X24 != null)
						{
							uri = The24X24;
						}
						else
						{
							if (The16X16 != null)
							{
								uri = The16X16;
							}
						}
					}
				}
				WebClient webClient = new WebClient();
				string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
				var byteArray = webClient.DownloadData(uri);
				img.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
				return img.Source;
			}
			set
			{
				image = value;
			}
		}

		[JsonProperty("48x48")]
		public Uri The48X48 { get; set; }

		[JsonProperty("24x24")]
		public Uri The24X24 { get; set; }

		[JsonProperty("16x16")]
		public Uri The16X16 { get; set; }

		[JsonProperty("32x32")]
		public Uri The32X32 { get; set; }
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
		public string colorName
		{
			//get; set;
			get
			{
				Color slateBlue = System.Drawing.Color.FromName(this.color.ToString());

				string str = slateBlue.ToHex().ToString();
				return slateBlue.ToHex().ToString();
			}
			set
			{

			}
		}
		public Color color
		{
			private get;
			set;
		}
		public string name { get; set; }
	}

	public class Status
	{
		public ImageSource icon
		{
			get
			{
				Image image = new Image();
				Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
				//выбираем изображение с максимальным разрешением, при его наличии
				if (iconUrl != null)
				{
					uri = iconUrl;
				}

				WebClient webClient = new WebClient();
				string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
				var byteArray = webClient.DownloadData(uri);
				image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
				return image.Source;
			}
			set
			{
				icon = value;
			}
		}
		public string self { get; set; }
		public string description { get; set; }
		public Uri iconUrl { get; set; }
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

		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
	}
	/// <summary>
	/// Значение внешний или внутренний будет комментарий
	/// </summary>
	public class Value
	{
		public bool Internal { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public string type { get; set; }
		public string searcherKey { get; set; }
		public string self { get; set; }
		public int numericId { get; set; }
		public bool isLocked { get; set; }
		public bool isManaged { get; set; }
		public bool isAllProjects { get; set; }
		public int projectsCount { get; set; }
		public int screensCount { get; set; }
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
		public string created
		{
			get
			{
				return _created;
			}
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
	public class To
	{
		public string self { get; set; }
		public string description { get; set; }
		public string iconUrl { get; set; }
		public string name { get; set; }
		public string id { get; set; }
		public StatusCategory statusCategory { get; set; }
	}
	public class Attachment
	{
		public bool required { get; set; }
		public string type { get; set; }
		public Schema schema { get; set; }
		public string name { get; set; }
		public List<object> operations { get; set; }
	}
	public class AllowedValue
	{
		public string self { get; set; }
		public string value { get; set; }
		public ImageSource icon
		{
			get
			{
				Image image = new Image();
				Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
				//выбираем изображение с максимальным разрешением, при его наличии
				if (iconUrl != null)
				{
					uri = new Uri(iconUrl);
				}

				WebClient webClient = new WebClient();
				string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
				var byteArray = webClient.DownloadData(uri);
				image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
				return image.Source;
			}
			set
			{
				icon = value;
			}
		}
		public string id { get; set; }
		public string description { get; set; }
		public string iconUrl { get; set; }
		public bool subtask { get; set; }
		public long avatarId { get; set; }
	}
	public class Schema
	{
		public string type { get; set; }
		public string items { get; set; }
		public string system { get; set; }
		public string custom { get; set; } = "";
		public long customId { get; set; }
	}
	public class Transition
	{
		public string id { get; set; }
		public string name { get; set; }
		public To to { get; set; }
		public List<Fields> fields { get; set; }
	}
	public class UpdateAuthor
	{
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
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
		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class Issuelink
	{
		public string id { get; set; }
		public string name { get; set; }
		public string inward { get; set; }
		public string outward { get; set; }
		public string self { get; set; }
		//public Type type { get; set; }
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
		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}
	public class Fields
	{
		public ObservableCollection<Comment> comment { get; set; }
		public List<AllowedValue> allowedValues { get; set; }
		public List<string> subtasks { get; set; }
		public List<string> operations { get; set; }
		public List<Issuelink> issuelinks { get; set; }
		public Resolution resolution { get; set; }
		public Assignee assignee { get; set; }
		public Attachment attachment { get; set; }
		public Reporter reporter { get; set; }
		public Votes votes { get; set; }
		public Issuetype issuetype { get; set; }
		public Project project { get; set; }
		public Status status { get; set; }
		public Creator creator { get; set; }
		public Watches watches { get; set; }
		public Schema schema { get; set; }
		public string resolutiondate
		{
			get { return _resolutiondate; }
			set
			{
				System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ru-RU");
				_resolutiondate = Convert.ToString(value, culture);
			}
		}
		private string _resolutiondate { get; set; }
		private string _updated;
		public string name { get; set; }
		public string key { get; set; }
		public string displayName { get; set; }
		public string autoCompleteUrl { get; set; }
		public string value { get; set; }
		public string defaultValue { get; set; } = "Заполните значение...";
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
		public string duedate { get; set; }
		private string _created;
		public string created
		{
			get
			{
				return _created;
			}
			set
			{
				_created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy hh:mm");
			}
		}
		public bool required { get; set; } = false;
		public bool hasScreen { get; set; } = true;
		public bool isAvailable { get; set; } = true;
		public bool hasDefaultValue { get; set; } = false;
		public bool isGlobal { get; set; } = false;
		public bool isConditional { get; set; } = false;
		public bool isInitial { get; set; } = false;


	}
	public class Issue
	{
		public string expand { get; set; }
		public string id { get; set; }
		public string self { get; set; }
		public string key { get; set; }
		//Изображение типа задачи в котором находится задача
		public string img { get; set; }
		//Тема задачи (используется при наличии в запросе на получение, связанные задачи)
		public string summary { get; set; }
		//Тема задачи (используется при наличии в запросе на получение, связанные задачи)
		public string summaryText { get; set; }
		public Fields fields { get; set; }
	}
	/// <summary>
	/// Класс для показа связанных задач
	/// </summary>
	public class Section
	{
		public string label { get; set; }
		public string sub { get; set; }
		public string id { get; set; }
		public List<Issue> issues { get; set; }
	}

	public class RootObject
	{
		public List<string> errorMessages { get; set; }
		public List<Watcher> watchers { get; set; }
		public List<User> users { get; set; }
		public List<Worklog> worklogs { get; set; }
		public List<Project> projects { get; set; }
		public List<Section> sections { get; set; }
		public List<Issuelink> issueLinkTypes { get; set; }
		public List<Transition> transitions { get; set; }
		public ObservableCollection<Issue> issues { get; set; }
		public ObservableCollection<Comment> comments { get; set; }
		public Changelog changelog { get; set; }
		public Session session { get; set; }
		public LoginInfo loginInfo { get; set; }
		public Groups groups { get; set; }
		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
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
