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
	/// Класс для отправки запроса на сервер
	/// </summary>
	public class JSONRequest
	{
		/// <summary>
		/// Поисковый запрос в формате JQL
		/// </summary>
		public string jql { get; set; }
		/// <summary>
		/// Отправляемы поля (те что будут находится в запросе в параметре fields)
		/// </summary>
		public string fields { get; set; }
		/// <summary>
		/// Дополнительные (расширенные) параметры получения запроса
		/// </summary>
		public string expand { get; set; }
		/// <summary>
		/// Начало запроса (с какого элемента показывать)
		/// </summary>
		public string startAt { get; set; }
		/// <summary>
		/// Максимальные искомый пул (максимальное количество врезультатов которое необходимо показать (MAX - 1000))
		/// </summary>
		public string maxResults { get; set; } 
		/// <summary>
		/// Содержание запроса (тело)
		/// </summary>
		public string body { get; set; }
		/// <summary>
		/// Сортировка которую необходимо произвести для полученного результата
		/// </summary>
		public string orderBy { get; set; }
		/// <summary>
		/// Адрес запроса на сервер, игнорируем при сериализации в JSON
		/// </summary>
		[JsonIgnore]
		public string urlRequest { get; set; }
		/// <summary>
		/// Метод запроса (POST, GET, PUT, ...), игнорируем при сериализации в JSON
		/// </summary>
		[JsonIgnore]
		public string methodRequest { get; set; }
	}

	/// <summary>
	/// Класс хранящий данные по истории (общие данные и список событий задачи)  задачи
	/// </summary>
	public class Changelog
	{
		/// <summary>
		/// Начальная позиция
		/// </summary>
		public int startAt { get; set; }
		/// <summary>
		/// максимальное количество выводимых результатов
		/// </summary>
		public int maxResults { get; set; }
		/// <summary>
		/// Общее количество полученных результатов
		/// </summary>
		public int total { get; set; }
		/// <summary>
		/// Коллекция истории задачи
		/// </summary>
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
		/// <summary>
		/// Номер записи
		/// </summary>
		public string id { get; set; }
		/// <summary>
		/// Автор записи
		/// </summary>
		public User author { get; set; }
		/// <summary>
		/// Дата создания записи
		/// </summary>
		private string _created { get; set; }
		/// <summary>
		/// Дата создания записи
		/// </summary>
		public string created
		{
			get { return _created; }
			//Конвертация даты в общепринятый формат 
			set => _created = Convert.ToDateTime(value).ToString("dd.MM.yyyy H:mm");
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
				string authorize = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + authorize;
				var byteArray = webClient.DownloadData(uri);
				webClient.Dispose();
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
		public string expand { get; set; }
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
				string authorize = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + authorize;
				var byteArray = webClient.DownloadData(uri);
				webClient.Dispose();
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

	public class Watchers
	{
		public string self { get; set; }
		public int watchCount { get; set; }
		public bool isWatching { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		[JsonProperty("avatarUrls")]
		public Urls AvatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
		public ObservableCollection<User> watchers { get; set; }
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
				string authorize = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + authorize;
				var byteArray = webClient.DownloadData(uri);
				webClient.Dispose();
				image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
				return image.Source;
			}
			set
			{
				icon = value;
			}
		}
		public string self { get; set; }
		/// <summary>
		/// Описание статуса
		/// </summary>
		public string description { get; set; }
		/// <summary>
		/// Адрес иконки статуса
		/// </summary>
		public Uri iconUrl { get; set; }
		/// <summary>
		/// Название статуса
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Номер статуса
		/// </summary>
		public string id { get; set; }
		/// <summary>
		/// Категория сатуса
		/// </summary>
		public StatusCategory statusCategory { get; set; }
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
		public int maxResults { get; set; }
		public int total { get; set; }
		public int startAt { get; set; }
		public string self { get; set; }
		public string id { get; set; }
		public User author { get; set; }
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
				_created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		private string _updated { get; set; }
		public string updated
		{
			get { return _updated; }
			set
			{
				_updated = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		public List<Property> properties { get; set; }
		public ObservableCollection<Comment> comments { get; set; }
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
		/// <summary>
		/// Классификация форматов заружаемых в систему
		/// </summary>
		private static Dictionary<string, string> countries = new Dictionary<string, string>
		{
			{"image/png", "image"},
			{"image/bmp", "image"},
			{"image/x-emf", "image"},
			{"image/jpeg", "image"},
			{"image/pjpeg", "image"},
			{"image/pict", "image"},
			{"image/x-portable-graymap", "image"},
			{"image/x-portable-anymap", "image"},
			{"image/x-macpaint", "image"},
			{"image/vnd.rn-realflash", "image"},
			{"image/x-quicktime", "image"},
			{"image/x-cmu-raster", "image"},
			{"image/x-portable-bitmap", "image"},
			{"image/gif", "image"},
			{"image/x-xbitmap", "image"},
			{"image/x-xpixmap", "image"},
			{"image/x-xwindowdump", "image"},
			{"image/vnd.ms-photo", "image"},
			{"image/vnd.wap.wbmp", "image"},
			{"image/x-rgb", "image"},
			{"image/ico", "image"},
			{"image/ief", "image"},
			{"image/tiff", "image"},
			{"image/wmf", "image"},
			{"image/x-jg", "image"},
			{"image/x-cmx", "image"},
			{"image/x-icon", "image"},
			{"image/cis-cod", "image"},
			{"text/xml", "text"},
			{"application/msword", "text"},
			{"application/vnd.ms-powerpoint.slide.macroEnabled.12", "text"},
			{"application/vnd.ms-powerpoint.presentation.macroEnabled.12", "text"},
			{"application/vnd.ms-powerpoint.slideshow.macroEnabled.12", "text"},
			{"application/vnd.ms-powerpoint.addin.macroEnabled.12", "text"},
			{"application/vnd.ms-powerpoint.template.macroEnabled.12", "text"},
			{"application/vnd.ms-powerpoint", "text"},
			{"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text"},
			{"application/vnd.ms-excel.sheet.macroenabled.12", "text"},
			{"application/vnd.ms-excel.sheet.binary.macroenabled.12", "text"},
			{"application/vnd.ms-excel.addin.macroenabled.12", "text"},
			{"application/vnd.ms-excel", "text"},
			{"application/vnd.openxmlformats-officedocument.wordprocessingml.template", "text"},
			{"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text"},
			{"application/vnd.ms-word.document.macroEnabled.12", "text"},
			{"text/css", "text"},
			{"text/csv", "text"},
			{"text/dlm", "text"},
			{"text/html", "text"},
			{"text/jscript", "text"},
			{"text/richtext", "text"},
			{"text/scriptlet", "text"},
			{"text/sgml", "text"},
			{"text/iuls", "text"},
			{"text/vbscript", "text"},
			{"text/vnd.wap.wmlscript", "text"},
			{"text/vnd.wap.wml", "text"},
			{"text/x-vcard", "text"},
			{"text/tab-separated-values", "text"},
			{"text/x-ms-rqy", "text"},
			{"text/x-ms-odc", "text"},
			{"text/x-html-insertion", "text"},
			{"text/x-ms-iqy", "text"},
			{"text/webviewhtml", "text"},
			{"text/x-component", "text"},
			{"text/x-hdml", "text"},
			{"text/x-ms-group", "text"},
			{"text/x-setext", "text"},
			{"text/x-ms-contact", "text"}
		};
		/// <summary>
		/// Обязательность поля для заполнения
		/// </summary>
		public bool required { get; set; }
		/// <summary>
		/// Тип вложения
		/// </summary>
		public string type { get; set; }
		/// <summary>
		/// Схема вложения
		/// </summary>
		public Schema schema { get; set; }
		/// <summary>
		/// Имя загруженного файла
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Список производимых операций с полем
		/// </summary>
		public List<object> operations { get; set; }
		public string self { get; set; }
		/// <summary>
		/// Номер загруженного файла
		/// </summary>
		public string id { get; set; }
		/// <summary>
		/// Имя загруженного файла с расширением
		/// </summary>
		public string filename { get; set; }
		/// <summary>
		/// Данные об авторе загруженного файла
		/// </summary>
		public User author { get; set; }
		private string _created { get; set; }
		/// <summary>
		/// Дата создания файла
		/// </summary>
		public string created
		{
			get { return _created; }
			set
			{
				_created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		/// <summary>
		/// Размер файла
		/// </summary>
		public int size { get; set; }
		public string extension { get; set; }
		private string _mimeType { get; set; }
		/// <summary>
		/// Тип файла (сравнивается со словарем после чего получает группу принадлежности файлов)
		/// </summary>
		public string mimeType
		{
			get
			{
				return _mimeType;
			}
			set
			{
				extension = Path.GetExtension(filename);
				if (countries.ContainsKey(value))
				{
					_mimeType = countries[value];
				}
				else
				{
					_mimeType = "other";
				}
			}
		}
		/// <summary>
		/// Исходное изображение
		/// </summary>
		public ImageSource contentImage
		{
			get
			{
				if (mimeType == "image")
				{
					Image image = new Image();
					Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
					//выбираем изображение с максимальным разрешением, при его наличии
					if (content != null)
					{
						uri = new Uri(content);
					}
					else
						return null;
					WebClient webClient = new WebClient();
					string authorize = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
					webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + authorize;
					var byteArray = webClient.DownloadData(uri);
					webClient.Dispose();
					image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
					return image.Source;
				}
				else
				{
					return null;
				}
			}
			set
			{
				contentImage = value;
			}
		}
		public string content { get; set; }
		/// <summary>
		/// Превью изображения
		/// </summary>
		public ImageSource thumbnailImage
		{
			get
			{
				Image image = new Image();
				Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
				//выбираем изображение с максимальным разрешением, при его наличии
				if (thumbnail != null)
				{
					uri = thumbnail;
				}

				WebClient webClient = new WebClient();
				string authorize = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + authorize;
				var byteArray = webClient.DownloadData(uri);
				webClient.Dispose();
				image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
				return image.Source;
			}
			set
			{
				thumbnailImage = value;
			}
		}
		/// <summary>
		/// Адрес превью изображения
		/// </summary>
		public Uri thumbnail { get; set; }
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
				string authorize = Convert.ToBase64String(Encoding.ASCII.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ":" + CrossSettings.Current.GetValueOrDefault("password", string.Empty)));
				webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + authorize;
				var byteArray = webClient.DownloadData(uri);
				webClient.Dispose();
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
		public List<Child> children { get; set; }
	}
	public class Child
	{
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
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
		public User author { get; set; }
		public UpdateAuthor updateAuthor { get; set; }
		public string comment { get; set; }
		private string _created { get; set; }
		public string created
		{
			get { return _created; }
			set
			{
				_created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		private string _updated { get; set; }
		public string updated
		{
			get { return _updated; }
			set
			{
				_updated = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		private string _started { get; set; }
		public string started
		{
			get { return _started; }
			set
			{
				_started = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		public string timeSpent { get; set; }
		public int timeSpentSeconds { get; set; }
		public string id { get; set; }
		public string issueId { get; set; }
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
	public class Fields
	{
		public Comment comment { get; set; }
		public List<AllowedValue> allowedValues { get; set; }
		public List<Issue> subtasks { get; set; }
		public List<string> operations { get; set; }
		public List<Issuelink> issuelinks { get; set; }
		public Resolution resolution { get; set; }
		public User assignee { get; set; }
		public ObservableCollection<Attachment> attachment { get; set; }
		public User reporter { get; set; }
		public Votes votes { get; set; }
		public Issuetype issuetype { get; set; }
		public Project project { get; set; }
		public Status status { get; set; }
		public User creator { get; set; }
		public Watchers watches { get; set; }
		public Schema schema { get; set; }
		public string resolutiondate
		{
			get { return _updated; }
			set => _updated = Convert.ToDateTime(value).ToString("dd.MM.yyyy H:mm");
		}
		private string _resolutiondate { get; set; }
		public string Resolutiondate
		{
			get { return _resolutiondate; }
			set => _resolutiondate = Convert.ToDateTime(value).ToString("dd.MM.yyyy H:mm");
		}
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
			set => _updated = Convert.ToDateTime(value).ToString("dd.MM.yyyy H:mm");
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
				_created = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy H:mm");
			}
		}
		public bool required { get; set; } = false;
		public bool hasScreen { get; set; } = true;
		public bool isAvailable { get; set; } = true;
		public bool hasDefaultValue { get; set; } = false;
		public bool isGlobal { get; set; } = false;
		public bool isConditional { get; set; } = false;
		public bool isInitial { get; set; } = false;
		public Guid idFieldScreen { get; set; }
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
	/// <summary>
	/// Класс работы с ошибками при ответе сервера
	/// </summary>
	public class Errors
	{
		public string assignee { get; set; }
		public string comment { get; set; }
	}

	public class RootObject
	{
		public List<string> errorMessages { get; set; }
		public Errors errors { get; set; }
		public List<User> users { get; set; }
		public List<Project> projects { get; set; }
		public List<Section> sections { get; set; }
		public List<Issuelink> issueLinkTypes { get; set; }
		public List<Transition> transitions { get; set; }
		public ObservableCollection<Issue> issues { get; set; }
		public ObservableCollection<Comment> comments { get; set; }
		public ObservableCollection<Worklog> worklogs { get; set; }
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

