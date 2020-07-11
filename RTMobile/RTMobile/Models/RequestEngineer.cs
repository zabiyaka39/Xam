using System;
using System.Collections.Generic;
using System.Text;

namespace RTMobile.Models
{
	public class RequestEngineer
	{
		public Location Location { get; set; }
		public User User { get; set; }
		public Event Event { get; set; }
	}
	public class Event
	{
		/// <summary>
		/// Наименование события по которому возникла геолокация
		/// </summary>
		public string EventName { get; set; }
		/// <summary>
		/// Номер задачи по которому возникает событие
		/// </summary>
		public string NumberIssue { get; set; }
	}
	public class Location
	{
		/// <summary>
		/// Широта
		/// </summary>
		public double Latitude { get; set; }
		/// <summary>
		/// Долгота
		/// </summary>
		public double Longitude { get; set; }
		/// <summary>
		/// Id записи
		/// </summary>
		public int Id { get; set; }
	}
}
