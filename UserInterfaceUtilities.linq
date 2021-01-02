<Query Kind="Program">
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var foo = await UserInterfaceUtilities.UserInputAsync("Write something", defaultResponse: "Something like this.");
	foo.Dump();
}

public static class UserInterfaceUtilities
{
	public static Task<string> UserInputAsync(string title, string defaultResponse = "", string continueButtonText = "OK", bool hideOnContinue = true)
	{
		TaskCompletionSource<string> completed = new TaskCompletionSource<string>();

		var heading = new Label();
		heading.Text = title;
		heading.Dump();

		// Create text box
		var textBox = new TextBox();
		textBox.Text = defaultResponse;
		textBox.Dump();

		// Create Button
		var button = new Button(continueButtonText);
		button.IsMultithreaded = true;
		button.Click += (sender, args) =>
		{
			textBox.Enabled = false;
			button.Enabled = false;

			if (hideOnContinue)
			{
				textBox.Visible = false;
				button.Visible = false;
				heading.Visible = false;
			}
			completed.TrySetResult(textBox.Text);
		};
		button.Dump();

		textBox.Focus();

		return completed.Task;
	}
}