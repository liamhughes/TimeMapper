<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#load ".\CalendarService"
#load ".\UserInterfaceUtilities"

const string CALENDAR_NAME = "Time Map";
const string TEMPLATE_FILE_NAME = "template.json";
const string TIME_SPAN_FORMAT_STRING = @"h\:mm";

private CalendarService Service { get; set; }

async Task Main()
{
	Service = new CalendarService();
	
	var entries = LoadEntries();
	
	for(var index = 0; index < entries.Count; index++)
	{
		var entry = entries[index];
		
		await UpdateEvent(entry);
		
		CreateEvent(entry);
		
		var nextEntry = index < entries.Count - 1 ? entries[index + 1] : null;
		if (nextEntry != null && !nextEntry.StartTime.HasValue)
			nextEntry.StartTime = entry.EndTime;
	}
	
	entries.Dump();
}

void CreateEvent(Entry entry)
 	=> Service.CreateEvent(entry, CALENDAR_NAME);

string GetScriptDirectoryPath()
	=> Path.GetDirectoryName(Util.CurrentQueryPath);

string GetTemplateFileFullPath()
	=> Path.Combine(GetScriptDirectoryPath(), TEMPLATE_FILE_NAME);

async Task<string> GetUserInputAsync(string prompt, string defaultResponse)
	=> await UserInterfaceUtilities.UserInputAsync(prompt, defaultResponse: defaultResponse, hideOnContinue: false);

async Task<TimeSpan> GetUserInputAsync(string prompt, TimeSpan? defaultResponse)
	=> TimeSpan.ParseExact(
		await GetUserInputAsync(
			prompt, 
			defaultResponse?.ToString(TIME_SPAN_FORMAT_STRING)
		), 
		TIME_SPAN_FORMAT_STRING, 
		CultureInfo.InvariantCulture
	);

List<Entry> LoadEntries()
{
	var filePath = GetTemplateFileFullPath();
	var entriesJson = File.ReadAllText(filePath);
	var entries = JsonConvert.DeserializeObject<List<Entry>>(entriesJson);
	return entries;
}

async Task UpdateEvent(Entry entry)
{
	entry.Title = await GetUserInputAsync("Title", entry.Title);
	entry.StartTime = await GetUserInputAsync("Start Time", entry.StartTime);
	entry.Duration = await GetUserInputAsync("Duration", entry.Duration);
}

