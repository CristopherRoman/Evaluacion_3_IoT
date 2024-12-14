using Firebase.Database;
using Firebase.Database.Query;

namespace Evaluacion_3.Vistas;

public partial class CrearEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");
    public CrearEstudiante()
	{
		InitializeComponent();
		BindingContext = this;
	}
	private async void guardarButton_Clicked(object sender, EventArgs e)
	{
		var Estudiante = new Estudiante()
		{
			NombreCompleto = NombreCompletoEntry.Text,
			Email = correoEntry.Text,
			Edad = int.Parse(EdadEntry.Text),
			Curso = CursoEntry.Text,
		};

		try
		{
            await client.Child("Estudiantes").PostAsync(Estudiante);

            await DisplayAlert("Exito", $"El estudiante {Estudiante.NombreCompleto} fue guardado correctamente", "OK");
            await Navigation.PopAsync();
        }
		catch (Exception ex)
		{

            await DisplayAlert("Error", ex.Message, "OK");
		}


		
	}
}