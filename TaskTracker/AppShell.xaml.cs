using CSharpMobileApp.Views;

namespace CSharpMobileApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("ToDoListPage", typeof(ToDoListPage));
		Routing.RegisterRoute("AddToDoPage", typeof(AddToDoPage));
		Routing.RegisterRoute("EditToDoPage", typeof(EditToDoPage));
	}
}
