using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace hdt10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected string randomNamesFile = "random_names.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void bnt_AgregarPaciente_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtBox_AgregarPaciente_Nombre.Text;
            string telefono = txtBox_AgregarPaciente_Tel.Text;
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(telefono)) return;
            try
            {
                neo4jdb.instance.AddPatient(nombre, telefono);
                MessageBox.Show("Paciente agregado...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBox_AgregarPaciente_Nombre.Text = string.Empty;
                txtBox_AgregarPaciente_Tel.Text = string.Empty;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btn_Visitar_Click(object sender, RoutedEventArgs e)
        {
            string paciente = txtBox_PacienteVisita_Paciente.Text;
            string doctor = txtBox_PacienteVisita_Doctor.Text;
            string fecha = txtBox_PacienteVisita_Fecha.Text;
            string medicina = txtBox_PacienteVisita_Medicina.Text;
            string desde = txtBox_PacienteVisita_Desde.Text;
            string hasta = txtBox_PacienteVisita_Hasta.Text;
            string dosis = txtBox_PacienteVisita_Dosis.Text;
            if (string.IsNullOrEmpty(paciente) || string.IsNullOrEmpty(doctor) || string.IsNullOrEmpty(fecha)
                || string.IsNullOrEmpty(medicina) || string.IsNullOrEmpty(desde) || string.IsNullOrEmpty(hasta)
                || string.IsNullOrEmpty(dosis))
                return;
            try
            {
                neo4jdb.instance.PatientVisitsDoctor(paciente, doctor, medicina, fecha, desde, hasta, dosis);
                MessageBox.Show("Visita de paciente agregada...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBox_PacienteVisita_Paciente.Text = string.Empty;
                txtBox_PacienteVisita_Doctor.Text = string.Empty;
                txtBox_PacienteVisita_Fecha.Text = string.Empty;
                txtBox_PacienteVisita_Medicina.Text = string.Empty;
                txtBox_PacienteVisita_Desde.Text = string.Empty;
                txtBox_PacienteVisita_Hasta.Text = string.Empty;
                txtBox_PacienteVisita_Dosis.Text = string.Empty;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btn_AgregarDoctores_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtBox_AgregarDoctor_Nombre.Text;
            string col = txtBox_AgregarDoctor_Col.Text;
            string esp = txtBox_AgregarDoctor_Esp.Text;
            string tel = txtBox_AgregarDoctor_Tel.Text;
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(col) || string.IsNullOrEmpty(esp)
                || string.IsNullOrEmpty(tel)) return;
            try
            {
                neo4jdb.instance.AddDoctor(nombre, col, esp, tel);
                MessageBox.Show("Doctor agregado...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBox_AgregarDoctor_Nombre.Text = string.Empty;
                txtBox_AgregarDoctor_Col.Text = string.Empty;
                txtBox_AgregarDoctor_Esp.Text = string.Empty;
                txtBox_AgregarDoctor_Tel.Text = string.Empty;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btn_PersonaConocePersona_Click(object sender, RoutedEventArgs e)
        {
            string p0 = txtBox_ConectarPersonas_P0.Text;
            string p1 = txtBox_ConectarPersonas_P1.Text;
            if (string.IsNullOrEmpty(p0) || string.IsNullOrEmpty(p1)) return;
            try
            {
                neo4jdb.instance.ConnectPersons(p0, p1);
                MessageBox.Show("Conexión entre personas agregada...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btn_AgregarMedicina_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtBox_AgregarMedicina_Nombre.Text;
            if (string.IsNullOrEmpty(nombre)) return;
            try
            {
                neo4jdb.instance.AddMedicine(nombre);
                MessageBox.Show("Medicina agregada...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btn_BuscarDoctor_Buscar_Click(object sender, RoutedEventArgs e)
        {
            string especialidad = txtBox_BuscarDoctor_Especialidad.Text;
            if (string.IsNullOrEmpty(especialidad)) return;
            try
            {
                IList<string> results = neo4jdb.instance.GetDoctorWithSpecialty(especialidad);
                // limpiar list view
                lView_Doctores.Items.Clear();
                if (results.Count == 0)
                {
                    MessageBox.Show("No se encontraron doctores...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                foreach (var doctor in results)
                {
                    // mostrar en list view
                    lView_Doctores.Items.Add(doctor);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btn_RecDoctor_Buscar_Click(object sender, RoutedEventArgs e)
        {
            string persona = txtBox_RecDoctor_NombrePersona.Text;
            if (string.IsNullOrEmpty(persona)) return;
            try
            {
                IList<string> results = neo4jdb.instance.RecommendDoctors(persona);
                // limpiar list view
                lView_RecDoctores.Items.Clear();
                if (results.Count == 0)
                {
                    MessageBox.Show("No se encontraron doctores...", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                lView_RecDoctores.Items.Clear();
                foreach(var doctor in results)
                {
                    lView_RecDoctores.Items.Add(doctor);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_RecPacientes_Buscar_Click(object sender, RoutedEventArgs e)
        {
            string doctor = txtBox_RecPacientes_NombreDoctor.Text;
            string especialidad = txtBox_RecPacientes_Especialidad.Text;
            if (string.IsNullOrEmpty(doctor) || string.IsNullOrEmpty(especialidad)) return;
            try
            {
                IList<string> results = neo4jdb.instance.RecommendPatients(doctor, especialidad);
                // limpiar list view
                lView_RecPacientes.Items.Clear();
                if (results.Count == 0)
                {
                    MessageBox.Show("No se encontraron pacientes.");
                    return;
                }
                foreach (var paciente in results)
                {
                    lView_RecPacientes.Items.Add(paciente);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
