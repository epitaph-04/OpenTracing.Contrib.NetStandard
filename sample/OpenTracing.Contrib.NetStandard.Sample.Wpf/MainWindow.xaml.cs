using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.NetStandard.Sample.Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var tracer = App.ServiceProvider.GetRequiredService<ITracer>();
			using (var span = tracer.BuildSpan("Button click").AsChildOf(tracer.ActiveSpan).WithTag("Wait", "10Sec").StartActive())
			{
				span.Span.Log("Method Call");
				Task.Delay(5000).Wait();
				HttpClient client = new HttpClient();
				var result = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, Url.Text)).Result;

				this.browser.Navigate(Url.Text);
			}
		}
	}
}
