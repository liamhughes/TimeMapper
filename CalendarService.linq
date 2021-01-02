<Query Kind="Program">
  <NuGetReference>Google.Apis.Calendar.v3</NuGetReference>
  <Namespace>Google</Namespace>
  <Namespace>Google.Apis.Auth.OAuth2</Namespace>
  <Namespace>Google.Apis.Calendar.v3</Namespace>
  <Namespace>Google.Apis.Calendar.v3.Data</Namespace>
  <Namespace>Google.Apis.Services</Namespace>
  <Namespace>Google.Apis.Util.Store</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var calendarService = new CalendarService();
	calendarService.CreateEvent(new Entry
	{
		Title = "Test",
		StartTime = TimeSpan.FromHours(9),
		Duration = TimeSpan.FromMinutes(42)
	}, "Time Map");
}

public class CalendarService
{
	private static readonly string ApplicationName = "TimeMapper";
	
	private static readonly string[] Scopes = { Google.Apis.Calendar.v3.CalendarService.Scope.Calendar };

	private List<CalendarListEntry> Calendars => LazyCalendars.Value;

	private Lazy<List<CalendarListEntry>> LazyCalendars { get; }

	private Google.Apis.Calendar.v3.CalendarService Service { get; }

	public CalendarService()
	{
		UserCredential credential;

		using (var stream =	new FileStream(
			GetCredentialsJsonFilePath(), 
			FileMode.Open, 
			FileAccess.Read)
		)
		{
			var tokenDirectoryPath = GetTokenDirectoryPath();
			
			credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
				GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
				"user",
				CancellationToken.None,
				new FileDataStore(tokenDirectoryPath, true)).Result;
		}

		Service = new Google.Apis.Calendar.v3.CalendarService(
			new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			}
		);
		
		LazyCalendars = new Lazy<List<CalendarListEntry>>(
			() => Service.CalendarList.List().Execute().Items.ToList()
		);
	}
	
	public void CreateEvent(Entry entry, string calendarName)
	{
		var calendars = LazyCalendars.Value;
		
		Service.Events.Insert(new Event
		{
			Summary = entry.Title,
			Start = new EventDateTime
			{
				DateTime = DateTime.Today.Add(entry.StartTime.Value)
			},
			End = new EventDateTime
			{
				DateTime = DateTime.Today.Add(entry.EndTime.Value)
			}
		}, 
		calendars.Single(c => c.Summary == calendarName).Id).Execute();
		
		"Event created.".Dump();
	}

	string GetScriptDirectoryPath()
		=> Path.GetDirectoryName(Util.CurrentQueryPath);

	string GetCredentialsJsonFilePath()
		=> Path.Combine(GetScriptDirectoryPath(), "credentials.json");

	string GetTokenDirectoryPath()
		=> Path.Combine(GetScriptDirectoryPath(), "token");
}

public class Entry
{
	public string Title { get; set; }
	public TimeSpan? Duration { get; set; }
	public TimeSpan? StartTime { get; set; }
	public TimeSpan? EndTime => StartTime + Duration;
}