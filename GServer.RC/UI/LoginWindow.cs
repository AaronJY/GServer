using Cairo;
using Gtk;

namespace GServer.RC.UI;

internal class LoginWindow : Window
{
    private Entry usernameEntry;
    private Entry passwordEntry;
    private Label errorLabel;
    
    public LoginWindow() : base("Login")
    {
        SetDefaultSize(400, 250);
        DeleteEvent += (_, _) => Application.Quit();
        SetPosition(WindowPosition.Center);
        Resizable = false;
        Present();
        
        // Controls
        
        // Create a vertical box to hold the widgets
        VBox vbox = new VBox(true, 2);
        vbox.BorderWidth = 10;

        // Username label and entry
        Label usernameLabel = new Label("Username:");
        usernameEntry = new Entry();
        
        // Password label and entry
        Label passwordLabel = new Label("Password:");
        passwordEntry = new Entry { Visibility = false }; // Hide the password
        passwordEntry.Text = ""; // Clear the text

        // Login button
        Button loginButton = new Button("Login");
        loginButton.Clicked += (_, _) =>
        {
            ShowError("Hello!");
        };
        
        loginButton.Halign = Align.End;
        loginButton.WidthRequest = 80;
        
        // Error label
        errorLabel = new Label();
        // errorLabel.Hide();
        // errorLabel.ModifyFg(StateType.Normal, new Gdk.Color(255, 0, 0));

        // Add all widgets to the vertical box
        vbox.PackStart(usernameLabel, false, false, 5);
        vbox.PackStart(usernameEntry, false, false, 5);
        vbox.PackStart(passwordLabel, false, false, 5);
        vbox.PackStart(passwordEntry, false, false, 5);
        vbox.PackStart(loginButton, false, false, 5);
        vbox.PackStart(errorLabel, false, false, 5);

        // Add the vertical box to the window
        Add(vbox);
        ShowAll();
        
        errorLabel.Hide();
    }

    public void ShowError(string error)
    {
        errorLabel.Text = error;
        errorLabel.Show();
    }
}