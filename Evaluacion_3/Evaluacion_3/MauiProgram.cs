﻿using System.Security.Cryptography.X509Certificates;
using Evaluacion_3.Modelos.Modelos;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Evaluacion_3
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            Registrar();
            ActualizarEstudiante();
            ActualizarNivel();
            return builder.Build();
        }

        public static async Task Registrar()
        {
            try
            {
                FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");

                var nivel = await client.Child("Nivel").OnceAsync<Nivel>();
                if (nivel.Count == 0) 
                {
                    await client.Child("Nivel").PostAsync(new Nivel { Nombre = "Basica" });
                    await client.Child("Nivel").PostAsync(new Nivel { Nombre = "Media" });
                }

                var cursos = await client.Child("Cursos").OnceAsync<Curso>();
                if (cursos.Count == 0)
                {
                    var cursosBasica = new string[] { "1°", "2°", "3°", "4°", "5°", "6°", "7°", "8°" };
                    foreach (var curso in cursosBasica)
                    {
                        await client.Child("Cursos").PostAsync(new Curso { Nombre = curso, Nivel = "Basica" , Estado = true});
                    }

                    var cursosMedia = new string[] { "1°", "2°", "3°", "4°" };
                    foreach (var curso in cursosMedia)
                    {
                        await client.Child("Cursos").PostAsync(new Curso { Nombre = curso, Nivel = "Media", Estado = true});
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Registrar: {ex.Message}");
            }
        }
        public static async Task ActualizarNivel()
        {
            FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");

            var nivels = await client.Child("Nivel").OnceAsync<Nivel>();

            if (nivels.Count == 0)
            {
                await client.Child("Nivel").PostAsync(new Nivel { Nombre = "Basica" });
                await client.Child("Nivel").PostAsync(new Nivel { Nombre = "Media" });
            }
            else
            {
                foreach (var nivel in nivels)
                {
                    if (nivel.Object.Estado == null)
                    {
                        var NivelActualizado = nivel.Object;
                        NivelActualizado.Estado = true;

                        await client.Child("Nivel").Child(nivel.Key).PutAsync(NivelActualizado);
                    }
                }
            }
        }

        public static async Task ActualizarEstudiante()
        {
            try
            {
                FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");

                var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();

                foreach (var estudiante in estudiantes)
                {
                    if (estudiante.Object.Estado == null)  
                    {
                        var estudianteActualizado = new Estudiante
                        {
                            Estado = true  
                        };

                        await client.Child("Estudiantes").Child(estudiante.Key).PatchAsync(estudianteActualizado);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarEstudiante: {ex.Message}");
            }
        }

    }
}
