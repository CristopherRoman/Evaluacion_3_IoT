using System.Collections.ObjectModel;
using Evaluacion_3.Modelos.Modelos;
using Firebase.Database;
using Firebase.Database.Query;

namespace Evaluacion_3.Vistas;

public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");
    public ObservableCollection<Nivel> ListarNil { get; set; } = new ObservableCollection<Nivel>();
    public ObservableCollection<Curso> ListarCursos { get; set; } = new ObservableCollection<Curso>();
    private readonly string estudianteID;

    public EditarEstudiante(string idEstudiante)
    {
        InitializeComponent();
        estudianteID = idEstudiante;
        ListarNivel();
        ListarCurso();
        CargarEstudiante(estudianteID);
    }

    // Método para cargar los niveles
    private async void ListarNivel()
    {
        try
        {
            var niveles = await client.Child("Nivel").OnceAsync<Nivel>();
            ListarNil.Clear();
            foreach (var nivel in niveles)
            {
                ListarNil.Add(nivel.Object);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar niveles: {ex.Message}", "OK");
        }
    }

    // Método para cargar los cursos
    private async void ListarCurso()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListarCursos.Clear();
            foreach (var curso in cursos)
            {
                ListarCursos.Add(curso.Object);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar cursos: {ex.Message}", "OK");
        }
    }

    // Método para cargar el estudiante a editar
    private async void CargarEstudiante(string idEstudiante)
    {
        try
        {
            var estudiante = await client.Child("Estudiantes").Child(idEstudiante).OnceSingleAsync<Estudiante>();

            if (estudiante != null)
            {
                EditPrimerNombreEntry.Text = estudiante.PrimerNombre;
                EditSegundoNombreEntry.Text = estudiante.SegundoNombre;
                EditPrimerApellidoEntry.Text = estudiante.PrimerApellido;
                EditSegundoApellidoEntry.Text = estudiante.SegundoApellido;
                EditCorreoEntry.Text = estudiante.CorreoElectronico;

                // Establecer el nivel y curso seleccionados
                nivelPicker.SelectedItem = estudiante.Nivel;
                cursoPicker.SelectedItem = estudiante.Curso;
                estadoSwitch.IsToggled = estudiante.Estado;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar el estudiante: {ex.Message}", "OK");
        }
    }

    // Método para actualizar los datos del estudiante
    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validaciones de campos
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditCorreoEntry.Text) ||
                nivelPicker.SelectedItem == null ||
                cursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            // Validación del correo electrónico
            if (!IsValidEmail(EditCorreoEntry.Text))
            {
                await DisplayAlert("Error", "El correo electrónico no es válido", "OK");
                return;
            }

            // Crear el objeto actualizado
            Estudiante estudianteActualizado = new Estudiante
            {
                PrimerNombre = EditPrimerNombreEntry.Text.Trim(),
                SegundoNombre = EditSegundoNombreEntry.Text.Trim(),
                PrimerApellido = EditPrimerApellidoEntry.Text.Trim(),
                SegundoApellido = EditSegundoApellidoEntry.Text.Trim(),
                CorreoElectronico = EditCorreoEntry.Text.Trim(),
                Nivel = nivelPicker.SelectedItem.ToString(),
                Curso = cursoPicker.SelectedItem.ToString(),
                Estado = estadoSwitch.IsToggled
            };

            // Actualizar los datos en Firebase
            await client.Child("Estudiantes").Child(estudianteID).PutAsync(estudianteActualizado);

            await DisplayAlert("Éxito", "El estudiante se ha actualizado correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al actualizar el estudiante: {ex.Message}", "OK");
        }
    }

    // Método para validar el correo electrónico utilizando una expresión regular
    private bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
    }
}
